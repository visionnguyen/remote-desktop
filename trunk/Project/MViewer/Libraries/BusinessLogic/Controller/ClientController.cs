﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenericDataLayer;

namespace BusinessLogicLayer
{
    public class ClientController : IClientController
    {
        #region private members

        Dictionary<string, MViewerClient> _clients;
        readonly object _syncClients = new object();

        #endregion

        #region c-tor

        public ClientController()
        {
            lock (_syncClients)
            {
                _clients = new Dictionary<string, MViewerClient>();
            }
        }

        #endregion

        #region private methods


        #endregion

        #region public methods
        
        public MViewerClient GetClient(string identity)
        {
            MViewerClient client = null;
            if (_clients != null && _clients.ContainsKey(identity))
            {
                client = _clients[identity];
            }
            return client;
        }

        public void AddClient(string identity)
        {
            lock (_syncClients)
            {
                if (_clients == null)
                {
                    _clients = new Dictionary<string, MViewerClient>();
                }
                if (_clients.ContainsKey(identity))
                {
                    _clients.Remove(identity);
                } 
                ContactEndpoint endpoint = IdentityResolver.ResolveIdentity(identity);
                MViewerClient client = ClientBuilder.BuildWCFClient(endpoint);
                _clients.Add(identity, client);
            }
        }

        public void RemoveClient(string identity)
        {
            lock (_syncClients)
            {
                if (_clients.ContainsKey(identity))
                {
                    _clients.Remove(identity);
                }
            }
        }

        public void StopClient(string identity)
        {
            lock (_syncClients)
            {
                if (_clients.ContainsKey(identity))
                {
                    _clients[identity].Close();
                }
            }
        }

        public void StartClient(string identity)
        {
            lock (_syncClients)
            {
                //if (!_clients.ContainsKey(identity))
                //{
                //    AddClient(identity);
                //} 
                MViewerClient mviewerClient = _clients[identity];
                mviewerClient.Open();
            }
        }

        public bool IsContactOnline(string identity)
        {
            ContactEndpoint endpoint = IdentityResolver.ResolveIdentity(identity);
            MViewerClient client = ClientBuilder.BuildWCFClient(endpoint);
            return client.Ping();
        }

        public Dictionary<string, byte[]> SendCapture(byte[] capture)
        {
            Dictionary<string, byte[]> receivedCaptures = new Dictionary<string, byte[]>();
            foreach (MViewerClient client in _clients.Values)
            {
                client.SendWebcamCapture(capture);
            }
            return receivedCaptures;
        }

        #endregion
    }
}
