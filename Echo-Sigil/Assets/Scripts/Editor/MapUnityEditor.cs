using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Map))]
[CanEditMultipleObjects]
public class MapUnityEditor : Editor
{
    SerializedProperty m_Size;
    SerializedProperty m_Sprite;
    SerializedProperty m_Tile;
    SerializedProperty m_TileSize;
    public override void OnInspectorGUI()
    {
        Map map = (Map)target;

        m_Size = serializedObject.FindProperty("size");
        m_Sprite = serializedObject.FindProperty("mapSprite");
        m_Tile = serializedObject.FindProperty("tile");
        m_TileSize = serializedObject.FindProperty("tileHeight");

        if (map.mapSprite == null)
        {
            EditorGUILayout.PropertyField(m_Sprite);
            EditorGUILayout.PropertyField(m_Size);
        } else
        {
            EditorGUILayout.PropertyField(m_Sprite);
        }

        EditorGUILayout.PropertyField(m_Tile);
        EditorGUILayout.PropertyField(m_TileSize);


        if (GUILayout.Button("GenerateMap"))
        {
            map.GenerateMap();
        }
        if (GUILayout.Button("DestroyMap"))
        {
            map.RemoveMap();
        }
        
    }
}
