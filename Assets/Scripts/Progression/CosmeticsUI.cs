using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using JumpQuest.Core;

namespace JumpQuest.Progression
{
    public class CosmeticsUI : MonoBehaviour
    {
        private struct CosmeticItem
        {
            public string Id;
            public string DisplayName;
            public string Category; // "hat", "trail", "skin"
            public int Cost;
            public Color PreviewColor;

            public CosmeticItem(string id, string name, string cat, int cost, Color color)
            {
                Id = id; DisplayName = name; Category = cat; Cost = cost; PreviewColor = color;
            }
        }

        private readonly CosmeticItem[] items = new[]
        {
            // Skins
            new CosmeticItem("default", "Blue (Default)", "skin", 0, new Color(0.2f, 0.6f, 1f)),
            new CosmeticItem("skin_red", "Red Hot", "skin", 30, Color.red),
            new CosmeticItem("skin_green", "Forest", "skin", 30, new Color(0.2f, 0.8f, 0.3f)),
            new CosmeticItem("skin_gold", "Golden", "skin", 100, new Color(1f, 0.84f, 0f)),
            new CosmeticItem("skin_rainbow", "Rainbow", "skin", 200, new Color(0.9f, 0.3f, 0.9f)),

            // Hats (placeholder)
            new CosmeticItem("hat_crown", "Crown", "hat", 75, new Color(1f, 0.85f, 0.2f)),
            new CosmeticItem("hat_wizard", "Wizard Hat", "hat", 50, new Color(0.4f, 0.2f, 0.8f)),
            new CosmeticItem("hat_pirate", "Pirate Bandana", "hat", 60, new Color(0.6f, 0.1f, 0.1f)),

            // Trails (placeholder)
            new CosmeticItem("trail_sparkle", "Sparkle Trail", "trail", 80, Color.white),
            new CosmeticItem("trail_fire", "Fire Trail", "trail", 120, new Color(1f, 0.4f, 0f)),
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
            var canvasGo = new GameObject("CosmeticsCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            var bg = CreatePanel(canvasGo.transform, "BG", new Color(0.12f, 0.1f, 0.2f));

            var title = CreateText(canvasGo.transform, "Title", "COSMETICS",
                48, TextAnchor.MiddleCenter, new Color(0.9f, 0.5f, 1f));
            title.anchorMin = new Vector2(0.5f, 1);
            title.anchorMax = new Vector2(0.5f, 1);
            title.anchoredPosition = new Vector2(0, -50);
            title.sizeDelta = new Vector2(600, 60);

            var backBtn = CreateButton(canvasGo.transform, "BackButton", "BACK",
                new Vector2(120, 50), new Color(0.5f, 0.5f, 0.5f));
            backBtn.anchorMin = new Vector2(0, 1);
            backBtn.anchorMax = new Vector2(0, 1);
            backBtn.anchoredPosition = new Vector2(80, -40);
            backBtn.GetComponent<Button>().onClick.AddListener(() =>
                GameManager.Instance?.ReturnToMainMenu());

            var gm = GameManager.Instance;
            int currency = gm != null ? gm.Progress.Currency : 0;
            var currText = CreateText(canvasGo.transform, "Currency", $"Coins: {currency}",
                28, TextAnchor.MiddleRight, Color.yellow);
            currText.anchorMin = new Vector2(1, 1);
            currText.anchorMax = new Vector2(1, 1);
            currText.anchoredPosition = new Vector2(-40, -50);
            currText.sizeDelta = new Vector2(300, 50);

            float yStart = -130;
            int col = 0;
            int row = 0;
            int itemsPerRow = 4;

            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                bool owned = item.Cost == 0 || (gm != null && gm.Progress.UnlockedCosmetics.Contains(item.Id));
                bool equipped = false;
                if (gm != null)
                {
                    if (item.Category == "skin") equipped = gm.Progress.EquippedSkin == item.Id;
                    else if (item.Category == "hat") equipped = gm.Progress.EquippedHat == item.Id;
                    else if (item.Category == "trail") equipped = gm.Progress.EquippedTrail == item.Id;
                }

                Color cardColor = equipped ? new Color(0.3f, 0.8f, 0.3f, 0.8f) :
                                  owned ? item.PreviewColor * 0.6f :
                                  new Color(0.3f, 0.3f, 0.3f, 0.5f);

                var card = CreateButton(canvasGo.transform, item.Id, item.DisplayName,
                    new Vector2(200, 90), cardColor);
                card.anchorMin = new Vector2(0.5f, 1);
                card.anchorMax = new Vector2(0.5f, 1);

                float xPos = -300 + col * 210;
                float yPos = yStart + row * -110;
                card.anchoredPosition = new Vector2(xPos, yPos);

                string label = equipped ? "EQUIPPED" : (owned ? "EQUIP" : $"{item.Cost} coins");
                var costLabel = CreateText(card, "Cost", label,
                    16, TextAnchor.LowerCenter, Color.white);
                costLabel.anchorMin = new Vector2(0, 0);
                costLabel.anchorMax = new Vector2(1, 0);
                costLabel.anchoredPosition = new Vector2(0, 5);
                costLabel.sizeDelta = new Vector2(0, 20);

                var btn = card.GetComponent<Button>();
                string itemId = item.Id;
                string category = item.Category;
                int cost = item.Cost;
                btn.onClick.AddListener(() =>
                {
                    if (gm == null) return;
                    bool isOwned = cost == 0 || gm.Progress.UnlockedCosmetics.Contains(itemId);

                    if (!isOwned)
                    {
                        if (gm.Progress.Currency >= cost)
                        {
                            gm.Progress.Currency -= cost;
                            gm.Progress.UnlockedCosmetics.Add(itemId);
                            SaveManager.Save(gm.Progress);
                        }
                        else return;
                    }

                    // Equip
                    if (category == "skin") gm.Progress.EquippedSkin = itemId;
                    else if (category == "hat") gm.Progress.EquippedHat = itemId;
                    else if (category == "trail") gm.Progress.EquippedTrail = itemId;
                    SaveManager.Save(gm.Progress);

                    // Rebuild
                    foreach (Transform child in canvasGo.transform)
                        Destroy(child.gameObject);
                    BuildUI();
                });

                col++;
                if (col >= itemsPerRow)
                {
                    col = 0;
                    row++;
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

            var txtRect = CreateText(rect, "Label", label, 18, TextAnchor.MiddleCenter, Color.white);
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.sizeDelta = Vector2.zero;
            txtRect.offsetMin = new Vector2(5, 15);
            txtRect.offsetMax = new Vector2(-5, -5);

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
