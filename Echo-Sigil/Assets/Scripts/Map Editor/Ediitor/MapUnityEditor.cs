using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapReaderBehavior))]
[CanEditMultipleObjects]
public class MapUnityEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapReader.backupMapSize = EditorGUILayout.Vector2IntField("Backup Map Size",MapReader.backupMapSize);
        if (GUILayout.Button("GenerateMap"))
        {
            MapReader.GeneratePhysicalMap(new Map(MapReader.backupMapSize));
        }
        if (GUILayout.Button("SaveMap"))
        {
            MapReader.SaveMap("test");
        }
        if (GUILayout.Button("LoadMap"))
        {
            MapReader.LoadMap("test");
        }

    }
}
