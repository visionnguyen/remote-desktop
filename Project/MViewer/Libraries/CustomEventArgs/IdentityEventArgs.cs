﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericObjects
{
    public class IdentityEventArgs : EventArgs
    {
        public string FriendlyName
        {
            get;
            set;
        }

        public string MyIdentity
        {
            get;
            set;
        }
    }
}
