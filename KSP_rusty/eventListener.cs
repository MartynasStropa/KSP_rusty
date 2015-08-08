using System;
using System.Collections.Generic;
using UnityEngine;


/* TODO: add events to relevant scenes */
namespace KSP_rusty
{
    public class eventListener
    {
        //private GameScenes lastScene = GameScenes.LOADING;

        public eventListener()
        {
            AddEvents();
        }
        ~eventListener()
        {
            RemoveEvents();
        }

        private void AddEvents()
        {
            KerbalGUIManager.print("[rusty] Adding recovery events");

            /// Triggered a vessel is recovered (ONLY the big green button at the top) from the flight scene
            /// public static EventData<Vessel>
            GameEvents.OnVesselRecoveryRequested.Add(OnVesselRecoveryRequested);

            /// onDestroy()

            /// When the recovery dialog window opens
            /// public static EventData<MissionRecoveryDialog>
            GameEvents.onGUIRecoveryDialogSpawn.Add(onGUIRecoveryDialogSpawn);

            /// Triggered in the space center or tracking station when a vessel is recovered;
            /// occurs before onVesselRecovered
            /// public static EventData<ProtoVessel, MissionRecoveryDialog, float> 
            GameEvents.onVesselRecoveryProcessing.Add(onVesselRecoveryProcessing);

            /// Triggered after a vessel has been recovered and the science data
            /// and part values have been accounted for;
            /// occurs in the space center or tracking station.
            /// Triggered after onVesselRecoveryProcessing
            /// public static EventData<ProtoVessel>
            GameEvents.onVesselRecovered.Add(onVesselRecovered);

            /// When the recovery dialog window closes
            /// public static EventData<MissionRecoveryDialog>
            GameEvents.onGUIRecoveryDialogDespawn.Add(onGUIRecoveryDialogDespawn);

            // saving
            GameEvents.onGameStateSave.Add(onGameStateSave);
            // loading
            GameEvents.onGameStateLoad.Add(onGameStateLoad);

            // flight loaded
            GameEvents.onFlightReady.Add(onFlightReady);

            // vessel loaded in editor
            //GameEvents.onEditorLoad.Add(onEditorLoad);
        }
        private void RemoveEvents()
        {
            KerbalGUIManager.print("[rusty] Removing recovery events");

            GameEvents.OnVesselRecoveryRequested.Remove(OnVesselRecoveryRequested);
            GameEvents.onVesselRecoveryProcessing.Remove(onVesselRecoveryProcessing);
            GameEvents.onVesselRecovered.Remove(onVesselRecovered);
            GameEvents.onGUIRecoveryDialogSpawn.Remove(onGUIRecoveryDialogSpawn);
            GameEvents.onGUIRecoveryDialogDespawn.Remove(onGUIRecoveryDialogDespawn);

            GameEvents.onGameStateSave.Remove(onGameStateSave);

            //GameEvents.OnFlightGlobalsReady.Remove(OnFlightGlobalsReady);
            GameEvents.onFlightReady.Remove(onFlightReady);

            //GameEvents.onEditorLoad.Remove(onEditorLoad);
        }

        public void onEditorLoad(ShipConstruct ship, CraftBrowser.LoadType type)
        {
            KerbalGUIManager.print("[rusty] onEditorLoad");
        }
        public void onFlightReady()
        {
            KerbalGUIManager.print("[rusty] onFlightReady");

            rustyPartModules.addUsageModulesToParts(FlightGlobals.ActiveVessel.parts);
        }
        /*public void OnFlightGlobalsReady(bool state)
        {
            KerbalGUIManager.print("[rusty] OnFlightGlobalsReady");
        }*/

        public void onGameStateSave(ConfigNode config)
        {
            KerbalGUIManager.print("[rusty] Saving...");

            ConfigNode invNode = config.AddNode("INVENTORY");

            if (KSP_rusty.Inventory != null)
            {
                foreach (ConfigNode conf in KSP_rusty.Inventory) {
                    invNode.AddNode("VESSEL", conf);
                }
            }
        }
        public void onGameStateLoad(ConfigNode config)
        {
            KerbalGUIManager.print("[rusty] Loading...");

            ConfigNode invNode = config.GetNode("INVENTORY");
            ConfigNode[] vesselNodes = invNode.GetNodes("VESSEL");
            KSP_rusty.Inventory = new List<ConfigNode>();

            foreach (ConfigNode vesselNode in vesselNodes)
            {
                KSP_rusty.Inventory.Add(vesselNode);
            }
        }

