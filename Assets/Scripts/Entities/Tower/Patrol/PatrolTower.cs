using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolTower : MonoBehaviour
{
    private Tower parent;
    void Start()
    {
        parent = gameObject.transform.root.GetComponent<Tower>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && other.name == "Body")
        {
            int enemyShield = other.transform.root.GetComponent<Enemy>().shieldHp;
            int enemyHp = other.transform.root.GetComponent<Enemy>().hp;
            if (parent.damage>=enemyShield)
            {
                other.transform.root.GetComponent<Enemy>().shieldHp = 0;
                parent.damage -= enemyShield;

                other.transform.root.GetComponent<Enemy>().hp -= Mathf.Clamp(parent.damage, 0, enemyHp);
                parent.damage -= Mathf.Clamp(enemyHp, 0, parent.damage);
            }
            else
            {
                other.transform.root.GetComponent<Enemy>().shieldHp -=parent.damage;
                parent.damage = 0;
            }
            if(parent.damage<=0)
            {
                Destroy(parent.gameObject);
            }
        }
    }
}
