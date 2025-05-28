using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    public Tower parent;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy") && other.name == "Body")
        {
            parent.enemiesInRange.Add(other.transform.parent.GetComponent<Enemy>());
        }
    }
    private void OnTriggerExit(Collider other )
    {
        if(other.CompareTag("Enemy") && other.name == "Body")
        {
            parent.enemiesInRange.Remove(other.transform.parent.GetComponent<Enemy>());
        }
    }
}
