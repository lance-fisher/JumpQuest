using UnityEngine;

namespace JumpQuest.Gameplay
{
    public class JumpPad : MonoBehaviour
    {
        public float LaunchForce = 18f;
        public float CooldownTime = 0.5f;

        private float cooldownTimer;

        private void Update()
        {
            if (cooldownTimer > 0f)
                cooldownTimer -= Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (cooldownTimer > 0f) return;

            var player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // Apply upward impulse via CharacterController velocity override
                // We access the CC directly since PlayerController manages velocity
                var cc = other.GetComponent<CharacterController>();
                if (cc != null)
                {
                    // PlayerController reads velocity.y, so we use reflection-free approach:
                    // Set a public launch field
                    player.JumpRequested = false; // cancel any pending jump
                    // Direct velocity set via the player's internal state
                    // We'll use SendMessage for simplicity
                    player.SendMessage("ApplyLaunch", LaunchForce, SendMessageOptions.DontRequireReceiver);
                }

                cooldownTimer = CooldownTime;
                AudioManager.Instance?.PlaySFX("jumppad");
            }
        }
    }
}
