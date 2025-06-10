using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuUI : MonoBehaviour
{
    [Header("To Set")]
    public TowerData[] towers;
    public GameObject towersMenu;
    public GameObject playBtn;
    public GameObject towersBtn;
    public GameObject gameName;
    public GameObject towerInfoPanel;
    public Image chosenTowerImage;
    public TMP_Text chosenTowerName;
    public TMP_Text tokensText;
    public Button buyBtn;
    public Button equipBtn;
    public Image[] loadoutTowers;
    public GameObject[] towersButtons;

    [Header("Other Variables")]
    public List<TowerData> loadout = new List<TowerData>();


    private int tokens;
    private TowerData chosenTower;
    private int selectedIndex = -1;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        if (PlayerPrefs.HasKey("Tokens"))
        {
            tokens = PlayerPrefs.GetInt("Tokens");
        }
        else
        {
            PlayerPrefs.SetInt("Tokens",99999);
            tokens = 99999;
            foreach (TowerData data in towers)
            {
                PlayerPrefs.SetInt(data.towerName, 0);
            }
            PlayerPrefs.SetInt("Scout", 1);
            for (int i = 1; i < 5; i++)
            {
                PlayerPrefs.SetString("Tower"+i,"none");
            }
            PlayerPrefs.SetString("Tower0", "Scout");
        }
        foreach (TowerData data in towers)
        {
            data.unlocked = PlayerPrefs.GetInt(data.towerName) == 1?true:false;
        }
        for (int i = 0; i < 5; i++)
        {
            if (PlayerPrefs.GetString("Tower"+i)!="none")
            {
                loadout.Add(towers.FirstOrDefault(x => x.towerName == PlayerPrefs.GetString("Tower" + i)));
            }
            else
            {
                loadout.Add(null);
            }
        }
        for (int i = 0; i < 5; i++)
        {
            if(PlayerPrefs.GetString("Tower"+i)!="none")
            {
                loadoutTowers[i].transform.GetChild(0).GetComponent<Image>().sprite = towers.FirstOrDefault(x => x.towerName == PlayerPrefs.GetString("Tower"+i)).towerIcon;
                loadoutTowers[i].transform.GetChild(0).gameObject.SetActive(true);
            }
        }
        PlayerPrefs.Save();
    }
    void Update()
    {
        tokensText.text = tokens.ToString();
    }
    public void ToggleTowersMenu(bool state)
    {
        towersMenu.SetActive(state);
        playBtn.SetActive(!state);
        towersBtn.SetActive(!state);
        gameName.SetActive(!state);
    }
    public void OnScroll(float value)
    {
        int i = 0;
        foreach (var item in towersButtons)
        {
            item.transform.localPosition = new Vector2(item.transform.localPosition.x,45-i*115+value);
            i++;
        }
    }
    public void TowerChosen(Image image)
    {
        towerInfoPanel.SetActive(true);
        chosenTowerImage.sprite = image.sprite;
        chosenTowerName.text = image.sprite.name;
        chosenTower = towers.FirstOrDefault(x => x.towerName == image.sprite.name);
        buyBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = chosenTower.unlockPrice.ToString();
        buyBtn.gameObject.SetActive(!chosenTower.unlocked);
        equipBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = loadout.Contains(chosenTower)?"UNEQUIP":"EQUIP";
        ButtonColors(!loadout.Contains(chosenTower));
        equipBtn.gameObject.SetActive(chosenTower.unlocked);
    }
    public void UnlockTower()
    {
        if(tokens>=chosenTower.unlockPrice)
        {
            tokens -=chosenTower.unlockPrice;
            PlayerPrefs.SetInt("Tokens", tokens);
            chosenTower.unlocked = true;
            buyBtn.gameObject.SetActive(false);
            equipBtn.gameObject.SetActive(true);
            PlayerPrefs.SetInt(chosenTower.towerName,1);
            PlayerPrefs.Save();
            ButtonColors(true);
        }
    }
    public void EquipTower()
    {
        if(equipBtn.transform.GetChild(0).GetComponent<TMP_Text>().text=="EQUIP")
        {
            ButtonColors(false);
            if(selectedIndex==-1) selectedIndex = loadout.FindIndex(x => x==null);
            
            loadout[selectedIndex] = chosenTower;
            loadoutTowers[selectedIndex].transform.GetChild(0).GetComponent<Image>().sprite = towers.FirstOrDefault(x => x.towerName == chosenTower.towerName).towerIcon;
            loadoutTowers[selectedIndex].transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            ButtonColors(true);
            int index = loadout.FindIndex(x => x != null && x.towerName == chosenTower.towerName);
            loadout[index] = null;
            loadoutTowers[index].transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    private void ButtonColors(bool equip)
    {
        if(!equip)
        {
            equipBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "UNEQUIP";
            equipBtn.GetComponent<Image>().color = new Color(0.97f, 0.24f, 0.23f);
            equipBtn.transform.GetChild(0).GetComponent<TMP_Text>().color = new Color(0.37f, 0.1f, 0.08f);
        }
        else
        {
            equipBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "EQUIP";
            equipBtn.GetComponent<Image>().color = new Color(0.63f, 1f, 0.97f);
            equipBtn.transform.GetChild(0).GetComponent<TMP_Text>().color = new Color(0f, 0.6f, 1f);
        }
    }
    public void LoadoutSlotChosen(int index)
    {
        if(selectedIndex!=-1) loadoutTowers[selectedIndex].color = Color.black;
        selectedIndex = index;
        loadoutTowers[selectedIndex].color = new Color(0.37f,0.75f,0.9f);
    }
    public void Play()
    {
        for (int i = 0; i < 5; i++)
        {
            if (loadout[i] != null) PlayerPrefs.SetString("Tower" + i, loadout[i].towerName);
            else PlayerPrefs.SetString("Tower" + i, "none");
        }
        PlayerPrefs.Save();
        SceneManager.LoadScene("SampleScene");
    }

}
