using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using JumpQuest.Core;
using System.Collections.Generic;

namespace JumpQuest.Progression
{
    public class SkillTreeUI : MonoBehaviour
    {
        private struct SkillNode
        {
            public string Id;
            public string DisplayName;
            public string Description;
            public int Cost;
            public int RequiredLevel;
            public string Prerequisite;  // another skill id, or ""
            public Vector2 UIPosition;
            public Color NodeColor;

            public SkillNode(string id, string name, string desc, int cost, int reqLvl,
                string prereq, Vector2 pos, Color color)
            {
                Id = id; DisplayName = name; Description = desc; Cost = cost;
                RequiredLevel = reqLvl; Prerequisite = prereq; UIPosition = pos; NodeColor = color;
            }
        }

        private readonly SkillNode[] skills = new[]
        {
            new SkillNode("featherstep_1", "Feather Step I", "Unlock Double Jump",
                50, 2, "", new Vector2(-250, 150), new Color(0.3f, 0.7f, 1f)),
            new SkillNode("featherstep_2", "Feather Step II", "Higher second jump",
                100, 4, "featherstep_1", new Vector2(-250, 50), new Color(0.2f, 0.6f, 1f)),

            new SkillNode("swiftcurrent_1", "Swift Current I", "Unlock Speed Burst",
                50, 3, "", new Vector2(0, 150), new Color(1f, 0.5f, 0.8f)),
            new SkillNode("swiftcurrent_2", "Swift Current II", "Longer burst duration",
                100, 6, "swiftcurrent_1", new Vector2(0, 50), new Color(1f, 0.4f, 0.7f)),

            new SkillNode("deepguard_1", "Deepguard I", "Unlock Shield",
                75, 5, "", new Vector2(250, 150), new Color(0.3f, 0.9f, 0.9f)),
            new SkillNode("deepguard_2", "Deepguard II", "Longer shield duration",
                120, 8, "deepguard_1", new Vector2(250, 50), new Color(0.2f, 0.8f, 0.8f)),

            new SkillNode("lucky_doubloon", "Lucky Doubloon", "Bonus currency on completion",
                80, 7, "", new Vector2(-125, -60), new Color(1f, 0.85f, 0.2f)),

            new SkillNode("cartographer", "Cartographer", "Reveals hidden coin trails",
                150, 10, "lucky_doubloon", new Vector2(125, -60), new Color(0.6f, 0.4f, 0.2f)),
        };

        private Dictionary<string, Button> nodeButtons = new Dictionary<string, Button>();

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
            var canvasGo = new GameObject("SkillTreeCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            var bg = CreatePanel(canvasGo.transform, "BG", new Color(0.1f, 0.12f, 0.2f));

            // Title
            var title = CreateText(canvasGo.transform, "Title", "SKILL TREE",
                48, TextAnchor.MiddleCenter, Color.white);
            title.anchorMin = new Vector2(0.5f, 1);
            title.anchorMax = new Vector2(0.5f, 1);
            title.anchoredPosition = new Vector2(0, -50);
            title.sizeDelta = new Vector2(600, 60);

            // Back button
            var backBtn = CreateButton(canvasGo.transform, "BackButton", "BACK",
                new Vector2(120, 50), new Color(0.5f, 0.5f, 0.5f));
            backBtn.anchorMin = new Vector2(0, 1);
            backBtn.anchorMax = new Vector2(0, 1);
            backBtn.anchoredPosition = new Vector2(80, -40);
            backBtn.GetComponent<Button>().onClick.AddListener(() =>
                GameManager.Instance?.ReturnToMainMenu());

            // Currency display
            var gm = GameManager.Instance;
            int currency = gm != null ? gm.Progress.Currency : 0;
            var currText = CreateText(canvasGo.transform, "Currency", $"Coins: {currency}",
                28, TextAnchor.MiddleRight, Color.yellow);
            currText.anchorMin = new Vector2(1, 1);
            currText.anchorMax = new Vector2(1, 1);
            currText.anchoredPosition = new Vector2(-40, -50);
            currText.sizeDelta = new Vector2(300, 50);

            // Skill nodes
            foreach (var skill in skills)
            {
                CreateSkillNode(canvasGo.transform, skill, gm);
            }

            // Connection lines between prerequisite nodes (visual only)
            DrawConnections(canvasGo.transform);
        }

        private void CreateSkillNode(Transform parent, SkillNode skill, GameManager gm)
        {
            bool unlocked = gm != null && gm.Progress.IsSkillUnlocked(skill.Id);
            bool canAfford = gm != null && gm.Progress.Currency >= skill.Cost;
            bool prereqMet = string.IsNullOrEmpty(skill.Prerequisite) ||
                             (gm != null && gm.Progress.IsSkillUnlocked(skill.Prerequisite));
            bool levelMet = gm != null && gm.Progress.Level >= skill.RequiredLevel;
            bool canBuy = !unlocked && canAfford && prereqMet && levelMet;

            Color nodeColor = unlocked ? skill.NodeColor :
                              canBuy ? new Color(skill.NodeColor.r, skill.NodeColor.g, skill.NodeColor.b, 0.7f) :
                              new Color(0.3f, 0.3f, 0.3f, 0.5f);

            var node = CreateButton(parent, skill.Id, skill.DisplayName,
                new Vector2(180, 70), nodeColor);
            node.anchorMin = new Vector2(0.5f, 0.5f);
            node.anchorMax = new Vector2(0.5f, 0.5f);
            node.anchoredPosition = skill.UIPosition;

            // Cost label below
            string costLabel = unlocked ? "OWNED" : $"{skill.Cost} coins | Lv {skill.RequiredLevel}";
            var costText = CreateText(node, "Cost", costLabel,
                16, TextAnchor.UpperCenter, new Color(0.8f, 0.8f, 0.8f));
            costText.anchorMin = new Vector2(0, 0);
            costText.anchorMax = new Vector2(1, 0);
            costText.anchoredPosition = new Vector2(0, -18);
            costText.sizeDelta = new Vector2(0, 20);

            var btn = node.GetComponent<Button>();
            btn.interactable = canBuy;
            string skillId = skill.Id;
            int cost = skill.Cost;
            btn.onClick.AddListener(() =>
            {
                if (gm != null && gm.Progress.TryUnlockSkill(skillId, cost))
                {
                    SaveManager.Save(gm.Progress);
                    gm.ApplyUnlockedPowerUps();
                    // Rebuild UI
                    foreach (Transform child in parent)
                        Destroy(child.gameObject);
                    BuildUI();
                }
            });

            nodeButtons[skill.Id] = btn;
        }

        private void DrawConnections(Transform parent)
        {
            // Simple lines between prerequisite nodes
            // In a real build, use a LineRenderer or UI Line; for now, skip
            // (Unity UI doesn't have native lines; a custom Image stretch hack works but is verbose)
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

            var txtRect = CreateText(rect, "Label", label, 20, TextAnchor.MiddleCenter, Color.white);
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
