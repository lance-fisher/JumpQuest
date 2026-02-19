using UnityEngine;

namespace JumpQuest.Gameplay
{
    public class RotatingObstacle : MonoBehaviour
    {
        public Vector3 RotationAxis = Vector3.up;
        public float RotationSpeed = 90f;

        private void Update()
        {
            transform.Rotate(RotationAxis.normalized, RotationSpeed * Time.deltaTime);
        }
    }
}
