﻿using System;
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

namespace PowerPointKinect
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor miKinect;

        WriteableBitmap bitmapImagenColor = null;
        byte[] bytesColor;

        Skeleton[] esqueleto = null;

        bool movimientoDerechaDer = false;
        bool movimientoIzquierdaIzq = false;
        bool movimientoArribaDerActivo = false;
        bool movimientoAbajoDerActivo = false;
        bool movimientoArribaIzqActivo = false;

        SolidColorBrush brushActivo = new SolidColorBrush(Colors.Green);
        SolidColorBrush brushInactivo = new SolidColorBrush(Colors.Red);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            miKinect = KinectSensor.KinectSensors.FirstOrDefault();
            if (miKinect == null)
            {
                MessageBox.Show("Esta aplicasion requiere de un sensor de kinect.");
                Application.Current.Shutdown();
            }

            miKinect.Start();
            miKinect.ColorStream.Enable();
            miKinect.SkeletonStream.Enable();

            miKinect.ColorFrameReady += miKinect_ColorFrameReady;
            miKinect.SkeletonFrameReady += miKinect_SkeletonFrameReady;
            Application.Current.Exit += Current_Exit;
        }

        void Current_Exit(object sender, ExitEventArgs e)
        {
            if (miKinect != null) {
                miKinect.Stop();
                miKinect = null;
            }
        }

        void miKinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    esqueleto = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(esqueleto);
                }
            }

            if (esqueleto == null) return;

            Skeleton esqueletoCercano = esqueleto.Where(s => s.TrackingState == SkeletonTrackingState.Tracked)
                                                 .OrderBy(s => s.Position.Z * Math.Abs(s.Position.X))
                                                 .FirstOrDefault();

            if (esqueletoCercano == null) return;

            var cabeza = esqueletoCercano.Joints[JointType.Head];
            var manoDer = esqueletoCercano.Joints[JointType.HandRight];
            var manoIzq = esqueletoCercano.Joints[JointType.HandLeft];

            if (cabeza.TrackingState == JointTrackingState.NotTracked ||
                manoDer.TrackingState == JointTrackingState.NotTracked ||
                manoIzq.TrackingState == JointTrackingState.NotTracked) 
            {
                    return;
            }



            posicionEllipse(ellipseCabeza, cabeza);
            posicionEllipse(ellipseManoIzq, manoIzq);
            posicionEllipse(ellipseManoDer, manoDer);

            verificarElipseIzquierdo(ellipseManoIzq, movimientoIzquierdaIzq, movimientoArribaIzqActivo);
            verificarElipseDerecho(ellipseManoDer, movimientoDerechaDer, movimientoAbajoDerActivo, movimientoArribaDerActivo);
            

            verificarElipse(ellipseManoIzq, movimientoIzquierdaIzq);

            procesoAdelanteAtras(cabeza, manoDer, manoIzq);
        }

        private void verificarElipseIzquierdo(Ellipse ellipse, bool movimientoIzquierdaIzq, bool movimientoArribaIzqActivo)
        {
            if (movimientoIzquierdaIzq || movimientoArribaIzqActivo)
            {
                ellipse.Width = 60;
                ellipse.Height = 60;
                ellipse.Fill = brushActivo;
            }
            else
            {
                ellipse.Width = 20;
                ellipse.Height = 20;
                ellipse.Fill = brushInactivo;
            }
        }

        private void verificarElipseDerecho(Ellipse ellipse, bool movimientoDerechaDer, bool movimientoAbajoDerActivo, bool movimientoArribaDerActivo)
        {
            if (movimientoDerechaDer || movimientoArribaDerActivo || movimientoAbajoDerActivo )
            {
                ellipse.Width = 60;
                ellipse.Height = 60;
                ellipse.Fill = brushActivo;
            }
            else
            {
                ellipse.Width = 20;
                ellipse.Height = 20;
                ellipse.Fill = brushInactivo;
            }
        }

        private void verificarElipse(Ellipse ellipse, bool activo)
        {
            if (activo)
            {
                ellipse.Width = 60;
                ellipse.Height = 60;
                ellipse.Fill = brushActivo;
            }
            else
            {
                ellipse.Width = 20;
                ellipse.Height = 20;
                ellipse.Fill = brushInactivo;
            }
        }

        private void procesoAdelanteAtras(Joint cabeza, Joint manoDer, Joint manoIzq)
        {
            
            if (manoDer.Position.X > cabeza.Position.X + 0.50) {
                if (!movimientoDerechaDer)
                {
                    movimientoDerechaDer = true;
                    System.Windows.Forms.SendKeys.SendWait("{Right}");
                }
            }
            else
            {
                movimientoDerechaDer = false;
            }
            
            if (manoDer.Position.Y > cabeza.Position.Y + 0.10)
            {
                if (!movimientoArribaDerActivo)
                {
                    movimientoArribaDerActivo = true;
                    System.Windows.Forms.SendKeys.SendWait("{Up}");
                }
            }
            else
            {
                movimientoArribaDerActivo = false;
            }

            if (manoDer.Position.Y < cabeza.Position.Y - 0.70)
            {
                if (!movimientoAbajoDerActivo)
                {
                    movimientoAbajoDerActivo = true;
                    System.Windows.Forms.SendKeys.SendWait("{Down}");
                }
            }
            else
            {
                movimientoAbajoDerActivo = false;
            }

            if (manoIzq.Position.Y > cabeza.Position.Y + 0.10)
            {
                if (!movimientoArribaIzqActivo)
                {
                    movimientoArribaIzqActivo = true;
                    System.Windows.Forms.SendKeys.SendWait("{Enter}");
                }
            }
            else
            {
                movimientoArribaIzqActivo = false;
            }
           
            if (manoIzq.Position.X < cabeza.Position.X - 0.50)
            {
                if (!movimientoIzquierdaIzq)
                {
                    movimientoIzquierdaIzq = true;
                    System.Windows.Forms.SendKeys.SendWait("{Left}");
                }
            }
            else
            {
                movimientoIzquierdaIzq = false;
            }
        }

        private void posicionEllipse(Ellipse ellipse, Joint joint)
        {

            CoordinateMapper mapping = miKinect.CoordinateMapper;

            var point = mapping.MapSkeletonPointToColorPoint(joint.Position, miKinect.ColorStream.Format);
            Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);
        }

        void miKinect_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame imagenColor = e.OpenColorImageFrame())
            {
                if (imagenColor == null)
                    return;

                if (bytesColor == null || bytesColor.Length != imagenColor.PixelDataLength)
                    bytesColor = new byte[imagenColor.PixelDataLength];

                imagenColor.CopyPixelDataTo(bytesColor);

                if (bitmapImagenColor == null)
                {
                    bitmapImagenColor = new WriteableBitmap(
                        imagenColor.Width,
                        imagenColor.Height,
                        96,
                        96,
                        PixelFormats.Bgr32,
                        null);
                }

                bitmapImagenColor.WritePixels(
                    new Int32Rect(0, 0, imagenColor.Width, imagenColor.Height),
                    bytesColor,
                    imagenColor.Width * imagenColor.BytesPerPixel,
                    0);

                imagenVideo.Source = bitmapImagenColor;
            }
        }
    }
}
