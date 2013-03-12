﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GenericDataLayer;
using Utils;
using UIControls.CrossThreadOperations;
using System.Threading;

namespace MViewer
{
    public partial class FormVideoRoom : Form, IVideoRoom
    {
        #region private members

        bool _formClosing;
        ManualResetEvent _syncClosing = new ManualResetEvent(true);

        public ManualResetEvent SyncClosing
        {
            get { return _syncClosing; }
        }

        //Container _components;
        //System.Windows.Forms.Timer _closeTimer;

        #endregion

        #region c-tor

        public FormVideoRoom(string identity)
        {
            InitializeComponent();

            //_components = new Container();
            //_closeTimer = new System.Windows.Forms.Timer(_components);
            //_closeTimer.Interval = 50;
            //_closeTimer.Tick += new EventHandler(CloseFormTimer);

            ContactIdentity = identity;
            //handle = this.Handle;
            _formClosing = false;
        }

        #endregion

        #region callbacks

        private void FormVideoRoom_Activated(object sender, EventArgs e)
        {
            // tell the controller to update the active form
            Program.Controller.ActiveRoomChanged(this.ContactIdentity, this.RoomType);
        }

        private void FormVideoRoom_Resize(object sender, EventArgs e)
        {
            pnlMain.Width = this.Width - 20 - 1;
            pnlMain.Height = this.Height - 20 - 1;

            videoControl.Width = pnlMain.Width - 15 - 5;
            videoControl.Height = pnlMain.Height - 38 - 5;
        }

        private void FormVideoRoom_FormClosing(object sender, FormClosingEventArgs e)
        {
            _formClosing = true;

            // todo: later - perform other specific actions when the Video Chat room is closing
            _syncClosing.Set();
        }

        #endregion

        #region public methods

        public void SetPartnerName(string friendlyName)
        {
            videoControl.SetPartnerName(friendlyName);
        }

        public void SetPicture(Image picture)
        {
            _syncClosing.WaitOne();
            if (!_formClosing)
            {
                videoControl.SetPicture(picture);
            }
        }

        //delegate void CloseDelegate();  

        //// todo: remove close room from base class
        //public void CloseRoom()
        //{
        //    _syncClosing.Reset();

        //    _formClosing = true;
        //    _closeTimer.Start();
        //}

        //private void CloseFormTimer(object sender, EventArgs args)
        //{
        //    this.Invoke(new CloseDelegate(this.Close));
        //}

        public void ShowRoom()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke
                        (
                        new MethodInvoker
                        (
                       delegate
                       {
                           this.Show();
                       }
                        )
                        );

                }
                else
                {
                    this.Show();
                }
            }
            catch
            {
            }
        }

        #endregion

        #region proprieties

        public string ContactIdentity
        {
            get;
            set;
        }

        public GenericEnums.RoomType RoomType
        {
            get
            {
                return Utils.GenericEnums.RoomType.Video;
            }
        }

        public IntPtr FormHandle
        {
            get
            {
                object value = null;
                ControlCrossThreading.GetValue(this, "Handle", ref value);
                return (IntPtr)value;
            }
        }

        #endregion
    }
}