using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KSP_rusty
{
    class usagePods : PartModule
    {
        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "Condition (Capsule)", isPersistant = true, guiFormat = "F8")]
        public double conditionCapsule = 100.0f;

        public override void OnStart(StartState state)
        {
            // parent OnStart
            base.OnStart(state);
        }
        public override void OnFixedUpdate()
        {
            updateUsage(Time.deltaTime * TimeWarp.CurrentRate);
        }

        public override void OnUpdate()
        {
            updateUsage(Time.deltaTime * TimeWarp.CurrentRate);
        }

        public override void OnInitialize()
        {
            // parent method
            base.OnInitialize();
        }


        private void updateUsage(float rate)
        {
            // usage due to time
            float timeUsage = usageUtils.getPartUsageInTime(rate);

            // usage modifier
            double usageMod = (float)base.part.protoModuleCrew.Count / (float)base.part.CrewCapacity;

            conditionCapsule -= (double)timeUsage * usageMod * (double)usageUtils.GLOBAL_MOD;
        }
    }
}
