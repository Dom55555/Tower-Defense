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
        public float spawnDelay;
        public float delayAfter; 

    }

    public EnemyInfo[] enemies;
}