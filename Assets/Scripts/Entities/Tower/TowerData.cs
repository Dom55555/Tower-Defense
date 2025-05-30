using UnityEngine;

[CreateAssetMenu(fileName = "Towers", menuName = "Tower/TowerData")]
public class TowerData : ScriptableObject
{
    public string towerName;
    public int placePrice;
    public GameObject towerPrefab;
    public GameObject extraPrefab;
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
