﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericDataLayer
{
    public class TransferStatusUptading
    {
        public bool IsVideoUpdating
        {
            get;
            set;
        }

        public bool IsAudioUpdating
        {
            get;
            set;
        }

        public bool IsRemotingUpdating
        {
            get;
            set;
        }
    }
}
