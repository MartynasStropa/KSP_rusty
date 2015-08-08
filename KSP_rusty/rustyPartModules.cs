using System.Collections.Generic;

namespace KSP_rusty
{
    class rustyPartModules
    {
        public static void addUsageModulesToParts(List<Part> parts)
        {
            foreach (Part part in parts)
            {
                addUsageModulesToPart(part);
            }
        }

        public static void addUsageModulesToPart(Part part)
        {
            // list of modules to add as a string
            string addModules = "";

            // check part resources
            foreach (PartResource resource in part.Resources)
            {
                // electric batteries (ignore empty batts, probably alternator)
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

            // add generic time usage (aging) for unknown part if no other modules were added
            if (addModules.Length == 0)
            {
				KerbalGUIManager.print("Adding usageGeneric to " + part.partInfo.name);
                part.AddModule("usageGeneric");
            }

            // add known modules
            else
            {
                string[] modulesIds = addModules.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (string moduleId in modulesIds)
                {
					KerbalGUIManager.print("Adding " + moduleId + " to " + part.partInfo.name);
                    part.AddModule(moduleId);
                }
            }
        }
    }
}
