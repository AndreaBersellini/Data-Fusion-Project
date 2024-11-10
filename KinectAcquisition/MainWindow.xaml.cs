//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.InfraredBasics
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Kinect;

    using System.IO;
    using System.Linq;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor sensor;

        private WriteableBitmap colorBitmap;
        private WriteableBitmap infraredBitmap;
        private WriteableBitmap depthBitmap;

        private byte[] colorPixels;
        private byte[] infraredPixels;
        private ushort[] depthPixels;

        private int img_count = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                //this.sensor.ColorStream.Enable(ColorImageFormat.InfraredResolution640x480Fps30);
                this.sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);

                this.colorPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];
                //this.infraredPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];
                this.depthPixels = new ushort[this.sensor.DepthStream.FramePixelDataLength / 2];


                this.colorBitmap = new WriteableBitmap(this.sensor.ColorStream.FrameWidth, this.sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);
                this.Image.Source = this.colorBitmap;

                //this.infraredBitmap = new WriteableBitmap(this.sensor.ColorStream.FrameWidth, this.sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Gray16, null);
                //this.Image.Source = this.infraredBitmap;

                this.depthBitmap = new WriteableBitmap(this.sensor.DepthStream.FrameWidth, this.sensor.DepthStream.FrameHeight, 96.0, 96.0, PixelFormats.Gray16, null);
                //this.DepthImage.Source = this.depthBitmap;

                this.sensor.ColorFrameReady += this.SensorColorFrameReady;
                //this.sensor.ColorFrameReady += this.SensorInfraredFrameReady;
                this.sensor.DepthFrameReady += this.SensorDepthFrameReady;

                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                this.statusBarText.Text = Properties.Resources.NoKinectReady;
            }
        }

        private void TakePicture()
        {   
            string rgb_path = $"../../../Kinecet_Images_Measured/rgb_{img_count}.txt";
            string depth_path = $"../../../Kinecet_Images_Measured/depth_{img_count}.txt";

            File.WriteAllLines(rgb_path, this.colorPixels.Select(n => n.ToString()));
            File.WriteAllLines(depth_path, this.depthPixels.Select(n => n.ToString()));

            img_count++;
            
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }

        private void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    colorFrame.CopyPixelDataTo(this.colorPixels);

                    this.colorBitmap.WritePixels(
                        new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                        this.colorPixels,
                        this.colorBitmap.PixelWidth * colorFrame.BytesPerPixel,
                        0);

                }
            }
        }

        private void SensorInfraredFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame infraredFrame = e.OpenColorImageFrame())
            {
                if (infraredFrame != null)
                {
                    infraredFrame.CopyPixelDataTo(this.infraredPixels);

                    this.infraredBitmap.WritePixels(
                        new Int32Rect(0, 0, this.infraredBitmap.PixelWidth, this.infraredBitmap.PixelHeight),
                        this.infraredPixels,
                        this.infraredBitmap.PixelWidth * infraredFrame.BytesPerPixel,
                        0);

                }
            }
        }

        private void SensorDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {
                    DepthImagePixel[] depthPixels = new DepthImagePixel[depthFrame.PixelDataLength];

                    depthFrame.CopyDepthImagePixelDataTo(depthPixels);

                    this.depthPixels = new ushort[depthFrame.PixelDataLength];

                    for (int i = 0; i < depthPixels.Length; i++)
                    {
                        ushort depthValue = (ushort)(depthPixels[i].Depth);

                        this.depthPixels[i] = depthValue;
                    }

                    this.depthBitmap.WritePixels(
                        new Int32Rect(0, 0, this.depthBitmap.PixelWidth, this.depthBitmap.PixelHeight),
                        this.depthPixels,
                        this.depthBitmap.PixelWidth * 2,
                        0);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TakePicture();
        }
    }
}