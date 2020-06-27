using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapReaderBehavior))]
[CanEditMultipleObjects]
public class MapUnityEditor : Editor
{
    SpritePallate pallate = null;
    bool addUnit = false;
    string mapName = "Test";
    public override void OnInspectorGUI()
    {
        MapReader.backupMapSize = EditorGUILayout.Vector2IntField("Backup Map Size",MapReader.backupMapSize);

        GUILayout.BeginHorizontal();
        addUnit = EditorGUILayout.Toggle("With Unit:",addUnit);
        pallate = EditorGUILayout.ObjectField(pallate, System.Type.GetType("SpritePallate"), false) as SpritePallate;
        if (GUILayout.Button("Generate Blank Map"))
        {
            MapReader.GeneratePhysicalMap(new Map(MapReader.backupMapSize,addUnit),pallate);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
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
