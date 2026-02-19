using UnityEngine;
using JumpQuest.Gameplay;

namespace JumpQuest.UI
{
    public class TouchInputBridge : MonoBehaviour
    {
        public VirtualJoystick Joystick;

        private PlayerController player;

        private void Update()
        {
            if (player == null)
            {
                var go = GameObject.Find("Player");
                if (go != null) player = go.GetComponent<PlayerController>();
                return;
            }

            if (Joystick != null)
            {
                player.MoveInput = Joystick.Direction;
            }
        }

        // Called by Jump button (UnityEvent)
        public void OnJumpPressed()
        {
            if (player != null)
                player.JumpRequested = true;
        }

        // Called by Run button (hold)
        public void OnRunDown()
        {
            if (player != null)
                player.RunHeld = true;
        }

        public void OnRunUp()
        {
            if (player != null)
                player.RunHeld = false;
        }

        // Power-up buttons
        public void OnSpeedBurstPressed()
        {
            if (player != null)
                player.ActivateSpeedBurst();
        }

        public void OnShieldPressed()
        {
            if (player != null)
                player.ActivateShield();
        }
    }
}
