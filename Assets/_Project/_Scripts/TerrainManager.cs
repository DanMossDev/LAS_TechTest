using System;
using System.Collections.Generic;
using UnityEngine;

namespace LAS
{
    public class TerrainManager : MonoBehaviour
    {
        [Header("Obstacle Spawning Logic")]
        [SerializeField] private LevelObstaclesModel _levelObstaclesModel;
        
        [SerializeField, Tooltip("X distance in Unity units ahead of the player that obstacles will be spawned")] private float _obstacleSpawnOffset = 25.0f;
        
        [SerializeField] private Vector2 _timeBetweenObstacleSpawns;
        
        [Space, Header("Visuals")]
        [SerializeField] private float _slopeSeverity = 0.3f;
        [SerializeField] private float _sineFrequency = 0.5f;
        [SerializeField] private float _sineStrength = 0.5f;
        
        [SerializeField] private Material _terrainMaterial;
        
        private float _timeTilNextObstacleSpawn;
        private float _timeSinceLastObstacleSpawn;
        
        private List<TerrainObstacle> _terrainObstacles = new List<TerrainObstacle>();
        private float _randomOffset;
        
        //Property Hashes
        private int _slopeSeverityHash = Shader.PropertyToID("_SlopeSeverity");
        private int _sineFrequencyHash = Shader.PropertyToID("_SineWaveFrequency");
        private int _sineStrengthHash = Shader.PropertyToID("_SineWaveStrength");
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!_terrainMaterial)
                return;
            
            _terrainMaterial.SetFloat("_SlopeSeverity", _slopeSeverity);
            _terrainMaterial.SetFloat("_SineWaveFrequency", _sineFrequency);
            _terrainMaterial.SetFloat("_SineWaveStrength", _sineStrength);
        }
#endif

        private void FixedUpdate()
        {
            UpdateMaterialProperties();

            _timeSinceLastObstacleSpawn += Time.deltaTime;
            if (_timeSinceLastObstacleSpawn >= _timeTilNextObstacleSpawn)
                SpawnNewObstacle();
        }

        private void UpdateMaterialProperties()
        {
            _terrainMaterial.SetFloat(_slopeSeverityHash, _slopeSeverity);
            _terrainMaterial.SetFloat(_sineFrequencyHash, _sineFrequency);
            _terrainMaterial.SetFloat(_sineStrengthHash, _sineStrength);
        }

        public float GetTerrainHeightAtX(float x)
        {
            var y = -(Mathf.Sin((x + _randomOffset) * _sineFrequency) * _sineStrength + (x * _slopeSeverity));

            foreach (var terrainObstacle in _terrainObstacles)
            {
                if (!terrainObstacle.IsXCoordinateWithinBounds(x))
                    continue;

                var obstacleY = terrainObstacle.GetHeight(x);
                if (obstacleY > y)
                    return obstacleY;
                break;
            }

            return y;
        }

        private void SpawnNewObstacle()
        {
            _timeSinceLastObstacleSpawn = 0;
            _timeTilNextObstacleSpawn = UnityEngine.Random.Range(_timeBetweenObstacleSpawns.x, _timeBetweenObstacleSpawns.y);
            
            var obstacle = _levelObstaclesModel.GetTerrainObstacle();
            
            float xPos = transform.position.x + _obstacleSpawnOffset; //TODO - adapt offset for larger obstacles?
            float yPos = GetTerrainHeightAtX(xPos);
            
            obstacle.transform.position = new Vector3(xPos, yPos, 0);
            obstacle.Initialise(this);
            _terrainObstacles.Add(obstacle);
        }

        public void RemoveObstacle(TerrainObstacle obstacle)
        {
            if (_terrainObstacles.Contains(obstacle))
                _terrainObstacles.Remove(obstacle);
        }
    }
}