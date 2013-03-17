﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericDataLayer;

namespace UIControls
{
    public interface IHookCommandInvoker
    {
        void PerformCommand(object sender, RemotingCommandEventArgs args);
    }
}
