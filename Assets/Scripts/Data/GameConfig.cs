using UnityEngine;

namespace TiroAlBlanco.Data
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "TiroAlBlanco/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Game Duration")]
        public float gameDuration = 60f;

        [Header("Weapon")]
        public int maxAmmo = 6;
        public float reloadTime = 1.5f;
        public float fireRate = 0.15f;

        [Header("Spawning")]
        public int maxConcurrentTargets = 5;
        public float initialSpawnInterval = 2f;
        public float minimumSpawnInterval = 0.5f;
        public float spawnIntervalDecreaseRate = 0.02f; // decreases per second

        [Header("Camera Shake")]
        public float shootShakeMagnitude = 0.05f;
        public float shootShakeDuration = 0.1f;

        [Header("Combo")]
        public float comboResetTime = 2f;
        public int comboTierSize = 3;

        [Header("Target Configs")]
        public TargetData[] targetTypes;
    }
}
