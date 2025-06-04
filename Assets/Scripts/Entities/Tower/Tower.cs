using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("To Set:")]
    public Animator animator;
    public string mode = "First";
    public string towerName;
    public float firerate;
    public int damage;
    public bool canSeeHiddens;
    public bool canSeeFlyings;
    [Header("Other Variables:")]
    public List<Enemy> enemiesInRange = new List<Enemy>();
    public int level = 1;
    public float rangeMult = 1;
    public float firerateMult = 1;
    public float priceMult = 1;
    public bool justPlaced;
    public bool preview = true;

    public float timer = 0;

    private Enemy targetEnemy = null;
    private TMP_Text firerateText;
    //ranger
    private LineRenderer lr;
    //patrol
    public Transform[] wayPoints;
    private int currentPoint = 1;
    private Transform soldier;
    //commander
    public List<Tower> towersInRange = new List<Tower>();
    public int[] firerateBuffs = new int[5] {10,15,20,25,35};


    private void Start()
    {
        if (justPlaced == false)
        {
            preview = true;
        }
        else preview = false;
        if (transform.Find("FirerateBuff") != null) firerateText = transform.Find("FirerateBuff").GetComponent<TMP_Text>();
    }
    private void Update()
    {
        if(justPlaced)
        {
            justPlaced = false;
            switch (towerName)
            {
                case "Farm":
                    break;
                case "Commander":
                    StartCoroutine(CommanderAbility());
                    StartCoroutine(CommanderBuffs());
                    break;
                case "Patrol":
                    StartCoroutine(PatrolSpawning());
                    break;
                default: //all other towers
                    StartCoroutine(Targeting());
                    StartCoroutine(Shooting());
                    break;
            }
        }
        if (towerName == "PatrolCar")
        {
            if (justPlaced && level > 3)
            {
                soldier = transform.Find("Soldier");
                soldier.gameObject.SetActive(true);
                StartCoroutine(Targeting());
                StartCoroutine(PatrolShooting());
                justPlaced = false;
            }
            transform.position += transform.forward * 2 * Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(wayPoints[currentPoint].position - transform.position), 4 * Time.deltaTime);
            if (Vector3.Distance(transform.position, wayPoints[currentPoint].position) < 0.1f)
            {
                currentPoint++;
                if (currentPoint >= wayPoints.Length) Destroy(gameObject);
            }
        }
    }
    private void OnDestroy()
    {
        Destroy(lr);
    }
    private IEnumerator Targeting()
    {
        while (true)
        {
            enemiesInRange.RemoveAll(x => x == null);
            if (enemiesInRange.Count==0)
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
                    if (enemy.hp+enemy.shieldHp>mostHp)
                    {
                        mostHp = enemy.hp+enemy.shieldHp;
                        targetEnemy = enemy;
                    }
                }
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
                    if(targetEnemy.shieldHp>=damage) targetEnemy.shieldHp -= damage;
                    else
                    {
                        int shieldDamageMade = targetEnemy.shieldHp;
                        targetEnemy.shieldHp = 0;
                        targetEnemy.hp -= damage - shieldDamageMade;
                    }
                }
                else targetEnemy.hp -= damage;
                if(towerName=="Ranger") RangerTrail();
                if(targetEnemy.hp<=0)
                {
                    enemiesInRange.Remove(targetEnemy);
                    Destroy(targetEnemy.gameObject);
                    targetEnemy = null;
                }
                Debug.Log(firerate*firerateMult+" at " + Time.time);
                yield return new WaitForSeconds(firerate*firerateMult);
            }
            else yield return new WaitForSeconds(0.05f);
        }
    }
    private void RangerTrail()
    {
        GameObject lineObj = new GameObject("BulletTrail");
        lr = lineObj.AddComponent<LineRenderer>();

        lr.material = new Material(Shader.Find("Sprites/Default"));

        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;

        lr.SetPosition(0, transform.Find("Muzzle").position);
        lr.SetPosition(1, targetEnemy.transform.Find("Target").position);

        lr.startColor = new Color(1f, 1f, 1f, 1f);
        lr.endColor = new Color(1f, 1f, 1f, 1f);
        lr.alignment = LineAlignment.View;

        StartCoroutine(FadeLineRenderer(0.5f));
    }
    private IEnumerator FadeLineRenderer(float duration)
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
    private IEnumerator PatrolSpawning()
    {
        timer = 2;
        while(true)
        {
            timer -= 1;
            if(timer<=0)
            {
                TowerManager.instance.SpawnPatrolCar(level);
                timer = firerate;
            }
            yield return new WaitForSeconds(1);
        }
    }
    private IEnumerator PatrolShooting()
    {
        while (true)
        {
            if (targetEnemy != null)
            {
                Vector3 direction = targetEnemy.transform.position - soldier.position;
                direction.y = 0;
                soldier.rotation = Quaternion.LookRotation(direction);
                soldier.Rotate(0, 90, 0);

                animator.Play("Shoot", -1, 0f);
                if (targetEnemy.shieldHp > 0)
                {
                    if (targetEnemy.shieldHp >= 2) targetEnemy.shieldHp -= 2;
                    else
                    {
                        int shieldDamageMade = targetEnemy.shieldHp;
                        targetEnemy.shieldHp = 0;
                        targetEnemy.hp -= 2 - shieldDamageMade;
                    }
                }
                else targetEnemy.hp -= 2;
                if (targetEnemy.hp <= 0)
                {
                    enemiesInRange.Remove(targetEnemy);
                    Destroy(targetEnemy.gameObject);
                    targetEnemy = null;
                }
                yield return new WaitForSeconds(firerate);
            }
            else yield return new WaitForSeconds(0.05f);
        }
    }
    private IEnumerator CommanderAbility()
    {
        timer = 2;
        while (true)
        {
            if(timer>0) timer -= 1;
            yield return new WaitForSeconds(1);
        }
    }
    private IEnumerator CommanderBuffs()
    {
        while (true)
        {
            towersInRange.RemoveAll(x => x == null);
            foreach (var tower in towersInRange)
            {
                if (tower.firerateMult > 1 / (1 + firerateBuffs[level - 1] / 100f)) tower.firerateMult = 1/(1+firerateBuffs[level - 1]/100f);
                tower.ChangeBuffValue();
            }
            yield return new WaitForSeconds(0.8f);
        }
    }
    public void ChangeBuffValue()
    {
        int percent = Mathf.RoundToInt((1 / firerateMult - 1) * 100);
        firerateText.text = "+" + percent + "%";
    }
}
