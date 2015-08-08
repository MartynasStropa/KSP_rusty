using System;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Contracts;


namespace KSP_rusty
{

    class EventObserver
    {
        public EventObserver()
        {
            GameEvents.onVesselRecovered.Add(this.OnVesselRecovered);
        }

        private void OnVesselRecovered(ProtoVessel vessel)
        {
        
        }
    }
}

