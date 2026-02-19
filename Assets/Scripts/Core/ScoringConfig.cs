using UnityEngine;

namespace JumpQuest.Core
{
    [CreateAssetMenu(fileName = "ScoringConfig", menuName = "JumpQuest/Scoring Config")]
    public class ScoringConfig : ScriptableObject
    {
        public float TimeBonusRate = 2f;       // currency per second under par
        public int XpPerCoin = 5;
        public int XpBase = 50;                // base XP for completing any level
        public float DifficultyXpScale = 0.1f; // extra XP % per difficulty point

        private static ScoringConfig _default;
        public static ScoringConfig Default
        {
            get
            {
                if (_default == null)
                {
                    _default = Resources.Load<ScoringConfig>("Configs/ScoringConfig");
                    if (_default == null)
                    {
                        _default = CreateInstance<ScoringConfig>();
                    }
                }
                return _default;
            }
        }
    }
}
