﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Utils;

namespace GenericDataLayer
{
    public interface IController
    {
        void StartApplication();
        void StopApplication();

        void StartVideoChat(object sender, RoomActionEventArgs args);
        void StopVideChat(object sender, RoomActionEventArgs args);
        void PauseVideoChat(object sender, RoomActionEventArgs args);
        void ResumeVideoChat(object sender, RoomActionEventArgs args);
        void SendFileHandler(object sender, RoomActionEventArgs args);
        void StopRemotingChat(object sender, RoomActionEventArgs args);
        void StartRemotingChat(object sender, RoomActionEventArgs args);
        void PauseRemotingChat(object sender, RoomActionEventArgs args);
        void ResumeRemotingChat(object sender, RoomActionEventArgs args);

        void ExecuteMouseCommand(object sender, RemotingCommandEventArgs args);
        void ExecuteKeyboardCommand(object sender, RemotingCommandEventArgs args);

        void IdentityObserver(object sender, IdentityEventArgs e);
        void RoomButtonAction(object sender, EventArgs e);
        Contact PerformContactsOperation(object sender, ContactsEventArgs ee);
        void StartVideoChat(WebcamCapture webcamControl);
        //void RoomClosingObserver(object sender, EventArgs e);
        void ActiveRoomChanged(string newIdentity, GenericEnums.RoomType roomType);

        string MyIdentity();
        void VideoImageCaptured(object source, EventArgs e);

        void RemotingImageCaptured(object source, EventArgs e);

        void InitializeSettings();
    }
}
