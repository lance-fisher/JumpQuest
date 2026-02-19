using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System.Collections.Generic;
using JumpQuest.Data;
using JumpQuest.Core;

namespace JumpQuest.LevelBuilder
{
    public class LevelDraftWizard : MonoBehaviour
    {
        // Wizard state
        private int step = 0;
        private string selectedWorld = "mountains";
        private string selectedSetting = "foothills";
        private string selectedDifficulty = "easy";
        private string selectedMechanic = "jumps";
        private string selectedLength = "short";

        private Transform canvasTransform;
        private RectTransform contentArea;

        private void Start()
        {
            if (FindObjectOfType<EventSystem>() == null)
            {
                var esGo = new GameObject("EventSystem");
                esGo.AddComponent<EventSystem>();
                esGo.AddComponent<StandaloneInputModule>();
            }
            BuildUI();
        }

        private void BuildUI()
        {
            var canvasGo = new GameObject("WizardCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();
            canvasTransform = canvasGo.transform;

            var bg = MakePanel(canvasTransform, "BG", new Color(0.08f, 0.1f, 0.18f));

            var title = MakeText(canvasTransform, "Title", "LEVEL DRAFT WIZARD",
                42, TextAnchor.MiddleCenter, new Color(1f, 0.7f, 0.2f));
            title.anchorMin = new Vector2(0.5f, 1);
            title.anchorMax = new Vector2(0.5f, 1);
            title.anchoredPosition = new Vector2(0, -50);
            title.sizeDelta = new Vector2(700, 60);

            var backBtn = MakeButton(canvasTransform, "Back", "BACK",
                new Vector2(120, 50), new Color(0.5f, 0.5f, 0.5f));
            backBtn.anchorMin = new Vector2(0, 1);
            backBtn.anchorMax = new Vector2(0, 1);
            backBtn.anchoredPosition = new Vector2(80, -40);
            backBtn.GetComponent<Button>().onClick.AddListener(() =>
                GameManager.Instance?.ReturnToMainMenu());

            // Content area
            var contentGo = new GameObject("Content");
            contentGo.transform.SetParent(canvasTransform, false);
            contentArea = contentGo.AddComponent<RectTransform>();
            contentArea.anchorMin = new Vector2(0.1f, 0.05f);
            contentArea.anchorMax = new Vector2(0.9f, 0.85f);
            contentArea.sizeDelta = Vector2.zero;

            ShowStep();
        }

        private void ShowStep()
        {
            // Clear content
            foreach (Transform child in contentArea)
                Destroy(child.gameObject);

            switch (step)
            {
                case 0: ShowWorldStep(); break;
                case 1: ShowSettingStep(); break;
                case 2: ShowDifficultyStep(); break;
                case 3: ShowMechanicStep(); break;
                case 4: ShowLengthStep(); break;
                case 5: ShowGenerateStep(); break;
            }
        }

        private void ShowWorldStep()
        {
            MakeQuestionText("Which world?");
            MakeChoiceButton("The Mountains", "mountains", 0);
            MakeChoiceButton("Jungle Ruins", "jungle", 1);
            MakeChoiceButton("Space Station", "space", 2);
            MakeChoiceButton("Candy Castle", "candy", 3);
            MakeChoiceButton("Pirate Cove", "pirate", 4);
        }

        private void ShowSettingStep()
        {
            MakeQuestionText($"Where in {selectedWorld}?");
            if (selectedWorld == "mountains")
            {
                MakeChoiceButton("Base & Foothills", "foothills", 0);
                MakeChoiceButton("Mid Climb", "midclimb", 1);
                MakeChoiceButton("Icy Peak", "peak", 2);
                MakeChoiceButton("Inside Cavern", "cavern", 3);
            }
            else
            {
                MakeChoiceButton("Entrance", "entrance", 0);
                MakeChoiceButton("Deep Interior", "deep", 1);
                MakeChoiceButton("Summit / Final", "summit", 2);
            }
        }

        private void ShowDifficultyStep()
        {
            MakeQuestionText("Difficulty?");
            MakeChoiceButton("Easy (1-3)", "easy", 0);
            MakeChoiceButton("Medium (4-6)", "medium", 1);
            MakeChoiceButton("Hard (7-10)", "hard", 2);
        }

        private void ShowMechanicStep()
        {
            MakeQuestionText("Main mechanic?");
            MakeChoiceButton("Jumps & Gaps", "jumps", 0);
            MakeChoiceButton("Moving Platforms", "moving", 1);
            MakeChoiceButton("Rotating Bars", "rotating", 2);
            MakeChoiceButton("Jump Pads", "jumppad", 3);
        }

        private void ShowLengthStep()
        {
            MakeQuestionText("Level length?");
            MakeChoiceButton("Short (30-60s)", "short", 0);
            MakeChoiceButton("Medium (1-2 min)", "medium", 1);
            MakeChoiceButton("Long (3-5 min)", "long", 2);
        }

        private void ShowGenerateStep()
        {
            MakeQuestionText("Ready to generate!");

            var summary = MakeText(contentArea, "Summary",
                $"World: {selectedWorld}\nSetting: {selectedSetting}\n" +
                $"Difficulty: {selectedDifficulty}\nMechanic: {selectedMechanic}\n" +
                $"Length: {selectedLength}",
                24, TextAnchor.MiddleCenter, Color.white);
            summary.anchorMin = new Vector2(0.5f, 0.5f);
            summary.anchorMax = new Vector2(0.5f, 0.5f);
            summary.anchoredPosition = new Vector2(0, -30);
            summary.sizeDelta = new Vector2(500, 180);

            var genBtn = MakeButton(contentArea, "Generate", "GENERATE & PLAY",
                new Vector2(300, 70), new Color(0.2f, 0.8f, 0.3f));
            genBtn.anchorMin = new Vector2(0.5f, 0);
            genBtn.anchorMax = new Vector2(0.5f, 0);
            genBtn.anchoredPosition = new Vector2(0, 60);
            genBtn.GetComponent<Button>().onClick.AddListener(GenerateAndPlay);

            var saveBtn = MakeButton(contentArea, "SaveOnly", "SAVE JSON ONLY",
                new Vector2(250, 50), new Color(0.4f, 0.4f, 0.6f));
            saveBtn.anchorMin = new Vector2(0.5f, 0);
            saveBtn.anchorMax = new Vector2(0.5f, 0);
            saveBtn.anchoredPosition = new Vector2(0, 0);
            saveBtn.GetComponent<Button>().onClick.AddListener(SaveOnly);
        }

        private void MakeQuestionText(string question)
        {
            var txt = MakeText(contentArea, "Question", question,
                34, TextAnchor.MiddleCenter, Color.white);
            txt.anchorMin = new Vector2(0.5f, 1);
            txt.anchorMax = new Vector2(0.5f, 1);
            txt.anchoredPosition = new Vector2(0, -30);
            txt.sizeDelta = new Vector2(600, 50);
        }

        private void MakeChoiceButton(string label, string value, int index)
        {
            var btn = MakeButton(contentArea, value, label,
                new Vector2(350, 60), new Color(0.25f, 0.35f, 0.55f));
            btn.anchorMin = new Vector2(0.5f, 1);
            btn.anchorMax = new Vector2(0.5f, 1);
            btn.anchoredPosition = new Vector2(0, -100 - index * 75);

            string val = value;
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                switch (step)
                {
                    case 0: selectedWorld = val; break;
                    case 1: selectedSetting = val; break;
                    case 2: selectedDifficulty = val; break;
                    case 3: selectedMechanic = val; break;
                    case 4: selectedLength = val; break;
                }
                step++;
                ShowStep();
            });
        }

