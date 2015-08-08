using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;



// DOESN'T UPDATE for some pods from Utilities category, maybe others too.
// Adding a manual update to rusty class would be a solution.
namespace KSP_rusty
{
    class usagePods : PartModule
    {
        [KSPField(guiActive = true, guiName = "Condition (Capsule)", isPersistant = true, guiFormat = "F8")]
        public double conditionCapsule = 100.0f;

        private ModuleCommand module;

        public override void OnStart(StartState state)
        {
            // parent OnStart
            base.OnStart(state);

            module = Utils.getModuleByType<ModuleCommand>(base.part);
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
            if (module == null) return;
            
            // usage due to time
            float timeUsage = usageUtils.getPartUsageInTime(rate);

            // usage modifier
            double usageMod = 1.0;
            
            int crew = base.part.protoModuleCrew.Count;
            // modify usage when crewed
            //if (module.)
            //{
            //    usageMod += 0.5f * module.currentThrottle + 0.2f;
            //}

            conditionCapsule -= (double)timeUsage * usageMod * (double)usageUtils.GLOBAL_MOD;
            //conditionCapsule = (float)crew / (float)base.part.CrewCapacity;//timeUsage * usageMod * usageUtils.GLOBAL_MOD;
        }
    }
}
