using UnityEngine;

namespace JumpQuest.Gameplay
{
    public class Checkpoint : MonoBehaviour
    {
        public int Index;
        private bool activated = false;

        private Renderer rend;

        private void Start()
        {
            rend = GetComponent<Renderer>();
        }

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponent<PlayerController>();
            if (player != null && !activated)
            {
                activated = true;
                player.SetCheckpoint(transform.position + Vector3.up * 1f);

                // Visual feedback
                if (rend != null)
                    rend.material.color = Color.green;

                AudioManager.Instance?.PlaySFX("checkpoint");
            }
        }
    }
}
