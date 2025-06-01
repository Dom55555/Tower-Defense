using UnityEngine;
using TMPro;
using UnityEngine.UI;

public abstract class EntityUI : MonoBehaviour
{
    public TMP_Text nameText;
    public Transform entityTransform;

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        
    }
    protected virtual void Bar(RectTransform bar, float maxSizeX, float minX, int valueCurrent, int valueMax, Color c1, Color c2, Color c3)
    {
        if (valueMax == 0)
        {
            return;
        }
        float percent = (float)valueCurrent / valueMax;
        Vector2 barSize = bar.sizeDelta;
        barSize.x = percent * maxSizeX;
        bar.sizeDelta = barSize;
        bar.localPosition = new Vector2(minX + percent * maxSizeX / 200, bar.localPosition.y);
        if (percent >= 0.5f)
        {
            bar.GetComponent<Image>().color = Color.Lerp(c2, c3, (percent - 0.5f) * 2);
        }
        else
        {
            bar.GetComponent<Image>().color = Color.Lerp(c1, c2, percent * 2);
        }
    }
}