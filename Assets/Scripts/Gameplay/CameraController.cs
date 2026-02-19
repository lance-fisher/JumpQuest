using UnityEngine;

namespace JumpQuest.Gameplay
{
    public class CameraController : MonoBehaviour
    {
        [Header("Follow")]
        public Transform Target;
        public Vector3 Offset = new Vector3(0f, 8f, -10f);
        public float SmoothSpeed = 8f;
        public float LookAheadDistance = 2f;

        [Header("Kid-Friendly Constraints")]
        public float MinPitch = 15f;
        public float MaxPitch = 45f;
        public float MinHeight = 3f;

        private void LateUpdate()
        {
            if (Target == null) return;

            Vector3 desiredPos = Target.position + Offset;

            // Clamp height so camera never goes below the player
            desiredPos.y = Mathf.Max(desiredPos.y, Target.position.y + MinHeight);

            Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, SmoothSpeed * Time.deltaTime);
            transform.position = smoothedPos;

            // Look at a point slightly ahead of the player for forward visibility
            Vector3 lookTarget = Target.position + Target.forward * LookAheadDistance + Vector3.up * 1.5f;
            transform.LookAt(lookTarget);
        }
    }
}
