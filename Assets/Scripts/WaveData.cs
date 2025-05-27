using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "Waves/WaveData")]
public class WaveData : ScriptableObject
{
    [System.Serializable]
    public class EnemyInfo
    {
        public GameObject enemyPrefab;
        public string name;
        public string status;
        public float speed;
        public int hp;
        public int shieldHp;
        public int amount;
        public float spawnDelay; // delay between this enemy type's spawns
        public float delayAfter; // delay before switching to the next type

    }

    public EnemyInfo[] enemies;
}