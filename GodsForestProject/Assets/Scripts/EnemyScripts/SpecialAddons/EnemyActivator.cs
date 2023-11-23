using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActivator : MonoBehaviour
{
    public List<AbstractEnemyBase> roomEnemies = new List<AbstractEnemyBase>();
    private List<EnemySpawner> spawners = new List<EnemySpawner>();
    public GameObject entryEffect, teleporter;


    public void DisableEnemies()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, 8);

        foreach(var hit in hits)
        {
            if (hit.gameObject.GetComponent<AbstractEnemyBase>() != null)
            {
                roomEnemies.Add(hit.gameObject.GetComponent<AbstractEnemyBase>());
                hit.transform.parent.gameObject.SetActive(false);
            }

            else if(hit.gameObject.GetComponent<EnemySpawner>() != null)
            {
                spawners.Add(hit.gameObject.GetComponent<EnemySpawner>());
                hit.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Contains("Player"))
        {
            EnableEnemies();
            StartCoroutine(RoomCheck());
            Destroy(GetComponent<CircleCollider2D>());
        }
    }

    IEnumerator RoomCheck()
    {
        yield return new WaitForSeconds(1.0f);

        roomEnemies.RemoveAll(empty => empty == null);
        spawners.RemoveAll(empty => empty == null);

        if (roomEnemies.Count > 0 || spawners.Count > 0)
        {
            StartCoroutine(RoomCheck());
        }
        else
        {
            if(Random.Range(0, 101) < TeleporterController.instance.nextSpawnChance)
            { 
            var hits = Physics2D.OverlapCircleAll(transform.position, 3.0f);
            foreach(var hit in hits)
            {
                if (hit.transform.tag.Contains("trap"))
                {
                    SpawnCloud(hit.transform.position, false);
                    Destroy(hit.gameObject);
                }
            }

                var porter = Instantiate(teleporter, transform.position, Quaternion.identity);
                SpawnCloud(porter.transform.position, true, new Color(200.0f/255.0f, 114.0f / 255.0f, 196.0f / 255.0f, 220.0f / 255.0f));
                TeleporterController.instance.AddPorter(porter.GetComponent<Teleporter>());
                TeleporterController.instance.nextSpawnChance = 0;
            }

            TeleporterController.instance.nextSpawnChance += 8;
            Destroy(this.gameObject);
        }
    }

    private void EnableEnemies()
    {
        foreach(var enemy in roomEnemies)
        {
            enemy.transform.parent.gameObject.SetActive(true);
            SpawnCloud(enemy.transform.position, false);
        }

        foreach(var spawn in spawners)
        {
            spawn.gameObject.SetActive(true);
            spawn.GetComponent<EnemySpawner>().StartSpawning();
            SpawnCloud(spawn.transform.position, true);
        }
    }

    private void SpawnCloud(Vector2 pos, bool isSpawner)
    {
        
        var cloud = Instantiate(entryEffect, pos, Quaternion.identity);
        if (isSpawner)
        {
            cloud.transform.localScale *= 2.5f;
        }
        Destroy(cloud, .35f);
    }

    private void SpawnCloud(Vector2 pos, bool isSpawner, Color shade)
    {

        var cloud = Instantiate(entryEffect, pos, Quaternion.identity);
        cloud.GetComponent<SpriteRenderer>().color = shade;
        if (isSpawner)
        {
            cloud.transform.localScale *= 2.5f;
        }
        Destroy(cloud, .35f);
    }
}
