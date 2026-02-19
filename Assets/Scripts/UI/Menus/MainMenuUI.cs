using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace JumpQuest.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        private void Start()
        {
            if (FindObjectOfType<EventSystem>() == null)
            {
                var esGo = new GameObject("EventSystem");
                esGo.AddComponent<EventSystem>();
                esGo.AddComponent<StandaloneInputModule>();
            }

            BuildMenu();
        }

        private void BuildMenu()
        {
            // Canvas
            var canvasGo = new GameObject("MenuCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            // Background
            var bg = CreatePanel(canvasGo.transform, "Background", new Color(0.15f, 0.2f, 0.35f));

            // Title
            var title = CreateText(canvasGo.transform, "Title", "JUMP QUEST",
                72, TextAnchor.MiddleCenter, new Color(1f, 0.85f, 0.2f));
            title.anchorMin = new Vector2(0.5f, 0.5f);
            title.anchorMax = new Vector2(0.5f, 0.5f);
            title.anchoredPosition = new Vector2(0, 200);
            title.sizeDelta = new Vector2(800, 100);

            // Subtitle
            var sub = CreateText(canvasGo.transform, "Subtitle", "A Mountain Adventure",
                28, TextAnchor.MiddleCenter, Color.white);
            sub.anchorMin = new Vector2(0.5f, 0.5f);
            sub.anchorMax = new Vector2(0.5f, 0.5f);
            sub.anchoredPosition = new Vector2(0, 130);
            sub.sizeDelta = new Vector2(600, 50);

            // Play button
            var playBtn = CreateMenuButton(canvasGo.transform, "PlayButton", "PLAY",
                new Vector2(0, 30), new Color(0.2f, 0.8f, 0.3f));
            playBtn.GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("WorldSelect"));

            // Cosmetics button
            var cosBtn = CreateMenuButton(canvasGo.transform, "CosmeticsButton", "COSMETICS",
                new Vector2(0, -60), new Color(0.7f, 0.3f, 0.9f));
            cosBtn.GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("Cosmetics"));

            // Skills button
            var skillBtn = CreateMenuButton(canvasGo.transform, "SkillsButton", "SKILLS",
                new Vector2(0, -150), new Color(0.2f, 0.6f, 1f));
            skillBtn.GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("SkillTree"));

            // Dev: Level Wizard button
            var devBtn = CreateMenuButton(canvasGo.transform, "DevButton", "DEV: LEVEL WIZARD",
                new Vector2(0, -240), new Color(0.5f, 0.5f, 0.5f));
            devBtn.GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("LevelWizard"));

            // Player info
            var gm = Core.GameManager.Instance;
            if (gm != null)
            {
                var info = CreateText(canvasGo.transform, "PlayerInfo",
                    $"Level {gm.Progress.Level}  |  {gm.Progress.Currency} Coins",
                    24, TextAnchor.MiddleCenter, new Color(0.8f, 0.8f, 0.8f));
                info.anchorMin = new Vector2(0.5f, 0);
                info.anchorMax = new Vector2(0.5f, 0);
                info.anchoredPosition = new Vector2(0, 60);
                info.sizeDelta = new Vector2(600, 40);
            }
        }

        private RectTransform CreateMenuButton(Transform parent, string name, string label,
            Vector2 offset, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = offset;
            rect.sizeDelta = new Vector2(350, 70);

            var img = go.AddComponent<Image>();
            img.color = color;
            go.AddComponent<Button>();

            var txtRect = CreateText(rect, "Label", label, 30, TextAnchor.MiddleCenter, Color.white);
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.sizeDelta = Vector2.zero;

            return rect;
        }

        private RectTransform CreatePanel(Transform parent, string name, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            var img = go.AddComponent<Image>();
            img.color = color;
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
