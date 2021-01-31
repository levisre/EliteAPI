﻿using System;

using EliteAPI.Event.Models;
using EliteAPI.Event.Models.Abstractions;

using Newtonsoft.Json;

namespace EliteAPI.Event.Models
{
    public partial class SrvDestroyedEvent : EventBase
    {
        internal SrvDestroyedEvent() { }        
    }

    public partial class SrvDestroyedEvent
    {
        public static SrvDestroyedEvent FromJson(string json)
        {
            return JsonConvert.DeserializeObject<SrvDestroyedEvent>(json);
        }
    }
}

namespace EliteAPI.Event.Handler
{
    public partial class EventHandler
    {
        public event EventHandler<SrvDestroyedEvent> SrvDestroyedEvent;

        internal void InvokeSrvDestroyedEvent(SrvDestroyedEvent arg)
        {
            SrvDestroyedEvent?.Invoke(this, arg);
        }
    }
}