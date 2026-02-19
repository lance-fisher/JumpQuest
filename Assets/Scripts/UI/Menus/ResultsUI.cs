using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using JumpQuest.Core;

namespace JumpQuest.UI
{
    public class ResultsUI : MonoBehaviour
    {
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
            var canvasGo = new GameObject("ResultsCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            var bg = CreatePanel(canvasGo.transform, "BG", new Color(0.1f, 0.15f, 0.3f));

            var gm = GameManager.Instance;
            int coins = gm != null ? gm.LastResultCoins : 0;
            float time = gm != null ? gm.LastResultTime : 0;
            int timeBonus = gm != null ? gm.LastResultTimeBonus : 0;
            int currency = gm != null ? gm.LastResultCurrency : 0;
            int xp = gm != null ? gm.LastResultXp : 0;
            int level = gm != null ? gm.Progress.Level : 1;

            // Title
            var title = CreateText(canvasGo.transform, "Title", "LEVEL COMPLETE!",
                56, TextAnchor.MiddleCenter, new Color(1f, 0.85f, 0.2f));
            title.anchorMin = new Vector2(0.5f, 0.5f);
            title.anchorMax = new Vector2(0.5f, 0.5f);
            title.anchoredPosition = new Vector2(0, 250);
            title.sizeDelta = new Vector2(800, 80);

            int mins = (int)(time / 60f);
            int secs = (int)(time % 60f);

            string[] labels = { "Time", "Coins", "Time Bonus", "Total Currency", "XP Gained", "Player Level" };
            string[] values = { $"{mins:00}:{secs:00}", coins.ToString(), $"+{timeBonus}", currency.ToString(), $"+{xp}", level.ToString() };
            Color[] colors = { Color.white, Color.yellow, new Color(0.5f, 1f, 0.5f), Color.yellow, new Color(0.5f, 0.8f, 1f), new Color(1f, 0.7f, 0.3f) };

            for (int i = 0; i < labels.Length; i++)
            {
                float yPos = 140 - i * 60;

                var lbl = CreateText(canvasGo.transform, $"Label_{i}", labels[i],
                    28, TextAnchor.MiddleRight, new Color(0.7f, 0.7f, 0.7f));
                lbl.anchorMin = new Vector2(0.5f, 0.5f);
                lbl.anchorMax = new Vector2(0.5f, 0.5f);
                lbl.anchoredPosition = new Vector2(-60, yPos);
                lbl.sizeDelta = new Vector2(300, 45);

                var val = CreateText(canvasGo.transform, $"Value_{i}", values[i],
                    32, TextAnchor.MiddleLeft, colors[i]);
                val.anchorMin = new Vector2(0.5f, 0.5f);
                val.anchorMax = new Vector2(0.5f, 0.5f);
                val.anchoredPosition = new Vector2(120, yPos);
                val.sizeDelta = new Vector2(300, 45);
            }

            // Buttons
            var nextBtn = CreateButton(canvasGo.transform, "NextButton", "NEXT LEVEL",
                new Vector2(260, 70), new Color(0.2f, 0.8f, 0.3f));
            nextBtn.anchorMin = new Vector2(0.5f, 0.5f);
            nextBtn.anchorMax = new Vector2(0.5f, 0.5f);
            nextBtn.anchoredPosition = new Vector2(-150, -250);
            nextBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (gm != null)
                    gm.StartLevel(gm.CurrentWorldId, gm.CurrentLevelIndex + 1);
            });

            var retryBtn = CreateButton(canvasGo.transform, "RetryButton", "RETRY",
                new Vector2(200, 70), new Color(1f, 0.6f, 0.2f));
            retryBtn.anchorMin = new Vector2(0.5f, 0.5f);
            retryBtn.anchorMax = new Vector2(0.5f, 0.5f);
            retryBtn.anchoredPosition = new Vector2(80, -250);
            retryBtn.GetComponent<Button>().onClick.AddListener(() => gm?.RestartLevel());

            var menuBtn = CreateButton(canvasGo.transform, "MenuButton", "MENU",
                new Vector2(160, 70), new Color(0.5f, 0.5f, 0.5f));
            menuBtn.anchorMin = new Vector2(0.5f, 0.5f);
            menuBtn.anchorMax = new Vector2(0.5f, 0.5f);
            menuBtn.anchoredPosition = new Vector2(260, -250);
            menuBtn.GetComponent<Button>().onClick.AddListener(() => gm?.ReturnToWorldSelect());
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

            var txtRect = CreateText(rect, "Label", label, 26, TextAnchor.MiddleCenter, Color.white);
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
