﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Utils;

namespace GenericObjects
{
    public interface IVideoRoom : IRoom
    {
        void SetPicture(byte[] picture, DateTime timestamp);

        IntPtr FormHandle
        {
            get;
        }

        DateTime LastAudioTimestamp { get; set; }
    }
}
