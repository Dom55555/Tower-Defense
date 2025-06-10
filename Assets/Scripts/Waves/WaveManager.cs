using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;
    [Header("To Set:")]
    public Transform wayPointsObject;
    public TMP_Text timerText;
    public TMP_Text waveText;
    public WaveData[] waves;
    public AudioClip waveStartSound;
    public AudioClip intermissionSound;
    [Header("Other Variables:")]
    public int wave = 0;
    public int baseHp = 100;
    public float gameTimer = 15;
    public bool intermission = true;
    public List<Enemy> enemiesOnMap = new List<Enemy>();


    private Transform spawnPoint;
    private bool allSpawned = false;

    void Start()
    {
        instance = this;
        spawnPoint =wayPointsObject.GetChild(0);
        StartCoroutine(CheckEnemies());
    }
    void Update()
    {
        if(baseHp<=0)
        {
            return;
        }
        gameTimer -= Time.deltaTime;

        if (!intermission && (gameTimer <= 0||(enemiesOnMap.Count==0&&allSpawned)))
        {
            UIFunctions.instance.PlaySound(intermissionSound);
            gameTimer = 5;
            TowerManager.instance.money += 100 + wave * 25;
            StartCoroutine(UIFunctions.instance.WaveMoney(100 + wave * 25));
            intermission = true;
            allSpawned = false;
        }

        if (gameTimer <= 0 && intermission)
        {
            UIFunctions.instance.PlaySound(waveStartSound);
            intermission = false;
            gameTimer = 60;
            if(wave==waves.Length-1)
            {
                gameTimer = 300;
            }
            if(TowerManager.instance.moneyPerWave>0)
            {
                TowerManager.instance.WaveMoney();
                StartCoroutine(UIFunctions.instance.WaveMoney(TowerManager.instance.moneyPerWave));
            }
            if (wave < waves.Length)
            {
                StartCoroutine(SpawnWave(waves[wave]));
                wave++;
            }
            else
            {
                UIFunctions.instance.Victory();
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
                slime.GetComponent<Enemy>().currentPoint = 1;
                enemiesOnMap.Add(slime.GetComponent<Enemy>());
                yield return new WaitForSeconds(enemy.spawnDelay);
            }
            yield return new WaitForSeconds(enemy.delayAfter);
        }
        allSpawned = true;
    }
    IEnumerator CheckEnemies()
    {
        while (true)
        {
            enemiesOnMap.RemoveAll(enemy => enemy == null || enemy.hp <= 0);
            yield return new WaitForSeconds(1);
        }
    }
}
