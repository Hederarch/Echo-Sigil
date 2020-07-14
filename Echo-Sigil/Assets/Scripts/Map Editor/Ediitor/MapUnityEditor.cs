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
        if (GUILayout.Button("Generate Blank Map"))
        {
            MapReader.GeneratePhysicalMap(SaveSystem.LoadPallate(Application.dataPath + "/Quests/Tests"), new Map(MapReader.backupMapSize, mapReaderBehavior.addUnit));
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save"))
        {
            MapReader.SaveMap(EditorUtility.SaveFilePanel("Save Map", Application.dataPath, "NewMap", "hedrap"),MapEditor.pallate);
        }
        if (GUILayout.Button("Load"))
        {
            MapReader.LoadMap(EditorUtility.OpenFilePanel("Load Map", Application.dataPath, "hedrap"), SaveSystem.LoadPallate(Application.dataPath + "/Quests/Tests"));
        }
        GUILayout.EndHorizontal();

    }
}
