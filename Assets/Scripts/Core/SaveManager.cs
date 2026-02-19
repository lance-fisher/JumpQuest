using UnityEngine;
using System.IO;

namespace JumpQuest.Core
{
    public static class SaveManager
    {
        private static string SavePath => Path.Combine(Application.persistentDataPath, "jumpquest_save.json");

        public static void Save(PlayerProgressData data)
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);
        }

        public static PlayerProgressData Load()
        {
            if (!File.Exists(SavePath))
                return new PlayerProgressData();

            string json = File.ReadAllText(SavePath);
            return JsonUtility.FromJson<PlayerProgressData>(json);
        }

        public static void DeleteSave()
        {
            if (File.Exists(SavePath))
                File.Delete(SavePath);
        }
    }
}
