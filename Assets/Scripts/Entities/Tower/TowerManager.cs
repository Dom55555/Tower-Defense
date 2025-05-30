using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    [Header("To Set:")]
    public static TowerManager instance;
    public int money = 1000;
    public TMP_Text moneyText;
    public LayerMask placementLayers;
    public TowerData[] towers = new TowerData[5];
    [Header("Tower Info Variables(UI):")]
    public TMP_Text towerName;
    public TMP_Text levelText;
    public TMP_Text priceText;
    public TMP_Text targetText;
    [Header("Current Level Stats Variables:")]
    public TMP_Text dmgText;
    public TMP_Text firerateText;
    public TMP_Text rangeText;
    public TMP_Text hiddensText;
    public TMP_Text flyingsText;
    [Header("Next Level Stats Variables:")]
    public TMP_Text nextDmgText;
    public TMP_Text nextFirerateText;
    public TMP_Text nextRangeText;
    public TMP_Text nextHiddensText;
    public TMP_Text nextFlyingsText;
    [Header("Other Variables:")]
    public bool placingTower = false;
    public Tower chosenTower;


    private GameObject previewTower;
    private Camera mainCam;
    private int selectedIndex = -1;

    void Start()
    {
        instance = this;
        mainCam = Camera.main;
    }

    void Update()
    {
        if (!placingTower)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) selectedIndex = 0;
            else if (Input.GetKeyDown(KeyCode.Alpha2)) selectedIndex = 1;

            if (selectedIndex != -1 && money >= towers[selectedIndex].placePrice)
            {
                SetPreviewTower();
            }
        }

        if (placingTower)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && selectedIndex != 0)
            {
                CancelPlacement();
                selectedIndex = 0;
                SetPreviewTower();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2)&& selectedIndex!=1)
            {
                CancelPlacement();
                selectedIndex = 1;
                SetPreviewTower();
            }
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
        moneyText.text = money+"$";
    }
    private void SetPreviewTower()
    {
        placingTower = true;
        previewTower = Instantiate(towers[selectedIndex].towerPrefab);
        previewTower.GetComponent<Tower>().justPlaced = false;
        UIFunctions.instance.ToggleTowerInfo(false);
        TowerDeselected();
        SetLayerRecursive(previewTower.transform, LayerMask.NameToLayer("Ignore Raycast"));
    }
    private void PreviewPlacement()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, placementLayers))
        {
            previewTower.transform.position = hit.point;
        }
    }

    private void TryPlaceTower()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f,placementLayers))
        {
            PlacementCheck check = previewTower.transform.Find("Placement").GetComponent<PlacementCheck>();
            if (check != null && check.IsValidPlacement)
            {
                GameObject realTower = Instantiate(towers[selectedIndex].towerPrefab, hit.point, Quaternion.identity);
                realTower.GetComponent<Tower>().justPlaced = true;
                money -= towers[selectedIndex].placePrice;
                CancelPlacement();
            }
        }
    }
    private void CancelPlacement()
    {
        if (previewTower != null) Destroy(previewTower);
        placingTower = false;
        selectedIndex = -1;
    }
    private void SetLayerRecursive(Transform obj, int layer)
    {
        obj.gameObject.layer = layer;
        foreach (Transform child in obj)
        {
            SetLayerRecursive(child, layer);
        }
    }
    public void UpgradeTower()
    {
        if (chosenTower.level == 5) return;
        TowerData.TowerLevel nextLevelInfo = chosenTower.towerInfo.levels[chosenTower.level];
        if (money >= nextLevelInfo.price)
        {
            money -= nextLevelInfo.price;
            chosenTower.transform.Find("Collider").localScale = new Vector3(nextLevelInfo.range,6,nextLevelInfo.range);
            chosenTower.transform.Find("Range").localScale=new Vector3(nextLevelInfo.range, 0.1f, nextLevelInfo.range);
            chosenTower.damage = nextLevelInfo.damage;
            chosenTower.firerate = nextLevelInfo.firerate;
            chosenTower.canSeeHiddens = nextLevelInfo.seeHidden;
            chosenTower.canSeeFlyings = nextLevelInfo.seeFlying;
            chosenTower.level++;
            SetInfoVariables();
        }
    }
    public void TowerSelected(Tower selectedTower)
    {
        if(chosenTower!=selectedTower && chosenTower!=null)
        {
            chosenTower.transform.Find("Range").gameObject.SetActive(false);
        }
        chosenTower = selectedTower;
        chosenTower.transform.Find("Range").gameObject.SetActive(true);
        SetInfoVariables();
    }
    public void TowerDeselected()
    {
        if (chosenTower == null) return;
        chosenTower.transform.Find("Range").gameObject.SetActive(false);
        chosenTower = null;
    }
    private void SetInfoVariables()
    {
        towerName.text = chosenTower.towerName;
        levelText.text = chosenTower.level + " Level";
        TowerData.TowerLevel levelInfo = chosenTower.towerInfo.levels[chosenTower.level - 1];
        dmgText.text = levelInfo.damage+"DMG";
        firerateText.text = levelInfo.firerate + " S";
        rangeText.text = levelInfo.range.ToString();
        hiddensText.text = levelInfo.seeHidden ? "Yes" : "No";
        hiddensText.color = levelInfo.seeHidden ? Color.green : Color.red;
        flyingsText.text = levelInfo.seeFlying ? "Yes" : "No";
        flyingsText.color = levelInfo.seeFlying ? Color.green : Color.red;

        if (chosenTower.level<5)
        {
            levelInfo = chosenTower.towerInfo.levels[chosenTower.level];
            nextDmgText.text = levelInfo.damage + " DMG";
            nextFirerateText.text = levelInfo.firerate + " S";
            nextRangeText.text = levelInfo.range.ToString();
            nextHiddensText.text = levelInfo.seeHidden ? "Yes" : "No";
            nextHiddensText.color = levelInfo.seeHidden ?Color.green:Color.red;
            nextFlyingsText.text = levelInfo.seeFlying ? "Yes" : "No";
            nextFlyingsText.color = levelInfo.seeFlying ? Color.green : Color.red;
            priceText.text = levelInfo.price + "$";
        }
        else
        {
            nextDmgText.text = "";
            nextFirerateText.text = "";
            nextRangeText.text = "";
            nextHiddensText.text = "";
            nextFlyingsText.text = "";
            priceText.text = "MAXED";
        }

    }
    public void DeleteTower()
    {
        Destroy(chosenTower.gameObject);
        chosenTower = null;
        UIFunctions.instance.ToggleTowerInfo(false);
    }
    public void ChangeTargetMode()
    {
        chosenTower.mode = chosenTower.mode == "First" ? "Strongest" : "First";
        targetText.text = chosenTower.mode;
    }
}