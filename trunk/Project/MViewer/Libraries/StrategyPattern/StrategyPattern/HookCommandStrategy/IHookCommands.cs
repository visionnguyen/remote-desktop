﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using GenericDataLayer;

namespace StrategyPattern
{
    public interface IHookCommands
    {
        // todo: implement IHookCommands
        void Execute(object sender, RemotingCommandEventArgs args);
    }
}
