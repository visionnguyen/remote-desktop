﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using GenericDataLayer;
using Utils;

namespace StrategyPattern
{
    public class VideoCommands : RoomCommandBase
    {
        public Delegates.RoomCommandDelegate StartVideo;
        public Delegates.RoomCommandDelegate StopVide;
        public Delegates.RoomCommandDelegate PauseVideo;
        public Delegates.RoomCommandDelegate ResumeVideo;


        // todo: move these to audio class
        public Delegates.RoomCommandDelegate StartAudio;
        public Delegates.RoomCommandDelegate StopAudio;


        public override void BindCommands()
        {
            _commands = new Dictionary<GenericEnums.SignalType, Delegates.RoomCommandDelegate>();
            _commands.Add(GenericEnums.SignalType.Start, StartVideo);
            _commands.Add(GenericEnums.SignalType.Stop, StopVide);
            _commands.Add(GenericEnums.SignalType.Pause, PauseVideo);
            _commands.Add(GenericEnums.SignalType.Resume, ResumeVideo);
        }
    }
}
