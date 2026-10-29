using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "ObstacleData", menuName = "ScriptableObjects/Obstacle  Data")]
    public class ObstacleData : ScriptableObject
    {
        public float noiseAmount = 15f;
    }
}