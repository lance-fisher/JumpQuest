using UnityEngine;
using JumpQuest.Core;

namespace JumpQuest.Gameplay
{
    public class Coin : MonoBehaviour
    {
        public int Value = 1;
        public float RotateSpeed = 180f;
        public float BobSpeed = 2f;
        public float BobHeight = 0.3f;

        private Vector3 startPos;

        private void Start()
        {
            startPos = transform.position;
        }

        private void Update()
        {
            transform.Rotate(Vector3.up, RotateSpeed * Time.deltaTime);
            float bob = Mathf.Sin(Time.time * BobSpeed) * BobHeight;
            transform.position = startPos + Vector3.up * bob;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerController>() != null)
            {
                if (GameManager.Instance != null)
                    GameManager.Instance.SessionCoins += Value;

                AudioManager.Instance?.PlaySFX("coin");
                Destroy(gameObject);
            }
        }
    }
}
