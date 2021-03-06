﻿#region License

/*
 
Copyright (c) 2010-2014 Danko Kozar

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 
*/

#endregion License

using System.Collections.Generic;
using eDriven.Core.Events;
using UnityEngine;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Networking.Rpc
{
    public class HttpServiceProgressAggregator : EventDispatcher
    {
#if DEBUG
        public new static bool DebugMode;
#endif

        public static List<HttpConnector> RegisteredServices = new List<HttpConnector>();

        private readonly Dictionary<HttpConnector, HttpServiceProgressEvent> _dict = new Dictionary<HttpConnector, HttpServiceProgressEvent>();

        internal void RegisterService(HttpConnector connector)
        {
            RegisteredServices.Add(connector);
            _dict.Add(connector, null);

            connector.AddEventListener(HttpServiceProgressEvent.PROGRESS_CHANGE, OnChange);
        }

        private void OnChange(Event e)
        {
            HttpServiceProgressEvent inEvent = (HttpServiceProgressEvent) e;

            _dict[(HttpConnector) e.Target] = inEvent;

            HttpServiceProgressEvent pe = new HttpServiceProgressEvent(HttpServiceProgressEvent.PROGRESS_CHANGE);
            
            foreach (HttpServiceProgressEvent progressEvent in _dict.Values)
            {
                if (null == progressEvent)
                    continue;
                
                pe.Total += progressEvent.Total;
                pe.Active += progressEvent.Active;
                pe.Finished += progressEvent.Finished;
            }

#if DEBUG
            if (DebugMode)
                Debug.Log("HttpServiceProgressAggregator: " + pe);
#endif
            DispatchEvent(pe);
        }
    }
}