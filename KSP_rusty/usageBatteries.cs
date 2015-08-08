using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KSP_rusty
{
    class usageBatteries : PartModule
    {
        [KSPField(guiActive = true, guiName = "Condition (Battery)", isPersistant = true, guiFormat = "F8")]
        public double conditionBattery = 100.0;

        [KSPField(guiActive = true, guiName = "originalMaxAmount", isPersistant = true, guiFormat = "F8")]
        private double originalMaxAmount = 0.0f;

        private PartResource resource;

        [KSPField(guiActive = true, guiName = "lastAmount", isPersistant = true, guiFormat = "F8")]
        private double lastAmount = 0.0;

        [KSPField(guiActive = true, guiName = "dAmount", isPersistant = true, guiFormat = "F8")]
        private double dAmount = 0.0;

        /*
         * DEFAULT VALUES
            active = True
			guiIcon = guiName
			category = guiName
			guiActiveUnfocused = False
			unfocusedRange = 2
			externalToEVAOnly = True
         */

        [KSPEvent(guiActive = true, guiName = "Replace Batteries")]
        public void ReplaceBatteries()
        {
            if (Funding.CanAfford(1000000.0f))
            {
                Funding.Instance.AddFunds(-1000.0f, TransactionReasons.Vessels);

                Events["ReplaceBatteries"].active = false;
                conditionBattery = 100.0;
                resource.maxAmount = originalMaxAmount;
                resource.amount = resource.maxAmount;
                lastAmount = resource.amount;
            }
            else
            {
                ScreenMessages.PostScreenMessage("Not enough funds to conduct repairs.", 3.0f, ScreenMessageStyle.UPPER_CENTER);
            }
        }

        public override void OnStart(StartState state)
        {
            // parent OnStart
            base.OnStart(state);

            // get current amount
            resource = Utils.getResourceByNameFromList(base.part.Resources, "ElectricCharge");
            if (resource != null)
            {
                lastAmount = resource.amount;
            }
            
            // get original amount
            String info = base.part.partInfo.resourceInfos[0].info.Replace("Amount: ", "");
            String[] infos = info.Split('M');
            originalMaxAmount = float.Parse(infos[0]);
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
            if (resource == null) return;
            
            // usage due to time
            float timeUsage = usageUtils.getPartUsageInTime(rate);

            // usage modifier
            float usageMod = 1.0f;

            // modify usage when charging/discharging
            // every battery can be discharged and recharged 1000 times
            // , so 2000 times the charge level can change by 100 percent
            // , so 200000 percent changes happen throughout the lifetime
            if (originalMaxAmount > 0.0)
            {
                dAmount = Math.Abs(lastAmount - resource.amount);
                double dAmountPerc = (dAmount / originalMaxAmount) * 100.0;
                double maxAmountDecrease = dAmountPerc / 200000.0;
                resource.maxAmount -= maxAmountDecrease * originalMaxAmount;

                // force lower maxAmount due to age
                double maxFullAmountDueToAge = originalMaxAmount * (conditionBattery / 100.0);

                // limit checks
                if (resource.maxAmount < 0.0) resource.maxAmount = 0.0;
                if (resource.maxAmount > maxFullAmountDueToAge) resource.maxAmount = maxFullAmountDueToAge;
                if (resource.amount > resource.maxAmount) resource.amount = resource.maxAmount;

                // update lastAmount
                lastAmount = resource.amount;
            }

            // update general battery condition
            conditionBattery -= timeUsage * usageMod * usageUtils.GLOBAL_MOD;

            // re-enable repair event once condition drops
            if (Events["ReplaceBatteries"].active == false &&
                        (resource.maxAmount / originalMaxAmount < 0.99 || conditionBattery < 99.0))
            {
                            Events["ReplaceBatteries"].active = true;
            }
             
        }
    }
}
