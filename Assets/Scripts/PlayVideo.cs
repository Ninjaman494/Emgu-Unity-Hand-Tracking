using UnityEngine;
using System.Collections;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.UI;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Tracking;
using System.Runtime.InteropServices;
using System;
using System.Drawing;
using static SkinDetection;

public class PlayVideo : MonoBehaviour {

    public Camera camera;

    VideoCapture capture;
    Texture2D texture;
    TrackerCSRT tracker;
    Rectangle handBox;// = new Rectangle(150, 150, 200, 200);
    bool isReady = false;
    // Start is called before the first frame update
    void Start() {
        capture = new VideoCapture(0);
        Image<Bgr, Byte> initialFrame = capture.QueryFrame().ToImage<Bgr,Byte>();
        tracker = new TrackerCSRT();
        
        // Drawing inital handBox
        Vector3 size = GetComponent<Renderer>().bounds.size;
        float width = size.x;
        float height = size.y;
        float widthX = width / initialFrame.Width;
        float heightX = height / initialFrame.Height;
        handBox = new Rectangle((int)(5/widthX), (int)(5/heightX), 1, 1);

        initialFrame.Draw(handBox, new Bgr(System.Drawing.Color.Green), 2);
        texture = TextureConvert.ImageToTexture2D<Bgr, Byte>(initialFrame, FlipType.None);
        GetComponent<Renderer>().material.mainTexture = texture;

        // Spawn "hand"
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = VideoCoordToScreenCoord(handBox.X, handBox.Y, initialFrame.Width, initialFrame.Height);
    }

    // Update is called once per frame
    void Update() {
        Image<Bgr, Byte> frame = capture.QueryFrame().ToImage<Bgr,Byte>();
        if (Input.GetKeyDown("space") && !isReady) {
            print("space key was pressed");
            tracker.Init(frame.Mat, handBox);
            isReady = true;
        }

        if (isReady) {
            Rectangle box;
            tracker.Update(frame.Mat, out box);
            if (box != null) {
                frame.Draw(box, new Bgr(System.Drawing.Color.Green), 3);
            } else {
                Debug.Log("Box is null");
            }
        } else {
            frame.Draw(handBox, new Bgr(System.Drawing.Color.Green), 3);
        }
        GetComponent<Renderer>().material.mainTexture = TextureConvert.ImageToTexture2D<Bgr, byte>(frame,FlipType.None);
    }

    void OnApplicationQuit() {
        capture.Dispose();
    }

    Vector3 VideoCoordToScreenCoord(float x, float y, float frameWidth, float frameHeight) {
        float widthY = (float)camera.pixelWidth / frameWidth;
        float heightY = (float)camera.pixelHeight / frameHeight;
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Vector3 vector = camera.ScreenToWorldPoint(new Vector3(x * widthY,y * heightY, 0));
        vector.z = 0;
        return vector;
    }
}