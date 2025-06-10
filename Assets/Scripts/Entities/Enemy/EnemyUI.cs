using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : EntityUI
{
    public TMP_Text hpText;
    public TMP_Text shieldText;
    public TMP_Text statusText;

    public RectTransform hpBarFill;
    public RectTransform shieldBarFill;

    private Enemy enemy;

    protected override void Start()
    {
        enemy = entityTransform.GetComponent<Enemy>();
        nameText.text = enemy.enemyName;
        statusText.text = enemy.status;
    }
    protected override void Update()
    {
        if(entityTransform==null)
        {
            transform.gameObject.SetActive(false);
            return;
        }
        transform.position = entityTransform.position;
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180f, 0);
        hpText.text = UIFunctions.instance.FormatNumber(enemy.hp) + "/" + UIFunctions.instance.FormatNumber(enemy.maxHp);
        shieldText.text = enemy.shieldHp==0?"0": UIFunctions.instance.FormatNumber(enemy.shieldHp) + "/" + UIFunctions.instance.FormatNumber(enemy.maxShield);

        Bar(hpBarFill,140f,-0.7f,enemy.hp,enemy.maxHp,Color.red,Color.yellow,Color.green);
        Bar(shieldBarFill,72f,0.042f,enemy.shieldHp,enemy.maxShield,new Color(1,0.45f,0),new Color(1,0.8f,0), Color.yellow);
    }
}
