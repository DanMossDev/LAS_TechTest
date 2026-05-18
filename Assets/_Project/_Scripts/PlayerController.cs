using UnityEngine;

namespace LAS
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private TerrainManager _terrainManager;
        [SerializeField] private float _speed = 10;

        void Update()
        {
            float x = transform.position.x + _speed * Time.deltaTime;
            float y = _terrainManager.GetTerrainHeightAtX(x);
            transform.position = new Vector3(x, y, 0);
        }
    }
}