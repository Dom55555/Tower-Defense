using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Towers", menuName = "Tower/TowerData")]
public class TowerData : ScriptableObject
{
    public string towerName;
    public Sprite towerIcon;
    public bool unlocked;
    public int unlockPrice;
    public int placePrice;
    public GameObject towerPrefab;
    public GameObject extraPrefab;
    public AudioClip shootSound;
    public AudioClip extraSound1;
    public AudioClip extraSound2;
    [System.Serializable]
    public class TowerLevel
    {
        public int price;
        public float range;
        public int damage;
        public float firerate;
        public bool seeHidden;
        public bool seeFlying;
    }
    public TowerLevel[] levels;
}