        private void GenerateAndPlay()
        {
            string json = GenerateLevelJson();
            string path = SaveLevelJson(json);
            Debug.Log($"Level saved to {path}");

            // Load into gameplay
            UnityEngine.SceneManagement.SceneManager.LoadScene("Gameplay");
            // Delay load via coroutine isn't clean here; instead set a flag
            // The GameplaySceneController will pick up a custom level path
            PlayerPrefs.SetString("CustomLevelJson", json);
        }

        private void SaveOnly()
        {
            string json = GenerateLevelJson();
            string path = SaveLevelJson(json);
            Debug.Log($"Level JSON saved to: {path}");

            // Show confirmation
            MakeQuestionText($"Saved to:\n{path}");
        }

        private string GenerateLevelJson()
        {
            int diffNum = selectedDifficulty == "easy" ? 2 : selectedDifficulty == "medium" ? 5 : 8;
            int platformCount = selectedLength == "short" ? 8 : selectedLength == "medium" ? 14 : 22;
            int coinCount = selectedLength == "short" ? 5 : selectedLength == "medium" ? 10 : 18;
            float parTime = selectedLength == "short" ? 45f : selectedLength == "medium" ? 90f : 180f;

            var level = new LevelData
            {
                worldId = selectedWorld,
                levelName = $"{selectedWorld}_{selectedSetting}_{selectedDifficulty}",
                difficulty = diffNum,
                parTime = parTime,
                setting = selectedSetting,
                finishType = "portal",
                playerSpawn = new PlayerSpawn { x = 0, y = 2, z = 0 }
            };

            // Generate a procedural platform path
            float x = 0, y = 0, z = 4;
            float gapBase = selectedDifficulty == "easy" ? 3f : selectedDifficulty == "medium" ? 4.5f : 6f;
            float heightVariance = selectedDifficulty == "easy" ? 1f : 2f;

            // Start platform
            level.platforms.Add(new PlatformData { x = 0, y = 0, z = 0, sx = 6, sy = 1, sz = 6, color = "stone" });

            System.Random rng = new System.Random(selectedWorld.GetHashCode() + selectedSetting.GetHashCode());

            for (int i = 0; i < platformCount; i++)
            {
                float gap = gapBase + (float)(rng.NextDouble() * 2f);
                float dy = (float)(rng.NextDouble() * heightVariance * 2f - heightVariance * 0.3f);
                z += gap;
                y = Mathf.Max(0.5f, y + dy);

                float pw = 3f + (float)(rng.NextDouble() * 2f);
                float pd = 3f + (float)(rng.NextDouble() * 2f);

                // Decide type based on mechanic
                if (selectedMechanic == "moving" && rng.NextDouble() < 0.35)
                {
                    var mp = new MovingPlatformData
                    {
                        x = x, y = y, z = z,
                        sx = pw, sy = 0.5f, sz = pd,
                        speed = 2f + diffNum * 0.3f,
                        loop = true,
                        waypoints = new List<WaypointData>
                        {
                            new WaypointData { x = 0, y = 0, z = 0 },
                            new WaypointData { x = (float)(rng.NextDouble() * 6f - 3f), y = 0, z = 0 },
                        }
                    };
                    level.movingPlatforms.Add(mp);
                }
                else
                {
                    level.platforms.Add(new PlatformData
                    {
                        x = x + (float)(rng.NextDouble() * 3f - 1.5f),
                        y = y, z = z,
                        sx = pw, sy = 0.5f, sz = pd,
                        color = i % 3 == 0 ? "stone" : "brown"
                    });
                }

                // Rotating obstacle
                if (selectedMechanic == "rotating" && rng.NextDouble() < 0.3 && i > 1)
                {
                    level.rotatingObstacles.Add(new RotatingObstacleData
                    {
                        x = x, y = y + 2f, z = z - gap * 0.4f,
                        sx = 5f, sy = 0.4f, sz = 0.4f,
                        ay = 1, speed = 60 + diffNum * 10
                    });
                }

                // Jump pad
                if (selectedMechanic == "jumppad" && rng.NextDouble() < 0.25)
                {
                    level.jumpPads.Add(new JumpPadData { x = x, y = y + 0.3f, z = z, force = 15 + diffNum });
                }

                // Coins along the path
                if (rng.NextDouble() < (float)coinCount / platformCount)
                {
                    level.coins.Add(new CoinData
                    {
                        x = x + (float)(rng.NextDouble() * 2f - 1f),
                        y = y + 2f,
                        z = z
                    });
                }

                // Checkpoints every few platforms
                if (i > 0 && i % 5 == 0)
                {
                    level.checkpoints.Add(new CheckpointData
                    {
                        x = x, y = y + 0.5f, z = z,
                        index = level.checkpoints.Count
                    });
                }
            }

            // Finish at end
            level.finish = new FinishData { x = x, y = y + 1f, z = z + 5f };

            // Final platform under finish
            level.platforms.Add(new PlatformData
            {
                x = x, y = y, z = z + 5f,
                sx = 6, sy = 1, sz = 6, color = "stone"
            });

            return JsonUtility.ToJson(level, true);
        }

