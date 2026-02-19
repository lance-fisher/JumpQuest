using UnityEngine;
using System.IO;
using System.Collections.Generic;
using JumpQuest.Gameplay;

namespace JumpQuest.Data
{
    public static class LevelLoader
    {
        public static void LoadLevel(string worldId, int levelIndex)
        {
            string fileName = $"{worldId}_level_{levelIndex}.json";
            string json = LoadLevelJson(worldId, fileName);

            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError($"Level file not found: {fileName}");
                CreateFallbackLevel();
                return;
            }

            LevelData data = JsonUtility.FromJson<LevelData>(json);
            if (data == null)
            {
                Debug.LogError($"Failed to parse level JSON: {fileName}");
                CreateFallbackLevel();
                return;
            }

            BuildLevel(data);
        }

        public static void LoadLevelFromJson(string json)
        {
            LevelData data = JsonUtility.FromJson<LevelData>(json);
            if (data == null)
            {
                Debug.LogError("Failed to parse custom level JSON");
                CreateFallbackLevel();
                return;
            }
            BuildLevel(data);
        }

        private static string LoadLevelJson(string worldId, string fileName)
        {
            // Try StreamingAssets first
            string streamingPath = Path.Combine(Application.streamingAssetsPath, "Levels", CapitalizeFirst(worldId), fileName);
            if (File.Exists(streamingPath))
                return File.ReadAllText(streamingPath);

            // Try persistent data (user-created levels)
            string persistentPath = Path.Combine(Application.persistentDataPath, "Levels", fileName);
            if (File.Exists(persistentPath))
                return File.ReadAllText(persistentPath);

            // Try Resources fallback
            string resourcePath = $"Levels/{CapitalizeFirst(worldId)}/{Path.GetFileNameWithoutExtension(fileName)}";
            TextAsset asset = Resources.Load<TextAsset>(resourcePath);
            if (asset != null)
                return asset.text;

            return null;
        }

