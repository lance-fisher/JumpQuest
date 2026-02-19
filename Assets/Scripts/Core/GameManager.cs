using UnityEngine;
using UnityEngine.SceneManagement;

namespace JumpQuest.Core
{
    /// <summary>
    /// Singleton game manager. Persists across scenes.
    /// Owns player state, current level info, and scene flow.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Runtime State")]
        public PlayerProgressData Progress;
        public string CurrentWorldId = "mountains";
        public int CurrentLevelIndex = 0;

        // Level session state (reset each level)
        public int SessionCoins { get; set; }
        public float SessionTime { get; set; }
        public bool SessionRunning { get; set; }

        // Power-up runtime state
        public bool HasDoubleJump { get; set; }
        public bool HasSpeedBurst { get; set; }
        public bool HasShield { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Progress = SaveManager.Load();
            ApplyUnlockedPowerUps();
        }

        public void ApplyUnlockedPowerUps()
        {
            HasDoubleJump = Progress.IsSkillUnlocked("featherstep_1");
            HasSpeedBurst = Progress.IsSkillUnlocked("swiftcurrent_1");
            HasShield = Progress.IsSkillUnlocked("deepguard_1");
        }

        public void StartLevel(string worldId, int levelIndex)
        {
            CurrentWorldId = worldId;
            CurrentLevelIndex = levelIndex;
            SessionCoins = 0;
            SessionTime = 0f;
            SessionRunning = true;
            SceneManager.LoadScene("Gameplay");
        }

        public void CompleteLevel()
        {
            SessionRunning = false;

            var config = ScoringConfig.Default;
            float parTime = GetParTime();
            int timeBonus = Mathf.Max(0, Mathf.RoundToInt((parTime - SessionTime) * config.TimeBonusRate));
            int totalCurrency = SessionCoins + timeBonus;

            int difficultyMultiplier = Mathf.Clamp(CurrentLevelIndex + 1, 1, 10);
            int xpGained = (int)((SessionCoins * config.XpPerCoin + config.XpBase) * (1f + difficultyMultiplier * 0.1f));

            Progress.Currency += totalCurrency;
            Progress.TotalXp += xpGained;
            Progress.RecalculateLevel();

            string levelKey = $"{CurrentWorldId}_{CurrentLevelIndex}";
            if (!Progress.CompletedLevels.Contains(levelKey))
                Progress.CompletedLevels.Add(levelKey);

            if (Progress.BestTimes.ContainsKey(levelKey))
            {
                if (SessionTime < Progress.BestTimes[levelKey])
                    Progress.BestTimes[levelKey] = SessionTime;
            }
            else
            {
                Progress.BestTimes[levelKey] = SessionTime;
            }

            SaveManager.Save(Progress);
            ApplyUnlockedPowerUps();

            // Store results for display
            LastResultCoins = SessionCoins;
            LastResultTime = SessionTime;
            LastResultTimeBonus = timeBonus;
            LastResultCurrency = totalCurrency;
            LastResultXp = xpGained;

            SceneManager.LoadScene("Results");
        }

        // Cached results for the results screen
        public int LastResultCoins;
        public float LastResultTime;
        public int LastResultTimeBonus;
        public int LastResultCurrency;
        public int LastResultXp;

        public void ReturnToWorldSelect()
        {
            SceneManager.LoadScene("WorldSelect");
        }

        public void ReturnToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void RestartLevel()
        {
            StartLevel(CurrentWorldId, CurrentLevelIndex);
        }

        private float GetParTime()
        {
            // Default par times by level index; overridden by level JSON if available
            return 60f + CurrentLevelIndex * 15f;
        }
    }
}
