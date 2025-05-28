using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerUI : EntityUI
{
    public TMP_Text levelText;

    public RectTransform levelBarFill;

    private Tower tower;

    protected override void Start()
    {
        tower = entityTransform.GetComponent<Tower>();
        nameText.text = tower.towerName;
        levelText.text = tower.level+" LVL";
        transform.position = entityTransform.position;
    }
    protected override void Update()
    {
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180f, 0);
        levelText.text = tower.level + " LVL";
        if(tower.level==5)
        {
            levelBarFill.GetComponent<Image>().color = Color.red;
        }
    }
}
