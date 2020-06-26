using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapReaderBehavior))]
[CanEditMultipleObjects]
public class MapUnityEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapReader.backupMapSize = EditorGUILayout.Vector2IntField("Backup Map Size",MapReader.backupMapSize);
        if (GUILayout.Button("Generate Blank Map"))
        {
            MapReader.GeneratePhysicalMap(new Map(MapReader.backupMapSize));
        }
        GUILayout.BeginHorizontal();
        string mapName = "Test";
        mapName = EditorGUILayout.TextField(new GUIContent("Map Name"), mapName);
        if (GUILayout.Button("Save"))
        {
            MapReader.SaveMap(mapName);
        }
        if (GUILayout.Button("Load"))
        {
            MapReader.LoadMap(mapName);
        }
        GUILayout.EndHorizontal();

    }
}
