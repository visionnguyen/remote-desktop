﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;
using System.Windows.Forms;
using BusinessLogicLayer;
using GenericDataLayer;
using StrategyPattern;
using Utils;
using WindowsInput;
using WindowsInputSimulator;

namespace MViewer
{
    public partial class Controller : IController
    {
        #region private members

        readonly object _syncRemotingCaptureSending = new object();
        ManualResetEvent _syncRemotingCaptureActivity = new ManualResetEvent(true);

        #endregion

        #region public methods

        public void KeyDown(object sender, RemotingCommandEventArgs args)
        {

        }

        public void KeyPress(object sender, RemotingCommandEventArgs args)
        {

        }

        public void KeyUp(object sender, RemotingCommandEventArgs args)
        {

        }

        public void LeftClickCommand(object sender, RemotingCommandEventArgs args)
        {
            int x = (int)Tools.Instance.RemotingUtils.ConvertXToAbsolute(args.X);
            int y = (int)Tools.Instance.RemotingUtils.ConvertYToAbsolute(args.Y);
            var input = new INPUT
            {
                type = (uint)WindowsInput.InputType.MOUSE,
                U = new InputUnion()
                {
                    mi = new MOUSEINPUT()
                    {
                        dx = x,
                        dy = y,
                        dwFlags = MOUSEEVENTF.LEFTDOWN,
                        mouseData = 0,
                        dwExtraInfo = UIntPtr.Zero,
                        time = 0
                    }
                }
            };
            var toSend = new INPUT[] { input };
            SetCursorPos(x, y);
            PInvoke.SendInput(1, toSend, Marshal.SizeOf(input));
        }
        //This is a replacement for Cursor.Position in WinForms
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public void RightClickCommand(object sender, RemotingCommandEventArgs args)
        {
            Thread t = new Thread(delegate()
            {
                int x = (int)Tools.Instance.RemotingUtils.ConvertXToAbsolute(args.X);
                int y = (int)Tools.Instance.RemotingUtils.ConvertYToAbsolute(args.Y);

                SetCursorPos((int)x, (int)y);
                // todo: perform right click

                
            });
            t.Start();
        }

        public void DoubleRightClickCommand(object sender, RemotingCommandEventArgs args)
        {

        }

        public void DoubleLeftClickCommand(object sender, RemotingCommandEventArgs args)
        {

        }

        public void MiddleClickCommand(object sender, RemotingCommandEventArgs args)
        {
            double x = Tools.Instance.RemotingUtils.ConvertXToAbsolute(args.X);
            double y = Tools.Instance.RemotingUtils.ConvertYToAbsolute(args.Y);

            var input = new INPUT
            {
                type = (uint)WindowsInput.InputType.MOUSE,
                U = new InputUnion()
                {
                    mi = new MOUSEINPUT()
                    {
                        dx = (int)x,
                        dy = (int)y,
                        dwFlags = MOUSEEVENTF.MIDDLEDOWN,
                        mouseData = 0,
                        dwExtraInfo = UIntPtr.Zero,
                        time = 0
                    }
                }
            };
            var toSend = new INPUT[] { input };
            SetCursorPos((int)x, (int)y); 
            PInvoke.SendInput(1, toSend, Marshal.SizeOf(input));
        }

        public void DoubleMiddleClickCommand(object sender, RemotingCommandEventArgs args)
        {
            
        }

        public void WheelCommand(object sender, RemotingCommandEventArgs args)
        {
            var input = new INPUT
            {
                type = (uint)WindowsInput.InputType.MOUSE,
                U = new InputUnion()
                {
                    mi = new MOUSEINPUT()
                    {
                        dx = 0,
                        dy = 0,
                        dwFlags = MOUSEEVENTF.WHEEL,
                        mouseData = 0,
                        dwExtraInfo = UIntPtr.Zero,
                        time = 0
                    }
                }
            };
            var toSend = new INPUT[] { input };
            PInvoke.SendInput(1, toSend, Marshal.SizeOf(input));
        }

