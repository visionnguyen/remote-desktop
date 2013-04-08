﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//

namespace GenericObjects
{
    public interface IPresenter
    {
        void StartVideoPresentation();
        void StopVideoPresentation();

        void StartAudioPresentation();
        void StopAudioPresentation();

        void StartRemotingPresentation();
        void StopRemotingPresentation();

        bool RemotingCaptureClosed();

        void PlayAudioCapture(byte[] capture);

        bool AudioCaptureClosed { get; }
    }
}