        private static string CapitalizeFirst(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        private static void BuildLevel(LevelData data)
        {
            GameObject levelRoot = new GameObject("LevelRoot");

            // Ground plane (visual only, platforms handle collision)
            CreateGroundPlane(levelRoot.transform);

            // Skybox / ambient based on world
            SetWorldAmbience(data.worldId);

            // Static platforms
            foreach (var p in data.platforms)
            {
                CreateStaticPlatform(p, levelRoot.transform);
            }

            // Moving platforms
            foreach (var mp in data.movingPlatforms)
            {
                CreateMovingPlatform(mp, levelRoot.transform);
            }

            // Rotating obstacles
            foreach (var ro in data.rotatingObstacles)
            {
                CreateRotatingObstacle(ro, levelRoot.transform);
            }

            // Jump pads
            foreach (var jp in data.jumpPads)
            {
                CreateJumpPad(jp, levelRoot.transform);
            }

            // Coins
            foreach (var c in data.coins)
            {
                CreateCoin(c, levelRoot.transform);
            }

            // Checkpoints
            foreach (var cp in data.checkpoints)
            {
                CreateCheckpoint(cp, levelRoot.transform);
            }

            // Hazards
            foreach (var h in data.hazards)
            {
                CreateHazard(h, levelRoot.transform);
            }

            // Finish
            if (data.finish != null)
            {
                CreateFinish(data.finish, data.finishType, levelRoot.transform);
            }

            // Spawn player
            SpawnPlayer(data.playerSpawn ?? new PlayerSpawn { x = 0, y = 2, z = 0 });

            // Directional light
            if (GameObject.Find("Directional Light") == null)
            {
                var lightGo = new GameObject("Directional Light");
                var light = lightGo.AddComponent<Light>();
                light.type = LightType.Directional;
                light.intensity = 1.2f;
                light.color = new Color(1f, 0.95f, 0.85f);
                lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            }
        }

        private static void CreateGroundPlane(Transform parent)
        {
            // Large visual ground below the level
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "GroundPlane";
            ground.transform.parent = parent;
            ground.transform.position = new Vector3(0, -0.5f, 0);
            ground.transform.localScale = new Vector3(20f, 1f, 20f);

            var rend = ground.GetComponent<Renderer>();
            rend.material = new Material(Shader.Find("Standard"));
            rend.material.color = new Color(0.35f, 0.55f, 0.3f); // grassy green

            // Add collider for fall safety
            var col = ground.GetComponent<Collider>();
            col.isTrigger = false;
        }

        private static void SetWorldAmbience(string worldId)
        {
            switch (worldId)
            {
                case "mountains":
                    Camera.main.backgroundColor = new Color(0.55f, 0.75f, 0.95f); // sky blue
                    RenderSettings.ambientLight = new Color(0.6f, 0.65f, 0.7f);
                    RenderSettings.fogColor = new Color(0.7f, 0.8f, 0.9f);
                    RenderSettings.fog = true;
                    RenderSettings.fogDensity = 0.005f;
                    break;
                default:
                    Camera.main.backgroundColor = new Color(0.4f, 0.6f, 0.9f);
                    break;
            }
        }

        private static void SpawnPlayer(PlayerSpawn spawn)
        {
            // Create player
            var playerGo = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            playerGo.name = "Player";
            playerGo.transform.position = spawn.ToVector3();
            playerGo.tag = "Player";

            // Remove default collider (CharacterController replaces it)
            Object.Destroy(playerGo.GetComponent<CapsuleCollider>());

            var cc = playerGo.AddComponent<CharacterController>();
            cc.height = 2f;
            cc.radius = 0.4f;
            cc.center = Vector3.up * 1f;
            cc.slopeLimit = 45f;
            cc.stepOffset = 0.4f;

            var player = playerGo.AddComponent<PlayerController>();

            // Player material
            var rend = playerGo.GetComponent<Renderer>();
            rend.material = new Material(Shader.Find("Standard"));
            rend.material.color = new Color(0.2f, 0.6f, 1f); // bright blue

            // Camera
            var camGo = Camera.main.gameObject;
            var camCtrl = camGo.GetComponent<CameraController>();
            if (camCtrl == null)
                camCtrl = camGo.AddComponent<CameraController>();
            camCtrl.Target = playerGo.transform;
        }

        private static void CreateStaticPlatform(PlatformData p, Transform parent)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Platform";
            go.transform.parent = parent;
            go.transform.position = p.Position;
            go.transform.localScale = p.Scale;

            var rend = go.GetComponent<Renderer>();
            rend.material = new Material(Shader.Find("Standard"));
            rend.material.color = ParseColor(p.color);
        }

        private static void CreateMovingPlatform(MovingPlatformData mp, Transform parent)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "MovingPlatform";
            go.transform.parent = parent;
            go.transform.position = mp.Position;
            go.transform.localScale = mp.Scale;

            var rend = go.GetComponent<Renderer>();
            rend.material = new Material(Shader.Find("Standard"));
            rend.material.color = new Color(0.3f, 0.7f, 0.4f); // green-ish

            // Add trigger on top for parenting player
            var triggerGo = new GameObject("PlatformTrigger");
            triggerGo.transform.parent = go.transform;
            triggerGo.transform.localPosition = new Vector3(0, 0.6f, 0);
            var triggerCol = triggerGo.AddComponent<BoxCollider>();
            triggerCol.isTrigger = true;
            triggerCol.size = new Vector3(1f, 0.3f, 1f);

            var mover = go.AddComponent<MovingPlatform>();
            mover.Speed = mp.speed;
            mover.Loop = mp.loop;

