﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;

namespace GenericDataLayer
{
    public interface ISessionManager
    {
        void AddSession(Session session);
        void RemoveSession(string identity);
        PeerStates GetPeerStatus(string identity);

        bool RemotingRoomsLeft();
        IList<string> GetConnectedSessions(GenericEnums.RoomType actionType);
        GenericEnums.SessionState GetSessionState(string identity, GenericEnums.RoomType roomType);
        PendingTransfer GetTransferStatus(string identity);
        TransferStatusUptading GetTransferActivity(string identity);
    }
}
