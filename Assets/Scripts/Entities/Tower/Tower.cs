using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Animator animator;
    public List<Enemy> enemiesInRange = new List<Enemy>();
    public TowerData towerInfo;

    public string mode = "First";
    public string towerName;
    public int level = 1;
    public float firerate;
    public int damage;
    public bool canSeeHiddens;
    public bool canSeeFlyings;
    public float rangeMult = 1;
    public float firerateMult = 1;
    public float priceMult = 1;

    public bool justPlaced;

    private Enemy targetEnemy = null;

    void Start()
    {
        damage = towerInfo.levels[level - 1].damage;
        firerate = towerInfo.levels[level - 1].firerate;
        canSeeHiddens = towerInfo.levels[level - 1].seeHidden;
        canSeeFlyings = towerInfo.levels[level - 1].seeFlying;
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
                    if (enemy.distanceWalked > mostWalked && (
                        (canSeeHiddens&&enemy.status.Contains("Hidden"))||
                        (!enemy.status.Contains("Hidden")&&enemy.status!="Flying")||
                        (canSeeFlyings&&enemy.status=="Flying")
                        ))
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
                    if (enemy.hp+enemy.shieldHp>mostHp)
                    {
                        mostHp = enemy.hp+enemy.shieldHp;
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
                Vector3 direction = targetEnemy.transform.position - transform.position;
                direction.y = 0;
                transform.rotation = Quaternion.LookRotation(direction);
                transform.Rotate(0,90,0);

                animator.Play("Shoot", -1, 0f);
                if (targetEnemy.shieldHp>0)
                {
                    if(targetEnemy.shieldHp>=damage)
                    {
                        targetEnemy.shieldHp -= damage;
                    }
                    else
                    {
                        int shieldDamageMade = targetEnemy.shieldHp;
                        targetEnemy.shieldHp = 0;
                        targetEnemy.hp -= damage - shieldDamageMade;
                    }
                }
                else
                {
                    targetEnemy.hp -= damage;
                }
                if(targetEnemy.hp<=0)
                {
                    enemiesInRange.Remove(targetEnemy);
                    Destroy(targetEnemy.gameObject);
                }
                yield return new WaitForSeconds(firerate*firerateMult);
                targetEnemy = null;
            }
            else
            {
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}
