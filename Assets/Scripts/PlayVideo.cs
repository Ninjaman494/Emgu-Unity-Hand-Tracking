using UnityEngine;
using UnityEngine.UI;
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
    public GameObject hand;

    VideoCapture capture;
    Texture2D texture;
    TrackerCSRT tracker;
    Rectangle handBox;// = new Rectangle(150, 150, 200, 200);
    bool isReady = false;
    RawImage RawImage;

    // Start is called before the first frame update
    void Start() {
        capture = new VideoCapture(0);
        Image<Bgr, Byte> initialFrame = capture.QueryFrame().ToImage<Bgr,Byte>();
        tracker = new TrackerCSRT();
        RawImage = GetComponent<RawImage>();

        // Drawing inital handBox
        float midX = (initialFrame.Width / 2);
        float midY = (initialFrame.Height / 2);
        handBox = new Rectangle((int)midX - 100,(int)midY - 100, 200, 200);

        initialFrame.Draw(handBox, new Bgr(System.Drawing.Color.Green), 3);
        texture = TextureConvert.ImageToTexture2D<Bgr, Byte>(initialFrame, FlipType.Vertical);
        RawImage.texture = texture;
    }

    // Update is called once per frame
    void Update() {
        Image<Bgr, Byte> frame = capture.QueryFrame().ToImage<Bgr,Byte>();
        if (Input.GetKeyDown("space") && !isReady) {
            print("space key was pressed");
            tracker.Init(frame.Mat, handBox);
            isReady = true;

            // Move Raw Image
            RawImage.rectTransform.anchorMin = new Vector2(1, 0);
            RawImage.rectTransform.anchorMax = new Vector2(1, 0);
            RawImage.rectTransform.pivot = new Vector2(1, 0);
            RawImage.rectTransform.sizeDelta = new Vector2(319, 179);
        }

        if (isReady) {
            Rectangle box;
            tracker.Update(frame.Mat, out box);
            if (box != null) {
                frame.Draw(box, new Bgr(System.Drawing.Color.Green), 3);

                // Invert y axis
                Vector3 pos = VideoCoordToScreenCoord(box.X, box.Y, frame.Width, frame.Height);
                pos.y = pos.y * -1;
                hand.transform.position = pos;
            } else {
                Debug.Log("Box is null");
            }
        } else {
            frame.Draw(handBox, new Bgr(System.Drawing.Color.Green), 3);
        }
        RawImage.texture = TextureConvert.ImageToTexture2D<Bgr, byte>(frame,FlipType.Vertical);
    }

    void OnApplicationQuit() {
        capture.Dispose();
    }

    Vector3 VideoCoordToScreenCoord(float x, float y, float frameWidth, float frameHeight) {
        Vector3 vector = camera.ViewportToWorldPoint(new Vector3(x - 218,y - 258,camera.nearClipPlane));
        vector.z = 0;
        return vector;
    }
}