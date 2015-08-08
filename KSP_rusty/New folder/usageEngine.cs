using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KSP_rusty
{
    class usageEngine : ModuleEngines, rustyPartModule
    {
        [KSPField(guiActive = true, guiName = "Condition (Engine)", isPersistant = true, guiFormat = "F8")]
        public float conditionEngine = 100.0f;

        public override void OnFixedUpdate()
        {
            float usageMod = 1.0f;

            // usage when firing
            // usage when not shutdown
            if (base.isActiveAndEnabled)
            {
                usageMod += 2.0f * base.currentThrottle + 0.2f;
            }
        }
    }
}
