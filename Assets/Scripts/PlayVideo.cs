using UnityEngine;
using System.Collections;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.UI;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Runtime.InteropServices;
using System;
using System.Drawing;
using static SkinDetection;

public class PlayVideo : MonoBehaviour {
    VideoCapture capture;
    Texture2D texture;


    // Start is called before the first frame update
    void Start() {
        capture = new VideoCapture("C:\\Users\\akash\\Downloads\\hand_vid.mp4");
        Image<Bgr, Byte> initialFrame = capture.QueryFrame().ToImage<Bgr,Byte>();
        //Image<Bgr,Byte> hand = GetSkin(initialFrame);
        //DoCascade(hand).Save("C:\\Users\\akash\\Desktop\\fromVid.jpg");

        texture = TextureConvert.ImageToTexture2D<Bgr, Byte>(initialFrame, FlipType.None);
        GetComponent<Renderer>().material.mainTexture = texture;
    }

    // Update is called once per frame
    void Update() {
        Image<Bgr, Byte> frame = capture.QueryFrame().ToImage<Bgr,Byte>();
        GetComponent<Renderer>().material.mainTexture = TextureConvert.ImageToTexture2D<Bgr, byte>(frame,FlipType.None);
    }
}