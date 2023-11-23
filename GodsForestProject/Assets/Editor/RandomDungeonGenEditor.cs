using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AbstractDungeonGenerator), true)]

public class RandomDungeonGenEditor : Editor
{
    AbstractDungeonGenerator generator;

    private void Awake()
    {
        generator = target as AbstractDungeonGenerator;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Create Dungeon"))
        {
            generator.GenerateDungeon();
        }

        if (GUILayout.Button("Delete Dungeon"))
        {
            generator.Delete();
        }
    }
}
