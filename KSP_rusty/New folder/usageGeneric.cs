using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KSP_rusty
{
    class usageGeneric : PartModule
    {
        [KSPField(guiActive = true, guiName = "Condition", isPersistant = true, guiFormat = "F8")]
        public float condition = 100.0f;

        // Runs on PartModule startup.
        public override void OnStart(StartState state)
        {
            // Startup the PartModule stuff first.
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

        private void updateUsage(float rate) {
            // usage due to time
            condition -= usageUtils.getPartUsageInTime(Time.deltaTime * TimeWarp.CurrentRate) * usageUtils.GLOBAL_MOD;
        }
    }
}
