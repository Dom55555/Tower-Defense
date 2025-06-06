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
    public float range;
    public bool canSeeHiddens;
    public bool canSeeFlyings;
    [Header("Other Variables:")]
    public List<Enemy> enemiesInRange = new List<Enemy>();
    public int level = 1;
    public float rangeMult = 1;
    public float firerateMult = 1;
    public float priceMult = 1;
    public bool justPlaced;
    public bool preview;

    public float timer = 0;

    private Enemy targetEnemy = null;
    private TMP_Text firerateText;
    private TMP_Text rangeText;
    private TMP_Text discountText;
    //ranger
    private LineRenderer lr;
    //patrol
    public Transform[] wayPoints;
    private int currentPoint = 1;
    private Transform soldier;
    //commander and dj
    public List<Tower> towersInRange = new List<Tower>();
    private int[] firerateBuffs = new int[5] {10,15,20,25,35};
    private int[] rangeBuffs = new int[5] { 10, 15, 20, 20, 30 };
    private int[] discountBuffs = new int[5] { 0, 0, 0, 10, 20 };
    //accelerator
    private bool charged = false;
    private bool isAcceleratorCharging = false;


    private void Start()
    {
        if (justPlaced == false)
        {
            preview = true;
        }
        else preview = false;
        if (transform.Find("FirerateBuff") != null) firerateText = transform.Find("FirerateBuff").GetComponent<TMP_Text>();
        if (transform.Find("RangeBuff") != null) rangeText = transform.Find("RangeBuff").GetComponent<TMP_Text>();
        if (transform.Find("DiscountBuff") != null) discountText = transform.Find("DiscountBuff").GetComponent<TMP_Text>();
        ChangeBuffsValues();
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
                    StartCoroutine(Buffs());
                    break;
                case "DJ":
                    StartCoroutine(Buffs());
                    break;
                case "Patrol":
                    StartCoroutine(PatrolSpawning());
                    break;
                case "PatrolCar":
                    if(level > 3)
                    {
                        soldier = transform.Find("Soldier");
                        soldier.gameObject.SetActive(true);
                        StartCoroutine(Targeting());
                        StartCoroutine(PatrolShooting());
                    }
                    break;
                case "Accelerator":
                    StartCoroutine(AcceleratorTargeting());
                    StartCoroutine(AcceleratorShooting());
                    break;
                default: //all other towers
                    StartCoroutine(Targeting());
                    StartCoroutine(Shooting());
                    break;
            }
        }
        if (towerName == "PatrolCar")
        {
            transform.position += transform.forward * 2 * Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(wayPoints[currentPoint].position - transform.position), 4 * Time.deltaTime);
            if (Vector3.Distance(transform.position, wayPoints[currentPoint].position) < 0.1f)
            {
                currentPoint++;
                if (currentPoint >= wayPoints.Length) Destroy(gameObject);
            }
        }
        if(towerName=="Accelerator" && targetEnemy!=null)
        {
            Vector3 direction = targetEnemy.transform.position - transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);
            transform.Rotate(0, 90, 0);
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
                    if (enemy.hp+enemy.shieldHp>mostHp && (
                        (canSeeHiddens && enemy.status.Contains("Hidden")) ||
                        (!enemy.status.Contains("Hidden") && enemy.status != "Flying") ||
                        (canSeeFlyings && enemy.status == "Flying")
                        ))
                    {
                        mostHp = enemy.hp+enemy.shieldHp;
                        targetEnemy = enemy;
                    }
                }
            }
            yield return new WaitForSeconds(0.1f); 
        }
    }
    private IEnumerator AcceleratorTargeting()
    {
        while (true)
        {
            if(targetEnemy==null || !enemiesInRange.Contains(targetEnemy))
            {
                AcceleratorTargetEnemy();
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
    private void AcceleratorTargetEnemy()
    {
        enemiesInRange.RemoveAll(x => x == null);
        if (enemiesInRange.Count == 0)
        {
            targetEnemy = null;
            charged = false;
        }
        if (mode == "First")
        {
            float mostWalked = -1;
            foreach (var enemy in enemiesInRange)
            {
                if (enemy.distanceWalked > mostWalked && enemy.status != "Flying")
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
                if (enemy.hp + enemy.shieldHp > mostHp && enemy.status != "Flying")
                {
                    mostHp = enemy.hp + enemy.shieldHp;
                    targetEnemy = enemy;
                }
            }
        }
        if(targetEnemy==null)
        {
            charged = false;
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
                yield return new WaitForSeconds(firerate*firerateMult);
            }
            else yield return new WaitForSeconds(0.05f);
        }
    }
    private IEnumerator AcceleratorShooting()
    {
        while(true)
        {
            if(targetEnemy!=null && !charged && !isAcceleratorCharging)
            {
                StartCoroutine(AcceleratorCharging());
            }
            if(targetEnemy!=null && charged)
            {
                animator.Play("Shoot", -1, 0f);
                if (targetEnemy.shieldHp > 0)
                {
                    if (targetEnemy.shieldHp >= damage) targetEnemy.shieldHp -= damage;
                    else
                    {
                        int shieldDamageMade = targetEnemy.shieldHp;
                        targetEnemy.shieldHp = 0;
                        targetEnemy.hp -= damage - shieldDamageMade;
                    }
                }
                else targetEnemy.hp -= damage;
                if (targetEnemy.hp <= 0)
                {
                    enemiesInRange.Remove(targetEnemy);
                    Destroy(targetEnemy.gameObject);
                    AcceleratorTargetEnemy();
                }
                yield return new WaitForSeconds(firerate * firerateMult);
            }
            else
            {
                yield return new WaitForSeconds(0.15f);
            }
        }
    }
    private IEnumerator AcceleratorCharging()
    {
        isAcceleratorCharging = true;
        float timer = 0;
        while(timer<2f)
        {
            timer += 0.5f;
            animator.Play("Shoot");
            yield return new WaitForSeconds(0.5f);
            if(targetEnemy==null)
            {
                AcceleratorTargetEnemy();
                if(targetEnemy==null)
                {
                    isAcceleratorCharging = false;
                    yield break;
                }
            }
        }
        charged = true;
        isAcceleratorCharging = false;
        StartCoroutine(AcceleratorLaser());
    }
    private void RangerTrail()
    {
        GameObject lineObj = new GameObject("BulletTrail");
        lr = lineObj.AddComponent<LineRenderer>();

        lr.material = new Material(Shader.Find("Sprites/Default"));

        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;

        lr.SetPosition(0, transform.Find("Gun").Find("Muzzle").position);
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
    private IEnumerator AcceleratorLaser()
    {
        GameObject lineObj = new GameObject("AcceleratorTrail");
        lr = lineObj.AddComponent<LineRenderer>();
        //Material mat = new Material(Shader.Find("Unlit/Color"));
        //mat.color = new Color(1.2f, 0f, 1.2f, 1f);
        Material mat = Resources.Load<Material>("AcceleratorNeon");
        lr.material = mat;

        lr.startWidth = 0.12f;
        lr.endWidth = 0.12f;
        lr.numCapVertices = 8;
        lr.numCornerVertices = 8;

        while (targetEnemy!=null)
        {
            lr.SetPosition(0, transform.Find("Gun").Find("Muzzle").position);
            lr.SetPosition(1, targetEnemy.transform.Find("Target").position);
            yield return new WaitForSeconds(0.05f);
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
    private IEnumerator Buffs()
    {
        while (true)
        {
            towersInRange.RemoveAll(x => x == null);
            foreach (var tower in towersInRange)
            {
                if(towerName=="Commander")
                {
                    if (tower.firerateMult > 1 / (1 + firerateBuffs[level - 1] / 100f)) tower.firerateMult = 1/(1+firerateBuffs[level - 1]/100f);
                    tower.ChangeBuffsValues();
                }
                else if (towerName=="DJ")
                {
                    tower.rangeMult = 1+rangeBuffs[level-1]/100f;
                    tower.priceMult = 1 - discountBuffs[level - 1] / 100f;
                    tower.ChangeBuffsValues();
                }
            }
            yield return new WaitForSeconds(0.8f);
        }
    }
    public void ChangeBuffsValues()
    {
        int percent = Mathf.RoundToInt((1 / firerateMult - 1) * 100);
        if(firerateText!=null) firerateText.text = "+" + percent + "%";

        percent = Mathf.RoundToInt((rangeMult-1)*100);
        if(rangeText!=null) rangeText.text = "+"+percent+"%";
        transform.Find("Collider").localScale = new Vector3(range * rangeMult, 6, range * rangeMult);
        transform.Find("Range").localScale = new Vector3(range * rangeMult, 0.1f, range * rangeMult);

        percent = Mathf.RoundToInt((1 - priceMult) * 100);
        if(discountText!=null) discountText.text = "-"+percent+"%";
    }
}
