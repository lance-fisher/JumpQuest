using UnityEngine;
using UnityEngine.SceneManagement;

namespace JumpQuest.Core
{
    /// <summary>
    /// Attach to a GameObject in each scene to set up required systems.
    /// Each scene type creates its own controller.
    /// </summary>
    public class BootstrapScene : MonoBehaviour
    {
        public enum SceneType
        {
            MainMenu,
            WorldSelect,
            Gameplay,
            Results,
            SkillTree,
            Cosmetics,
            LevelWizard
        }

        public SceneType Type = SceneType.MainMenu;

        private void Awake()
        {
            // Ensure GameManager exists
            if (GameManager.Instance == null)
            {
                var go = new GameObject("GameManager");
                go.AddComponent<GameManager>();
            }

            // Ensure AudioManager exists
            if (AudioManager.Instance == null)
            {
                var go = new GameObject("AudioManager");
                go.AddComponent<AudioManager>();
            }
        }

        private void Start()
        {
            switch (Type)
            {
                case SceneType.MainMenu:
                    gameObject.AddComponent<UI.MainMenuUI>();
                    break;
                case SceneType.WorldSelect:
                    gameObject.AddComponent<UI.WorldSelectUI>();
                    break;
                case SceneType.Gameplay:
                    gameObject.AddComponent<Gameplay.GameplaySceneController>();
                    gameObject.AddComponent<UI.HUDBuilder>();
                    break;
                case SceneType.Results:
                    gameObject.AddComponent<UI.ResultsUI>();
                    break;
                case SceneType.SkillTree:
                    gameObject.AddComponent<Progression.SkillTreeUI>();
                    break;
                case SceneType.Cosmetics:
                    gameObject.AddComponent<Progression.CosmeticsUI>();
                    break;
                case SceneType.LevelWizard:
                    gameObject.AddComponent<LevelBuilder.LevelDraftWizard>();
                    break;
            }
        }
    }
}
