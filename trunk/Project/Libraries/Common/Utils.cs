﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.Net.Sockets;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace Common
{
    public static class Utils
    {
        public static System.Windows.Point CorrectGetPosition(Visual relativeTo)
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return relativeTo.PointFromScreen(new System.Windows.Point(w32Mouse.X, w32Mouse.Y));
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        public static System.Windows.Controls.Image ConvertDrawingImageToWPFImage(System.Drawing.Image gdiImg, ref System.Windows.Controls.Image img)
        {
            //System.Windows.Controls.Image img = new System.Windows.Controls.Image();

            //convert System.Drawing.Image to WPF image
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(gdiImg);
            IntPtr hBitmap = bmp.GetHbitmap();
            System.Windows.Media.ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            img.Source = wpfBitmap;
            img.Width = 500;
            img.Height = 600;
            img.Stretch = System.Windows.Media.Stretch.Fill;
            
            //bmp.Save("c://test3.bmp");


            return img;
        }

        public static string LocalIPAddress()
        {
            IPHostEntry host;
            string localIP = string.Empty;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }


        #region thread safe methods

        static void SetStringValue(ContentControl control, string newContent)
        {
            control.Content = newContent;
        }

        static void SetEnabledValue(ContentControl control, bool newVal)
        {
            control.IsEnabled = newVal;
        }

        public static void UpdateControlContent(Dispatcher dispatcher, ContentControl control, object newContent, ValueType valueType)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(state =>
            {
                switch (valueType)
                {
                    case ValueType.Boolean:
                        dispatcher.Invoke((Action<ContentControl, bool>)SetEnabledValue, control, newContent);
                        break;
                    case ValueType.String:
                        dispatcher.Invoke((Action<ContentControl, string>)SetStringValue, control, newContent);
                        break;
                }
            });
        }

        public enum ValueType { undefined = 0, Boolean = 1, String = 2 };

        #endregion
    }
}
