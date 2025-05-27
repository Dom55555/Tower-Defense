using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public int wave = 0;
    public float gameTimer = 5;
    public bool intermission = true;
    public Transform spawnPoint;

    public Transform wayPointsObject;

    public WaveData[] waves;

    public TMP_Text timerText;
    public TMP_Text waveText;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Update()
    {
        gameTimer -= Time.deltaTime;

        if (gameTimer <= 0 && !intermission)
        {
            intermission = true;
            gameTimer = 5; 
        }

        if (gameTimer <= 0 && intermission)
        {
            intermission = false;
            gameTimer = 60; 

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
                yield return new WaitForSeconds(enemy.spawnDelay);
            }

            yield return new WaitForSeconds(enemy.delayAfter);
        }
    }
}
