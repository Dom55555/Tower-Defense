using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIFunctions : MonoBehaviour
{
    public static UIFunctions instance;
    public Transform loadoutTowers;
    public GameObject placingModePanel;
    public GameObject towerInfoPanel;

    public GameObject firerateBuffIcon;
    public GameObject rangeBuffIcon;
    public GameObject discountBuffIcon;

    public GameObject farmInfoPanel;
    public GameObject patrolInfoPanel;
    public GameObject commanderInfoPanel;
    public GameObject djInfoPanel;

    public TMP_Text patrolTimerText;
    public TMP_Text commanderTimerText;

    public TMP_Text waveMoneyText;
    public TMP_Text baseHpText;
    public GameObject defeatPanel;
    public GameObject victoryPanel;
    public Image hpFillBar;

    public AudioSource gameAudioSource;
    public AudioClip getMoneySound;
    public AudioClip baseDamageSound;
    public AudioClip victorySound;
    public AudioClip defeatSound;

    public bool freezeCameraPosition = false;
    public bool freezeCameraRotation = false;

    private Tower chosenTower;
    void Start()
    {
        instance = this;
        int index = 0;
        foreach (Transform item in loadoutTowers)
        {
            if (TowerManager.instance.towers[index] != null)
            {
                item.GetChild(0).GetComponent<Image>().sprite = TowerManager.instance.towers[index].towerIcon;
                item.GetChild(1).GetComponent<TMP_Text>().text = TowerManager.instance.towers[index].placePrice+"$";
            }
            else item.GetChild(0).gameObject.SetActive(false);
            index++;
        }
        UpdateHPBar(100);
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
        if (chosenTower != null)
        {
            if (chosenTower.towerName == "Patrol")
            {
                patrolTimerText.text = chosenTower.timer + " S";
            }
            else if (chosenTower.towerName == "Commander")
            {
                if (chosenTower.timer == 0)
                {
                    commanderTimerText.text = "READY";
                    commanderTimerText.fontSize = 17;
                }
                else
                {
                    commanderTimerText.text = chosenTower.timer.ToString();
                    commanderTimerText.fontSize = 25;
                }
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
        commanderInfoPanel.SetActive(false);
        djInfoPanel.SetActive(false);

        firerateBuffIcon.SetActive(false);
        rangeBuffIcon.SetActive(false);
        discountBuffIcon.SetActive(false);
        freezeCameraRotation = state;
        Cursor.lockState = state?CursorLockMode.None:CursorLockMode.Locked;
    }
    public void ShowExtraTowerInfo(Tower tower)
    {
        chosenTower = tower;
        if(tower.towerName=="Farm")
        {
            farmInfoPanel.SetActive(true);
        }
        else if(tower.towerName=="Patrol")
        {
            patrolInfoPanel.SetActive(true);
        }
        else if (tower.towerName == "Commander")
        {
            commanderInfoPanel.SetActive(true);
        }
        else if (tower.towerName=="DJ")
        {
            djInfoPanel.SetActive(true);
        }
        if (tower.firerateMult < 1) firerateBuffIcon.SetActive(true);
        if (tower.rangeMult > 1) rangeBuffIcon.SetActive(true);
        if (tower.priceMult < 1) discountBuffIcon.SetActive(true);
    }
    public void TogglePlacingMode(bool state)
    {
        placingModePanel.SetActive(state);
    }
    public IEnumerator WaveMoney(int number)
    {
        waveMoneyText.gameObject.SetActive(true);
        waveMoneyText.transform.localPosition = new Vector2(0,82);
        waveMoneyText.text = "+" + number + "$";
        float timer = 0;
        PlaySound(getMoneySound);
        while(timer<1)
        {
            waveMoneyText.transform.localPosition = new Vector2(0,82+23*timer);
            waveMoneyText.color = new Color(0.34f,1,0.26f,1-timer);
            timer += Time.deltaTime;
            yield return null;
        }
        waveMoneyText.gameObject.SetActive(false);
    }
    public static bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
    public string FormatNumber(int number)
    {
        if (number >= 1000000)
        {
            float result = number / 1000000f;
            return result.ToString("0.#") + "M";
        }
        if (number >= 1000)
        {
            float result = number / 1000f;
            return result.ToString("0.#") + "K";
        }
        return number.ToString();
    }
    public void UpdateHPBar(int hp)
    {
        baseHpText.text = Mathf.Clamp(hp,0,100).ToString();
        float hpPercent = (float)hp / 100;
        hpFillBar.GetComponent<RectTransform>().sizeDelta = new Vector2(hpPercent*200,22.6f);
        if (hpPercent > 0.5f) hpFillBar.color = Color.Lerp(new Color(0.95f,0.9f,0.1f), new Color(0.27f,0.81f,0.12f), (hpPercent - 0.5f) / 0.5f);
        else hpFillBar.color = Color.Lerp(Color.red, new Color(0.95f, 0.9f, 0.1f), hpPercent / 0.5f);

        if(hp<=0)
        {
            Defeat();
            var enemies = WaveManager.instance.enemiesOnMap;
            enemies.RemoveAll(x=>x==null);
            foreach (var enemy in enemies)
            {
                enemy.Die();
            }
        }
    }
    public void Victory()
    {
        PlaySound(victorySound);
        victoryPanel.SetActive(true);
        victoryPanel.transform.GetChild(0).Find("TokensText").GetComponent<TMP_Text>().text = "+" + 250;
        victoryPanel.transform.GetChild(0).Find("WaveText").GetComponent<TMP_Text>().text = "WAVE 40";
        PlayerPrefs.SetInt("Tokens", PlayerPrefs.GetInt("Tokens")+250);
        PlayerPrefs.Save();
        StartCoroutine(ToMenu());
    }
    public void Defeat()
    {
        PlaySound(defeatSound);
        defeatPanel.SetActive(true);
        int tokens = Mathf.RoundToInt(150 * WaveManager.instance.wave / 40f);
        defeatPanel.transform.GetChild(0).Find("TokensText").GetComponent<TMP_Text>().text = "+" + tokens;
        defeatPanel.transform.GetChild(0).Find("WaveText").GetComponent<TMP_Text>().text = "WAVE " + WaveManager.instance.wave;
        PlayerPrefs.SetInt("Tokens", PlayerPrefs.GetInt("Tokens") + tokens);
        PlayerPrefs.Save();
        StartCoroutine(ToMenu());
    }
    private IEnumerator ToMenu()
    {
        yield return new WaitForSeconds(6);
        SceneManager.LoadScene("StartMenu");
    }
    public void PlaySound(AudioClip clip)
    {
        gameAudioSource.PlayOneShot(clip);
    }
}
