using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform wayPointsObject;
    public int currentPoint = 0;
    public float speed;
    public int maxHp;
    public int maxShield;
    public int hp;
    public int shieldHp;
    public string enemyName;
    public string status;


    private Transform[] wayPoints;
    void Start()
    {
        hp = maxHp;
        shieldHp = maxShield;
        wayPoints = new Transform[wayPointsObject.childCount];
        for (int i = 0; i < wayPointsObject.childCount; i++)
        {
            wayPoints[i] = wayPointsObject.GetChild(i);
        }
    }
    void Update()
    {
        transform.position += transform.forward * speed*Time.deltaTime;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(wayPoints[currentPoint].position - transform.position),speed*Time.deltaTime);
        if (Vector3.Distance(transform.position, wayPoints[currentPoint].position) < 0.1f)
        {
            currentPoint++;
            if (currentPoint > wayPoints.Length - 1)
            {
                Destroy(gameObject);
            }
        }
    }
}