        public void MoveCommand(object sender, RemotingCommandEventArgs args)
        {
            Thread t = new Thread(delegate()
            {
                int x = (int)Tools.Instance.RemotingUtils.ConvertXToAbsolute(args.X);
                int y = (int)Tools.Instance.RemotingUtils.ConvertYToAbsolute(args.Y);

                var input = new INPUT
                {
                    type = (uint)WindowsInput.InputType.MOUSE,
                    U = new InputUnion()
                    {
                        mi = new MOUSEINPUT()
                        {
                            dx = x,
                            dy = y,
                            dwFlags = MOUSEEVENTF.MOVE,
                            mouseData = 0,
                            dwExtraInfo = UIntPtr.Zero,
                            time = 0
                        }
                    }
                };
                var toSend = new INPUT[] { input };
                PInvoke.SendInput(1, toSend, Marshal.SizeOf(input));
            });
            t.Start();
        }

        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const int MOUSEEVENTF_MIDDLEUP = 0x0040;

        public void RightMouseDownCommand(object sender, RemotingCommandEventArgs args)
        {
            Thread t = new Thread(delegate()
            {
                int x = (int)Tools.Instance.RemotingUtils.ConvertXToAbsolute(args.X);
                int y = (int)Tools.Instance.RemotingUtils.ConvertYToAbsolute(args.Y);

                SetCursorPos((int)x, (int)y);
                mouse_event(MOUSEEVENTF_RIGHTDOWN, x, y, 0, 0);

            });
            t.Start();
        }

        public void RightMouseUpCommand(object sender, RemotingCommandEventArgs args)
        {
            Thread t = new Thread(delegate()
            {
                int x = (int)Tools.Instance.RemotingUtils.ConvertXToAbsolute(args.X);
                int y = (int)Tools.Instance.RemotingUtils.ConvertYToAbsolute(args.Y);
                SetCursorPos((int)x, (int)y);
                mouse_event(MOUSEEVENTF_RIGHTUP, x, y, 0, 0);
            });
            t.Start();
        }

        public void MiddleMouseDownCommand(object sender, RemotingCommandEventArgs args)
        {
            Thread t = new Thread(delegate()
            {
                int x = (int)Tools.Instance.RemotingUtils.ConvertXToAbsolute(args.X);
                int y = (int)Tools.Instance.RemotingUtils.ConvertYToAbsolute(args.Y);

                SetCursorPos((int)x, (int)y);
                mouse_event(MOUSEEVENTF_MIDDLEDOWN, x, y, 0, 0);

            });
            t.Start();
        }

        public void MiddleMouseUpCommand(object sender, RemotingCommandEventArgs args)
        {
            Thread t = new Thread(delegate()
            {
                int x = (int)Tools.Instance.RemotingUtils.ConvertXToAbsolute(args.X);
                int y = (int)Tools.Instance.RemotingUtils.ConvertYToAbsolute(args.Y);
                SetCursorPos((int)x, (int)y);
                mouse_event(MOUSEEVENTF_MIDDLEUP, x, y, 0, 0);
            });
            t.Start();
        }

        public void LeftMouseDownCommand(object sender, RemotingCommandEventArgs args)
        {
            Thread t = new Thread(delegate()
               {
                   int x = (int)Tools.Instance.RemotingUtils.ConvertXToAbsolute(args.X);
                   int y = (int)Tools.Instance.RemotingUtils.ConvertYToAbsolute(args.Y);
                   
                   SetCursorPos((int)x, (int)y);
                   mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);

               });
            t.Start();
        }

