using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// Nihil est perpetuum
namespace KSP_rusty
{
    //[KSPAddon(KSPAddon.Startup.Flight, false)]
    public class rustyFlight : MonoBehaviour
    {
        void Start()
        {
            try
            {
                Vessel vessel = FlightGlobals.ActiveVessel;
                if (vessel == null) return;

                foreach (Part part in vessel.parts)
                {
                    // list of modules to add as a string
                    String addModules = "";

                    // check part resources
                    foreach (PartResource resource in part.Resources)
                    {
                        // electric batteries
                        if (resource.resourceName == "ElectricCharge" &&
                            resource.maxAmount > 0.0 && resource.hideFlow == false)
                        {
                            addModules += "usageBatteries;";
                        }
                    }

                    // check part modules
                    bool addUsagePods = (part.CrewCapacity > 0);
                    foreach (PartModule module in part.Modules)
                    {
                        // add engine usage
                        if (module is ModuleEngines)
                        {
                            addModules += "usageEngines;";
                        }
                        // add gimbal usage
                        else if (module is ModuleGimbal)
                        {
                            addModules += "usageGimbal;";
                        }
                        // add crewed command pod usage
                        else if (module is ModuleCommand)
                        {
                            addModules += "usagePods;";
                            addUsagePods = false;
                        }
                        // add crewed command pod usage
                        else if (module is ModuleReactionWheel)
                        {
                            addModules += "usageReactionWheels;";
                        }
                    }

                    // add other modules based on part parameters
                    if (addUsagePods)
                    {
                        addModules += "usagePods;";
                    }

                    // add generic time usage for unknown part if no other were added
                    if (addModules.Length == 0)
                    {
                        part.AddModule("usageGeneric");
                    }
                    // add known modules
                    else
                    {
                        String[] modulesIds = addModules.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);
                        foreach (String moduleId in modulesIds)
                        {
                            part.AddModule(moduleId);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                KerbalGUIManager.print("Exception rusty.cs:Start(): " + e.Message);
            }
        }

        void FixedUpdate()
        {
            //Vessel vessel = FlightGlobals.ActiveVessel;
            //if (vessel == null) return;

            //foreach (Part part in vessel.parts)
            //{
                //part.AddModule("myPartModule");
                //print("myPartModule added to " + part.name);
            //}
        }
    }
}
