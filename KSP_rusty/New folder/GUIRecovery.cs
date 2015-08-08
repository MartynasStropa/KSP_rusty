using UnityEngine;

namespace KSP_rusty
{
    class GUIRecovery : GUIWindow
    {
        public double recoveryPrice = 0.0f;
        public string vesselTitle = "";
        public double vesselMass = 0.0;
        public double recoveryDistance = 0.0;

        public GUIRecovery()
        {
            base.setTitle("Recovery operation");
            base.minWidth = 500.0f;
        }

        public override void drawWindow(int windowID)
        {
            GUIStyle mySty = new GUIStyle(GUI.skin.button);
            mySty.normal.textColor = mySty.focused.textColor = Color.white;
            mySty.hover.textColor = mySty.active.textColor = Color.yellow;
            mySty.onNormal.textColor = mySty.onFocused.textColor = mySty.onHover.textColor = mySty.onActive.textColor = Color.green;
            mySty.padding = new RectOffset(8, 8, 8, 8);

            GUILayout.BeginVertical();

            GUILayout.Label("Vessel: " + vesselTitle);
            GUILayout.Label("Total mass [kg]: " + vesselMass);
            GUILayout.Label("Distance [km]: " + recoveryDistance);
            GUILayout.Label("\nRecovery cost: " + recoveryPrice);

            GUILayout.EndVertical();
        }
    }
}
