using UnityEngine;

namespace LAS
{
    [CreateAssetMenu(fileName = "LevelObstaclesModel", menuName = "LAS/LevelObstaclesModel")]
    public class LevelObstaclesModel : ScriptableObject
    {
        [SerializeField] private TerrainObstacleSpawnData[] _obstaclePrefabs;

        public TerrainObstacle GetTerrainObstacle(System.Random rand)
        {
            var obstacleData = _obstaclePrefabs[rand.Next(0, _obstaclePrefabs.Length)];
            var gameObject = MossUtils.ObjectPoolController.Instance.Get(obstacleData.ObstaclePrefab.name);
            gameObject.transform.localScale = Vector3.one * rand.Next(Mathf.FloorToInt(obstacleData.ScaleRange.x), Mathf.CeilToInt(obstacleData.ScaleRange.y));
            if (!gameObject.TryGetComponent(out TerrainObstacle terrainObstacle))
                Debug.LogError("[LevelObstaclesModel] TerrainObstacle component was somehow destroyed in object pool");
            return terrainObstacle;
        }
    }

    [System.Serializable]
    public struct TerrainObstacleSpawnData
    {
        public TerrainObstacle ObstaclePrefab;
        public Vector2 ScaleRange;
        //Could add chance to spawn
    }
}
