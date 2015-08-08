using System;
using UnityEngine;


/* TODO: add events to relevant scenes */
namespace KSP_rusty
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class rustyRecoveryEvents : MonoBehaviour
    {
        private GameScenes lastScene = GameScenes.LOADING;
        public float vesselMass = 999.0f;

        // virtual methods
        void Start()
        {
            KerbalGUIManager.print("[rusty] Started scene " + HighLogic.LoadedScene);

            lastScene = HighLogic.LoadedScene;
            AddEvents();
        }
        void OnDestroy()
        {
            KerbalGUIManager.print("[rusty] Destroyed scene " + lastScene);

            RemoveEvents();

            lastScene = HighLogic.LoadedScene;
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
        }
        private void RemoveEvents()
        {
            KerbalGUIManager.print("[rusty] Removing recovery events");

            GameEvents.OnVesselRecoveryRequested.Remove(OnVesselRecoveryRequested);
            GameEvents.onVesselRecoveryProcessing.Remove(onVesselRecoveryProcessing);
            GameEvents.onVesselRecovered.Remove(onVesselRecovered);
            GameEvents.onGUIRecoveryDialogSpawn.Remove(onGUIRecoveryDialogSpawn);
            GameEvents.onGUIRecoveryDialogDespawn.Remove(onGUIRecoveryDialogDespawn);
        }


        public void OnVesselRecoveryRequested(Vessel v)
        {
            vesselMass = v.GetTotalMass();

            //Get total ship cost
            KerbalGUIManager.print("[rusty] OnVesselRecoveryRequested");
        }
        // recoveryFraction - percentage of funds added after recovery, based on distance from KSC
        public void onVesselRecoveryProcessing(ProtoVessel v, MissionRecoveryDialog dialog, float recoveryFraction)
        {
            KerbalGUIManager.print("[rusty] onVesselRecoveryProcessing: x = " + recoveryFraction);
            KerbalGUIManager.print("[rusty] onVesselRecoveryProcessing: AddFunds = " + -dialog.fundsEarned);
            
            // deduct recovered funds
            Funding.Instance.AddFunds(-dialog.fundsEarned, TransactionReasons.VesselRecovery);

            // calculate distance
            // longest possible ground distance on Kerbin [km]
            //const double HALF_CIRCLE = 2 * Mathf.PI * 600 / 2;
            // distance from KSC [m]
            double distance = SpaceCenter.Instance.GreatCircleDistance(SpaceCenter.Instance.cb.GetRelSurfaceNVector(v.latitude, v.longitude));
            // KSC is supposedly 25 m^2 in size, so no more than 5 km can be considered free
            if (distance <= 5000.0)
            {
                distance = 0.0f;
            }
            KerbalGUIManager.print("[rusty] onVesselRecoveryProcessing: distance = " + distance + " m");
    
            // calculate recovery price
            // price of 1 metric tone per 1km
            const double recoveryRate = 100.0;
            double recoveryPrice = (double)vesselMass * (distance / 1000.0) * recoveryRate;

            // reset funds
            dialog.fundsEarned = 0.0;

            

            // show recovery GUI
            GUIRecovery wndRecovery = new GUIRecovery();
            wndRecovery.vesselMass = vesselMass;
            wndRecovery.vesselTitle = v.vesselName;
            wndRecovery.recoveryPrice = recoveryPrice;
            wndRecovery.recoveryDistance = Math.Ceiling(distance / 1000.0);
            wndRecovery.show();
        }
        public void onVesselRecovered(ProtoVessel v)
        {
            KerbalGUIManager.print("[rusty] onVesselRecovered");
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
