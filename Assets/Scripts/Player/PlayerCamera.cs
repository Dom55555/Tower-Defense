using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public float mouseSensitivity = 100f;
    public float maxSpeed = 5f;

    [Header("To Set:")]
    public LayerMask EntityLayer;
    public GameObject enemyInfoPrefab;
    public GameObject towerInfoPrefab;


    private float xRotation = 0f;
    private float yRotation = 0f;

    private float speed = 0;
    private GameObject chosenEntityInfo = null;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(CheckEntityInfo());
    }

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

        if (!UIFunctions.instance.freezeCameraPosition)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 moveInput = new Vector3(horizontal, 0, vertical).normalized;
            speed = moveInput.magnitude > 0 ? maxSpeed : 0;

            Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.z;
            transform.position += moveDirection * speed * Time.deltaTime;

            Vector3 pos = transform.position;
            pos.y = Mathf.Clamp(pos.y, 3f, 10f);
            pos.x = Mathf.Clamp(pos.x, -5f, 32f);
            pos.z = Mathf.Clamp(pos.z, -20f, 32f);
            transform.position = pos;
        }
        if(Input.GetMouseButtonDown(0)&&chosenEntityInfo!=null && chosenEntityInfo.GetComponent<EntityUI>().entityTransform.CompareTag("Tower"))
        {
            UIFunctions.instance.ToggleTowerInfo(true);
            TowerManager.instance.TowerSelected(chosenEntityInfo.GetComponent<EntityUI>().entityTransform.GetComponent<Tower>());
        }
        
    }

    IEnumerator CheckEntityInfo()
    {
        while (true)
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, EntityLayer))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    if (chosenEntityInfo == null || chosenEntityInfo.GetComponent<EntityUI>().entityTransform != hit.collider.transform.parent)
                    {
                        if (chosenEntityInfo != null) Destroy(chosenEntityInfo);
                        chosenEntityInfo = Instantiate(enemyInfoPrefab, hit.collider.transform.position, Quaternion.identity);
                        chosenEntityInfo.GetComponent<EntityUI>().entityTransform = hit.collider.transform.parent;
                    }
                }
                else if (hit.collider.CompareTag("Tower"))
                {
                    if (chosenEntityInfo == null || chosenEntityInfo.GetComponent<EntityUI>().entityTransform != hit.collider.transform.parent)
                    {
                        if (chosenEntityInfo != null)
                        {
                            var placement = chosenEntityInfo.GetComponent<EntityUI>().entityTransform.Find("Placement");
                            if (placement != null)
                                placement.GetComponent<MeshRenderer>().enabled = false;

                            Destroy(chosenEntityInfo);
                        }
                        chosenEntityInfo = Instantiate(towerInfoPrefab, hit.collider.transform.position, Quaternion.identity);
                        chosenEntityInfo.GetComponent<EntityUI>().entityTransform = hit.collider.transform.parent;

                        var newPlacement = chosenEntityInfo.GetComponent<EntityUI>().entityTransform.Find("Placement");
                        if (newPlacement != null)
                            newPlacement.GetComponent<MeshRenderer>().enabled = true;
                    }
                }
            }
            else
            {
                if (chosenEntityInfo != null)
                {
                    if (chosenEntityInfo.GetComponent<EntityUI>().entityTransform != null)
                    {
                        Transform placement = chosenEntityInfo.GetComponent<EntityUI>().entityTransform.Find("Placement");
                        if (placement != null)
                            placement.GetComponent<MeshRenderer>().enabled = false;
                    }
                    Destroy(chosenEntityInfo);
                    chosenEntityInfo = null;
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }
}