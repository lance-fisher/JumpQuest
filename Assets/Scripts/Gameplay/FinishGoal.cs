using UnityEngine;
using JumpQuest.Core;

namespace JumpQuest.Gameplay
{
    public class FinishGoal : MonoBehaviour
    {
        public float RotateSpeed = 90f;
        private bool triggered = false;

        private void Update()
        {
            transform.Rotate(Vector3.up, RotateSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (triggered) return;
            if (other.GetComponent<PlayerController>() != null)
            {
                triggered = true;
                AudioManager.Instance?.PlaySFX("finish");
                GameManager.Instance?.CompleteLevel();
            }
        }
    }
}
