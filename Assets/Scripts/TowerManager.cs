using UnityEngine;

public class TowerManager : MonoBehaviour
{
    [Header("To Set:")]
    public int money = 1000;
    public TowerData[] towers = new TowerData[5];
    private bool placingTower = false;
    private GameObject previewTower;
    private Camera mainCam;

    public LayerMask placementLayers;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        if (!placingTower && Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (money >= towers[0].placePrice)
            {
                placingTower = true;
                previewTower = Instantiate(towers[0].towerPrefab);
                previewTower.GetComponent<Tower>().justPlaced = false;
                SetLayerRecursive(previewTower.transform, LayerMask.NameToLayer("Ignore Raycast"));
            }
        }

        if (placingTower)
        {
            PreviewPlacement();

            if (Input.GetMouseButtonDown(0))
            {
                TryPlaceTower();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                CancelPlacement();
            }
        }
    }

    void PreviewPlacement()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, placementLayers))
        {
            previewTower.transform.position = hit.point;
        }
    }

    void TryPlaceTower()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f,placementLayers))
        {
            PlacementCheck check = previewTower.transform.Find("Placement").GetComponent<PlacementCheck>();
            if (check != null && check.IsValidPlacement)
            {
                GameObject realTower = Instantiate(towers[0].towerPrefab, hit.point, Quaternion.identity);
                realTower.GetComponent<Tower>().justPlaced = true;
                money -= towers[0].placePrice;
                CancelPlacement();
            }
            else
            {
                Debug.Log("Invalid placement");
            }
        }
    }

    void CancelPlacement()
    {
        if (previewTower != null) Destroy(previewTower);
        placingTower = false;
    }

    void SetLayerRecursive(Transform obj, int layer)
    {
        obj.gameObject.layer = layer;
        foreach (Transform child in obj)
        {
            SetLayerRecursive(child, layer);
        }
    }
}