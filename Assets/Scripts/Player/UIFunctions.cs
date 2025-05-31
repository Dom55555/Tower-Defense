using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFunctions : MonoBehaviour
{
    public static UIFunctions instance;
    public GameObject towerInfoPanel;
    public GameObject placingModePanel;
    public bool freezeCameraPosition = false;
    public bool freezeCameraRotation = false;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
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
        freezeCameraRotation = state;
        Cursor.lockState = state?CursorLockMode.None:CursorLockMode.Locked;
    }
    public void TogglePlacingMode(bool state)
    {
        placingModePanel.SetActive(state);
    }
}
