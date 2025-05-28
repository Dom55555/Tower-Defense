using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Animator animator;
    public List<Enemy> enemiesInRange = new List<Enemy>();

    public string mode = "First";
    public string towerName = "Scout";
    public int level = 1;
    public float firerate = 1f;
    public int damage = 4;

    public bool justPlaced;

    private Enemy targetEnemy = null;

    void Start()
    {

    }


    void Update()
    {
        if(justPlaced)
        {
            StartCoroutine(Targeting());
            StartCoroutine(Shooting());
            justPlaced = false;
        }
    }
    private IEnumerator Targeting()
    {
        while (true)
        {
            List<Enemy> enemiesToRemove = new List<Enemy>();
            if(enemiesInRange.Count==0)
            {
                targetEnemy = null;
                yield return new WaitForSeconds(0.2f);
                continue;
            }
            if (mode == "First")
            {
                float mostWalked = -1;
                foreach (var enemy in enemiesInRange)
                {
                    if(enemy==null)
                    {
                        enemiesToRemove.Add(enemy);
                    }
                    if (enemy.distanceWalked > mostWalked)
                    {
                        mostWalked = enemy.distanceWalked;
                        targetEnemy = enemy;
                    }
                }
            }
            else if (mode == "Strongest")
            {
                float mostHp = 0;
                foreach (var enemy in enemiesInRange)
                {
                    if (enemy == null)
                    {
                        enemiesToRemove.Add(enemy);
                    }
                    if (enemy.hp>mostHp)
                    {
                        mostHp = enemy.hp;
                        targetEnemy = enemy;
                    }
                }
            }
            foreach (var enemy in enemiesToRemove)
            {
                enemiesInRange.Remove(enemy);
            }
            yield return new WaitForSeconds(0.1f); 
        }
    }
    private IEnumerator Shooting()
    {
        while (true)
        {
            if (targetEnemy != null)
            {
                transform.LookAt(targetEnemy.transform);
                transform.Rotate(0, 90f, 0);
                animator.Play("Shoot");
                targetEnemy.hp -= damage;
                if(targetEnemy.hp<=0)
                {
                    enemiesInRange.Remove(targetEnemy);
                    Destroy(targetEnemy.gameObject);
                }
                yield return new WaitForSeconds(firerate);
                targetEnemy = null;
            }
            else
            {
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}
