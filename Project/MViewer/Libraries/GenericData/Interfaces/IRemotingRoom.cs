﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericDataLayer
{
    public interface IRemotingRoom : IRoom
    {
        void ShowScreenCapture(byte[] screenCapture, byte[] mouseCapture);
    }
}