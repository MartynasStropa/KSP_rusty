using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KSP_rusty
{
    class usageReactionWheels : PartModule
    {
        [KSPField(guiActive = true, guiName = "Condition (Reaction)", isPersistant = true, guiFormat = "F8")]
        public float conditionReactionWheels = 100.0f;

        private ModuleReactionWheel module;

        public override void OnStart(StartState state)
        {
            // parent OnStart
            base.OnStart(state);

            module = Utils.getModuleByType<ModuleReactionWheel>(base.part);
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

            // usage when reaction wheels are used
            // when the wheels are active in all three axis inputSum = 3,
            // so, 100% all-axis activate shortens the lifespan by half
            usageMod += (module.inputSum / 3.0f);

            // also, add 0.2f when the wheel is enabled
            if (module.wheelState == ModuleReactionWheel.WheelState.Active)
            {
                usageMod += 0.2f;
            }

            conditionReactionWheels -= timeUsage * usageMod * usageUtils.GLOBAL_MOD;
        }
    }
}
