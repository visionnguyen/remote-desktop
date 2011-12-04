﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public interface IServerModel
    {
        #region proprieties

        bool IsListening { get; }
        string ChannelName { get; }
        int Port { get; }
        string Host { get; }
        int ConnectedClients { get; }
        IList<ConnectedClient> Clients {get;}

        #endregion

        #region methods

        int AddClient(string ip, string hostname);
        void RemoveClient(int id);
        void RemoveAllClients();
        void StartServer();
        void StopServer();
        void UpdateDesktop();
        void UpdateMouseCursor();
        void AddObserver(IServerView serverView);
        void RemoveObserver(IServerView serverView);
        void NotifyObservers();

        #endregion
    }
}
