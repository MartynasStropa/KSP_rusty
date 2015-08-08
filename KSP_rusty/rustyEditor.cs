using UnityEngine;

namespace KSP_rusty 
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    class rustyEditor : MonoBehaviour
    {
        GUIInventory wndInventory = null;

        void Start()
        {
            wndInventory = new GUIInventory();
            wndInventory.show();

            rustyPartModules.addUsageModulesToParts(EditorLogic.fetch.ship.parts);
        }

        void OnDestroy()
        {
            wndInventory = null;
        }
    }
}
