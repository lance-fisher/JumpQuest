using UnityEngine;
using JumpQuest.Core;
using JumpQuest.Data;

namespace JumpQuest.Gameplay
{
    public class GameplaySceneController : MonoBehaviour
    {
        private void Start()
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogWarning("No GameManager found. Creating default for editor testing.");
                var go = new GameObject("GameManager");
                go.AddComponent<GameManager>();
                gm = GameManager.Instance;
                gm.CurrentWorldId = "mountains";
                gm.CurrentLevelIndex = 0;
                gm.SessionRunning = true;
            }

            // Check if a custom level from the wizard was set
            string customJson = PlayerPrefs.GetString("CustomLevelJson", "");
            if (!string.IsNullOrEmpty(customJson))
            {
                PlayerPrefs.DeleteKey("CustomLevelJson");
                LevelLoader.LoadLevelFromJson(customJson);
            }
            else
            {
                LevelLoader.LoadLevel(gm.CurrentWorldId, gm.CurrentLevelIndex);
            }
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.SessionRunning)
            {
                GameManager.Instance.SessionTime += Time.deltaTime;
            }

            // Pause toggle
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        public void TogglePause()
        {
            if (Time.timeScale > 0f)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
    }
}
