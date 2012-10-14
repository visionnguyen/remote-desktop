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

        IView _view;
        IModel _model;

        #endregion

        #region c-tor

        public Controller()
        {
            // initialize the model

            ControllerEventHandlers handlers = new ControllerEventHandlers()
            {
                ClientConnectedHandler = this.ClientConnected,
                VideoCaptureHandler = this.ShowVideoCapture
            };

            _model = new Model(handlers);
            // initalize the view
            _view = new View(_model);


        }

        #endregion

        #region event handlers

        private void WebCamImageCaptured(object source, EventArgs e)
        {
            try
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

                // todo: send to server
                // loop on all connected contacts and send them the captures
                IDictionary<string, byte[]> receivedCaptures = _model.ClientController.SendCapture(bitmapBytes);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                //_webcamCapture.StopCapturing();
            }
        }

        #endregion

        #region public methods

        public void GetContactsStatus()
        {
            _model.PingContacts();
        }

        public void IdentityUpdated(object sender, IdentityEventArgs e)
        {
            _model.Identity.UpdateFriendlyName(e.FriendlyName);
        }

        public void StartVideoChat(WebcamCapture webcamControl, RoomActionEventArgs e)
        {
            // create Presenter and start the presentation
            int timerInterval = 100;
            int height = 354, width = 360;

            IPresenter presenter = new Presenter(webcamControl, e.Identity, timerInterval, height, width, new EventHandler(this.WebCamImageCaptured));
            _model.PresenterManager.AddPresenter(e.Identity, presenter);
            _model.PresenterManager.StartPresentation(e.Identity);

            Session serverSession = new ServerSession(e.Identity);
            serverSession.SessionState = GenericEnums.SessionState.Opened;
            _model.SessionManager.AddSession(serverSession);
            MViewerClient client = _model.ClientController.GetClient(e.Identity);
            client.InitializeRoom(e.Identity, GenericEnums.RoomActionType.Video);
        }

        public void ClientConnected(object sender, EventArgs e)
        {
            // todo: implement ClientConnected
            RoomActionEventArgs args = (RoomActionEventArgs)e;
            switch (args.ActionType)
            {
                case GenericEnums.RoomActionType.Audio:

                    break;
                case GenericEnums.RoomActionType.Video:
                    StartVideoChat(args.Identity);
                    break;
                case GenericEnums.RoomActionType.Remoting:

                    break;
                case GenericEnums.RoomActionType.Send:

                    break;
            }
        }

        public void StartVideoChat(string identity)
        {
            Session clientSession = new ClientSession(identity);
            clientSession.SessionState = GenericEnums.SessionState.Opened;
            _model.SessionManager.AddSession(clientSession);
            _model.SessionManager.UpdateSession(identity, 
                new ConnectedPeers() 
                { 
                    Video = true 
                }, 
                clientSession.SessionState);
            Thread t = new Thread(delegate()
            {
                IntPtr handle = IntPtr.Zero;
                FormVideoRoom videoRoom = new FormVideoRoom(ref handle);
                _view.RoomManager.AddRoom(identity, videoRoom);
                Contact contact = _model.GetContact(identity);
                // get friendly name from contacts list
                _view.RoomManager.SetPartnerName(identity, contact.FriendlyName);
                _view.RoomManager.ShowRoom(identity);
            }
            );
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            // initialize new video chat form
        }

        public void PerformRoomAction(object sender, RoomActionEventArgs e)
        {
            // todo: perform specific action when action event has been triggered
            switch (e.ActionType)
            {
                case GenericEnums.RoomActionType.Audio:

                    break;
                case GenericEnums.RoomActionType.Remoting:

                    break;
                case GenericEnums.RoomActionType.Send:

                    break;
                case GenericEnums.RoomActionType.Video:
                    switch (e.SignalType)
                    {
                        case GenericEnums.SignalType.Start:
                            // start the video chat
                            Thread t = new Thread(delegate()
                            {
                                StartVideoChat(sender, e);
                            });
                            t.SetApartmentState(ApartmentState.STA);
                            t.Start();
                            break;
                        case GenericEnums.SignalType.Pause:

                            break;
                    }
                    break;
            }
        }

        public Contact PerformContactsOperation(object sender, ContactsEventArgs e)
        {
            Contact contact = null;
            if (e.Operation == GenericEnums.ContactsOperation.Load)
            {
                NotifyContactsObserver();
            }
            else
            {
                contact = _model.PerformContactOperation(e);
            }
            return contact;
        }

        public void ShowVideoCapture(object sender, EventArgs e)
        {
            VideoCaptureEventArgs args = (VideoCaptureEventArgs)e;
            _view.RoomManager.ShowPicture(args.Identity, args.CapturedImage);
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

            // todo: bind client connected observer

            _model.ServerController.StartServer();

            Thread.Sleep(2000);

            // ping every single contact in the list and update it's status
            GetContactsStatus();
        }

        public void StopApplication()
        {
            // unbind the observers
            _view.BindObservers(false);

            // todo: update the StopApplication method
            _model.ServerController.StopServer();

            // exit the environment
            Environment.Exit(0);
        }

        public void NotifyContactsObserver()
        {
            //_model.ContactsUpdated();
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

        void StartVideoChat(object sender, RoomActionEventArgs e)
        {
            _model.ClientController.AddClient(e.Identity);
            _model.ClientController.StartClient(e.Identity);
            _view.ShowMyWebcamForm(e);
            //Thread.Sleep(10000);
            
        }

        #endregion

        #region proprieties



        #endregion
    }
}