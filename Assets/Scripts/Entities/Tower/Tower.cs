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
                if(towerName=="Ranger")
                {
                    RangerTrail();
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
    private void RangerTrail()
    {
        GameObject lineObj = new GameObject("BulletTrail");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();

        lr.material = new Material(Shader.Find("Sprites/Default"));

        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;

        lr.SetPosition(0, transform.Find("Muzzle").position);
        lr.SetPosition(1, targetEnemy.transform.Find("Target").position);

        lr.startColor = new Color(1f, 1f, 1f, 1f);
        lr.endColor = new Color(1f, 1f, 1f, 1f);

        //lr.receiveShadows = false;
        //lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.alignment = LineAlignment.View;

        StartCoroutine(FadeLineRenderer(lr, 0.5f));
    }
    private IEnumerator FadeLineRenderer(LineRenderer lr, float duration)
    {
        float time = 0f;
        Color startColor = lr.startColor;
        while (time < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, time / duration);
            lr.startColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            lr.endColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            time += Time.deltaTime;
            yield return null;
        }
        Destroy(lr.gameObject);
    }
}
