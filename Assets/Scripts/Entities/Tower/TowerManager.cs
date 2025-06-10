using System.Runtime.CompilerServices;
using TMPro;
using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public class TowerManager : MonoBehaviour
{
    public static TowerManager instance;
    [Header("To Set:")]
    public int money = 1000;
    public TowerData[] allTowers;
    public LayerMask placementLayers;
    public TMP_Text moneyText;
    public Transform wayPointsObject;
    public TMP_Text towersAmountText;
    [Header("Sounds:")]
    public AudioClip upgradeSound;
    public AudioClip deleteSound;
    public AudioClip placeSound;
    public AudioClip pickSound;
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
    [Header("Extra Info Panel Variables")]
    public TMP_Text moneyPerWaveText;
    public TMP_Text firerateBuffText;
    public TMP_Text rangeBuffText;
    public TMP_Text discountBuffText;
    [Header("Other Variables:")]
    public Tower chosenTower;
    public bool placingTower = false;
    public int moneyPerWave = 0;
    public TowerData[] towers = new TowerData[5];

    private Transform[] wayPoints;
    private int towersAmount = 0;
    private GameObject previewTower;
    private Camera mainCam;
    private int[] farmIncomeByLevel = { 50, 50, 100, 300, 1000 };
    private int[] firerateBuffs = {10,15,20,25,35};
    private int[] rangeBuffs = { 10, 15, 20, 20, 30 };
    private int[] discountBuffs = { 0, 0, 0, 10, 20 };
    private bool DJPlaced = false;
    private int selectedIndex = -1;

    void Start()
    {
        instance = this;
        mainCam = Camera.main;
        wayPoints = new Transform[wayPointsObject.childCount];
        for (int i = wayPointsObject.childCount-1; i >=0; i--)
        {
            wayPoints[wayPointsObject.childCount-i-1] = wayPointsObject.GetChild(i);
        }
        for (int i = 0; i < 5; i++)
        {
            if (PlayerPrefs.GetString("Tower" + i) != "none") towers[i] = allTowers.FirstOrDefault(x => x.towerName == PlayerPrefs.GetString("Tower" + i));
            else towers[i] = null;
        }
    }

    void Update()
    {
        moneyText.text = money + "$";
        for (int i = 0; i < 5; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) && towers[i]!=null && (towers[i].towerName != "DJ" || (towers[i].towerName=="DJ"&&!DJPlaced)))
            {
                if (!placingTower)
                {
                    selectedIndex = i;
                    if (money >= towers[selectedIndex].placePrice) SetPreviewTower();
                }
                else if (selectedIndex != i)
                {
                    CancelPlacement();
                    selectedIndex = i;
                    if (money >= towers[selectedIndex].placePrice) SetPreviewTower();
                }
                break;
            }
        }
        if (placingTower)
        {
            PreviewPlacement();
            if (Input.GetMouseButtonDown(0) && towersAmount<15) TryPlaceTower();
            else if (Input.GetMouseButtonDown(1)) CancelPlacement();
        }
    }
    private void SetPreviewTower()
    {
        placingTower = true;
        previewTower = Instantiate(towers[selectedIndex].towerPrefab);
        previewTower.GetComponent<Tower>().justPlaced = false;
        UIFunctions.instance.ToggleTowerInfo(false);
        UIFunctions.instance.TogglePlacingMode(true);
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
                towersAmount++;
                towersAmountText.text = towersAmount + "/15 Towers";
                GameObject realTower = Instantiate(towers[selectedIndex].towerPrefab, hit.point, Quaternion.identity);
                Destroy(realTower.transform.Find("Placement").GetComponent<PlacementCheck>());
                Tower towerComponent = realTower.GetComponent<Tower>();
                UIFunctions.instance.PlaySound(placeSound);
                towerComponent.justPlaced = true;
                towerComponent.range = towers[selectedIndex].levels[0].range;
                towerComponent.damage = towers[selectedIndex].levels[0].damage;
                towerComponent.firerate = towers[selectedIndex].levels[0].firerate;
                towerComponent.canSeeHiddens = towers[selectedIndex].levels[0].seeHidden;
                towerComponent.canSeeFlyings = towers[selectedIndex].levels[0].seeFlying;
                realTower.transform.Find("Placement").GetComponent<MeshRenderer>().enabled = false;
                money -= towers[selectedIndex].placePrice;
                if (towerComponent.towerName == "Farm") moneyPerWave += 50;
                else if (towerComponent.towerName == "DJ") DJPlaced = true;
                CancelPlacement();
            }
        }
    }
    private void CancelPlacement()
    {
        if (previewTower != null) Destroy(previewTower);
        placingTower = false;
        selectedIndex = -1;
        UIFunctions.instance.TogglePlacingMode(false);
    }
    private void SetLayerRecursive(Transform obj, int layer)
    {
        obj.gameObject.layer = layer;
        foreach (Transform child in obj) SetLayerRecursive(child, layer);
    }
    public void UpgradeTower()
    {
        if (chosenTower.level == 5) return;
        TowerData.TowerLevel nextLevelInfo = towers.FirstOrDefault(x => x!=null&&x.towerName == chosenTower.towerName).levels[chosenTower.level];
        if (money >= Mathf.RoundToInt(nextLevelInfo.price*chosenTower.priceMult))
        {
            UIFunctions.instance.PlaySound(upgradeSound);
            money -= Mathf.RoundToInt(nextLevelInfo.price*chosenTower.priceMult);
            chosenTower.range = nextLevelInfo.range;
            chosenTower.damage = nextLevelInfo.damage;
            chosenTower.firerate = nextLevelInfo.firerate;
            chosenTower.canSeeHiddens = nextLevelInfo.seeHidden;
            chosenTower.canSeeFlyings = nextLevelInfo.seeFlying;
            chosenTower.level++;
            chosenTower.ChangeBuffsValues();
            SetInfoVariables();
            SetExtraInfoVariables(true);
        }
    }
    public void TowerSelected(Tower selectedTower)
    {
        if (chosenTower != selectedTower)
        {
            if(chosenTower!=null)
            {
                chosenTower.transform.Find("Range").gameObject.SetActive(false);
                ToggleBuffText(false);
            }
            UIFunctions.instance.PlaySound(pickSound);
            chosenTower = selectedTower;
            chosenTower.transform.Find("Range").gameObject.SetActive(true);
            ToggleBuffText(chosenTower.towerName=="Commander" || chosenTower.towerName=="DJ");
            SetInfoVariables();
            SetExtraInfoVariables(false);
        }

    }
    public void TowerDeselected()
    {
        if (chosenTower == null) return;
        chosenTower.transform.Find("Range").gameObject.SetActive(false);
        if(chosenTower.towerName=="Commander" || chosenTower.towerName=="DJ") ToggleBuffText(false);
        chosenTower = null;
    }
    private void SetInfoVariables()
    {
        towerName.text = chosenTower.towerName;
        levelText.text = chosenTower.level + " Level";
        TowerData.TowerLevel levelInfo = towers.FirstOrDefault(x => x!=null&&x.towerName == chosenTower.towerName).levels[chosenTower.level-1];
        dmgText.text = levelInfo.damage+" DMG";
        firerateText.text = levelInfo.firerate + " S";
        rangeText.text = levelInfo.range.ToString();
        hiddensText.text = levelInfo.seeHidden ? "Yes" : "No";
        hiddensText.color = levelInfo.seeHidden ? Color.green : Color.red;
        flyingsText.text = levelInfo.seeFlying ? "Yes" : "No";
        flyingsText.color = levelInfo.seeFlying ? Color.green : Color.red;

        if (chosenTower.level<5)
        {
            levelInfo = towers.FirstOrDefault(x => x!=null && x.towerName == chosenTower.towerName).levels[chosenTower.level];
            nextDmgText.text = levelInfo.damage + " DMG";
            nextFirerateText.text = levelInfo.firerate + " S";
            nextRangeText.text = levelInfo.range.ToString();
            nextHiddensText.text = levelInfo.seeHidden ? "Yes" : "No";
            nextHiddensText.color = levelInfo.seeHidden ?Color.green:Color.red;
            nextFlyingsText.text = levelInfo.seeFlying ? "Yes" : "No";
            nextFlyingsText.color = levelInfo.seeFlying ? Color.green : Color.red;
            priceText.text = Mathf.RoundToInt(levelInfo.price*chosenTower.priceMult) + "$";
        }
        else
        {
            nextDmgText.text = nextFirerateText.text = nextRangeText.text = nextHiddensText.text = nextFlyingsText.text = "";
            priceText.text = "MAXED";
        }
    }
    public void SetExtraInfoVariables(bool changeValues)
    {
        if (chosenTower.towerName == "Farm")
        {
            moneyPerWaveText.text = "+" + farmIncomeByLevel.Take(chosenTower.level).Sum()+ "$";
            if (changeValues) moneyPerWave += farmIncomeByLevel[chosenTower.level - 1];
        }
        if(chosenTower.towerName == "Commander")
        {
            firerateBuffText.text = "+" + firerateBuffs[chosenTower.level - 1] + "%";
        }
        if(chosenTower.towerName=="DJ")
        {
            rangeBuffText.text = "+" + rangeBuffs[chosenTower.level - 1] + "%";
            discountBuffText.text = "-" + discountBuffs[chosenTower.level - 1] + "%";
        }
    }
    public void DeleteTower()
    {
        if (chosenTower.towerName == "Farm")
        {
            moneyPerWave -= farmIncomeByLevel.Take(chosenTower.level).Sum();
        }
        else if(chosenTower.towerName=="Commander")
        {
            foreach (var tower in chosenTower.towersInRange) tower.firerateMult = 1;
            ToggleBuffText(false);
        }
        else if (chosenTower.towerName=="DJ")
        {
            DJPlaced = false;
            foreach (var tower in chosenTower.towersInRange)
            {
                tower.rangeMult = 1;
                tower.priceMult = 1;
                tower.ChangeBuffsValues();
            }
            ToggleBuffText(false);
        }
        money += towers.FirstOrDefault(x => x != null && x.towerName == chosenTower.towerName).placePrice/2;
        UIFunctions.instance.PlaySound(deleteSound);
        towersAmount--;
        towersAmountText.text = towersAmount + "/15 Towers";
        Destroy(chosenTower.gameObject);
        chosenTower = null;
        UIFunctions.instance.ToggleTowerInfo(false);
    }
    public void ChangeTargetMode()
    {
        chosenTower.mode = chosenTower.mode == "First" ? "Strongest" : "First";
        targetText.text = chosenTower.mode;
    }
    public void WaveMoney()
    {
        money += moneyPerWave;
    }
    public void SpawnPatrolCar(int level)
    {
        var patrolInfo = towers.FirstOrDefault(x => x.towerName == "Patrol");
        GameObject patrolPrefab = patrolInfo.extraPrefab;
        GameObject patrolCar = Instantiate(patrolPrefab, wayPoints[0].position,Quaternion.identity);
        Tower towerComponent = patrolCar.GetComponent<Tower>();
        towerComponent.wayPoints = wayPoints;
        towerComponent.justPlaced = true;
        towerComponent.level = level;
        towerComponent.range = 12;
        towerComponent.damage = patrolInfo.levels[level-1].damage;
        towerComponent.firerate = level==4?0.18f:0.12f;
        towerComponent.canSeeHiddens = level==4?false:true;
        towerComponent.canSeeFlyings = true;
    }
    private void ToggleBuffText(bool state)
    {
        foreach (var tower in chosenTower.towersInRange)
        {
            Transform textObject;
            if(chosenTower.towerName=="Commander")
            {
                textObject = tower.transform.Find("FirerateBuff");
                textObject.gameObject.SetActive(state);
            }
            else if (chosenTower.towerName=="DJ")
            {
                textObject = tower.transform.Find("RangeBuff");
                if(textObject!=null) textObject.gameObject.SetActive(state);
                textObject = tower.transform.Find("DiscountBuff");
                textObject.gameObject.SetActive(state);
            }
        }
    }
    public void UseAbility()
    {
        if (chosenTower.timer > 0) return;
        chosenTower.animator.Play("Ability", -1, 0f);
        chosenTower.timer = 30;
        StartCoroutine(AbilityTimer(chosenTower));
        foreach (var tower in chosenTower.towersInRange)
        {
            if(tower!=null)
            {
                tower.firerateMult = 1 / (1 + 2 * (firerateBuffs[chosenTower.level - 1] / 100f));
                tower.transform.Find("AbilityRing").gameObject.SetActive(true);
            }
        }
        
    }
    private IEnumerator AbilityTimer(Tower abilityTower)
    {
        yield return new WaitForSeconds(10);
        if (abilityTower != null)
        {
            foreach (var tower in abilityTower.towersInRange)
            {
                if (tower != null)
                {
                    tower.firerateMult = 1 / (1 + firerateBuffs[abilityTower.level - 1] / 100f);
                    tower.transform.Find("AbilityRing").gameObject.SetActive(false);
                }
            }
        }
    }
}