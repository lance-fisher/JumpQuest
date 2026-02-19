using UnityEngine;

namespace JumpQuest.Gameplay
{
    public class MovingPlatform : MonoBehaviour
    {
        public Vector3[] LocalWaypoints;
        public float Speed = 3f;
        public bool Loop = true;

        private Vector3[] worldWaypoints;
        private int currentIndex = 0;
        private bool forward = true;

        private void Start()
        {
            if (LocalWaypoints == null || LocalWaypoints.Length == 0)
            {
                LocalWaypoints = new[] { Vector3.zero, Vector3.right * 5f };
            }

            worldWaypoints = new Vector3[LocalWaypoints.Length];
            for (int i = 0; i < LocalWaypoints.Length; i++)
                worldWaypoints[i] = transform.position + LocalWaypoints[i];
        }

        private void Update()
        {
            if (worldWaypoints.Length < 2) return;

            Vector3 target = worldWaypoints[currentIndex];
            transform.position = Vector3.MoveTowards(transform.position, target, Speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target) < 0.05f)
            {
                if (Loop)
                {
                    currentIndex = (currentIndex + 1) % worldWaypoints.Length;
                }
                else
                {
                    if (forward) currentIndex++;
                    else currentIndex--;

                    if (currentIndex >= worldWaypoints.Length)
                    {
                        currentIndex = worldWaypoints.Length - 2;
                        forward = false;
                    }
                    else if (currentIndex < 0)
                    {
                        currentIndex = 1;
                        forward = true;
                    }
                }
            }
        }

        // Move player with platform
        private void OnTriggerStay(Collider other)
        {
            if (other.GetComponent<PlayerController>() != null)
            {
                other.transform.parent = transform;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<PlayerController>() != null)
            {
                other.transform.parent = null;
            }
        }
    }
}