        ConfigNode saveVesselLater = null;
        public void OnVesselRecoveryRequested(Vessel v)
        {
            //Get total ship cost
            KerbalGUIManager.print("[rusty] OnVesselRecoveryRequested");
            KerbalGUIManager.print("[rusty] parts.Count: " + v.parts.Count);
            KerbalGUIManager.print("[rusty] Parts.Count: " + v.Parts.Count);
            KerbalGUIManager.print("[rusty] GetActiveParts().Count: " + v.GetActiveParts().Count);
            if (v.rootPart != null)
            {
                KerbalGUIManager.print("[rusty] Root part is alive");
            }
            else
            {
                KerbalGUIManager.print("[rusty] Root part is NULL");
            }


            ShipConstruct ship = new ShipConstruct(v.vesselName, "", v.rootPart);
            ship.shipFacility = EditorFacility.VAB;
            saveVesselLater = ship.SaveShip();
        }
        // recoveryFraction - percentage of funds added after recovery, based on distance from KSC
        public void onVesselRecoveryProcessing(ProtoVessel v, MissionRecoveryDialog dialog, float recoveryFraction)
        {
			KerbalGUIManager.print("[rusty] onVesselRecoveryProcessing");

            if (saveVesselLater != null)
            {
                //KSP_rusty.Inventory.Add(saveVesselLater);
                saveVesselLater = null;
                //return;
            }


			Vessel recoveredVessel = null;
			try {
				recoveredVessel = Utils.getVesselByGuid(v.vesselID);
			}
			catch (Exception e)
			{
				KerbalGUIManager.print("[rusty] Exception on trying to find vessel" + e.Message);
			}

            if (recoveredVessel == null)
            {
                KerbalGUIManager.print("[rusty] onVesselRecoveryProcessing: vessel not found..");

				// try by guid from game
                return;
            }
            
            // deduct recovered funds
			try
			{
				if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
				{
					Funding.Instance.AddFunds(-dialog.fundsEarned, TransactionReasons.VesselRecovery);
				}
			}
			catch (Exception e)
			{
				KerbalGUIManager.print("[rusty] Exception on deducting funds " + e.Message);
			}
            // calculate distance
            // longest possible ground distance on Kerbin [km]
            //const double HALF_CIRCLE = 2 * Mathf.PI * 600 / 2;
            // distance from KSC [m]
			double distance = 0.0;
			try {
				distance = SpaceCenter.Instance.GreatCircleDistance(SpaceCenter.Instance.cb.GetRelSurfaceNVector(v.latitude, v.longitude));
			}
			catch (Exception e)
			{
				KerbalGUIManager.print("[rusty] Exception on calculating distance" + e.Message);
			}
            // KSC is supposedly 25 m^2 in size, so no more than 5 km can be considered free
            if (distance <= 5000.0)
            {
                distance = 0.0f;
            }
			KerbalGUIManager.print("[rusty] distance calculated: " + distance);


            // calculate recovery price
            // price of 1 metric tone per 1km
            const double recoveryRate = 100.0;
            double recoveryPrice = 1000.0 + (double)recoveredVessel.GetTotalMass() * (distance / 1000.0) * recoveryRate;

			KerbalGUIManager.print("[rusty] recoveryPrice calculated: " + recoveryPrice);

            // reset funds
            dialog.fundsEarned = 0.0;            

            // show recovery GUI
            KSP_rusty.wndRecovery.vesselMass = recoveredVessel.GetTotalMass();
            KSP_rusty.wndRecovery.vesselTitle = recoveredVessel.vesselName;
            KSP_rusty.wndRecovery.recoveryPrice = recoveryPrice;
            KSP_rusty.wndRecovery.recoveryDistance = Math.Ceiling(distance / 1000.0);
            KSP_rusty.wndRecovery.show();

			KerbalGUIManager.print("[rusty] wndRecovery shown");

            /// RECOVER VESSEL
            KerbalGUIManager.print("[rusty] Trying to recover. Guid: " + v.vesselID.ToString().Replace("-", ""));
            ConfigNode recoveredVesselConfigNode = Utils.getVesselConfigByGuidFromGame(v.vesselID);
            if (recoveredVesselConfigNode == null)
            {
                KerbalGUIManager.print("[rusty] recoveredVesselConfigNode is empty");
            }

            // adjust data
            /// ADJUST VESSEL NODE
            // get values
            string name = recoveredVesselConfigNode.GetValue("name");
            if (name == null) name = "";
            // set values
            recoveredVesselConfigNode.SetValue("type", "VAB");
            recoveredVesselConfigNode.SetValue("ship", name, true);
            recoveredVesselConfigNode.SetValue("description", "", true);
            // remove values
            recoveredVesselConfigNode.RemoveNode("ORBIT");
            recoveredVesselConfigNode.RemoveNode("ACTIONGROUPS");
            recoveredVesselConfigNode.RemoveNode("DISCOVERY");
            recoveredVesselConfigNode.RemoveNode("FLIGHTPLAN");
            recoveredVesselConfigNode.RemoveNode("CTRLSTATE");
            recoveredVesselConfigNode.RemoveValues(new string[] {"pid", "name", "sit", "landed", "landedAt",
                    "splashed", "met", "lct", "lat", "lon", "alt", "hgt", "nrm", "rot", "CoM", "stg", "prst",
                    "ref", "ctrl", "cPch", "cHdg", "cMod", "root"});

            // cycle proto part snapshots
            // gather needed information and internal values
            KerbalGUIManager.print("[rusty] We have " + v.protoPartSnapshots.Count + " part snapshots");
            ConfigNode[] configParts = recoveredVesselConfigNode.GetNodes("PART");
            KerbalGUIManager.print("[rusty] And we have " + (configParts.GetUpperBound(0)+1) + " parts");

            int pid = 0;
            Vector3 rootPos = Vector3.zero;

            recoveredVesselConfigNode.Save("D:/rusty_from_flight_state.cfg");
            foreach (ProtoPartSnapshot snapshot in v.protoPartSnapshots)
            {
                KerbalGUIManager.print("[rusty] SNAP " + snapshot.partName + "_" + snapshot.craftID);
                if (configParts[pid] == null)
                {
                    KerbalGUIManager.print("[rusty] Mathing not found. id: " + pid);
                    ++pid;
                    continue;
                }
                KerbalGUIManager.print("[rusty] PART " + configParts[pid].GetValue("name") + "_" + configParts[pid].GetValue("cid"));
                configParts[pid].SetValue("pos", configParts[pid].GetValue("position"));

                // get values
                string name_Part = configParts[pid].GetValue("name");
				string name_partName = "Part";
				if (name_Part == "strutConnector" || name_Part == "fuelLine")
				{
					name_partName = "CompoundPart";
				}
                string cid = configParts[pid].GetValue("cid");
                if (cid != null) name_Part = name_Part + "_" + cid;

                // remove some values
                //configParts[pid].RemoveValues(new string[] { "launchID", "parent", "position", "rotation", "mirror", "symMethod", "cid", "uid", "mid", "name" });

                // get staging (probably) values
                int istg = int.Parse(configParts[pid].GetValue("istg"));
                int dstg = int.Parse(configParts[pid].GetValue("dstg"));
                int sqor = int.Parse(configParts[pid].GetValue("sqor"));
                int sepI = int.Parse(configParts[pid].GetValue("sepI"));
                int sidx = int.Parse(configParts[pid].GetValue("sidx"));
                int attm = int.Parse(configParts[pid].GetValue("attm"));

				// something else
				int sym = 0;
				try
				{
					sym = int.Parse(configParts[pid].GetValue("sym"));
				}
				catch (Exception e)
				{
					sym = 0;
				}

                // attachments
                string[] attcfg = configParts[pid].GetValues("attN");
				string[] srfAttcfg = configParts[pid].GetValues("srfN");

                // remove some more values
                /*configParts[pid].RemoveValues(new string[] { "istg", "dstg", "sqor", "sepI", "sidx", "attm", "srfN", "attN", "mass", "temp", "expt", "state" ,
                                                             "connected" , "attached" , "flag" , "rTrf", "modCost", "crew" });
                */

                // poof!
                configParts[pid].ClearValues();

                // move the part, so that when it appears, appears not on the ground of VAB/SPH
                snapshot.position += new Vector3d(1.0, 14.0, 0.0);

                // root position to offset all other parts
                if (pid == 0)
                {
                    rootPos = snapshot.position;
                }

                // set values
                configParts[pid].SetValue("part", name_Part, true);
				configParts[pid].SetValue("partName", name_partName, true);

                // how to get attPos?
                // Should be ok for now to give it's value to attPos0 and keep attPos a zero
                Vector3d attPos = Vector3d.zero;        
                Vector3d attPos0 = snapshot.position - rootPos; // - attPos

                // basic part position in assembly
                configParts[pid].SetValue("pos", Utils.Vector3dToString(snapshot.position), true);
                // moved offset by offset gizmo
                configParts[pid].SetValue("attPos", "0,0,0", true);
                // this.attPos0 = this.pos - root.pos;
                configParts[pid].SetValue("attPos0", Utils.Vector3dToString(attPos0), true);

                configParts[pid].SetValue("rot", Utils.QuaternionToString(snapshot.rotation), true);
                configParts[pid].SetValue("attRot", Utils.QuaternionToString(snapshot.rotation), true);    // not sure
                configParts[pid].SetValue("attRot0", "0,0,0,1", true);                      // not sure

                configParts[pid].SetValue("mir", Utils.Vector3ToString(snapshot.mirror), true);
                configParts[pid].SetValue("symMethod", ""+snapshot.symMethod, true);

                // add some previuosly removed values
                configParts[pid].SetValue("istg", istg.ToString(), true);
                configParts[pid].SetValue("dstg", dstg.ToString(), true);
                configParts[pid].SetValue("sqor", sqor.ToString(), true);
                configParts[pid].SetValue("sepI", sepI.ToString(), true);
                configParts[pid].SetValue("sidx", sidx.ToString(), true);
                configParts[pid].SetValue("attm", attm.ToString(), true);

				configParts[pid].SetValue("modCost", "0", true);
				configParts[pid].SetValue("modMass", "0", true);
				configParts[pid].SetValue("modSize", "(0.0, 0.0, 0.0)", true);

                // add attachments
				KerbalGUIManager.print("[rusty] adding attN attachments");
                foreach (string attN in attcfg)
                {
                    // find if attachment is not negative (unused)
                    string tmp = attN.Replace(" ", "");
                    string[] attN_spl = tmp.Split(',');
                    
                    int attachId = int.Parse(attN_spl[1]);
                    if (attachId == -1) continue;

                    // get the attachment position: bottom, top, etc..
                    tmp = attN_spl[0];    // we will use this tmp var to add value to confignode
                    // generate the unique part identifier
                    if (attachId > configParts.GetUpperBound(0)) return;

                    string partId = "";
                    // if the target parts wasn't yet iterated, use original values
                    if (attachId > pid) {
                        partId = configParts[attachId].GetValue("name") + "_";
                        partId += configParts[attachId].GetValue("cid");
                    // of the target target was already iterated, use the new value
                    } else {
                        partId = configParts[attachId].GetValue("part");
                    }
                    // add to this configNode
                    configParts[pid].AddValue("attN", tmp + "," + partId);
                }

				// add sym
				KerbalGUIManager.print("[rusty] Got sym: " + sym);
				if (sym > 0 && sym <= configParts.GetUpperBound(0))
				{
					string partId = "";
					// if the target parts wasn't yet iterated, use original values
					if (sym > pid)
					{
						partId = configParts[sym].GetValue("name") + "_";
						partId += configParts[sym].GetValue("cid");
						// of the target target was already iterated, use the new value
					}
					else
					{
						partId = configParts[sym].GetValue("part");
					}
					// add to this configNode
					KerbalGUIManager.print("[rusty] Adding sym: " + sym);
					configParts[pid].AddValue("sym", partId);
				}

				// add surface attachments
				KerbalGUIManager.print("[rusty] adding srfN attachments");
				foreach (string srfN in srfAttcfg)
				{
					// find if attachment is not negative (unused)
					string tmp = srfN.Replace(" ", "");
					string[] srfN_spl = tmp.Split(',');

					int attachId = int.Parse(srfN_spl[1]);
					if (attachId == -1) continue;

					// get the attachment position: bottom, top, etc..
					tmp = srfN_spl[0];    // we will use this tmp var to add value to confignode
					// generate the unique part identifier
					if (attachId > configParts.GetUpperBound(0)) return;

					string partId = "";
					// if the target parts wasn't yet iterated, use original values
					if (attachId > pid)
					{
						partId = configParts[attachId].GetValue("name") + "_";
						partId += configParts[attachId].GetValue("cid");
						// of the target target was already iterated, use the new value
					}
					else
					{
						partId = configParts[attachId].GetValue("part");
					}
					// add to this configNode
					configParts[pid].AddValue("srfN", tmp + "," + partId);
				}


                // tell your daddy 'hello'
                if (configParts[snapshot.parentIdx] != null && pid != snapshot.parentIdx)
                {
                    KerbalGUIManager.print("[rusty] adding link");
                    configParts[snapshot.parentIdx].AddValue("link", snapshot.partName + "_" + snapshot.craftID);
                }

                // add sym links. nope.. "sym" is just "sym" in config.
				/*
                if (snapshot.symLinks != null)
                {
                    KerbalGUIManager.print("[rusty] adding symlinks");
                    foreach (ProtoPartSnapshot child in snapshot.symLinks)
                    {
                        configParts[pid].AddValue("sym", child.partName + "_" + child.craftID);
                    }
                }
				 * */

                ++pid;
            }

			recoveredVesselConfigNode.Save("D:/rusty_after_flight_state.cfg");

            KSP_rusty.Inventory.Add(recoveredVesselConfigNode);
            KerbalGUIManager.print("[rusty] recovered ship added to inventory");

            recoveredVessel = null;
            return;
            /*

            if (recoveredVesselConfigNode == null)
            {
                KerbalGUIManager.print("[rusty] recoveredVesselConfigNode is empty");
            }
            else
            {
                // ADJUST PART NODES
                ConfigNode[] partNodes = recoveredVesselConfigNode.GetNodes("PART");
                if (partNodes != null)
                {
                    KerbalGUIManager.print("[rusty] adjusting parts");
                    foreach (ConfigNode partNode in partNodes)
                    {
                        // get values
                        string name_Part = partNode.GetValue("name");
                        string cid = partNode.GetValue("cid");
                        if (cid != null) name_Part = name_Part + "_" + cid;
                        string mir = partNode.GetValue("mirror");
                        string position = partNode.GetValue("position");

                        // set values
                        partNode.SetValue("part", name_Part, true);
                        partNode.SetValue("partName", "Part", true);
                        partNode.SetValue("mir", mir, true);
                        partNode.SetValue("pos", position, true);

                        partNode.SetValue("attPos0", position, true);

                        // remove values
                        recoveredVesselConfigNode.RemoveValues(new string[] {"name", "cid", "uid", "mid"});
                    }
                }

                KSP_rusty.Inventory.Add(recoveredVesselConfigNode);
                KerbalGUIManager.print("[rusty] recovered ship added to inventory");
            }
           
            recoveredVessel = null;*/
        }



