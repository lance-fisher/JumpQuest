using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace JumpQuest.UI
{
    /// <summary>
    /// Builds the entire HUD programmatically at runtime.
    /// Attach to a GameObject in the Gameplay scene — it creates the Canvas, joystick, buttons, and HUD.
    /// </summary>
    public class HUDBuilder : MonoBehaviour
    {
        private void Start()
        {
            // Ensure EventSystem exists
            if (FindObjectOfType<EventSystem>() == null)
            {
                var esGo = new GameObject("EventSystem");
                esGo.AddComponent<EventSystem>();
                esGo.AddComponent<StandaloneInputModule>();
            }

            BuildHUD();
        }

        private void BuildHUD()
        {
            // Canvas
            var canvasGo = new GameObject("GameCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGo.AddComponent<GraphicRaycaster>();

            // Input bridge
            var bridgeGo = new GameObject("TouchInputBridge");
            bridgeGo.transform.parent = canvasGo.transform;
            var bridge = bridgeGo.AddComponent<TouchInputBridge>();

            // === Left side: Joystick ===
            var joystickBg = CreateUIImage(canvasGo.transform, "JoystickBG",
                new Vector2(180, 180), new Color(1, 1, 1, 0.25f));
            joystickBg.anchorMin = new Vector2(0, 0);
            joystickBg.anchorMax = new Vector2(0, 0);
            joystickBg.anchoredPosition = new Vector2(160, 160);

            var joystickHandle = CreateUIImage(joystickBg, "JoystickHandle",
                new Vector2(70, 70), new Color(1, 1, 1, 0.7f));
            joystickHandle.anchoredPosition = Vector2.zero;

            var joystick = joystickBg.gameObject.AddComponent<VirtualJoystick>();
            joystick.Background = joystickBg;
            joystick.Handle = joystickHandle;
            joystick.HandleRange = 60f;

            bridge.Joystick = joystick;

            // === Right side: Jump button ===
            var jumpBtn = CreateButton(canvasGo.transform, "JumpButton", "JUMP",
                new Vector2(130, 130), new Color(0.2f, 0.75f, 1f, 0.7f));
            jumpBtn.anchorMin = new Vector2(1, 0);
            jumpBtn.anchorMax = new Vector2(1, 0);
            jumpBtn.anchoredPosition = new Vector2(-140, 140);

            var jumpButton = jumpBtn.GetComponent<Button>();
            jumpButton.onClick.AddListener(() => bridge.OnJumpPressed());

            // === Right side: Run button ===
            var runBtn = CreateButton(canvasGo.transform, "RunButton", "RUN",
                new Vector2(100, 100), new Color(1f, 0.6f, 0.2f, 0.7f));
            runBtn.anchorMin = new Vector2(1, 0);
            runBtn.anchorMax = new Vector2(1, 0);
            runBtn.anchoredPosition = new Vector2(-280, 100);

            // Run uses EventTrigger for hold
            var runTrigger = runBtn.gameObject.AddComponent<EventTrigger>();
            AddTriggerEvent(runTrigger, EventTriggerType.PointerDown, () => bridge.OnRunDown());
            AddTriggerEvent(runTrigger, EventTriggerType.PointerUp, () => bridge.OnRunUp());

            // === Top HUD: Coins + Timer ===
            var hudPanel = new GameObject("HUDPanel");
            hudPanel.transform.SetParent(canvasGo.transform, false);
            var hudRect = hudPanel.AddComponent<RectTransform>();
            hudRect.anchorMin = new Vector2(0, 1);
            hudRect.anchorMax = new Vector2(1, 1);
            hudRect.anchoredPosition = new Vector2(0, -40);
            hudRect.sizeDelta = new Vector2(0, 60);

            var coinText = CreateText(hudPanel.transform, "CoinText", "0",
                36, TextAnchor.MiddleLeft, Color.yellow);
            coinText.anchorMin = new Vector2(0, 0.5f);
            coinText.anchorMax = new Vector2(0, 0.5f);
            coinText.anchoredPosition = new Vector2(80, 0);
            coinText.sizeDelta = new Vector2(200, 50);

            // Coin icon label
            var coinIcon = CreateText(hudPanel.transform, "CoinIcon", "\u25CF",
                40, TextAnchor.MiddleCenter, Color.yellow);
            coinIcon.anchorMin = new Vector2(0, 0.5f);
            coinIcon.anchorMax = new Vector2(0, 0.5f);
            coinIcon.anchoredPosition = new Vector2(30, 0);
            coinIcon.sizeDelta = new Vector2(50, 50);

            var timerText = CreateText(hudPanel.transform, "TimerText", "00:00",
                36, TextAnchor.MiddleCenter, Color.white);
            timerText.anchorMin = new Vector2(0.5f, 0.5f);
            timerText.anchorMax = new Vector2(0.5f, 0.5f);
            timerText.anchoredPosition = new Vector2(0, 0);
            timerText.sizeDelta = new Vector2(200, 50);

            // Pause button (top right)
            var pauseBtn = CreateButton(canvasGo.transform, "PauseButton", "| |",
                new Vector2(60, 60), new Color(0.3f, 0.3f, 0.3f, 0.6f));
            pauseBtn.anchorMin = new Vector2(1, 1);
            pauseBtn.anchorMax = new Vector2(1, 1);
            pauseBtn.anchoredPosition = new Vector2(-50, -40);

            // === Pause Panel (hidden) ===
            var pausePanel = CreatePanel(canvasGo.transform, "PausePanel", new Color(0, 0, 0, 0.7f));
            pausePanel.gameObject.SetActive(false);

            var resumeBtn = CreateButton(pausePanel, "ResumeButton", "RESUME",
                new Vector2(240, 80), new Color(0.2f, 0.8f, 0.3f, 0.9f));
            resumeBtn.anchoredPosition = new Vector2(0, 60);

            var restartBtn = CreateButton(pausePanel, "RestartButton", "RESTART",
                new Vector2(240, 80), new Color(1f, 0.6f, 0.2f, 0.9f));
            restartBtn.anchoredPosition = new Vector2(0, -40);

            var quitBtn = CreateButton(pausePanel, "QuitButton", "QUIT",
                new Vector2(240, 80), new Color(0.9f, 0.2f, 0.2f, 0.9f));
            quitBtn.anchoredPosition = new Vector2(0, -140);

            // Wire up HUD component
            var hud = canvasGo.AddComponent<GameHUD>();
            hud.CoinText = coinText.GetComponent<Text>();
            hud.TimerText = timerText.GetComponent<Text>();
            hud.PausePanel = pausePanel.gameObject;

            pauseBtn.GetComponent<Button>().onClick.AddListener(() => hud.OnPausePressed());
            resumeBtn.GetComponent<Button>().onClick.AddListener(() => hud.OnResumePressed());
            restartBtn.GetComponent<Button>().onClick.AddListener(() => hud.OnRestartPressed());
            quitBtn.GetComponent<Button>().onClick.AddListener(() => hud.OnQuitPressed());

            // === Power-up buttons (shown if unlocked) ===
            var gm = Core.GameManager.Instance;
            if (gm != null && gm.HasSpeedBurst)
            {
                var sbBtn = CreateButton(canvasGo.transform, "SpeedBurstBtn", "BOOST",
                    new Vector2(90, 90), new Color(1f, 0.4f, 0.8f, 0.7f));
                sbBtn.anchorMin = new Vector2(1, 0);
                sbBtn.anchorMax = new Vector2(1, 0);
                sbBtn.anchoredPosition = new Vector2(-160, 280);
                sbBtn.GetComponent<Button>().onClick.AddListener(() => bridge.OnSpeedBurstPressed());
            }

            if (gm != null && gm.HasShield)
            {
                var shBtn = CreateButton(canvasGo.transform, "ShieldBtn", "SHIELD",
                    new Vector2(90, 90), new Color(0.3f, 0.8f, 1f, 0.7f));
                shBtn.anchorMin = new Vector2(1, 0);
                shBtn.anchorMax = new Vector2(1, 0);
                shBtn.anchoredPosition = new Vector2(-280, 240);
                shBtn.GetComponent<Button>().onClick.AddListener(() => bridge.OnShieldPressed());
            }
        }

        // === UI Helpers ===

        private RectTransform CreateUIImage(Transform parent, string name, Vector2 size, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = size;
            var img = go.AddComponent<Image>();
            img.color = color;
            return rect;
        }

        private RectTransform CreateButton(Transform parent, string name, string label,
            Vector2 size, Color bgColor)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = size;
            var img = go.AddComponent<Image>();
            img.color = bgColor;
            go.AddComponent<Button>();

            var textRect = CreateText(rect, "Label", label, 24, TextAnchor.MiddleCenter, Color.white);
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

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

        private void AddTriggerEvent(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction action)
        {
            var entry = new EventTrigger.Entry { eventID = type };
            entry.callback.AddListener((_) => action());
            trigger.triggers.Add(entry);
        }
    }
}
