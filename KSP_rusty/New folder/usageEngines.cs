using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KSP_rusty
{
    class usageEngines : PartModule
    {
        [KSPField(guiActive = true, guiName = "Condition (Engine)", isPersistant = true, guiFormat = "F8")]
        public float conditionEngine = 100.0f;

        private ModuleEngines module;

        public override void OnStart(StartState state)
        {
            // parent OnStart
            base.OnStart(state);

            module = Utils.getModuleByType<ModuleEngines>(base.part);
        }


        public override void OnFixedUpdate()
        {
            updateUsage(Time.deltaTime * TimeWarp.CurrentRate);
        }

        public override void OnUpdate()
        {
            updateUsage(Time.deltaTime * TimeWarp.CurrentRate);
        }


        private void updateUsage(float rate) 
        {
            if (module == null) return;
            
            // usage due to time
            float timeUsage = usageUtils.getPartUsageInTime(rate);

            // usage modifier
            float usageMod = 1.0f;

            // modify usage when firing
            // modify usage when not shutdown
            // FIX: enabled/disabled?
            if (base.isActiveAndEnabled)
            {
                usageMod += 0.5f * module.currentThrottle + 0.2f;
            }

            conditionEngine -= timeUsage * usageMod * usageUtils.GLOBAL_MOD;
        }
    }
}
