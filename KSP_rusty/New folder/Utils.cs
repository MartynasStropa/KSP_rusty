using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KSP_rusty
{
    public static partial class Utils
    {
        public static T getModuleByType<T>(this Part part) where T : PartModule
        {
            if (part == null)
            {
                return null;
            }

            foreach (PartModule module in part.Modules)
            {
                if (module is T)
                {
                    return module as T;
                }
            }

            return null;
        }

        public static PartResource getResourceByNameFromList(PartResourceList list, String name)
        {
            foreach (PartResource resource in list)
            {
                if (resource.resourceName == name)
                {
                    return resource;
                }
            }

            return null;
        }
    }
}
