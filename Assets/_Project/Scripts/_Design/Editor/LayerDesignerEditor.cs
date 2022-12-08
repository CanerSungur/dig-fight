using UnityEngine;
using UnityEditor;

namespace DigFight
{
    [CustomEditor(typeof(LayerDesigner))]
    public class LayerDesignerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            LayerDesigner layerHandler = (LayerDesigner)target;

            GUILayout.Label("/-------------------------/");
            if (GUILayout.Button("Spawn Layers!"))
                layerHandler.SpawnLayers();
        }
    }
}
