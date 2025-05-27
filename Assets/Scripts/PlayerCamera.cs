using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public float maxSpeed = 5f;

    public LayerMask enemyLayer;
    public GameObject enemyInfoPrefab;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private float speed = 0;
    private GameObject chosenEnemyInfo = null;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!UIFunctions.instance.freezeCameraRotation)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            yRotation += mouseX;
            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
        if(!UIFunctions.instance.freezeCameraPosition)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");     
            Vector3 moveInput = new Vector3(horizontal, 0, vertical).normalized;
            if (moveInput.magnitude > 0) speed = maxSpeed;
            else speed = 0;
            Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.z;
            transform.position += moveDirection * speed * Time.deltaTime;
            Vector3 pos = transform.position;
            pos.y = Mathf.Clamp(pos.y, 3f, 10f);
            pos.x = Mathf.Clamp(pos.x,-5f,32f);
            pos.z = Mathf.Clamp(pos.z,-20f,32f);
            transform.position = pos;
        }
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f,enemyLayer))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                if(chosenEnemyInfo==null || chosenEnemyInfo.GetComponent<EnemyUI>().enemyTransform != hit.collider.transform.parent)
                {
                    if (chosenEnemyInfo != null) Destroy(chosenEnemyInfo);
                    Destroy(chosenEnemyInfo);
                    chosenEnemyInfo = Instantiate(enemyInfoPrefab,hit.collider.transform.position,Quaternion.identity);
                    chosenEnemyInfo.GetComponent<EnemyUI>().enemyTransform = hit.collider.transform.parent;
                }
            }
        }
        else
        {
            if(chosenEnemyInfo!=null)
            {
                Destroy(chosenEnemyInfo);
                chosenEnemyInfo = null;
            }
        }

    }
}
