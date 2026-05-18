using System;
using System.Collections.Generic;
using UnityEngine;

namespace LAS
{
    public class TerrainManager : MonoBehaviour
    {
        [SerializeField] private float _slopeSeverity = 0.3f;
        [SerializeField] private float _sineFrequency = 0.5f;
        [SerializeField] private float _sineStrength = 0.5f;
        
        [SerializeField] private Material _terrainMaterial;
        
        [SerializeField] private List<TerrainObstacle> _terrainObstacles;

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
    }
}