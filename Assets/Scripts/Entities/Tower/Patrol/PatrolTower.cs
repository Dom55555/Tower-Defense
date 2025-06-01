using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolTower : MonoBehaviour
{
    private Tower parent;
    // Start is called before the first frame update
    void Start()
    {
        parent = gameObject.transform.root.GetComponent<Tower>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && other.name == "Body")
        {
            int enemyHp = other.transform.root.GetComponent<Enemy>().hp;
            other.transform.root.GetComponent<Enemy>().hp -= Mathf.Clamp(parent.damage,0,enemyHp);
            if (other.transform.root.GetComponent<Enemy>().hp <= 0)
            {
                Destroy(other.transform.root.gameObject);
            }
            parent.damage -= Mathf.Clamp(enemyHp, 0, parent.damage);
            if(parent.damage<=0)
            {
                Destroy(parent.gameObject);
                print("Destroyed");
            }
        }
    }
}
