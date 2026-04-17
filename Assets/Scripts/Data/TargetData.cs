using UnityEngine;

namespace TiroAlBlanco.Data
{
    public enum TargetType
    {
        Normal,     // Blanco estándar - 100 pts
        Moving,     // Se mueve horizontalmente - 150 pts
        Fast,       // Aparece y desaparece rápido - 200 pts
        Bonus,      // Dorado - 300 pts
        Penalty     // Rojo oscuro - resta 100 pts
    }

    [CreateAssetMenu(fileName = "TargetData", menuName = "TiroAlBlanco/Target Data")]
    public class TargetData : ScriptableObject
    {
        [Header("Identity")]
        public TargetType targetType;
        public string displayName;

        [Header("Scoring")]
        public int pointValue = 100;

        [Header("Timing")]
        public float minActiveTime = 2f;
        public float maxActiveTime = 4f;

        [Header("Movement")]
        public bool moves = false;
        public float moveSpeed = 2f;
        public float moveDistance = 3f;

        [Header("Visuals")]
        public Color targetColor = Color.red;
        public Color ringColor = Color.white;
        public float scale = 1f;

        [Header("Spawn Weight")]
        [Range(0f, 1f)]
        public float spawnWeight = 1f;
    }
}
