using System;
using System.Collections.Generic;
using UnityEngine;

namespace JumpQuest.Core
{
    [Serializable]
    public class PlayerProgressData
    {
        public int TotalXp;
        public int Level = 1;
        public int Currency;
        public List<string> CompletedLevels = new List<string>();
        public List<string> UnlockedSkills = new List<string>();
        public List<string> UnlockedCosmetics = new List<string>();
        public string EquippedHat = "";
        public string EquippedTrail = "";
        public string EquippedSkin = "default";
        public SerializableDictionary BestTimes = new SerializableDictionary();

        // XP curve: each level requires more XP
        public int XpForNextLevel()
        {
            return 100 + (Level - 1) * 50;
        }

        public void RecalculateLevel()
        {
            while (TotalXp >= XpForNextLevel())
            {
                TotalXp -= XpForNextLevel();
                Level++;
            }
        }

        public bool IsSkillUnlocked(string skillId)
        {
            return UnlockedSkills.Contains(skillId);
        }

        public bool TryUnlockSkill(string skillId, int cost)
        {
            if (UnlockedSkills.Contains(skillId)) return false;
            if (Currency < cost) return false;
            Currency -= cost;
            UnlockedSkills.Add(skillId);
            return true;
        }
    }

    [Serializable]
    public class SerializableDictionary : ISerializationCallbackReceiver
    {
        [SerializeField] private List<string> keys = new List<string>();
        [SerializeField] private List<float> values = new List<float>();

        private Dictionary<string, float> dict = new Dictionary<string, float>();

        public float this[string key]
        {
            get => dict[key];
            set => dict[key] = value;
        }

        public bool ContainsKey(string key) => dict.ContainsKey(key);

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (var kvp in dict)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            dict = new Dictionary<string, float>();
            for (int i = 0; i < Mathf.Min(keys.Count, values.Count); i++)
                dict[keys[i]] = values[i];
        }
    }
}
