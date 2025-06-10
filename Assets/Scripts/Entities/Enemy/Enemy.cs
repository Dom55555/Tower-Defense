
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform wayPointsObject;
    public int currentPoint = 1;
    public string enemyName;
    public float speed;
    public int maxHp;
    public int maxShield;
    public int hp;
    public int shieldHp;
    public string status;
    public float distanceWalked = 0f;
    public GameObject deathEffectPrefab;

    public List<GameObject> slimesPrefabs;

    private bool speedChanged = false;
    private Transform[] wayPoints;
    private bool canMove = true;
    void Start()
    {
        hp = maxHp;
        shieldHp = maxShield;
        wayPoints = new Transform[wayPointsObject.childCount];
        for (int i = 0; i < wayPointsObject.childCount; i++)
        {
            wayPoints[i] = wayPointsObject.GetChild(i);
        }
        if(enemyName=="Baron Slime")
        {
            StartCoroutine(SpawnSlimes());
        }
    }
    void Update()
    {
        if(hp<=0)
        {
            Die();
        }
        if(canMove)
        {
            Vector3 direction = wayPoints[currentPoint].position - transform.position;
            Vector3 movement = transform.forward * speed * Time.deltaTime;
            transform.position += movement;
            distanceWalked += movement.magnitude;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction),speed*1.5f*Time.deltaTime);
            if (Vector3.Distance(transform.position, wayPoints[currentPoint].position) < 0.1f)
            {
                currentPoint++;
                if (currentPoint >= wayPoints.Length)
                {
                    WaveManager.instance.baseHp -= hp + shieldHp;
                    UIFunctions.instance.PlaySound(UIFunctions.instance.baseDamageSound);
                    UIFunctions.instance.UpdateHPBar(WaveManager.instance.baseHp);
                    Die();
                }
            }
        }
        if(enemyName=="Balloon Slime" && shieldHp<=0 && !speedChanged)
        {
            speed /= 2;
            speedChanged = true;
        }
        if((enemyName=="Final Boss" || enemyName=="Electric Slime")&& shieldHp<=0 && !speedChanged)
        {
            speed *= 3;
            speedChanged = true;
            if(enemyName == "Final Boss")
            {
                transform.Find("Shield").gameObject.SetActive(false);
            }
        }
    }
    private IEnumerator SpawnSlimes()
    {
        yield return new WaitForSeconds(5);
        while(true)
        {
            canMove = false;
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < 3; i++)
            {
                int index = Random.Range(0, 4);
                SpawnChosenSlime(index);
                yield return new WaitForSeconds(0.6f);
            }
            canMove = true;
            yield return new WaitForSeconds(10);
        }
    }
    public void Die()
    {
        GameObject popEffect = Instantiate(deathEffectPrefab, transform.position+Vector3.up*0.5f, Quaternion.identity);
        popEffect.transform.rotation = Quaternion.LookRotation(Vector3.up);
        Destroy(popEffect, 1);
        if (currentPoint < wayPoints.Length && WaveManager.instance.baseHp>0)
        {
            TowerManager.instance.money += maxHp;
            if(enemyName == "Mystery Slime")
            {
                int random = Random.Range(0, 100);
                int index;
                if (WaveManager.instance.wave < 26)
                {
                    if (random < 50) index = 0;
                    else if (random < 85) index = 1;
                    else index = 2;
                }
                else if (WaveManager.instance.wave < 32)
                {
                    if (random < 80) index = 2;
                    else index = 3;
                }
                else
                {
                    if (random < 30) index = 2;
                    else if (random < 75) index = 3;
                    else index = 4;
                }
                SpawnChosenSlime(index);
            }
        }
        Destroy(gameObject);
    }
    private void SpawnChosenSlime(int index)
    {
        var slime = Instantiate(slimesPrefabs[index], transform.position, transform.rotation);
        var slimeEnemy = slime.GetComponent<Enemy>();
        var slimeInfo = slimesPrefabs[index].GetComponent<Enemy>();
        slimeEnemy.speed = slimeInfo.speed;
        slimeEnemy.GetComponent<Enemy>().maxShield = slimeInfo.maxShield;
        slimeEnemy.GetComponent<Enemy>().maxHp = slimeInfo.maxHp;
        slimeEnemy.GetComponent<Enemy>().status = slimeInfo.status;
        slimeEnemy.GetComponent<Enemy>().enemyName = slimeInfo.enemyName;
        slimeEnemy.GetComponent<Enemy>().wayPointsObject = wayPointsObject;
        slimeEnemy.GetComponent<Enemy>().currentPoint = currentPoint;
        slimeEnemy.GetComponent<Enemy>().distanceWalked = distanceWalked;
        WaveManager.instance.enemiesOnMap.Add(slimeEnemy);
    }
}
