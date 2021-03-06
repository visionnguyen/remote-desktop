﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Utils;
using GenericObjects;

namespace UIControls
{
    public partial class ActionsControl : UserControl
    {
        #region private members

        EventHandler _roomActionTriggered;

        #endregion

        #region c-tor

        public ActionsControl()
        {
            InitializeComponent();
        }

        public ActionsControl(EventHandler actionTriggered)
        {
            try
            {
                InitializeComponent();
                _roomActionTriggered = actionTriggered;
            }
            catch (Exception ex)
            {
                Tools.Instance.Logger.LogError(ex.ToString());
            }
        }

        #endregion

        #region public methods

        public void ChangeLanguage(string language)
        {
            Tools.Instance.GenericMethods.ChangeLanguage(language, this.Controls, typeof(ActionsControl));
        }

        public void UpdateLabels(bool start, bool pause, GenericEnums.RoomType roomType)
        {
            try
            {
                switch (roomType)
                {
                    case GenericEnums.RoomType.Video:
                        if (pause)
                        {
                            btnPauseVideo.Text = ButtonStatus.ButtonPauseStatus.Pause.ToString();
                        }
                        else
                        {
                            btnPauseVideo.Text = ButtonStatus.ButtonPauseStatus.Resume.ToString();
                        }
                        if (start)
                        {
                            btnVideo.Text = ButtonStatus.ButtonStartStatus.Start.ToString();
                        }
                        else
                        {
                            btnVideo.Text = ButtonStatus.ButtonStartStatus.Stop.ToString();
                        }
                        break;
                    case GenericEnums.RoomType.Audio:
                        if (pause)
                        {
                            btnMuteAudio.Text = ButtonStatus.ButtonPauseStatus.Pause.ToString();
                        }
                        else
                        {
                            btnMuteAudio.Text = ButtonStatus.ButtonPauseStatus.Resume.ToString();
                        }
                        if (start)
                        {
                            btnAudio.Text = ButtonStatus.ButtonStartStatus.Start.ToString();
                        }
                        else
                        {
                            btnAudio.Text = ButtonStatus.ButtonStartStatus.Stop.ToString();
                        }
                        break;
                    case GenericEnums.RoomType.Remoting:
                        if (pause)
                        {
                            btnPauseRemote.Text = ButtonStatus.ButtonPauseStatus.Pause.ToString();
                        }
                        else
                        {
                            btnPauseRemote.Text = ButtonStatus.ButtonPauseStatus.Resume.ToString();
                        }
                        if (start)
                        {
                            btnRemote.Text = ButtonStatus.ButtonStartStatus.Start.ToString();
                        }
                        else
                        {
                            btnRemote.Text = ButtonStatus.ButtonStartStatus.Stop.ToString();
                        }
                        break;
                    case GenericEnums.RoomType.Undefined:
                        btnVideo.Text = ButtonStatus.ButtonStartStatus.Start.ToString();
                        btnPauseVideo.Text = ButtonStatus.ButtonPauseStatus.Pause.ToString();
                        btnRemote.Text = ButtonStatus.ButtonStartStatus.Start.ToString();
                        btnPauseRemote.Text = ButtonStatus.ButtonPauseStatus.Pause.ToString();
                        btnAudio.Text = ButtonStatus.ButtonStartStatus.Start.ToString();
                        btnMuteAudio.Text = ButtonStatus.ButtonPauseStatus.Pause.ToString();
                        break;
                }
            }
            catch (Exception ex)
            {
                Tools.Instance.Logger.LogError(ex.ToString());
            }
        }

        #endregion

        #region event callbacks

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                // send the file to the selected contact
                // provide the file path to the File transfer module
                _roomActionTriggered.Invoke(null, new RoomActionEventArgs()
                {
                    RoomType = GenericEnums.RoomType.Send,
                    SignalType = GenericEnums.SignalType.Start
                });
            }
            catch (Exception ex)
            {
                Tools.Instance.Logger.LogError(ex.ToString());
            }
        }

        private void BtnAction_Click(object sender, EventArgs e)
        {
            try
            {
                GenericEnums.SignalType signalType = GenericEnums.SignalType.Undefined;
                // do specific action , check what button was clicked by looking at the sender
                if (sender == btnAudio || sender == btnVideo || sender == btnRemote)
                {
                    if (((Button)sender).Text.ToLower().Equals(GenericEnums.SignalType.Start.ToString().ToLower()))
                    {
                        signalType = GenericEnums.SignalType.Start;
                    }
                    else
                    {
                        signalType = GenericEnums.SignalType.Stop;
                    }
                }
                else
                {
                    if (((Button)sender).Text.ToLower().Equals(GenericEnums.SignalType.Pause.ToString().ToLower()))
                    {
                        signalType = GenericEnums.SignalType.Pause;
                    }
                    else
                    {
                        signalType = GenericEnums.SignalType.Resume;
                    }
                }
                GenericEnums.RoomType actionType = GenericEnums.RoomType.Undefined;
                if (sender == btnAudio || sender == btnMuteAudio)
                {
                    actionType = GenericEnums.RoomType.Audio;
                }
                else
                {
                    if (sender == btnVideo || sender == btnPauseVideo)
                    {
                        actionType = GenericEnums.RoomType.Video;
                    }
                    else
                    {
                        actionType = GenericEnums.RoomType.Remoting;
                    }
                }

                // finally, update the button text
                ToggleStatusUpdate(signalType, (Button)sender);

                // trigger the event so that the Controller does specific action
                // provide the action type as event arg
                RoomActionEventArgs args = new RoomActionEventArgs()
                {
                    RoomType = actionType,
                    SignalType = signalType
                };
                _roomActionTriggered.BeginInvoke(this, args, null, null);
            }
            catch (Exception ex)
            {
                Tools.Instance.Logger.LogError(ex.ToString());
            }
        }

        #endregion

        #region private methods

        void ToggleStatusUpdate(GenericEnums.SignalType buttonType, Button button)
        {
            try
            {
                switch (buttonType)
                {
                    case GenericEnums.SignalType.Start:
                        if (button.Text == ButtonStatus.ButtonStartStatus.Start.ToString())
                        {
                            button.Text = ButtonStatus.ButtonStartStatus.Stop.ToString();
                        }
                        else
                        {
                            button.Text = ButtonStatus.ButtonStartStatus.Start.ToString();
                        }
                        break;
                    case GenericEnums.SignalType.Stop:
                        if (button.Text == ButtonStatus.ButtonStartStatus.Start.ToString())
                        {
                            button.Text = ButtonStatus.ButtonStartStatus.Stop.ToString();
                        }
                        else
                        {
                            button.Text = ButtonStatus.ButtonStartStatus.Start.ToString();
                        }
                        break;
                    case GenericEnums.SignalType.Pause:
                        if (button.Text == ButtonStatus.ButtonPauseStatus.Pause.ToString())
                        {
                            button.Text = ButtonStatus.ButtonPauseStatus.Resume.ToString();
                        }
                        else
                        {
                            button.Text = ButtonStatus.ButtonPauseStatus.Pause.ToString();
                        }
                        break;
                    case GenericEnums.SignalType.Resume:
                        if (button.Text == ButtonStatus.ButtonPauseStatus.Pause.ToString())
                        {
                            button.Text = ButtonStatus.ButtonPauseStatus.Resume.ToString();
                        }
                        else
                        {
                            button.Text = ButtonStatus.ButtonPauseStatus.Pause.ToString();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Tools.Instance.Logger.LogError(ex.ToString());
            }
        }

        #endregion
    }
}
