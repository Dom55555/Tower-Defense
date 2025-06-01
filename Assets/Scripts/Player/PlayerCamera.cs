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
    public GameObject patrolCarInfoPrefab;

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
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        bool allowRotation = !UIFunctions.instance.freezeCameraRotation || Input.GetMouseButton(1);
        if (allowRotation)
        {
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
        if(chosenEntityInfo!=null)
        {
            Transform entityTransform = chosenEntityInfo.GetComponent<EntityUI>().entityTransform;
            if(Input.GetMouseButtonDown(0) && entityTransform.CompareTag("Tower"))
            {
                if (entityTransform.GetComponent<Tower>().towerName == "PatrolCar") return;
                UIFunctions.instance.ToggleTowerInfo(true);
                TowerManager.instance.TowerSelected(chosenEntityInfo.GetComponent<EntityUI>().entityTransform.GetComponent<Tower>());
                string towerName = chosenEntityInfo.GetComponent<EntityUI>().entityTransform.GetComponent<Tower>().towerName;
                if (towerName == "Farm" || towerName == "Patrol") UIFunctions.instance.ShowExtraTowerInfo(towerName, entityTransform.GetComponent<Tower>());
            }
        }
    }

    IEnumerator CheckEntityInfo()
    {
        while (true)
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, EntityLayer))
            {
                if((hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Tower"))&&(chosenEntityInfo == null || chosenEntityInfo.GetComponent<EntityUI>().entityTransform != hit.collider.transform.root))
                {
                    if (chosenEntityInfo != null)
                    {
                        if(chosenEntityInfo.GetComponent<EntityUI>().entityTransform!=null)
                        {
                            var placement = chosenEntityInfo.GetComponent<EntityUI>().entityTransform.Find("Placement");
                            if (placement != null) placement.GetComponent<MeshRenderer>().enabled = false;
                        }
                        Destroy(chosenEntityInfo);
                    }
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        chosenEntityInfo = Instantiate(enemyInfoPrefab, hit.collider.transform.position, Quaternion.identity);
                    }
                    else if (hit.collider.CompareTag("Tower"))
                    {
                        if(hit.collider.transform.root.GetComponent<Tower>().towerName=="PatrolCar") chosenEntityInfo = Instantiate(patrolCarInfoPrefab, hit.collider.transform.root.Find("Placement").position, Quaternion.identity);
                        else chosenEntityInfo = Instantiate(towerInfoPrefab, hit.collider.transform.root.Find("Placement").position, Quaternion.identity);
                    }
                    chosenEntityInfo.GetComponent<EntityUI>().entityTransform = hit.collider.transform.root;
                    var newPlacement = chosenEntityInfo.GetComponent<EntityUI>().entityTransform.Find("Placement");
                    if (newPlacement != null) newPlacement.GetComponent<MeshRenderer>().enabled = true;
                }
                else if (!hit.collider.CompareTag("Enemy") && !hit.collider.CompareTag("Tower"))
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
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
}