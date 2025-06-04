using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    private Tower parent;
    private void Start()
    {
        parent = transform.root.GetComponent<Tower>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(parent.towerName=="Commander")
        {
            var tower = other.transform.root.GetComponent<Tower>();
            if (other.CompareTag("Tower")&& other.name == "Placement"&&!parent.towersInRange.Contains(tower) && !parent.preview && !tower.preview)
            {
                if(tower.towerName != "Commander" && !tower.towerName.Contains("Patrol") && tower.towerName!="Farm")
                {
                    parent.towersInRange.Add(other.transform.root.GetComponent<Tower>());
                }
            }
        }
        else if (other.CompareTag("Enemy") && other.name == "Body")
        {
            parent.enemiesInRange.Add(other.transform.parent.GetComponent<Enemy>());
        }
    }
    private void OnTriggerExit(Collider other )
    {
        if(parent.towerName=="Commander")
        {

        }
        else if (other.CompareTag("Enemy") && other.name == "Body")
        {
            parent.enemiesInRange.Remove(other.transform.parent.GetComponent<Enemy>());
        }
    }
}
