using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System.IO;

namespace KinectChris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor kinectChris;

        List<Span> recognitionSpans;

        SpeechRecognitionEngine speechEngine;

        public MainWindow()
        {
            InitializeComponent();
        }

        private static RecognizerInfo GetKinectRecognizer()
        {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }

            return null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            kinectChris = KinectSensor.KinectSensors[0];
            kinectChris.SkeletonStream.Enable();
            kinectChris.Start();
            kinectChris.ColorStream.Enable();

            RecognizerInfo ri = GetKinectRecognizer();


            if (null != ri)
            {
                recognitionSpans = new List<Span> { photoSpan, upSpan, downSpan, restoreSpan, faceSpan};

                this.speechEngine = new SpeechRecognitionEngine(ri.Id);

                
                using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(Properties.Resources.SpeechGrammar)))
                {
                    var g = new Grammar(memoryStream);
                    speechEngine.LoadGrammar(g);
                }

                speechEngine.SpeechRecognized += SpeechRecognized;

            

                speechEngine.SetInputToAudioStream(
                    kinectChris.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
            else
            {
                this.statusBarText.Text = Properties.Resources.NoSpeechRecognizer;
            }



            kinectChris.ColorFrameReady += kinectChris_ColorFrameReady;
            kinectChris.SkeletonFrameReady += miKinect_SkeletonFrameReady;
            angule.Content = kinectChris.ElevationAngle;
        }

        byte[] datosColor = null;
        WriteableBitmap bitmapEficiente = null;

        int controlAzul = 0;
        int controlVerde = 0;
        int controlRojo = 0;
        int nuevoValor;

        void kinectChris_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame framesImagen = e.OpenColorImageFrame())
            {
                if (framesImagen == null) return;

                if (datosColor == null)
                    datosColor = new byte[framesImagen.PixelDataLength];

                framesImagen.CopyPixelDataTo(datosColor);


                for (int i = 0; i < datosColor.Length; i = i + 4)
                {
                    nuevoValor = datosColor[i] + controlAzul;
                    if (nuevoValor < 0) nuevoValor = 0;
                    if (nuevoValor > 255) nuevoValor = 255;
                    datosColor[i] = (byte)nuevoValor;

                    nuevoValor = datosColor[i + 1] + controlVerde;
                    if (nuevoValor < 0) nuevoValor = 0;
                    if (nuevoValor > 255) nuevoValor = 255;
                    datosColor[i + 1] = (byte)nuevoValor;

                    nuevoValor = datosColor[i + 2] + controlRojo;
                    if (nuevoValor < 0) nuevoValor = 0;
                    if (nuevoValor > 255) nuevoValor = 255;
                    datosColor[i + 2] = (byte)nuevoValor;
                }

                if (grabarFoto)
                {
                    bitmapImagen = BitmapSource.Create(
                        framesImagen.Width, framesImagen.Height, 96, 96, PixelFormats.Bgr32, null,
                        datosColor, framesImagen.Width * framesImagen.BytesPerPixel);
                    grabarFoto = false;
                }

                if (bitmapEficiente == null)
                {
                    bitmapEficiente = new WriteableBitmap(
                        framesImagen.Width,
                        framesImagen.Height,
                        96,
                        96,
                        PixelFormats.Bgr32,
                        null);
                    mostrarVideo.Source = bitmapEficiente;
                }

                bitmapEficiente.WritePixels(
                    new Int32Rect(0, 0, framesImagen.Width, framesImagen.Height),
                    datosColor,
                    framesImagen.Width * framesImagen.BytesPerPixel,
                    0
                    );

            }
        }

        bool grabarFoto;
        BitmapSource bitmapImagen = null;

        private void UP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                kinectChris.ElevationAngle += 3;
                angule.Content = kinectChris.ElevationAngle;
            }
            catch (ArgumentOutOfRangeException argumentExcpetion)
            {
                MessageBox.Show("No se puede subir mas la camara");
            }

        }

        private void DOWN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                kinectChris.ElevationAngle -= 3;
                angule.Content = kinectChris.ElevationAngle;
            }
            catch (ArgumentOutOfRangeException argumentExcpetion)
            {
                MessageBox.Show("No se puede subir mas la camara");
            }
        }

        private void REST_Click(object sender, RoutedEventArgs e)
        {

            kinectChris.ElevationAngle = 0;
            angule.Content = kinectChris.ElevationAngle;

        }

        private void FOTO_Click(object sender, RoutedEventArgs e)
        {
            grabarFoto = true;

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "capturaDeKinect";
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "Pictures (.jpg)|*.jpg";

            if (dlg.ShowDialog() == true)
            {
                string nombreArchivo = dlg.FileName;
                using (FileStream stream = new FileStream(nombreArchivo, FileMode.Create))
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapImagen));
                    encoder.Save(stream);
                }
            }
        }

        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.3;

            // Number of degrees in a right angle.
            const int DegreesInRightAngle = 90;

            // Number of pixels turtle should move forwards or backwards each time.
            const int DisplacementAmount = 60;


            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                switch (e.Result.Semantics.Value.ToString())
                {
                    case "PHOTO":
                        grabarFoto = true;
                        Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                        dlg.FileName = "capturaDeKinect";
                        dlg.DefaultExt = ".jpg";
                        dlg.Filter = "Pictures (.jpg)|*.jpg";

                        if (dlg.ShowDialog() == true)
                        {
                            string nombreArchivo = dlg.FileName;
                            using (FileStream stream = new FileStream(nombreArchivo, FileMode.Create))
                            {
                                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                                encoder.Frames.Add(BitmapFrame.Create(bitmapImagen));
                                encoder.Save(stream);
                            }
                        }
                        break;
                    case "UP":
                        try
                        {
                            kinectChris.ElevationAngle += 3;
                            angule.Content = kinectChris.ElevationAngle;
                        }
                        catch (ArgumentOutOfRangeException argumentExcpetion)
                        {
                            MessageBox.Show("No se puede subir mas la camara");
                        }
                        break;
                    case "DOWN":
                        try
                        {
                            kinectChris.ElevationAngle -= 3;
                            angule.Content = kinectChris.ElevationAngle;
                        }
                        catch (ArgumentOutOfRangeException argumentExcpetion)
                        {
                            MessageBox.Show("No se puede subir mas la camara");
                        }
                        break;
                    case "RESTORE":
                        kinectChris.ElevationAngle = 0;
                        angule.Content = kinectChris.ElevationAngle;
                        break;
                    case "FACE":
                        System.Diagnostics.Process.Start("http://www.facebook.com"); 
                        break;
                }
            }
        }




        private void sliderAzul_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            controlAzul = (int)sliderAzul.Value;
        }

        private void sliderVerde_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            controlVerde = (int)sliderVerde.Value;
        }
        private void sliderRojo_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            controlRojo = (int)sliderRojo.Value;
        }

        void miKinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            canvasEsqueleto.Children.Clear();
            Skeleton[] esqueletos = null;

            using (SkeletonFrame frameEsqueleto = e.OpenSkeletonFrame())
            {
                if (frameEsqueleto != null)
                {
                    esqueletos = new Skeleton[frameEsqueleto.SkeletonArrayLength];
                    frameEsqueleto.CopySkeletonDataTo(esqueletos);
                }
            }

            if (esqueletos == null) return;

            foreach (Skeleton esqueleto in esqueletos)
            {
                if (esqueleto.TrackingState == SkeletonTrackingState.Tracked)
                {
                    Joint handJoint = esqueleto.Joints[JointType.HandRight];
                    Joint elbowJoint = esqueleto.Joints[JointType.ElbowRight];

                    // Columna Vertebral
                    agregarLinea(esqueleto.Joints[JointType.Head], esqueleto.Joints[JointType.ShoulderCenter]);
                    agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.Spine]);

                    // Pierna Izquierda
                    agregarLinea(esqueleto.Joints[JointType.Spine], esqueleto.Joints[JointType.HipCenter]);
                    agregarLinea(esqueleto.Joints[JointType.HipCenter], esqueleto.Joints[JointType.HipLeft]);
                    agregarLinea(esqueleto.Joints[JointType.HipLeft], esqueleto.Joints[JointType.KneeLeft]);
                    agregarLinea(esqueleto.Joints[JointType.KneeLeft], esqueleto.Joints[JointType.AnkleLeft]);
                    agregarLinea(esqueleto.Joints[JointType.AnkleLeft], esqueleto.Joints[JointType.FootLeft]);

                    // Pierna Derecha
                    agregarLinea(esqueleto.Joints[JointType.HipCenter], esqueleto.Joints[JointType.HipRight]);
                    agregarLinea(esqueleto.Joints[JointType.HipRight], esqueleto.Joints[JointType.KneeRight]);
                    agregarLinea(esqueleto.Joints[JointType.KneeRight], esqueleto.Joints[JointType.AnkleRight]);
                    agregarLinea(esqueleto.Joints[JointType.AnkleRight], esqueleto.Joints[JointType.FootRight]);

                    // Brazo Izquierdo
                    agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderLeft]);
                    agregarLinea(esqueleto.Joints[JointType.ShoulderLeft], esqueleto.Joints[JointType.ElbowLeft]);
                    agregarLinea(esqueleto.Joints[JointType.ElbowLeft], esqueleto.Joints[JointType.WristLeft]);
                    agregarLinea(esqueleto.Joints[JointType.WristLeft], esqueleto.Joints[JointType.HandLeft]);

                    // Brazo Derecho
                    agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderRight]);
                    agregarLinea(esqueleto.Joints[JointType.ShoulderRight], esqueleto.Joints[JointType.ElbowRight]);
                    agregarLinea(esqueleto.Joints[JointType.ElbowRight], esqueleto.Joints[JointType.WristRight]);
                    agregarLinea(esqueleto.Joints[JointType.WristRight], esqueleto.Joints[JointType.HandRight]);
                }
            }
        }

        void agregarLinea(Joint j1, Joint j2)
        {
            Line lineaHueso = new Line();
            lineaHueso.Stroke = new SolidColorBrush(Colors.Red);
            lineaHueso.StrokeThickness = 5;

            ColorImagePoint j1P = kinectChris.CoordinateMapper.MapSkeletonPointToColorPoint(j1.Position, ColorImageFormat.RgbResolution640x480Fps30);
            lineaHueso.X1 = j1P.X;
            lineaHueso.Y1 = j1P.Y;

            ColorImagePoint j2P = kinectChris.CoordinateMapper.MapSkeletonPointToColorPoint(j2.Position, ColorImageFormat.RgbResolution640x480Fps30);
            lineaHueso.X2 = j2P.X;
            lineaHueso.Y2 = j2P.Y;

            canvasEsqueleto.Children.Add(lineaHueso);
        }
    }




}
