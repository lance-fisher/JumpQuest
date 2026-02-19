#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;
using JumpQuest.Core;

public class ProjectSetup : EditorWindow
{
    [MenuItem("JumpQuest/Setup Project (Run Once)")]
    public static void SetupProject()
    {
        SetupTags();
        CreateAllScenes();
        SetupBuildSettings();
        Debug.Log("JumpQuest project setup complete!");
        EditorUtility.DisplayDialog("JumpQuest Setup", "Project setup complete!\n\nScenes created and build settings configured.\n\nOpen MainMenu scene and press Play to test.", "OK");
    }

    [MenuItem("JumpQuest/Validate Level Data")]
    public static void ValidateLevelData()
    {
        string levelsPath = Path.Combine(Application.streamingAssetsPath, "Levels");
        if (!Directory.Exists(levelsPath))
        {
            Debug.LogError("No Levels folder found in StreamingAssets");
            return;
        }

        string[] files = Directory.GetFiles(levelsPath, "*.json", SearchOption.AllDirectories);
        int valid = 0, invalid = 0;

        foreach (string file in files)
        {
            string json = File.ReadAllText(file);
            string result = JumpQuest.Data.LevelLoader.ValidateLevelJson(json);
            if (result == "OK")
            {
                Debug.Log($"VALID: {Path.GetFileName(file)}");
                valid++;
            }
            else
            {
                Debug.LogError($"INVALID: {Path.GetFileName(file)} - {result}");
                invalid++;
            }
        }

        EditorUtility.DisplayDialog("Level Validation",
            $"Checked {files.Length} files.\nValid: {valid}\nInvalid: {invalid}", "OK");
    }

    private static void SetupTags()
    {
        // Ensure tags exist
        AddTag("Player");
        AddTag("Hazard");
        Debug.Log("Tags configured.");
    }

    private static void AddTag(string tag)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            if (tagsProp.GetArrayElementAtIndex(i).stringValue == tag)
                return; // already exists
        }

        tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
        tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tag;
        tagManager.ApplyModifiedProperties();
    }

    private static void CreateAllScenes()
    {
        string scenesPath = "Assets/Scenes";
        if (!Directory.Exists(scenesPath))
            Directory.CreateDirectory(scenesPath);

        CreateScene(scenesPath, "MainMenu", BootstrapScene.SceneType.MainMenu);
        CreateScene(scenesPath, "WorldSelect", BootstrapScene.SceneType.WorldSelect);
        CreateScene(scenesPath, "Gameplay", BootstrapScene.SceneType.Gameplay, true);
        CreateScene(scenesPath, "Results", BootstrapScene.SceneType.Results);
        CreateScene(scenesPath, "SkillTree", BootstrapScene.SceneType.SkillTree);
        CreateScene(scenesPath, "Cosmetics", BootstrapScene.SceneType.Cosmetics);
        CreateScene(scenesPath, "LevelWizard", BootstrapScene.SceneType.LevelWizard);

        Debug.Log("All scenes created.");
    }

    private static void CreateScene(string folder, string sceneName, BootstrapScene.SceneType type, bool needsCamera = false)
    {
        string path = Path.Combine(folder, sceneName + ".unity");

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Bootstrap object
        var bootstrapGo = new GameObject("SceneBootstrap");
        var bootstrap = bootstrapGo.AddComponent<BootstrapScene>();
        bootstrap.Type = type;

        // Camera
        var camGo = new GameObject("Main Camera");
        camGo.tag = "MainCamera";
        var cam = camGo.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;

        if (needsCamera)
        {
            // Gameplay camera positioned for 3D
            cam.backgroundColor = new Color(0.55f, 0.75f, 0.95f);
            camGo.transform.position = new Vector3(0, 8, -10);
            camGo.transform.rotation = Quaternion.Euler(30, 0, 0);
            camGo.AddComponent<AudioListener>();
        }
        else
        {
            cam.backgroundColor = new Color(0.15f, 0.2f, 0.35f);
            camGo.AddComponent<AudioListener>();
        }

        EditorSceneManager.SaveScene(scene, path);
        Debug.Log($"Created scene: {path}");
    }

    private static void SetupBuildSettings()
    {
        string[] sceneNames = { "MainMenu", "WorldSelect", "Gameplay", "Results", "SkillTree", "Cosmetics", "LevelWizard" };
        var scenes = new EditorBuildSettingsScene[sceneNames.Length];

        for (int i = 0; i < sceneNames.Length; i++)
        {
            string path = $"Assets/Scenes/{sceneNames[i]}.unity";
            scenes[i] = new EditorBuildSettingsScene(path, true);
        }

        EditorBuildSettings.scenes = scenes;
        Debug.Log("Build settings configured with all scenes.");
    }
}
#endif
