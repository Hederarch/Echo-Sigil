using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapReaderBehavior))]
[CanEditMultipleObjects]
public class MapUnityEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapReaderBehavior mapReaderBehavior = (MapReaderBehavior)target;

        MapReader.backupMapSize = EditorGUILayout.Vector2IntField("Backup Map Size",MapReader.backupMapSize);

        GUILayout.BeginHorizontal();
        mapReaderBehavior.addUnit = EditorGUILayout.Toggle("With Unit:",mapReaderBehavior.addUnit);
        mapReaderBehavior.pallate = EditorGUILayout.ObjectField(mapReaderBehavior.pallate, System.Type.GetType("SpritePallate"), false) as SpritePallate;
        if (GUILayout.Button("Generate Blank Map"))
        {
            MapReader.GeneratePhysicalMap(new Map(MapReader.backupMapSize, mapReaderBehavior.addUnit), mapReaderBehavior.pallate);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        mapReaderBehavior.mapName = EditorGUILayout.TextField(new GUIContent("Map Name"), mapReaderBehavior.mapName);
        if (GUILayout.Button("Save"))
        {
            MapReader.SaveMap(mapReaderBehavior.mapName);
        }
        if (GUILayout.Button("Load"))
        {
            MapReader.LoadMap(mapReaderBehavior.mapName);
        }
        GUILayout.EndHorizontal();

    }
}
