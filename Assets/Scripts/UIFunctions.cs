using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFunctions : MonoBehaviour
{
    public static UIFunctions instance;
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
        if(Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        if(Input.GetKeyDown(KeyCode.T))
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
    }
}
