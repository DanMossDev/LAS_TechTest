using UnityEngine;

namespace LAS
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private TerrainManager _terrainManager;
        [SerializeField] private float _speed = 10;
        [SerializeField] private float _gravity = 0.5f;

        //Placeholder - Just to visualise the detection of ground height with no collision and some simple smoothing so it's not too jarring when you exit an obstacle
        void Update()
        {
            float x = transform.position.x + _speed * Time.deltaTime;
            float y = _terrainManager.GetTerrainHeightAtX(x);
            
            if (y > transform.position.y)
                transform.position = new Vector3(x, y, 0);
            else
                transform.position = new Vector3(x, Mathf.Max(transform.position.y - Time.deltaTime * _gravity, y), 0);
        }
    }
}