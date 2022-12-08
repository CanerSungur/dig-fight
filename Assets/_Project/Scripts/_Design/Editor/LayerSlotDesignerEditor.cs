using UnityEngine;
using UnityEditor;

namespace DigFight
{
    [CustomEditor(typeof(LayerSlotDesigner))]
    public class LayerSlotDesignerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            LayerSlotDesigner layerSlotHandler = (LayerSlotDesigner)target;

            GUILayout.Label("BREAKABLES");

            if (GUILayout.Button("STONE"))
                layerSlotHandler.MakeStoneBox();

            if (GUILayout.Button("COPPER"))
                layerSlotHandler.MakeCopperBox();

            if (GUILayout.Button("DIAMOND"))
                layerSlotHandler.MakeDiamondBox();

            GUILayout.Space(20);
            GUILayout.Label("EXPLOSIVES");

            if (GUILayout.Button("EXPLOSIVE"))
                layerSlotHandler.MakeExplosiveBox();

            GUILayout.Space(20);
            GUILayout.Label("PUSHABLES");

            if (GUILayout.Button("PUSHABLE"))
                layerSlotHandler.MakePushableBox();

            GUILayout.Space(20);
            GUILayout.Label("BOOSTS");

            if (GUILayout.Button("SPEED_CHEST"))
                layerSlotHandler.MakeSpeedChest();

            if (GUILayout.Button("POWER_CHEST"))
                layerSlotHandler.MakePowerChest();

            if (GUILayout.Button("DURABILITY_CHEST"))
                layerSlotHandler.MakeDurabilityChest();
        }
    }
}
