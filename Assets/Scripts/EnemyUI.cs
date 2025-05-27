using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    public TMP_Text hpText;
    public TMP_Text nameText;
    public TMP_Text shieldText;
    public TMP_Text statusText;
    public Transform enemyTransform;

    public RectTransform hpBarFill;
    public RectTransform shieldBarFill;

    private Enemy enemy;

    private void Start()
    {
        enemy = enemyTransform.GetComponent<Enemy>();
        nameText.text = enemy.enemyName;
        statusText.text = enemy.status;
    }
    private void Update()
    {
        transform.position = enemyTransform.position;
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180f, 0);
        hpText.text = enemy.hp + "/" + enemy.maxHp;
        shieldText.text = enemy.shieldHp==0?"0": enemy.shieldHp + "/" + enemy.maxShield;

        Bar(hpBarFill,140f,-0.7f,enemy.hp,enemy.maxHp,Color.red,Color.yellow,Color.green);
        Bar(shieldBarFill,72f,0,enemy.shieldHp,enemy.maxShield,new Color(1,0.45f,0),new Color(1,0.8f,0), Color.yellow);
    }
    private void Bar(RectTransform bar,float maxSizeX,float minX, int valueCurrent, int valueMax, Color c1, Color c2, Color c3)
    {
        float percent = (float)valueCurrent / valueMax;
        Vector2 barSize = bar.sizeDelta;
        barSize.x = percent * maxSizeX;
        bar.sizeDelta = barSize;
        bar.localPosition = new Vector2(minX+percent*maxSizeX/200,bar.localPosition.y);
        if(percent>=0.5f)
        {
            bar.GetComponent<Image>().color = Color.Lerp(c2, c3, (percent - 0.5f) * 2);
        }
        else
        {
            bar.GetComponent<Image>().color = Color.Lerp(c1, c2, percent*2);
        }
    }
}