        private string SaveLevelJson(string json)
        {
            string dir = Path.Combine(Application.persistentDataPath, "Levels");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string fileName = $"custom_{selectedWorld}_{selectedSetting}_{System.DateTime.Now:yyyyMMdd_HHmmss}.json";
            string path = Path.Combine(dir, fileName);
            File.WriteAllText(path, json);
            return path;
        }

        // UI helpers
        private RectTransform MakePanel(Transform parent, string name, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            go.AddComponent<Image>().color = color;
            return rect;
        }

        private RectTransform MakeButton(Transform parent, string name, string label,
            Vector2 size, Color bgColor)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = size;
            go.AddComponent<Image>().color = bgColor;
            go.AddComponent<Button>();

            var txtRect = MakeText(rect, "Label", label, 22, TextAnchor.MiddleCenter, Color.white);
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.sizeDelta = Vector2.zero;

            return rect;
        }

        private RectTransform MakeText(Transform parent, string name, string text,
            int fontSize, TextAnchor alignment, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            var txt = go.AddComponent<Text>();
            txt.text = text;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (txt.font == null)
                txt.font = Font.CreateDynamicFontFromOSFont("Arial", fontSize);
            txt.fontSize = fontSize;
            txt.alignment = alignment;
            txt.color = color;
            txt.fontStyle = FontStyle.Bold;
            return rect;
        }
    }
}
