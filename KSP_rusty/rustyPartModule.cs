using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// DEPRECATED


namespace KSP_rusty
{
    class rustyPartModule : PartModule
    {
        [KSPField(guiActive = true, guiName = "Condition", isPersistant = true, guiFormat="F8")]
        public float condition = 100.0f;

        /*
        [KSPEvent(guiActive = true, guiName = "Deactivate")]
        public void Deactivate()
        {
            Events["Deactivate"].active = false;
            Events["Activate"].active = true;
        }
         * */
         

        // Runs on PartModule startup.
        public override void OnStart(StartState state)
        {
            // Startup the PartModule stuff first.
            base.OnStart(state);

            //base.part.SetHighlightColor(new Color(0.72f, 0.26f, 0.06f));
            //base.part.SetHighlightType(Part.HighlightType.AlwaysOn);

            //ConfigNode conf = new ConfigNode("Condition");
            //KSPField f = new KSPField();
            //base.part.addF
            //base.part.AddResource(conf);

            
        }

        public override void OnFixedUpdate()
        {
            float warp = TimeWarp.CurrentRate;  // remove?
            float dt = Time.deltaTime;          // remove?
            float rate = dt * warp;

            // amount of condition used up in this frame
            float frameUsage = 0.0f;

            // wiki.kerbalspaceprogram.com/wiki/Time
            // Kerbin year: 2556.50 hours
            // Kerbin year: 426.08 days
            // Kerbin year: 66.23 months
            const float YEAR = 2556.5f * 3600.0f;
            const float PART_LIFETIME = 1 * YEAR;

            // decrease condition based on:
            // time (all parts last 20 kerbal years: 184068000 seconds)
            frameUsage += (100.0f / PART_LIFETIME) * rate;

            // usage (part specific) increase a multiplier of usage
            // Command pods
            switch (base.part.partInfo.category) {
                case PartCategories.Pods:
                    frameUsage *= usagePods();
                    break;
                case PartCategories.FuelTank:
                    frameUsage *= usageFuelTanks();
                    break;
                case PartCategories.Engine:
                    frameUsage *= usageEngines();
                    break;
                case PartCategories.Control:
                    frameUsage *= usageControls();
                    break;
                case PartCategories.Structural:
                    frameUsage *= usageStructurals();
                    break;
                case PartCategories.Aero:
                    frameUsage *= usageAeros();
                    break;
                case PartCategories.Utility:
                    frameUsage *= usageUtilities();
                    break;
                case PartCategories.Science:
                    frameUsage *= usageSciences();
                    break;
                case PartCategories.Propulsion:
                    frameUsage *= usageEngines();   // What is propulsions??
                    break;
            }

            //base.part.name
            //base.part.partInfo
            //base.part.partName
            //base.part.

            // heating


            // G forces

            condition -= frameUsage;
        }


        private float usagePods()
        {
            // X usage when crewed
            // X usage when reaction wheels are used
            // usage when batteries charged/discharged
            return 0.0f;
        }
        private float usageFuelTanks()
        {
            // usage when fuel quantity changes
            return 0.0f;
        }
        private float usageEngines()
        {
            // X usage when firing
            // X usage when not shutdown
            // X usage when gimbaling

            return 1.0f;
        }
        private float usageControls()
        {
            // usage when reaction wheels are used
            // usage when firing (RCS)
            return 1.0f;
        }
        private float usageStructurals()
        {
            // usage when decoupled
            // when stressed (maybe even when weight is applied)
            return 1.0f;
        }
        private float usageAeros()
        {
            // usage when control surfaces are operated
            // usage when fairing is deployed
            // usage when intake is used (similar to engine firing)
            // usage when ablator is used
            // aero stresses will be applied to all parts anyway
            return 1.0f;
        }
        private float usageUtilities()
        {
            // usage when the CLAW has something grabbed (also increase with stress?)
            // usage when something is docked (also increase with stress?)
            // usage when drilling
            // usage when fuel cell is firing
            // usage when solar cell is receiving sun exposure (blankening of tiles)
            // usage when solar cells are operating (opening/closing)
            // usage when a light is on
            // usage when resource converter is working (based on loads)
            // usage when LES is firing
            // usage when landing legs are on a surface (static stress, dynamic stress is G)
            // usage when gear is on a surface (static stress, dynamic stress is G)
            // usage when gear is rolling (based on speed)
            // usage when gear is braking
            // usage when gear operating
            // usage when gear is steered
            // usage when parachute is deployed (based on depl. percentage)
            // usage when cargo bay is loaded (stress)
            // usage when cargo bay door is operating
            // usage when crewed components are crew (based on crew size)
            // usage when RTG is used (basically increased aging, since it's always used)
            // usage when ladders are extended/retracted
            // usage when rover wheels are on a surface (static stress, dynamic stress is G)
            // usage when rover wheels are rolling (based on speed)
            // usage when rover wheels are steered
            // usage when service does what? Heat and stress is calculated anyway
            // usage when battery is charged/discharged.
            return 1.0f;
        }
        private float usageSciences()
        {
            // usage when experiment is conducted
            // usage when antenna is tranferring
            // usage when resource scanner is active
            return 1.0f;
        }

        private void tryFailures()
        {
            // if the part is old enough, set an upcoming failure in the future
        }
    }
}
