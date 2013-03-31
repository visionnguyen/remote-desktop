﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using GenericObjects;

namespace StrategyPattern
{
    public interface IRoomCommands
    {
        void Execute(object sender, RoomActionEventArgs args);
        void BindCommands();
    }
}