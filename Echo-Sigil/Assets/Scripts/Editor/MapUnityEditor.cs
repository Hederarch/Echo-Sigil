using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Map))]
[CanEditMultipleObjects]
public class MapUnityEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Map map = (Map)target;


        base.OnInspectorGUI();

        GUILayout.Label("Warning! Pressing this button will re-generate the map, destroying all current tiles _permanantly_");

        if (GUILayout.Button("GenerateMap"))
        {
            map.GenerateMap(map.mapTexture);
        }

    }
}
