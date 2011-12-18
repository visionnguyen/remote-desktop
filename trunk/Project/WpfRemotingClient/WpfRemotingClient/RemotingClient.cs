﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Timers;

namespace WpfRemotingClient
{
    [Serializable]
    public class RemotingClient : Client
    {
        public RemotingClient(int timerInterval, string serverHost, ElapsedEventHandler timerTick) 
            : base(timerInterval, serverHost, timerTick) { }
    }
}
