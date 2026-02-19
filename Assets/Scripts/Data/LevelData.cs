using System;
using System.Collections.Generic;
using UnityEngine;

namespace JumpQuest.Data
{
    [Serializable]
    public class LevelData
    {
        public string worldId;
        public string levelName;
        public int difficulty; // 1-10
        public float parTime;  // seconds
        public string setting; // e.g. "foothills", "peak"
        public string finishType; // "flag", "portal", "chest"

        public PlayerSpawn playerSpawn;
        public List<PlatformData> platforms = new List<PlatformData>();
        public List<MovingPlatformData> movingPlatforms = new List<MovingPlatformData>();
        public List<RotatingObstacleData> rotatingObstacles = new List<RotatingObstacleData>();
        public List<JumpPadData> jumpPads = new List<JumpPadData>();
        public List<CoinData> coins = new List<CoinData>();
        public List<CheckpointData> checkpoints = new List<CheckpointData>();
        public List<HazardData> hazards = new List<HazardData>();
        public FinishData finish;
    }

    [Serializable]
    public class PlayerSpawn
    {
        public float x, y, z;
        public Vector3 ToVector3() => new Vector3(x, y, z);
    }

    [Serializable]
    public class PlatformData
    {
        public float x, y, z;
        public float sx = 4f, sy = 0.5f, sz = 4f; // scale
        public string color = "gray";
        public Vector3 Position => new Vector3(x, y, z);
        public Vector3 Scale => new Vector3(sx, sy, sz);
    }

    [Serializable]
    public class MovingPlatformData
    {
        public float x, y, z;
        public float sx = 3f, sy = 0.5f, sz = 3f;
        public List<WaypointData> waypoints = new List<WaypointData>();
        public float speed = 3f;
        public bool loop = true;
        public Vector3 Position => new Vector3(x, y, z);
        public Vector3 Scale => new Vector3(sx, sy, sz);
    }

    [Serializable]
    public class WaypointData
    {
        public float x, y, z;
        public Vector3 ToVector3() => new Vector3(x, y, z);
    }

    [Serializable]
    public class RotatingObstacleData
    {
        public float x, y, z;
        public float sx = 6f, sy = 0.5f, sz = 0.5f;
        public float ax, ay = 1f, az; // rotation axis
        public float speed = 90f;
        public Vector3 Position => new Vector3(x, y, z);
        public Vector3 Scale => new Vector3(sx, sy, sz);
        public Vector3 Axis => new Vector3(ax, ay, az);
    }

    [Serializable]
    public class JumpPadData
    {
        public float x, y, z;
        public float force = 18f;
        public Vector3 Position => new Vector3(x, y, z);
    }

    [Serializable]
    public class CoinData
    {
        public float x, y, z;
        public int value = 1;
        public Vector3 Position => new Vector3(x, y, z);
    }

    [Serializable]
    public class CheckpointData
    {
        public float x, y, z;
        public int index;
        public Vector3 Position => new Vector3(x, y, z);
    }

    [Serializable]
    public class HazardData
    {
        public float x, y, z;
        public float sx = 2f, sy = 0.5f, sz = 2f;
        public string type = "spike"; // "spike", "lava", "boulder"
        public Vector3 Position => new Vector3(x, y, z);
        public Vector3 Scale => new Vector3(sx, sy, sz);
    }

    [Serializable]
    public class FinishData
    {
        public float x, y, z;
        public Vector3 Position => new Vector3(x, y, z);
    }
}
