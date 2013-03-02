﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using UIControls;
using Utils;
using GenericDataLayer;
using BusinessLogicLayer;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace MViewer
{
    public class Controller : IController
    {
        #region private members

        /// <summary>
        /// flag used to tell the capturing thread to wait before ending it's activity
        /// </summary>
        bool _capturePending;

        /// <summary>
        /// object used to synchronize the capture sent to partners
        /// </summary>
        readonly object _syncCaptureSending = new object();

        ManualResetEvent _syncCaptureActivity = new ManualResetEvent(true);

        Presenter _presenter;
        IView _view;
        IModel _model;

        #endregion

        #region c-tor

        public Controller()
        {
            // initialize the model

            ControllerEventHandlers handlers = new ControllerEventHandlers()
            {
                ClientConnectedObserver = this.ClientConnected,
                VideoCaptureObserver = this.NotifyVideoCaptureObserver,
                ContactsObserver = this.ContactRequest,
                RoomClosingObserver = this.RoomClosingObserver,
                WaitRoomActionObserver = this.WaitRoomButtonActionObserver
            };

            _model = new Model(handlers);
            // initalize the view and bind it to the model
            _view = new View(_model);
        }

        #endregion

        #region event handlers

        private void WebCamImageCaptured(object source, EventArgs e)
        {
            try
            {
                _syncCaptureActivity.WaitOne(); // wait for any room action to end

                _capturePending = true;
                if (_view.WebcaptureClosed == false)
                {
                    lock (_syncCaptureSending)
                    {
                        VideoCaptureEventArgs args = (VideoCaptureEventArgs)e;
                        // display the captured picture
                        _view.UpdateWebcapture(args.CapturedImage);

                        //while (!_audioCapture.AudioCaptureReady)
                        //{
                        //    Thread.Sleep(200);
                        //}

                        //we want to get a byte[] representation ... a MemoryStreams buffer will do
                        MemoryStream ms = new MemoryStream();
                        //save image to stream ... the stream will write it into the buffer
                        args.CapturedImage.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

                        //get the buffer 
                        byte[] bitmapBytes = ms.GetBuffer();

                        // broadcast the webcaptures to all connected peers

                        IList<string> connectedSessions = _model.SessionManager.GetConnectedSessions(GenericEnums.RoomType.Video);
                        if (connectedSessions.Count == 0)
                        {
                            _presenter.StopPresentation();
                        }
                        else
                        {
                            foreach (string receiverIdentity in connectedSessions)
                            {
                                //Thread.Sleep(10 * 1000);

                                TransferStatusUptading transfer = _model.SessionManager.GetTransferActivity(receiverIdentity);
                                while (transfer.IsVideoUpdating)
                                {
                                    Thread.Sleep(200);
                                }

                                PendingTransfer transferStatus = _model.SessionManager.GetTransferStatus(receiverIdentity);
                                // check if the stop signal has been sent from the UI

                                // check if the stop signal has been sent by the partner

                                PeerStates peers = _model.SessionManager.GetPeerStatus(receiverIdentity);
                                if (peers.VideoSessionState == GenericEnums.SessionState.Opened
                                    || peers.VideoSessionState == GenericEnums.SessionState.Pending)
                                {
                                    // send the capture if the session isn't paused
                                    transferStatus.Video = true;
                                    _model.ClientController.SendCapture(bitmapBytes, receiverIdentity,
                                        _model.Identity.MyIdentity);
                                }

                                transferStatus.Video = false;
                                if (peers.AudioSessionState == GenericEnums.SessionState.Opened
                                    || peers.AudioSessionState == GenericEnums.SessionState.Pending)
                                {
                                    transferStatus.Audio = true;

                                    // todo: send audio capture

                                    transferStatus.Audio = false;
                                }

                            }
                        }
                    }
                }
                else
                {
                    _presenter.StopPresentation();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                //_webcamCapture.StopCapturing();
            }
            finally
            {
                _capturePending = false;
            }
        }

        #endregion

        #region public methods

        public void ActiveRoomChanged(string newIdentity, GenericEnums.RoomType roomType)
        {
            // update the active room identity
            _view.RoomManager.ActiveRoom = newIdentity;
            // update the actions form button labels
            _view.UpdateLabels(newIdentity, roomType);
        }

        public void GetContactsStatus()
        {
            _model.PingContacts(null);
        }

        public void IdentityObserver(object sender, IdentityEventArgs e)
        {
            _model.Identity.UpdateFriendlyName(e.FriendlyName);
            // notify online contacts of updated friendly name
            _model.NotifyContacts(e.FriendlyName);
        }

        public void StartVideoChat(WebcamCapture webcamControl)
        {
            // create Presenter and start the presentation
            int timerInterval = 100;
            int height = 354, width = 360;

            // initialize the presenter that will send webcam captures to all Server Sessions
            if (_presenter == null)
            {
                _presenter = PresenterManager.Instance(webcamControl, _model.Identity.MyIdentity,
                    timerInterval, height, width,
                    new EventHandler(this.WebCamImageCaptured));
                _presenter.StartPresentation(true);
            }
            else
            {
                _presenter.StartPresentation(false);
            }
        }

        public void RoomClosingObserver(object sender, EventArgs e)
        {
            RoomActionEventArgs args = (RoomActionEventArgs)e;
            switch (args.RoomType)
            {
                case GenericEnums.RoomType.Audio:

                    break;
                case GenericEnums.RoomType.Video:
                        RoomButtonAction(sender, args);
                    break;
                case GenericEnums.RoomType.Remoting:

                    break;
            }

        }

        public void ClientConnected(object sender, EventArgs e)
        {
            // todo: complete implementation of ClientConnected
            RoomActionEventArgs args = (RoomActionEventArgs)e;
            switch (args.RoomType)
            {
                case GenericEnums.RoomType.Audio:

                    break;
                case GenericEnums.RoomType.Video:
                    // open new Video Chat form to receive the captures
                    Thread t = new Thread(delegate()
                    {
                        StartVideoChat(args.Identity);
                    });
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                    // open my webcam form and send my captures to the connected contact
                    Program.Controller.RoomButtonAction(sender, args);

                    break;
                case GenericEnums.RoomType.Remoting:

                    break;
                case GenericEnums.RoomType.Send:

                    break;
            }
        }

        public void RoomButtonAction(object sender, RoomActionEventArgs e)
        {
            _syncCaptureActivity.Reset();

            // perform specific action when room action event has been triggered
            switch (e.RoomType)
            {
                case GenericEnums.RoomType.Audio:

                    break;
                case GenericEnums.RoomType.Remoting:

                    break;
                case GenericEnums.RoomType.Send:

                    break;
                case GenericEnums.RoomType.Video:
                    switch (e.SignalType)
                    {
                        case GenericEnums.SignalType.Start:
                            // start the video chat
                            PerformVideoChatAction(sender, e);
                            break;
                        case GenericEnums.SignalType.Pause:
                            // pause the video chat (stop sending the captured images, don't disconnect the webcam yet)
                            PerformVideoChatAction(sender, e);
                            break;
                        case GenericEnums.SignalType.Resume:
                            PerformVideoChatAction(sender, e);
                            break;
                        case GenericEnums.SignalType.Stop:
                            PerformVideoChatAction(sender, e);
                            // close the webcapture form if there s no room left
                            if (!_view.RoomManager.RoomsLeft())
                            {
                                StopWebCapturing();
                            }
                            break;
                    }
                    break;
            }
            _syncCaptureActivity.Set();
        }

        public Contact PerformContactsOperation(object sender, ContactsEventArgs e)
        {
            Contact contact = null;
            if (e.Operation == GenericEnums.ContactsOperation.Load)
            {
                // don't need to send signal to the Model
                NotifyContactsObserver();
            }
            else
            {
                // add/remove/get/status/name update
                contact = _model.PerformContactOperation(e);
                NotifyContactsObserver();
            }
            return contact;
        }

        public void StartApplication()
        {
            // bind the observers
            _view.BindObservers(true);

            // open main form
            _view.ShowMainForm(false);
            NotifyContactsObserver();

            // todo: use manual reset event instead of thread.sleep(0)
            Thread.Sleep(2000);

            _view.NotifyIdentityObserver();
            _model.ServerController.StartServer();

            Thread.Sleep(2000);

            // ping every single contact in the list and update it's status
            GetContactsStatus();

            // notify all online contacts that you came on too
            _model.NotifyContacts(GenericEnums.ContactStatus.Online);
        }

        public void StopApplication()
        {
            // todo: update the StopApplication method with other actions

            // check for running video/audio/remoting chats
            bool canExit = _view.ExitConfirmation();
            if (canExit)
            {
                // stop all active rooms
                IList<string> partnerIdentities = _model.SessionManager.GetConnectedSessions(GenericEnums.RoomType.Video);
                foreach (string identity in partnerIdentities)
                {
                    StopVideChat(identity, true);
                }
                // stop my webcapture form
                StopWebCapturing();

                // todo: stop the audio & remoting rooms also

                // unbind the observers
                _view.BindObservers(false);

                _model.ServerController.StopServer();

                // notify all contacts that you exited the chat
                _model.NotifyContacts(GenericEnums.ContactStatus.Offline);

                // exit the environment
                Environment.Exit(0);
            }
        }

        public void NotifyContactsObserver()
        {
            _view.NotifyContactsObserver();
        }

        public void NotifyIdentityObserver()
        {
            //_model.ContactsUpdated();
            _view.NotifyIdentityObserver();
        }

        public void NotifyActionsObserver()
        {
            //_model.ContactsUpdated();
            _view.NotifyActionsObserver();
        }

        #endregion

        #region private methods

        void StopVideChat(string identity, bool sendStopSignal)
        {
            // tell the partner to pause capturing & sending while processing room Stop command

            _model.ClientController.WaitRoomButtonAction(identity, _model.Identity.MyIdentity, GenericEnums.RoomType.Video,
                true);

            TransferStatusUptading transfer = _model.SessionManager.GetTransferActivity(identity);
            transfer.IsVideoUpdating = true;

            // check if the webcapture is pending for being sent
            PendingTransfer transferStatus = _model.SessionManager.GetTransferStatus(identity);
            while (transferStatus.Video)
            {
                // wait for it to finish and block the next sending
                Thread.Sleep(200);
            }

            PeerStates peers = _model.SessionManager.GetPeerStatus(identity);
            // update the session status to closed
            if (peers.VideoSessionState != GenericEnums.SessionState.Closed)
            {
                peers.AudioSessionState = GenericEnums.SessionState.Closed;
                peers.VideoSessionState = GenericEnums.SessionState.Closed;
                _model.SessionManager.UpdateSession(identity, peers);

                if (sendStopSignal)
                {
                    // send the stop signal to the server session
                    _model.ClientController.SendRoomCommand(_model.Identity.MyIdentity, identity,
                        GenericEnums.RoomType.Video, GenericEnums.SignalType.Stop);
                }

                // remove the connected client session
                _model.SessionManager.RemoveSession(identity);
                _view.RoomManager.CloseRoom(identity);
                _view.RoomManager.RemoveRoom(identity);
            }

            _model.ClientController.WaitRoomButtonAction(identity, _model.Identity.MyIdentity, GenericEnums.RoomType.Video,
                false);

            if (_view.RoomManager.RoomsLeft() == false)
            {
                _view.ResetLabels(GenericEnums.RoomType.Video);
            }
        }
        void StartVideoChat(string identity)
        {
            if (!_view.IsRoomActivated(identity, GenericEnums.RoomType.Video))
            {
                Thread t = new Thread(delegate()
                {
                    //IntPtr handle = IntPtr.Zero;
                    FormVideoRoom videoRoom = new FormVideoRoom(identity);
                    _view.RoomManager.AddRoom(identity, videoRoom);
                    // initialize new video chat form
                    Thread.Sleep(1000);

                    PeerStates peers = _model.SessionManager.GetPeerStatus(identity);
                    peers.VideoSessionState = GenericEnums.SessionState.Opened;
                    peers.AudioSessionState = GenericEnums.SessionState.Opened;

                    _model.SessionManager.UpdateSession(identity, peers);

                    peers = _model.SessionManager.GetPeerStatus(identity);

                    Contact contact = _model.GetContact(identity);
                    // get friendly name from contacts list
                    _view.RoomManager.SetPartnerName(identity, contact.FriendlyName);
                    // finally, show the video chat form where we'll see the webcam captures
                    _view.RoomManager.ShowRoom(identity);
                }
                );
                t.IsBackground = true;
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
            }
        }

        void WaitRoomButtonActionObserver(object sender, EventArgs e)
        {
            // todo: complete implemention of WaitRoomButtonAction
            RoomActionEventArgs args = (RoomActionEventArgs)e;
            switch (args.RoomType)
            {
                case GenericEnums.RoomType.Video:
                    bool freezeVideo = false;
                    if (args.SignalType == GenericEnums.SignalType.Wait)
                    {
                        freezeVideo = true;
                    }
                    // send the freeze signal to the webcapture obj
                    _view.WaitRoomButtonAction(freezeVideo);
                    break;
            }
        }


        void NotifyVideoCaptureObserver(object sender, EventArgs e)
        {
            VideoCaptureEventArgs args = (VideoCaptureEventArgs)e;
            PeerStates peer = _model.SessionManager.GetPeerStatus(args.Identity);

            if (peer.VideoSessionState == GenericEnums.SessionState.Undefined ||
                peer.VideoSessionState == GenericEnums.SessionState.Pending)
            {
                // receiving captures for the first time, have to initalize a form
                InitializeRoom(args.Identity, GenericEnums.RoomType.Video);
                while (peer.VideoSessionState != GenericEnums.SessionState.Opened)
                {
                    Thread.Sleep(2000);
                    peer = _model.SessionManager.GetPeerStatus(args.Identity); // update the peer status
                }
            }

            // check the videochat status before displaying the picture
            if (peer.VideoSessionState == GenericEnums.SessionState.Opened)
            {
                _view.RoomManager.ShowPicture(args.Identity, args.CapturedImage);
            }
        }

        void ContactRequest(object sender, EventArgs e)
        {
            PerformContactsOperation(sender, (ContactsEventArgs)e);
        }

        void InitializeRoom(string identity, GenericEnums.RoomType roomType)
        {
            switch (roomType)
            {
                case GenericEnums.RoomType.Video:
                    // initialize video chat form to receive captures from the client
                    // initialize my webcam form so that I can send my captures to the connected contact
                    ClientConnected(null, new RoomActionEventArgs()
                    {
                        RoomType = GenericEnums.RoomType.Video,
                        SignalType = GenericEnums.SignalType.Start,
                        Identity = identity
                    });
                    break;
                case GenericEnums.RoomType.Audio:

                    break;
                case GenericEnums.RoomType.Remoting:

                    break;
                case GenericEnums.RoomType.Send:

                    break;
            }
        }

        void StopWebCapturing()
        {
            if (_presenter != null)
            {
                _presenter.StopPresentation();
            }
            Thread.Sleep(1000);
            while (_capturePending)
            {
                Thread.Sleep(200);
            }
            _view.ShowMyWebcamForm(false);
        }

        void PerformVideoChatAction(object sender, RoomActionEventArgs eArgs)
        {
            string identity = eArgs.Identity; // this will be the active/selected room
            PeerStates peers = _model.SessionManager.GetPeerStatus(identity);
                    
            switch (eArgs.SignalType)
            {
                case GenericEnums.SignalType.Start:

                    // I am going to send my captures by using the below client
                    _model.ClientController.AddClient(eArgs.Identity);
                    _model.ClientController.StartClient(eArgs.Identity);

                    // create client session
                    Session clientSession = new ClientSession(eArgs.Identity);
                    switch (eArgs.RoomType)
                    {
                        case GenericEnums.RoomType.Audio:
                            clientSession.AudioSessionState = GenericEnums.SessionState.Pending;
                            break;
                        case GenericEnums.RoomType.Video:
                            clientSession.AudioSessionState = GenericEnums.SessionState.Pending;
                            clientSession.VideoSessionState = GenericEnums.SessionState.Pending;
                            break;
                    }
                    // save the proxy to which we are sending the webcam captures
                    _model.SessionManager.AddSession(clientSession);

                    // initialize the webcamCapture form
                    // this form will be used to capture the images and send them to all Server Sessions _presenter.StopPresentation();
                    _view.ShowMyWebcamForm(true);
                    break;
                case GenericEnums.SignalType.Stop:
                    bool sendStopSignal = true;
                    if(sender.GetType().IsEquivalentTo(typeof(MViewerServer)))
                    {
                        sendStopSignal = false;
                    }
                    StopVideChat(eArgs.Identity, sendStopSignal);
                    break;
                case GenericEnums.SignalType.Pause:
                    // use the peer status of the selected chatroom
                    peers.VideoSessionState = GenericEnums.SessionState.Paused; // pause the video chat
                    _model.SessionManager.UpdateSession(identity, peers);
                    break;
                case GenericEnums.SignalType.Resume:
                    peers.VideoSessionState = GenericEnums.SessionState.Opened; // resume the video chat
                    _model.SessionManager.UpdateSession(identity, peers);
                    break;
            }
        }

        #endregion

        #region proprieties



        #endregion
    }
}