        public void onVesselRecovered(ProtoVessel pv)
        {
            KerbalGUIManager.print("[rusty] onVesselRecovered");
            
            Vessel v = pv.vesselRef;

            try
            {
               // pv.vesselRef.Load();
            }
            catch (Exception e)
            {
                KerbalGUIManager.print("[rusty] onVesselRecovered: Exception: " + e.Message + "\n\n" + e.StackTrace);
            }

            //Get total ship cost
            KerbalGUIManager.print("[rusty] parts.Count: " + v.parts.Count);
            KerbalGUIManager.print("[rusty] Parts.Count: " + v.Parts.Count);
            KerbalGUIManager.print("[rusty] GetActiveParts().Count: " + v.GetActiveParts().Count);

            //pv.vesselRef.Unload();

            if (v.rootPart != null)
            {
                KerbalGUIManager.print("[rusty] Root part is alive");
            }
            else
            {
                KerbalGUIManager.print("[rusty] Root part is NULL");
            }
        }
        public void onGUIRecoveryDialogSpawn(MissionRecoveryDialog dialog)
        {
            KerbalGUIManager.print("[rusty] onGUIRecoveryDialogSpawn");
        }
        public void onGUIRecoveryDialogDespawn(MissionRecoveryDialog dialog)
        {
            KerbalGUIManager.print("[rusty] onGUIRecoveryDialogDespawn");
        }

        public static float GetTotalVesselCost(ProtoVessel vessel)
        {
            float total = 0;
            foreach (ProtoPartSnapshot part in vessel.protoPartSnapshots)
            {
                float dry, wet;
                total += ShipConstruction.GetPartCosts(part, part.partInfo, out dry, out wet);
            }
            return total;
        }
    }
}
