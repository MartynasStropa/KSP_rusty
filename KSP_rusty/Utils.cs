using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace KSP_rusty
{
    public static partial class Utils
    {
        public static T DeepClone<T>(T obj)
        {
            T objResult;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                ms.Position = 0;
                objResult = (T)bf.Deserialize(ms);
            }
            return objResult;
        }

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

        public static Vessel getVesselByGuid(Guid id)
        {
            foreach (Vessel vessel in FlightGlobals.Vessels)
            {
                if (vessel.id == id)
                {
                    return vessel;
                }
            }

            return null;
        }

        public static ConfigNode getVesselConfigByGuidFromGame(Guid id)
        {
            string id_str = id.ToString().Replace("-", "");
            KerbalGUIManager.print("[rusty] getVesselConfigByGuidFromGame looking for " + id_str);

            // get FLIGHTSTATEs, where VESSELs are stored
            //HighLogic.CurrentGame.Load();
            KerbalGUIManager.print("[rusty] getVesselConfigByGuidFromGame: config nodes: " + HighLogic.CurrentGame.config.CountNodes);
            
            ConfigNode flightstates = HighLogic.CurrentGame.config.GetNode("FLIGHTSTATE");
            if (flightstates == null)
            {
                KerbalGUIManager.print("[rusty] getVesselConfigByGuidFromGame: FLIGHTSTATE is null");
                return null;
            }

            ConfigNode[] vesselNodes = flightstates.GetNodes("VESSEL");
            KerbalGUIManager.print("[rusty] getVesselConfigByGuidFromGame: got " + vesselNodes.Length + " VESSELs");

            foreach (ConfigNode vesselNode in vesselNodes)
            {
                string pid = vesselNode.GetValue("pid");
                if (pid == id_str)
                {
                    return vesselNode;
                }
                else
                {
                    KerbalGUIManager.print("[rusty] getVesselConfigByGuidFromGame: " + pid + " is not " + id_str);
                }
            }

            return null;
        }


        public static ConfigNode savePart(Part part)
        {
            ConfigNode conf = new ConfigNode("PART");

            // part = fuelTankSmallFlat_4294820046
            conf.AddValue("part", part.partInfo.name + "_" + part.GetInstanceID());
			// partName = Part
            conf.AddValue("partName", part.partName);
			// pos = -0.7752697,10.69909,3.000129
            conf.AddValue("pos", part.transform.localPosition);
            // attPos = 0,0,0
            conf.AddValue("attPos", part.attPos);
			// attPos0 = -0.7752697,10.69909,3.000129
            conf.AddValue("attPos0", part.attPos0);
            // rot = 0,0,0,1
            conf.AddValue("rot", part.transform.localRotation);
			// attRot = 0,0,0,1
            conf.AddValue("attRot", part.attRotation);
			// attRot0 = 0,0,0,1
            conf.AddValue("attRot0", part.attRotation0);
			// mir = 1,1,1
            conf.AddValue("mir", part.mirrorVector);
			// symMethod = Radial
            conf.AddValue("symMethod", part.symMethod);
			// istg = 0    wrong
            conf.AddValue("istg", part.inStageIndex);
			// dstg = 0
            conf.AddValue("dstg", part.stageOffset);
            // sidx = -1   wrong
            conf.AddValue("sidx", part.separationIndex);
            // sqor = -1
            //conf.AddValue("sqor", part.);
            
			//sepI = 0
            conf.AddValue("sepI", part.separationIndex);
			//attm = 0
            conf.AddValue("attm", part.attachMethod);
            //modCost = 0
            //conf.AddValue("modCost", part.GetModuleCosts());
			//modMass = 0
            conf.AddValue("modMass", part.GetModuleMass(0.0f));
			//modSize = (0.0, 0.0, 0.0)
            conf.AddValue("modSize", part.GetModuleSize(Vector3.zero));
			//link = fuelTankSmallFlat_4294819860
            conf.AddValue("link", part.editorLinks[0].partInfo.name);

			//attN = bottom,fuelTankSmallFlat_4294819860
            //conf.AddValue("attm", part.attachNodes[0].attachedPart.partInfo.name + "_" + part.attachNodes[0].attachedPartId);
            
            return conf;
        }

        public static void addPartToShipRecursive(ShipConstruct ship, Part part)
        {
            ship.Add(part);

            // add children parts
            foreach (Part child in part.children)
            {
                addPartToShipRecursive(ship, child);
            }
        }

        public static String Vector3ToString(Vector3 vec)
        {
            String[] replaceSymbols = {" ", "[", "]", "(", ")"};

            String ret = vec.ToString();
            for (int i = 0; i <= replaceSymbols.GetUpperBound(0); ++i) {
                ret = ret.Replace(replaceSymbols[i], "");
            }

            return ret;
        }
        public static String Vector3dToString(Vector3d vec)
        {
            String[] replaceSymbols = { " ", "[", "]", "(", ")" };

            String ret = vec.ToString();
            for (int i = 0; i <= replaceSymbols.GetUpperBound(0); ++i)
            {
                ret = ret.Replace(replaceSymbols[i], "");
            }

            return ret;
        }
        public static String QuaternionToString(Quaternion vec)
        {
			String ret = vec.x + "," + vec.y + "," + vec.z + "," + vec.w;
			return ret;
			/*
            String[] replaceSymbols = { " ", "[", "]", "(", ")" };

            String ret = vec.ToString();
            for (int i = 0; i <= replaceSymbols.GetUpperBound(0); ++i)
            {
                ret = ret.Replace(replaceSymbols[i], "");
            }

            return ret;*/
        }

		public static Vector3 StringToVector3(string str) {
			string[] split = str.Split(',');
			Vector3 ret;

			ret.x = float.Parse(split[0]);
			ret.y = float.Parse(split[1]);
			ret.z = float.Parse(split[2]);
			
			return ret;
		}

		public static Quaternion StringToQuaternion(string str)
		{
			string[] split = str.Split(',');
			Quaternion ret;

			ret.x = float.Parse(split[0]);
			ret.y = float.Parse(split[1]);
			ret.z = float.Parse(split[2]);
			ret.w = float.Parse(split[3]);

			return ret;
		}
    }
}

