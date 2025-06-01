using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("To Set:")]
    public Transform wayPointsObject;
    public WaveData[] waves;
    public TMP_Text timerText;
    public TMP_Text waveText;
    [Header("Other Variables:")]
    public int wave = 0;
    public float gameTimer = 5;
    public bool intermission = true;
    public List<Enemy> enemiesOnMap = new List<Enemy>();

    private Transform spawnPoint;
    private bool allSpawned = false;

    void Start()
    {
        spawnPoint =wayPointsObject.GetChild(0);
        StartCoroutine(CheckEnemies());
    }
    void Update()
    {
        gameTimer -= Time.deltaTime;

        if (!intermission && (gameTimer <= 0||(enemiesOnMap.Count==0&&allSpawned)))
        {
            gameTimer = 5; 
            intermission = true;
            allSpawned = false;
        }

        if (gameTimer <= 0 && intermission)
        {
            intermission = false;
            gameTimer = 60;
            if(TowerManager.instance.moneyPerWave>0)
            {
                TowerManager.instance.WaveMoney();
                StartCoroutine(UIFunctions.instance.WaveMoney());
            }
            if (wave < waves.Length)
            {
                StartCoroutine(SpawnWave(waves[wave]));
                wave++;
            }
            else
            {
                Debug.Log("All waves completed!");
            }
        }
        timerText.text = (int)gameTimer / 60 + ":" + ((int)(gameTimer % 60)).ToString("D2");
        waveText.text = "Wave " + wave;
    }
    IEnumerator SpawnWave(WaveData wave)
    {
        foreach (var enemy in wave.enemies)
        {
            for (int i = 0; i < enemy.amount; i++)
            {
                var slime = Instantiate(enemy.enemyPrefab, spawnPoint.position, Quaternion.identity);
                slime.GetComponent<Enemy>().speed = enemy.speed;
                slime.GetComponent<Enemy>().maxShield = enemy.shieldHp;
                slime.GetComponent<Enemy>().maxHp = enemy.hp;
                slime.GetComponent<Enemy>().status = enemy.status;
                slime.GetComponent<Enemy>().enemyName = enemy.name;
                slime.GetComponent<Enemy>().wayPointsObject = wayPointsObject;
                enemiesOnMap.Add(slime.GetComponent<Enemy>());
                yield return new WaitForSeconds(enemy.spawnDelay);
            }
            yield return new WaitForSeconds(enemy.delayAfter);
        }
        allSpawned = true;
        print("ALL SPAWNED");
    }
    IEnumerator CheckEnemies()
    {
        while (true)
        {
            List<Enemy>toRemove= new List<Enemy>();
            for (int i = enemiesOnMap.Count - 1; i >= 0; i--)
            {
                var enemy = enemiesOnMap[i];
                if (enemy == null || enemy.hp <= 0)
                {
                    toRemove.Add(enemy);
                }
            }
            foreach (Enemy enemy in toRemove)
            {
                enemiesOnMap.Remove(enemy);
            }
            yield return new WaitForSeconds(1);
        }
    }
}
