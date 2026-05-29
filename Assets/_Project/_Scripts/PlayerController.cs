using UnityEngine;

namespace LAS
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private TerrainManager _terrainManager;
        [SerializeField] private float _speed = 10;
        [SerializeField] private float _gravity = 0.5f;

        [SerializeField] private Transform _playerVisuals;
        [SerializeField] private float _rotationSpeed = 10.0f;

        //Placeholder - Just to visualise the detection of ground height with no collision and some simple smoothing so it's not too jarring when you exit an obstacle
        private void Update()
        {
            float x = transform.position.x + _speed * Time.deltaTime;
            float y = _terrainManager.GetTerrainHeightAtX(x);

            LerpTransformUp(_terrainManager.GetNormalAtPosition(x));
            if (y > transform.position.y)
                transform.position = new Vector3(x, y, 0);
            else
                transform.position = new Vector3(x, Mathf.Max(transform.position.y - Time.deltaTime * _gravity, y), 0);
        }

        private void LerpTransformUp(Vector3 targetUp)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_playerVisuals.forward, targetUp);
            _playerVisuals.rotation = Quaternion.Slerp(_playerVisuals.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
        }
    }
}