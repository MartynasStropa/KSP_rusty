using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KSP_rusty
{
    class usageGimbal : PartModule
    {
        [KSPField(guiActive = true, guiName = "Condition (Gimbal)", isPersistant = true, guiFormat = "F8")]
        public float conditionGimbal = 100.0f;

        private ModuleGimbal module;

        public override void OnStart(StartState state)
        {
            // parent OnStart
            base.OnStart(state);

            module = Utils.getModuleByType<ModuleGimbal>(base.part);

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
			if (module == null)
			{
				module = Utils.getModuleByType<ModuleGimbal>(base.part);
				return;
			}
            
            // usage due to time
            float timeUsage = usageUtils.getPartUsageInTime(rate);

            // usage modifier
            float usageMod = 1.0f;

            // modify usage when moving
            float angle = Math.Abs(module.gimbalAnglePitch) + Math.Abs(module.gimbalAngleRoll) + Math.Abs(module.gimbalAngleYaw);
            angle = (angle / 3) / module.gimbalRange;
            usageMod += 1.0f * angle;

            conditionGimbal = timeUsage * usageMod * usageUtils.GLOBAL_MOD;
        }
    }
}
