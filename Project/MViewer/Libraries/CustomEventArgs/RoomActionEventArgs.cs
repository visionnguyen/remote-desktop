﻿using Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;

namespace GenericObjects
{
    public class RoomActionEventArgs : EventArgs
    {
        public GenericEnums.RoomType RoomType
        {
            get;
            set;
        }

        public GenericEnums.SignalType SignalType
        {
            get;
            set;
        }

        public string Identity
        {
            get;
            set;
        }

        public bool HasPermission
        {
            get;
            set;
        }

        public TransferInfoBase TransferInfo 
        { 
            get; 
            set; 
        }
    }
}
