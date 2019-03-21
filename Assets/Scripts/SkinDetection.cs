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

public class SkinDetection : MonoBehaviour {

    static bool R1(int R, int G, int B) {
        bool e1 = (R > 95) && (G > 40) && (B > 20) && ((Math.Max(R, Math.Max(G, B)) - Math.Min(R, Math.Min(G, B))) > 15) && (Math.Abs(R - G) > 15) && (R > G) && (R > B);
        bool e2 = (R > 220) && (G > 210) && (B > 170) && (Math.Abs(R - G) <= 15) && (R > B) && (G > B);
        return (e1 || e2);
    }

    static bool R2(float Y, float Cr, float Cb) {
        bool e3 = Cr <= 1.5862 * Cb + 20;
        bool e4 = Cr >= 0.3448 * Cb + 76.2069;
        bool e5 = Cr >= -4.5652 * Cb + 234.5652;
        bool e6 = Cr <= -1.15 * Cb + 301.75;
        bool e7 = Cr <= -2.2857 * Cb + 432.85;
        return e3 && e4 && e5 && e6 && e7;
    }

    static bool R3(float H, float S, float V) {
        return (H < 25) || (H > 230);
    }

    public static Image<Bgr,Byte> GetSkin(Image<Bgr, Byte> image) {
        // allocate the result matrix
        Image<Bgr,Byte> dst = image.Clone();
        Bgr cwhite = new Bgr(255,255,255);//Vec3b::all(255);
        Bgr cblack = new Bgr(0,0,0);//Vec3b::all(0);

        Image<Ycc,Byte> src_ycrcb = new Image<Ycc,Byte>(dst.Width,dst.Height);// = new Mat(); 
        Image<Hsv,Single> src_hsv = new Image<Hsv,Single>(dst.Width,dst.Height);// = new Mat();
        // OpenCV scales the YCrCb components, so that they
        // cover the whole value range of [0,255], so there's
        // no need to scale the values:
        Emgu.CV.CvInvoke.CvtColor(image, src_ycrcb, Emgu.CV.CvEnum.ColorConversion.Bgr2YCrCb);
        // OpenCV scales the Hue Channel to [0,180] for
        // 8bit images, so make sure we are operating on
        // the full spectrum from [0,360] by using floating
        // point precision:
        //image.ConvertTo(src_hsv, Emgu.CV.CvEnum.DepthType.Cv32F);
        Emgu.CV.CvInvoke.CvtColor(image, src_hsv, Emgu.CV.CvEnum.ColorConversion.Bgr2Hsv); // src_hsv to image
        // Now scale the values between [0,255]:
        Emgu.CV.CvInvoke.Normalize(src_hsv, src_hsv, 0.0, 255.0, Emgu.CV.CvEnum.NormType.MinMax, Emgu.CV.CvEnum.DepthType.Cv32F);

        for(int i = 0; i<image.Rows; i++) {
            for(int j = 0; j<image.Cols; j++) {

                //Vec3b pix_bgr = src.ptr<Vec3b>(i)[j];
                Bgr pix_bgr = image[i,j];
                int B = (int)pix_bgr.Blue;
                int G = (int)pix_bgr.Green;
                int R = (int)pix_bgr.Red;
                // apply rgb rule
                bool a = R1(R, G, B);

                //Vec3b pix_ycrcb = src_ycrcb.ptr<Vec3b>(i)[j];
                Ycc pix_ycrcb = src_ycrcb[i,j];
                float Y = (float)pix_ycrcb.Y;
                float Cr = (float)pix_ycrcb.Cr;
                float Cb = (float)pix_ycrcb.Cb;
                // apply ycrcb rule
                bool b = R2(Y, Cr, Cb);

                Hsv pix_hsv = src_hsv[i,j];
                float H = (float)pix_hsv.Hue;
                float S = (float)pix_hsv.Satuation;
                float V = (float)pix_hsv.Value;
                // apply hsv rule
                bool c = R3(H, S, V);

                if(!(a&&b&&c)) {
                    dst[i,j] = cblack;
                } else {
                    dst[i,j] = cwhite; // Make hand white
                }
            }
        }
        return dst;
    }

    // Start is called before the first frame update
    void Start() {
        Image<Bgr, Byte> picture = new Image<Bgr, Byte>("C:\\Users\\akash\\Downloads\\new_hand.jpg"); 
        Image<Bgr,Byte> hand = GetSkin(picture);
        DoCascade(hand).Save("C:\\Users\\akash\\Desktop\\picture2.jpg");
        //hand.Save("C:\\Users\\akash\\Desktop\\picture2.jpg");
    }

    public static Image<Bgr, Byte> DoCascade(Image<Bgr,Byte> source) {
        //Bitmap Source; //your Bitmap
        Image<Bgr, Byte> ImageFrame = source;//new Image<Bgr, Byte>("C:\\Users\\akash\\Downloads\\hand.jpg"); //image that stores your bitmap
        Image<Gray, Byte> grayFrame = ImageFrame.Convert<Gray, Byte>(); //grayscale of your image
        CascadeClassifier haar = new CascadeClassifier("C:\\Users\\akash\\Downloads\\Hand.Cascade.1.xml"); //the object used for detection

        Rectangle[] faces = haar.DetectMultiScale(grayFrame, 1.1, 10, Size.Empty); //the actual face detection happens here
        foreach (Rectangle face in faces)
            ImageFrame.Draw(face, new Bgr(System.Drawing.Color.Green), 3); //draws a rectangle on top of your detection
        return ImageFrame; //returns your bitmap with detection applied;
    }
}