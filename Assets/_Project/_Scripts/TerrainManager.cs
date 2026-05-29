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
        [SerializeField] private Material _terrainParalaxMaterialNear;
        [SerializeField] private Material _terrainParalaxMaterialFar;

        [Space, Header("Cameras")] 
        [SerializeField] private Camera _mainCamera;
        
        [Space, Header("Randomness")]
        [SerializeField, Tooltip("Editor only, forces the random offset to this value")] private int _randomSeed = 0;

        [SerializeField] private float _nearTerrainRandomOffset = -4.0f;
        [SerializeField] private float _farTerrainRandomOffset = -20.0f;
        
        private float _randomOffset;
        private System.Random _random;
        
        public Camera MainCamera => _mainCamera;
        
        private float _timeTilNextObstacleSpawn;
        private float _timeSinceLastObstacleSpawn;
        
        private List<TerrainObstacle> _terrainObstacles = new List<TerrainObstacle>();
        
        
        //Property Hashes
        private int _slopeSeverityHash = Shader.PropertyToID("_SlopeSeverity");
        private int _sineFrequencyHash = Shader.PropertyToID("_SineWaveFrequency");
        private int _sineStrengthHash = Shader.PropertyToID("_SineWaveStrength");
        private int _metaSineFrequencyHash = Shader.PropertyToID("_MetaSineWaveFrequency");
        private int _metaSineSubFrequencyHash = Shader.PropertyToID("_MetaSineWaveSubFrequency");
        private int _metaSineStrengthHash = Shader.PropertyToID("_MetaSineWaveStrength");
        private int _metaCosineFrequencyHash = Shader.PropertyToID("_MetaCosineWaveFrequency");
        private int _metaCosineStrengthHash = Shader.PropertyToID("_MetaCosineWaveStrength");
        private int _randomOffsetHashHash = Shader.PropertyToID("_RandomOffset");

        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!_terrainMaterial)
                return;
            
            UpdateMaterialProperties(_terrainParalaxMaterialNear, _nearTerrainRandomOffset);
            UpdateMaterialProperties(_terrainParalaxMaterialFar, _farTerrainRandomOffset);
        }
#endif

        private void Awake()
        {
            #if UNITY_EDITOR
            _randomOffset = _randomSeed;
            #else
            _randomOffset = UnityEngine.Random.Range(0, 100000);
            #endif
            _random = new System.Random(_randomSeed);
        }

        private void FixedUpdate()
        {
            UpdateMaterialProperties(_terrainMaterial);

            _timeSinceLastObstacleSpawn += Time.deltaTime;
            if (_timeSinceLastObstacleSpawn >= _timeTilNextObstacleSpawn)
                SpawnNewObstacle();
        }

        private void UpdateMaterialProperties(Material material, float randomOffset = 0)
        {
            material.SetFloat(_slopeSeverityHash, _slopeSeverity);
            material.SetFloat(_sineFrequencyHash, _sineFrequency);
            material.SetFloat(_sineStrengthHash, _sineStrength);
            material.SetFloat(_metaSineFrequencyHash, _metaSineFrequency);
            material.SetFloat(_metaSineSubFrequencyHash, _metaSineSubFrequency);
            material.SetFloat(_metaSineStrengthHash, _metaSineStrength);
            material.SetFloat(_metaCosineFrequencyHash, _metaCosineFrequency);
            material.SetFloat(_metaCosineStrengthHash, _metaCosineStrength);
            material.SetFloat(_randomOffsetHashHash, _randomOffset + randomOffset);
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

        public Vector2 GetNormalAtPosition(float x)
        {
            float x1 = x - 0.1f;
            float x2 = x + 0.1f;
            
            Vector2 pos1 = new Vector2(x1, GetTerrainHeightAtX(x1));
            Vector2 pos2 = new Vector2(x2, GetTerrainHeightAtX(x2));
            
            Vector2 tan = (pos2 - pos1).normalized;
            Vector2 normal = new Vector2(-tan.y, tan.x);
            if (normal.y < 0) normal = -normal;

            return normal;
        }

        private void SpawnNewObstacle()
        {
            _timeSinceLastObstacleSpawn = 0;
            _timeTilNextObstacleSpawn = _random.Next(Mathf.FloorToInt(_timeBetweenObstacleSpawns.x), Mathf.CeilToInt(_timeBetweenObstacleSpawns.y));
            
            var obstacle = _levelObstaclesModel.GetTerrainObstacle(_random);
            
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