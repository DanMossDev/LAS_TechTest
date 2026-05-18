using UnityEngine;

namespace LAS
{
    [CreateAssetMenu(fileName = "LevelObstaclesModel", menuName = "LAS/LevelObstaclesModel")]
    public class LevelObstaclesModel : ScriptableObject
    {
        [SerializeField] private TerrainObstacleSpawnData[] _obstaclePrefabs;

        public TerrainObstacle GetTerrainObstacle()
        {
            var obstacleData = _obstaclePrefabs[Random.Range(0, _obstaclePrefabs.Length)];
            var gameObject = MossUtils.ObjectPoolController.Instance.Get(obstacleData.ObstaclePrefab.name);
            gameObject.transform.localScale = Vector3.one * Random.Range(obstacleData.ScaleRange.x, obstacleData.ScaleRange.y);
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
