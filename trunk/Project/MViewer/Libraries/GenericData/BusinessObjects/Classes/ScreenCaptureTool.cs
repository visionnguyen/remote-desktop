﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using DesktopSharing;
using Utils;
using System.Timers;
using GenericDataLayer;
using System.Windows.Forms;

namespace GenericDataLayer
{
    public class ScreenCaptureTool : IScreenCaptureTool
    {
        #region members
        
        ScreenCapture _captureToolInstance;

        bool _remotingClosed;

        int _timerInterval;
        System.Timers.Timer _remotingTimer;
        EventHandler _captureReady;

        #endregion

        #region c-tor

        public ScreenCaptureTool(int timerInterval, EventHandler captureReady)
        {
            _captureToolInstance = new ScreenCapture();
            _captureReady = captureReady;
            InitializeTimer(timerInterval);
            _timerInterval = timerInterval;
        }

        #endregion

        #region methods

        void InitializeTimer(int timerInterval)
        {
            _remotingTimer = new System.Timers.Timer(timerInterval);
            _remotingTimer.Elapsed += new ElapsedEventHandler(TimerTick);
        }

        void TimerTick(object sender, ElapsedEventArgs e)
        {
            // todo: implement TimerTick
            try
            {
                _remotingTimer.Stop();

                byte[] serializedScreen = CaptureDekstopImage();
                byte[] serializedMouse = CaptureMouseImage();

                _captureReady.Invoke(this, 
                    new RemotingCaptureEventArgs()
                    {
                        ScreenCapture = serializedScreen,
                        MouseCapture = serializedMouse
                    });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                _remotingTimer.Start();
            }

        }

        //int testNo = 1;

        public void TogglerTimer(bool start)
        {
            if (start)
            {
                if (_remotingTimer == null)
                {
                    InitializeTimer(_timerInterval);
                }
                _remotingClosed = false;
                _remotingTimer.Start();
            }
            else
            {
                if (_remotingTimer != null)
                {
                    _remotingTimer.Stop();
                }
                _remotingClosed = true;
            }
        }

        /// <summary>
        /// method used to capture and serialize the desktop image
        /// </summary>
        /// <returns>serialized desktop image</returns>
        byte[] CaptureDekstopImage()
        {
            byte[] serialized = null;
            Rectangle rect = new Rectangle();
            Bitmap screenCapture = _captureToolInstance.CaptureScreen(ref rect);

            if (screenCapture != null)
            {
                // something has changed on the screen
                serialized = Tools.Instance.RemotingUtils.SerializeDesktopCapture(screenCapture, rect);

                System.Drawing.Image partialDesktop;
                System.Drawing.Rectangle rect2;
                Guid id;
                Tools.Instance.RemotingUtils.Deserialize(serialized, out partialDesktop, out rect2, out id);

            }
            else
            {
                // nothing has changed
                // todo: display the trafic
            }
            return serialized;
        }

        /// <summary>
        /// method used to capture and serialize the mouse cursor image
        /// </summary>
        /// <returns>serialized mouse cursor image</returns>
        byte[] CaptureMouseImage()
        {
            byte[] serialized = null;
            int x = 0, y = 0;
            Image cursorCapture = _captureToolInstance.GetCursorCapture(ref x, ref y);
            if (cursorCapture != null)
            {
                // something has changed
                serialized = Tools.Instance.RemotingUtils.SerializeMouseCapture(cursorCapture, x, y);
                // todo: display the trafic
            }
            else
            {
                // nothing has changed to the cursor
                // todo: display the trafic
            }
            return serialized;
        }

        #endregion

        #region proprieties

        public bool RemotingCaptureClosed
        {
            get
            {
                return _remotingClosed;
            }
        }
        #endregion

    }
}