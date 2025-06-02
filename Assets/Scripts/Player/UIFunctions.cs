using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIFunctions : MonoBehaviour
{
    public static UIFunctions instance;
    public GameObject placingModePanel;
    public GameObject towerInfoPanel;
    public GameObject farmInfoPanel;
    public GameObject patrolInfoPanel;
    public TMP_Text patrolTimerText;
    public TMP_Text waveMoneyText;
    public bool freezeCameraPosition = false;
    public bool freezeCameraRotation = false;

    private Tower chosenTower;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        if(Input.GetKeyDown(KeyCode.T) && !TowerManager.instance.placingTower && !TowerManager.instance.chosenTower)
        {
            ToggleCameraAbove();
        }
        if(chosenTower!=null)
        {
            if(chosenTower.towerName=="Patrol")
            {
                patrolTimerText.text = chosenTower.timer + " S";
            }
            else if (chosenTower.towerName == "Commander")
            {
                //
            }
        }
    }
    public void ToggleCameraAbove()
    {
        freezeCameraPosition = !freezeCameraPosition;
        freezeCameraRotation = !freezeCameraRotation;
        if (freezeCameraPosition)
        {
            Camera.main.transform.position = new Vector3(14.5f,31,0);
            Camera.main.transform.rotation = Quaternion.Euler(90,180,-90);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    public void ToggleTowerInfo(bool state)
    {
        towerInfoPanel.SetActive(state);
        farmInfoPanel.SetActive(false);
        patrolInfoPanel.SetActive(false);
        freezeCameraRotation = state;
        Cursor.lockState = state?CursorLockMode.None:CursorLockMode.Locked;
    }
    public void ShowExtraTowerInfo(string type, Tower tower=null)
    {
        if(type=="Farm")
        {
            farmInfoPanel.SetActive(true);
        }
        chosenTower = tower;
        if(type=="Patrol")
        {
            patrolInfoPanel.SetActive(true);
        }
    }
    public void TogglePlacingMode(bool state)
    {
        placingModePanel.SetActive(state);
    }
    public IEnumerator WaveMoney()
    {
        waveMoneyText.gameObject.SetActive(true);
        waveMoneyText.transform.localPosition = new Vector2(0,82);
        waveMoneyText.text = "+" + TowerManager.instance.moneyPerWave + "$";
        float timer = 0;
        while(timer<1)
        {
            waveMoneyText.transform.localPosition = new Vector2(0,82+23*timer);
            waveMoneyText.color = new Color(0.34f,1,0.26f,1-timer);
            timer += Time.deltaTime;
            yield return null;
        }
        waveMoneyText.gameObject.SetActive(false);
    }
}
