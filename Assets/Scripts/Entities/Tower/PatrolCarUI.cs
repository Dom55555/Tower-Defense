using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PatrolCarUI : EntityUI
{
    public TMP_Text hpText;
    public TMP_Text levelText;
    public RectTransform hpBarFill;

    private int maxDamage;
    protected override void Start()
    {
        
    }
    protected override void Update()
    {
        if (entityTransform == null)
        {
            transform.gameObject.SetActive(false);
            return;
        }
        transform.position = entityTransform.position;
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180f, 0);
        maxDamage=TowerManager.instance.towers.FirstOrDefault(x => x.towerName == "Patrol").levels[entityTransform.GetComponent<Tower>().level-1].damage;
        hpText.text = entityTransform.GetComponent<Tower>().damage + "/" + maxDamage+" HP";
        levelText.text = entityTransform.GetComponent<Tower>().level+" LVL";

        Bar(hpBarFill, 100f, -0.25f, entityTransform.GetComponent<Tower>().damage, maxDamage, Color.red, Color.yellow, Color.green);
    }
}
