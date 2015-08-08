using System.Collections.Generic;
using UnityEngine;


/*
[90%] Recovery from flight
[90%] Recovery from tracking station
[90%] Recovery from spacecenter view
[95%] Possible bug: fuel lines and struts after respawing from inventory
 X Handle srfN
 X Handle sym
[100%] BUG: Pods activation
[~100%] Part offset position by gizmo. Use only base position
[~100%] Part rotation by gizmo. Use only base rotation
[ ] Clear editor after launch, so when coming back to editor, there is no vessel
[ ] Recovery from VAB/SPH
[ ] Check/fix recovery after rerooting, because 0th part may become not first in the file
[ ] Spawn part at mouse
[ ] Register pod on spawn as control part
[ ] All modules usage: module specific
[ ] All modules condition persistancy: flight, editor
[ ] All modules usage; general: age, G, heat, weight stress (?)
[ ] All modules maximum condition
[ ] All modules failures
[ ] All modules inspection of parts on EVA
 * Show very rough estimation when building level low
 * Show more precise estimation when engineer level higher
[ ] All modules repair on EVA
[ ] All modules repair in VAB/SPH
[ ] Part condition shown in flight only after buildng upgrade
 * Show very rough estimation when building level low
[ ] Price recalculation before launch:
 * deduct price from parts from inventory
 * mark parts from inventory
 * deduct price from resources from inventory
[ ] Overthrust the engines, increased usage
 * Allow engineer to do that at higher level
[ ] Overspeed rover wheels, increased usage
 * Allow engineer to do that at higher level
[ ] Sell parts in inventory
[ ] Scrap parts in inventory
[ ] Use scrap to repair in VAB/SPH
[ ] Hybernate probe cores to reduce usage
[ ] Access resource inventory from launch pad/runway (requires building upgrade)
[ ] Recovery price: 5 km from KSC: free; 1000kg * 1km = 100 funds + 1000
 X deduct only when in career mode
[ ] Reduce price if strategy selected
[ ] Inventory limits based on building upgrades
[ ] Option to sell right after recovery
[ ] Remove part components (gimbals, reaction wheels, etc)
[ ] Add part components (twice the price for parts that don't have the component normally - twice the usage)
[ ] Add redundant part components (two gimbals, in case one fails, etc)
[ ] Addon config window + small tweaks
[ ] Choose part quality: short lifetime, cheap; long lifetime, expensive;
[ ] FMRS
-- MAYBE --
[ ] Generate thumbnail of assemblies in inventory
[ ] Tweak: Set parachute semi-deployement altitude
[ ] Tweak: Added drag from cargo bay
[ ] Tweak: Kerbals get XP during missions
[ ] Use scrap to repair in EVA
*/

// MonoBehaviour
// Constructor -> Awake() -> Start() -> Update/FixedUpdate() [repeats] -> OnDestroy()

namespace KSP_rusty
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    class KSP_rusty : MonoBehaviour
    {
        private static eventListener observer = new eventListener();
        public static List<ConfigNode> Inventory = new List<ConfigNode>();
        public static GUIRecovery wndRecovery = new GUIRecovery();

        public void onDestroy()
        {
            observer = null;
            wndRecovery = null;
        }
    }
}
