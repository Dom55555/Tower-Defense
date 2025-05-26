using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform wayPointsObject;
    public int currentPoint = 0;
    public float speed;
    public float hp;
    public float shieldHp;

    private Transform[] wayPoints;
    // Start is called before the first frame update
    void Start()
    {
        wayPoints = new Transform[wayPointsObject.childCount];
        for (int i = 0; i < wayPointsObject.childCount; i++)
        {
            wayPoints[i] = wayPointsObject.GetChild(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = Vector3.MoveTowards(transform.position, wayPoints[currentPoint].position,speed*Time.deltaTime);
        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(wayPoints[currentPoint].position - transform.position), 2*Time.deltaTime);
        //if (Vector3.Distance(transform.position, wayPoints[currentPoint].position)<0.05f)
        //{
        //    currentPoint++;
        //    if(currentPoint>wayPoints.Length-1)
        //    {
        //        Destroy(gameObject);
        //    }
        //}
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