            var waypoints = new Vector3[mp.waypoints.Count];
            for (int i = 0; i < mp.waypoints.Count; i++)
                waypoints[i] = mp.waypoints[i].ToVector3();
            mover.LocalWaypoints = waypoints;
        }

        private static void CreateRotatingObstacle(RotatingObstacleData ro, Transform parent)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "RotatingObstacle";
            go.tag = "Hazard";
            go.transform.parent = parent;
            go.transform.position = ro.Position;
            go.transform.localScale = ro.Scale;

            var rend = go.GetComponent<Renderer>();
            rend.material = new Material(Shader.Find("Standard"));
            rend.material.color = new Color(0.9f, 0.2f, 0.2f); // red

            var rot = go.AddComponent<RotatingObstacle>();
            rot.RotationAxis = ro.Axis;
            rot.RotationSpeed = ro.speed;
        }

        private static void CreateJumpPad(JumpPadData jp, Transform parent)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = "JumpPad";
            go.transform.parent = parent;
            go.transform.position = jp.Position;
            go.transform.localScale = new Vector3(1.5f, 0.2f, 1.5f);

            var rend = go.GetComponent<Renderer>();
            rend.material = new Material(Shader.Find("Standard"));
            rend.material.color = new Color(1f, 0.85f, 0f); // gold

            // Replace collider with trigger
            Object.Destroy(go.GetComponent<Collider>());
            var col = go.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(1f, 2.5f, 1f);

            var pad = go.AddComponent<JumpPad>();
            pad.LaunchForce = jp.force;
        }

        private static void CreateCoin(CoinData c, Transform parent)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = "Coin";
            go.transform.parent = parent;
            go.transform.position = c.Position;
            go.transform.localScale = new Vector3(0.6f, 0.05f, 0.6f);
            go.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

            var rend = go.GetComponent<Renderer>();
            rend.material = new Material(Shader.Find("Standard"));
            rend.material.color = new Color(1f, 0.84f, 0f); // gold
            rend.material.SetFloat("_Metallic", 0.8f);
            rend.material.SetFloat("_Glossiness", 0.9f);

            // Replace collider with trigger
            Object.Destroy(go.GetComponent<Collider>());
            var col = go.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 1.2f;

            var coin = go.AddComponent<Coin>();
            coin.Value = c.value;
        }

        private static void CreateCheckpoint(CheckpointData cp, Transform parent)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = $"Checkpoint_{cp.index}";
            go.transform.parent = parent;
            go.transform.position = cp.Position;
            go.transform.localScale = new Vector3(0.4f, 2f, 0.4f);

            var rend = go.GetComponent<Renderer>();
            rend.material = new Material(Shader.Find("Standard"));
            rend.material.color = new Color(1f, 0.5f, 0f); // orange

            // Trigger
            Object.Destroy(go.GetComponent<Collider>());
            var col = go.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(3f, 1.5f, 3f);

            var checkpoint = go.AddComponent<Checkpoint>();
            checkpoint.Index = cp.index;
        }

        private static void CreateHazard(HazardData h, Transform parent)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = $"Hazard_{h.type}";
            go.tag = "Hazard";
            go.transform.parent = parent;
            go.transform.position = h.Position;
            go.transform.localScale = h.Scale;

            var rend = go.GetComponent<Renderer>();
            rend.material = new Material(Shader.Find("Standard"));

            switch (h.type)
            {
                case "lava":
                    rend.material.color = new Color(1f, 0.3f, 0f);
                    rend.material.SetColor("_EmissionColor", new Color(1f, 0.3f, 0f) * 0.5f);
                    rend.material.EnableKeyword("_EMISSION");
                    break;
                case "spike":
                    rend.material.color = new Color(0.4f, 0.4f, 0.4f);
                    break;
                default:
                    rend.material.color = Color.red;
                    break;
            }
        }

        private static void CreateFinish(FinishData f, string finishType, Transform parent)
        {
            GameObject go;
            switch (finishType)
            {
                case "portal":
                    go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go.transform.localScale = new Vector3(2f, 3f, 0.5f);
                    var rend = go.GetComponent<Renderer>();
                    rend.material = new Material(Shader.Find("Standard"));
                    rend.material.color = new Color(0.5f, 0.2f, 1f);
                    rend.material.SetColor("_EmissionColor", new Color(0.5f, 0.2f, 1f) * 1.5f);
                    rend.material.EnableKeyword("_EMISSION");
                    break;
                case "chest":
                    go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    go.transform.localScale = new Vector3(1.5f, 1f, 1f);
                    go.GetComponent<Renderer>().material = new Material(Shader.Find("Standard"));
                    go.GetComponent<Renderer>().material.color = new Color(0.7f, 0.5f, 0.1f);
                    break;
                default: // flag
                    go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    go.transform.localScale = new Vector3(0.2f, 3f, 1.5f);
                    go.GetComponent<Renderer>().material = new Material(Shader.Find("Standard"));
                    go.GetComponent<Renderer>().material.color = Color.white;
                    break;
            }

            go.name = "Finish";
            go.transform.parent = parent;
            go.transform.position = f.Position;

            // Replace collider with trigger
            Object.Destroy(go.GetComponent<Collider>());
            var col = go.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(2f, 3f, 2f);

            go.AddComponent<FinishGoal>();
        }

        private static Color ParseColor(string color)
        {
            switch (color?.ToLower())
            {
                case "gray": case "grey": return new Color(0.5f, 0.5f, 0.5f);
                case "brown": return new Color(0.55f, 0.35f, 0.2f);
                case "stone": return new Color(0.6f, 0.58f, 0.55f);
                case "snow": case "white": return new Color(0.92f, 0.94f, 0.96f);
                case "green": return new Color(0.3f, 0.6f, 0.3f);
                case "blue": return new Color(0.3f, 0.4f, 0.8f);
                case "red": return Color.red;
                case "wood": return new Color(0.6f, 0.4f, 0.2f);
                default: return new Color(0.5f, 0.5f, 0.5f);
            }
        }

        private static void CreateFallbackLevel()
        {
            // Minimal playable level if JSON fails
            var root = new GameObject("FallbackLevel");

            // Ground
            var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.transform.parent = root.transform;
            ground.transform.position = new Vector3(0, 0, 0);
            ground.transform.localScale = new Vector3(20, 1, 20);

            // A few platforms
            for (int i = 1; i <= 3; i++)
            {
                var plat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                plat.transform.parent = root.transform;
                plat.transform.position = new Vector3(i * 6, i * 2, 0);
                plat.transform.localScale = new Vector3(4, 0.5f, 4);
            }

            // Finish
            var finishGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            finishGo.name = "Finish";
            finishGo.transform.parent = root.transform;
            finishGo.transform.position = new Vector3(24, 8, 0);
            finishGo.transform.localScale = new Vector3(2, 2, 2);
            Object.Destroy(finishGo.GetComponent<Collider>());
            var finCol = finishGo.AddComponent<BoxCollider>();
            finCol.isTrigger = true;
            finCol.size = new Vector3(2, 3, 2);
            finishGo.AddComponent<FinishGoal>();

            SpawnPlayer(new PlayerSpawn { x = 0, y = 2, z = 0 });
        }

        public static string ValidateLevelJson(string json)
        {
            try
            {
                var data = JsonUtility.FromJson<LevelData>(json);
                if (data == null) return "Failed to parse JSON";
                if (string.IsNullOrEmpty(data.worldId)) return "Missing worldId";
                if (string.IsNullOrEmpty(data.levelName)) return "Missing levelName";
                if (data.finish == null) return "Missing finish goal";
                if (data.playerSpawn == null) return "Missing playerSpawn";
                if (data.platforms.Count == 0 && data.movingPlatforms.Count == 0) return "No platforms defined";
                return "OK";
            }
            catch (System.Exception e)
            {
                return $"JSON error: {e.Message}";
            }
        }
    }
}
