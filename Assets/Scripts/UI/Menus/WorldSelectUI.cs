using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using JumpQuest.Core;

namespace JumpQuest.UI
{
    public class WorldSelectUI : MonoBehaviour
    {
        private struct WorldInfo
        {
            public string Id;
            public string DisplayName;
            public Color Color;
            public int LevelCount;
            public int RequiredLevel;

            public WorldInfo(string id, string name, Color color, int levels, int reqLevel)
            {
                Id = id; DisplayName = name; Color = color; LevelCount = levels; RequiredLevel = reqLevel;
            }
        }

        private readonly WorldInfo[] worlds = new[]
        {
            new WorldInfo("mountains", "The Mountains", new Color(0.4f, 0.55f, 0.7f), 3, 1),
            new WorldInfo("jungle", "Jungle Ruins", new Color(0.2f, 0.6f, 0.3f), 3, 5),
            new WorldInfo("space", "Space Station", new Color(0.15f, 0.1f, 0.3f), 3, 10),
            new WorldInfo("candy", "Candy Castle", new Color(0.9f, 0.4f, 0.6f), 3, 15),
            new WorldInfo("pirate", "Pirate Cove", new Color(0.3f, 0.4f, 0.6f), 3, 20),
        };

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
            var canvasGo = new GameObject("WorldSelectCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            // BG
            var bg = CreatePanel(canvasGo.transform, "BG", new Color(0.12f, 0.15f, 0.25f));

            // Title
            var title = CreateText(canvasGo.transform, "Title", "SELECT WORLD",
                48, TextAnchor.MiddleCenter, Color.white);
            title.anchorMin = new Vector2(0.5f, 1);
            title.anchorMax = new Vector2(0.5f, 1);
            title.anchoredPosition = new Vector2(0, -60);
            title.sizeDelta = new Vector2(600, 60);

            // Back button
            var backBtn = CreateButton(canvasGo.transform, "BackButton", "BACK",
                new Vector2(120, 50), new Color(0.5f, 0.5f, 0.5f));
            backBtn.anchorMin = new Vector2(0, 1);
            backBtn.anchorMax = new Vector2(0, 1);
            backBtn.anchoredPosition = new Vector2(80, -40);
            backBtn.GetComponent<Button>().onClick.AddListener(() =>
                GameManager.Instance?.ReturnToMainMenu());

            var gm = GameManager.Instance;
            int playerLevel = gm != null ? gm.Progress.Level : 1;

            // World cards
            float startY = -140;
            for (int i = 0; i < worlds.Length; i++)
            {
                var w = worlds[i];
                bool unlocked = playerLevel >= w.RequiredLevel;

                var card = CreatePanel(canvasGo.transform, $"World_{w.Id}",
                    unlocked ? w.Color : new Color(0.3f, 0.3f, 0.3f, 0.5f));
                card.anchorMin = new Vector2(0.5f, 1);
                card.anchorMax = new Vector2(0.5f, 1);
                card.anchoredPosition = new Vector2(0, startY - i * 130);
                card.sizeDelta = new Vector2(700, 110);

                var nameText = CreateText(card, "Name", w.DisplayName,
                    32, TextAnchor.MiddleLeft, Color.white);
                nameText.anchorMin = new Vector2(0, 0);
                nameText.anchorMax = new Vector2(1, 1);
                nameText.offsetMin = new Vector2(20, 0);
                nameText.offsetMax = new Vector2(-20, 0);

                if (unlocked)
                {
                    // Level buttons
                    for (int j = 0; j < w.LevelCount; j++)
                    {
                        int levelIdx = j;
                        string worldId = w.Id;

                        string levelKey = $"{worldId}_{levelIdx}";
                        bool completed = gm != null && gm.Progress.CompletedLevels.Contains(levelKey);

                        var lvlBtn = CreateButton(card, $"Level_{j}", $"{j + 1}",
                            new Vector2(60, 60), completed ? new Color(0.3f, 0.8f, 0.3f) : new Color(0.5f, 0.5f, 0.5f, 0.8f));
                        lvlBtn.anchorMin = new Vector2(1, 0.5f);
                        lvlBtn.anchorMax = new Vector2(1, 0.5f);
                        lvlBtn.anchoredPosition = new Vector2(-40 - (w.LevelCount - 1 - j) * 75, 0);

                        lvlBtn.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            GameManager.Instance?.StartLevel(worldId, levelIdx);
                        });
                    }
                }
                else
                {
                    var lockText = CreateText(card, "Lock", $"Unlock at Level {w.RequiredLevel}",
                        22, TextAnchor.MiddleRight, new Color(0.8f, 0.8f, 0.8f));
                    lockText.anchorMin = new Vector2(0, 0);
                    lockText.anchorMax = new Vector2(1, 1);
                    lockText.offsetMin = new Vector2(20, 0);
                    lockText.offsetMax = new Vector2(-30, 0);
                }
            }
        }

        private RectTransform CreatePanel(Transform parent, string name, Color color)
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

        private RectTransform CreateButton(Transform parent, string name, string label,
            Vector2 size, Color bgColor)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = size;
            go.AddComponent<Image>().color = bgColor;
            go.AddComponent<Button>();

            var txtRect = CreateText(rect, "Label", label, 24, TextAnchor.MiddleCenter, Color.white);
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.sizeDelta = Vector2.zero;

            return rect;
        }

        private RectTransform CreateText(Transform parent, string name, string text,
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
