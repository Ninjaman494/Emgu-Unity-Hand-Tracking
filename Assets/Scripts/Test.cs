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

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        Image<Bgr, byte> picture = new Image<Bgr, byte>("C:\\Users\\akash\\Pictures\\murals\\mural1.jpg"); 
        Bgr myWhiteColor = new Bgr(255, 255, 255);
        for (int i=0; i<200; i++) {
            picture[i,i]= myWhiteColor;
        }

        picture.Save("C:\\Users\\akash\\Desktop\\picture2.jpg");
    }

    // Update is called once per frame
    void Update() {
        
    }
}