using UnityEngine;
using UnityEngine.UI;
using JumpQuest.Core;

namespace JumpQuest.UI
{
    public class GameHUD : MonoBehaviour
    {
        [Header("HUD Elements")]
        public Text CoinText;
        public Text TimerText;
        public Text RunIndicator;
        public GameObject PausePanel;

        [Header("Power-Up Buttons")]
        public Button SpeedBurstButton;
        public Button ShieldButton;

        private void Start()
        {
            if (PausePanel != null)
                PausePanel.SetActive(false);

            UpdatePowerUpButtons();
        }

        private void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;

            if (CoinText != null)
                CoinText.text = gm.SessionCoins.ToString();

            if (TimerText != null)
            {
                int mins = (int)(gm.SessionTime / 60f);
                int secs = (int)(gm.SessionTime % 60f);
                TimerText.text = $"{mins:00}:{secs:00}";
            }
        }

        private void UpdatePowerUpButtons()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;

            if (SpeedBurstButton != null)
                SpeedBurstButton.gameObject.SetActive(gm.HasSpeedBurst);

            if (ShieldButton != null)
                ShieldButton.gameObject.SetActive(gm.HasShield);
        }

        public void OnPausePressed()
        {
            if (PausePanel != null)
                PausePanel.SetActive(true);
            Time.timeScale = 0f;
        }

        public void OnResumePressed()
        {
            if (PausePanel != null)
                PausePanel.SetActive(false);
            Time.timeScale = 1f;
        }

        public void OnQuitPressed()
        {
            Time.timeScale = 1f;
            GameManager.Instance?.ReturnToWorldSelect();
        }

        public void OnRestartPressed()
        {
            Time.timeScale = 1f;
            GameManager.Instance?.RestartLevel();
        }
    }
}
