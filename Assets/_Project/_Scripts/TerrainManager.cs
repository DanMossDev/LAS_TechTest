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
        
        [SerializeField] private float _metaSineFrequency = 0.5f;
        [SerializeField] private float _metaSineSubFrequency = 0.5f;
        [SerializeField] private float _metaSineStrength = 0.5f;
        
        [SerializeField] private float _metaCosineFrequency = 0.5f;
        [SerializeField] private float _metaCosineStrength = 0.5f;
        
        [SerializeField] private Material _terrainMaterial;

        [Space, Header("Cameras")] 
        [SerializeField] private Camera _mainCamera;
        
        public Camera MainCamera => _mainCamera;
        
        private float _timeTilNextObstacleSpawn;
        private float _timeSinceLastObstacleSpawn;
        
        private List<TerrainObstacle> _terrainObstacles = new List<TerrainObstacle>();
        private float _randomOffset;
        
        //Property Hashes
        private int _slopeSeverityHash = Shader.PropertyToID("_SlopeSeverity");
        private int _sineFrequencyHash = Shader.PropertyToID("_SineWaveFrequency");
        private int _sineStrengthHash = Shader.PropertyToID("_SineWaveStrength");
        private int _metaSineFrequencyHash = Shader.PropertyToID("_MetaSineWaveFrequency");
        private int _metaSineSubFrequencyHash = Shader.PropertyToID("_MetaSineWaveSubFrequency");
        private int _metaSineStrengthHash = Shader.PropertyToID("_MetaSineWaveStrength");
        private int _metaCosineFrequencyHash = Shader.PropertyToID("_MetaCosineWaveFrequency");
        private int _metaCosineStrengthHash = Shader.PropertyToID("_MetaCosineWaveStrength");
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!_terrainMaterial)
                return;
            
            UpdateMaterialProperties();
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
            _terrainMaterial.SetFloat(_metaSineFrequencyHash, _metaSineFrequency);
            _terrainMaterial.SetFloat(_metaSineSubFrequencyHash, _metaSineSubFrequency);
            _terrainMaterial.SetFloat(_metaSineStrengthHash, _metaSineStrength);
            _terrainMaterial.SetFloat(_metaCosineFrequencyHash, _metaCosineFrequency);
            _terrainMaterial.SetFloat(_metaCosineStrengthHash, _metaCosineStrength);
        }

        public float GetTerrainHeightAtX(float x)
        {
            float sin = Mathf.Sin((x + _randomOffset) * _sineFrequency) * _sineStrength;
            float metaSubSin = Mathf.Sin(x * _metaSineSubFrequency);
            float metaSin = Mathf.Sin((x + _randomOffset - metaSubSin) * _metaSineFrequency) * _metaSineStrength;
            float metaCos = Mathf.Cos((x + _randomOffset) * _metaCosineFrequency) * _metaCosineStrength;
            
            var y = -(sin + metaSin + metaCos + x * _slopeSeverity);

            float highestY = y;

            foreach (var terrainObstacle in _terrainObstacles)
            {
                if (!terrainObstacle.IsXCoordinateWithinBounds(x))
                    continue;

                var obstacleY = terrainObstacle.GetHeight(x);
                if (obstacleY > highestY)
                    highestY = obstacleY;
            }

            return highestY;
        }

        private void SpawnNewObstacle()
        {
            _timeSinceLastObstacleSpawn = 0;
            _timeTilNextObstacleSpawn = UnityEngine.Random.Range(_timeBetweenObstacleSpawns.x, _timeBetweenObstacleSpawns.y);
            
            var obstacle = _levelObstaclesModel.GetTerrainObstacle();
            
            float xPos = transform.position.x + _obstacleSpawnOffset * _mainCamera.orthographicSize;
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