        public void LeftMouseUpCommand(object sender, RemotingCommandEventArgs args)
        {
            Thread t = new Thread(delegate()
               {
                   int x = (int)Tools.Instance.RemotingUtils.ConvertXToAbsolute(args.X);
                   int y = (int)Tools.Instance.RemotingUtils.ConvertYToAbsolute(args.Y);
                   SetCursorPos((int)x, (int)y);
                   mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, 0);
               });
            t.Start();
        }

        public void ExecuteRemotingCommand(object sender, EventArgs e)
        {
            RemotingCommandEventArgs args = (RemotingCommandEventArgs)e;
            _commandInvoker.PerformCommand(sender, args);
        }

        public void SendRemotingCommand(object sender, RemotingCommandEventArgs args)
        {
            _model.ClientController.SendRemotingCommand(_view.RoomManager.ActiveRoom, args);
        }

        public void StartRemotingChat(object sender, RoomActionEventArgs args)
        {
            _syncRemotingCaptureActivity.Reset();

            // I am going to send my captures by using the below client
            _model.ClientController.AddClient(args.Identity);
            _model.ClientController.StartClient(args.Identity);

            // create client session
            Session clientSession = new ClientSession(args.Identity, args.RoomType);
            // save the proxy to which we are sending the remoting captures
            _model.SessionManager.AddSession(clientSession);

            // initialize the remoting tool and start it's timer
            PresenterManager.Instance(SystemConfiguration.Instance.PresenterSettings).StartRemotingPresentation();

            _syncRemotingCaptureActivity.Set();
        }

        public void StopRemotingChat(object sender, RoomActionEventArgs args)
        {
            _syncRemotingCaptureActivity.Reset();

            // todo: wait untill the peding capture is being sent to the partner
            TransferStatusUptading transfer = _model.SessionManager.GetTransferActivity(args.Identity);
            transfer.IsRemotingUpdating = true;

            // check if the screen capture is pending for being sent
            PendingTransfer transferStatus = _model.SessionManager.GetTransferStatus(args.Identity);
            while (transferStatus.Remoting)
            {
                // wait for it to finish and block the next sending
                Thread.Sleep(200);
            }

            if (!sender.GetType().IsInstanceOfType(typeof(MViewerServer)))
            {
                // send the stop command to the partner
                _model.ClientController.SendRoomCommand(MyIdentity(), args.Identity, args.RoomType, args.SignalType);
            }

            PeerStates peers = _model.SessionManager.GetPeerStatus(args.Identity);
            peers.RemotingSessionState = GenericEnums.SessionState.Closed;
            _model.SessionManager.RemoveSession(args.Identity);

            _model.RemoveClient(args.Identity);

            _view.RoomManager.CloseRoom(args.Identity);
            _view.RoomManager.RemoveRoom(args.Identity);

            if (!_model.SessionManager.RemotingRoomsLeft())
            {
                // check if any remoting session is still active
                PresenterManager.Instance(SystemConfiguration.Instance.PresenterSettings).StopRemotingPresentation();
            }

            // unblock the capture sending
            transfer.IsRemotingUpdating = false;

            _syncRemotingCaptureActivity.Set();
        }

        public void ResumeRemotingChat(object sender, RoomActionEventArgs args)
        {
            PeerStates peers = _model.SessionManager.GetPeerStatus(args.Identity);
            peers.RemotingSessionState = GenericEnums.SessionState.Opened; // resume the remoting

        }

        public void PauseRemotingChat(object sender, RoomActionEventArgs args)
        {
            // use the peer status of the selected chatroom
            PeerStates peers = _model.SessionManager.GetPeerStatus(args.Identity);
            peers.RemotingSessionState = GenericEnums.SessionState.Paused; // pause the remoting

        }

        public void RemotingImageCaptured(object source, EventArgs e)
        {
            // binded RemotingImageCaptured to the remoting capture object
            try
            {
                _syncRemotingCaptureActivity.WaitOne(); // wait for any room action to end

                if (PresenterManager.Instance(SystemConfiguration.Instance.PresenterSettings).RemotingCaptureClosed() == false)
                {
                    lock (_syncRemotingCaptureSending)
                    {
                        // broadcast the webcaptures to all connected peers
                        RemotingCaptureEventArgs args = (RemotingCaptureEventArgs)e;
                        IList<string> connectedSessions = _model.SessionManager.GetConnectedSessions(GenericEnums.RoomType.Remoting);
                        if (connectedSessions.Count == 0)
                        {
                            PresenterManager.Instance(SystemConfiguration.Instance.PresenterSettings).StopRemotingPresentation();
                        }
                        else
                        {
                            foreach (string receiverIdentity in connectedSessions)
                            {
                                TransferStatusUptading transfer = _model.SessionManager.GetTransferActivity(receiverIdentity);
                                while (transfer.IsRemotingUpdating)
                                {
                                    Thread.Sleep(200);
                                }

                                PendingTransfer transferStatus = _model.SessionManager.GetTransferStatus(receiverIdentity);
                                // check if the stop signal has been sent from the UI

                                // check if the stop signal has been sent by the partner

                                PeerStates peers = _model.SessionManager.GetPeerStatus(receiverIdentity);
                                if (peers.RemotingSessionState == GenericEnums.SessionState.Opened
                                    || peers.RemotingSessionState == GenericEnums.SessionState.Pending)
                                {
                                    // send the capture if the session isn't paused
                                    transferStatus.Remoting = true;

                                    _model.ClientController.SendRemotingCapture(args.ScreenCapture,
                                        args.MouseCapture, receiverIdentity,
                                        _model.Identity.MyIdentity);
                                }

                                transferStatus.Remoting = false;
                            }
                        }
                    }
                }
                else
                {
                    PresenterManager.Instance(SystemConfiguration.Instance.PresenterSettings).StopRemotingPresentation();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion

        #region private methods

        void OpenRemotingForm(string identity)
        {
            _syncRemotingCaptureActivity.Reset();

            if (!_view.IsRoomActivated(identity, GenericEnums.RoomType.Remoting))
            {
                Thread t = new Thread(delegate()
                {
                    //IntPtr handle = IntPtr.Zero;
                    FormRemotingRoom remotingRoom = new FormRemotingRoom(identity);
                    _view.RoomManager.AddRoom(identity, remotingRoom);
                    remotingRoom.BindCommandHandlers(SystemConfiguration.Instance.RemotingCommand);
                    // initialize new remoting chat form

                    Contact contact = _model.GetContact(identity);
                    // get friendly name from contacts list
                    _view.RoomManager.SetPartnerName(identity, contact.FriendlyName);

                    // I am going to send my captures by using the below client

                    // create session
                    Session clientSession = new ClientSession(identity, GenericEnums.RoomType.Remoting);
                    _model.SessionManager.AddSession(clientSession);

                    PeerStates peers = _model.SessionManager.GetPeerStatus(identity);
                    peers.RemotingSessionState = GenericEnums.SessionState.Opened;

                    // finally, show the video chat form where we'll see the webcam captures
                    _view.RoomManager.ShowRoom(identity);
                }
                );
                t.IsBackground = true;
                t.SetApartmentState(ApartmentState.STA);
                t.Start();

                Thread.Sleep(500);
                _syncRemotingCaptureActivity.Set();

            }
        }

        private void RemotingCaptureObserver(object sender, EventArgs e)
        {
            RemotingCaptureEventArgs args = (RemotingCaptureEventArgs)e;
            PeerStates peer = _model.SessionManager.GetPeerStatus(args.Identity);

            if (peer.RemotingSessionState == GenericEnums.SessionState.Undefined ||
                peer.RemotingSessionState == GenericEnums.SessionState.Pending)
            {
                // receiving captures for the first time, have to initalize a form
                OpenRemotingForm(args.Identity);
                while (peer.RemotingSessionState != GenericEnums.SessionState.Opened)
                {
                    Thread.Sleep(2000);
                    peer = _model.SessionManager.GetPeerStatus(args.Identity); // update the peer status
                }
            }

            // check the videochat status before displaying the picture
            if (peer.RemotingSessionState == GenericEnums.SessionState.Opened)
            {
                //display the remoting capture in the opened form
                _view.RoomManager.ShowRemotingCapture(args.Identity, args.ScreenCapture, args.MouseCapture);
            }
        }

        #endregion
    }
}