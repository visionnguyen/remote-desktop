﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenericObjects;
using Utils;

namespace StrategyPattern
{
    public class RoomCommands : RoomCommandBase
    {
        public Delegates.RoomCommandDelegate StartRemoting;
        public Delegates.RoomCommandDelegate StopRemoting;
        public Delegates.RoomCommandDelegate PauseRemoting;
        public Delegates.RoomCommandDelegate ResumeRemoting;

        public override void BindCommands()
        {
            _commands = new Dictionary<GenericEnums.SignalType, Delegates.RoomCommandDelegate>();
            _commands.Add(GenericEnums.SignalType.Start, StartRemoting);
            _commands.Add(GenericEnums.SignalType.Stop, StopRemoting);
            _commands.Add(GenericEnums.SignalType.Pause, PauseRemoting);
            _commands.Add(GenericEnums.SignalType.Resume, ResumeRemoting);
        }
    }
}
