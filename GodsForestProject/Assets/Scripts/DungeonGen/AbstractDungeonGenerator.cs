using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField]
    protected TilemapRenderer tilemapRenderer;
    [SerializeField]
    protected Vector2Int startPos = Vector2Int.zero;
    [SerializeField]
    protected DungeonEnemyGenerator enemyGenerator;

    public void GenerateDungeon()
    {

        try
        {
            tilemapRenderer.Clear();
            //enemyGenerator.Clear();
            RunProceduralGeneration();
        }
        catch
        {
            GenerateDungeon();
        }
    }

    public void Delete()
    {
        enemyGenerator.Clear();
        tilemapRenderer.Clear();
    }

    protected abstract void RunProceduralGeneration();


}
