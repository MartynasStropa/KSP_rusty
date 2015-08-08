using System.Collections.Generic;

namespace KSP_rusty
{
	static class rustyInventory
	{
		public static ShipConstruct buildConstruct(ConfigNode vessel)
		{
			ShipConstruct ship = new ShipConstruct();
			ConfigNode[] partNodes = vessel.GetNodes("PART");
			List<Part> parts = new List<Part>();
			string[] partNameByIdx = new string[partNodes.Length];
			Dictionary<string, Part> partByName = new Dictionary<string, Part>();

			int part_idx = 0;
			foreach (ConfigNode partNode in partNodes)
			{
				// get part name
				string[] str_part = partNode.GetValue("part").Split('_');
				partNameByIdx[part_idx] = partNode.GetValue("part");

				// get part from loader
				Part part = PartLoader.getPartInfoByName(str_part[0]).partPrefab;
				partByName[partNode.GetValue("part")] = part;

				// add info
				part.orgPos = Utils.StringToVector3(partNode.GetValue("pos"));
				part.attPos = Utils.StringToVector3(partNode.GetValue("attPos"));
				part.attPos0 = Utils.StringToVector3(partNode.GetValue("attPos0"));

				part.orgRot = Utils.StringToQuaternion(partNode.GetValue("rot"));
				part.attRotation = Utils.StringToQuaternion(partNode.GetValue("attRot"));
				part.attRotation0 = Utils.StringToQuaternion(partNode.GetValue("attRot0"));

				part.mirrorVector = Utils.StringToVector3(partNode.GetValue("mir"));

				string symMethod = partNode.GetValue("symMethod");
				if (symMethod == "Radial")
				{
					part.symMethod = SymmetryMethod.Radial;
				}
				else if (symMethod == "Mirror")
				{
					part.symMethod = SymmetryMethod.Mirror;
				}

				part.inverseStage = int.Parse(partNode.GetValue("istg"));
				part.defaultInverseStage = int.Parse(partNode.GetValue("istg"));
				part.stageOffset = int.Parse(partNode.GetValue("sidx"));
				//part.manualStageOffset = int.Parse(partNode.GetValue("sqor"));
				part.separationIndex = int.Parse(partNode.GetValue("sepI"));
				string attm = partNode.GetValue("attm");
				if (attm == "0")
				{
					part.attachMode = AttachModes.STACK;
				}
				else if (attm == "1")
				{
					part.attachMode = AttachModes.SRF_ATTACH;
				}
				// modCost = 0
				// modMass = 0
				// modSize = (0.0, 0.0, 0.0)

				// add to part list
				parts.Add(part);
				ship.Add(part);
				++part_idx;
			}

			// add links
			part_idx = 0;
			foreach (Part part in parts)
			{
				string[] links = partNodes[part_idx].GetValues("link");
				foreach (string link in links)
				{
					Part linkedPart = partByName[link];
					if (linkedPart != null)
					{
						part.editorLinks.Add(linkedPart);
						part.children.Add(linkedPart);

						//AttachNode attN = new AttachNode(
						//part.attachNodes.Add
						//part.AddAttachNode(partNodes[part_idx]);
					}
				}
				++part_idx;
			}

			ship.shipFacility = EditorFacility.VAB;
			return ship;
		}
	}
}
