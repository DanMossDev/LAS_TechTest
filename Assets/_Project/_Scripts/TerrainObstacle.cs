using UnityEngine;

namespace LAS
{
    public class TerrainObstacle : MonoBehaviour
    {
        [SerializeField] private ObstacleCollisionModel _collisionModel;
        
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private Bounds _spriteBounds => _spriteRenderer.sprite.bounds;

        public bool IsXCoordinateWithinBounds(float x)
        {
            x -= transform.position.x;
            return x >= _spriteBounds.min.x * transform.localScale.x && x <= _spriteBounds.max.x * transform.localScale.x;
        }

        public float GetHeight(float x)
        {
            x -= transform.position.x;
            float scaledHalfWidth = _spriteBounds.extents.x * transform.localScale.x;
            
            float t = Mathf.InverseLerp(-scaledHalfWidth, scaledHalfWidth, x);
            t = Mathf.Clamp01(t);
            
            float floatIndex = t * (_collisionModel.Resolution - 1);
            int lowerIndex = Mathf.FloorToInt(floatIndex);
            int higherIndex = Mathf.Min(lowerIndex + 1, _collisionModel.Resolution - 1);
            float lerpAmount = floatIndex - lowerIndex;
            
            float normalizedHeight = Mathf.Lerp(_collisionModel.HeightTable[lowerIndex], _collisionModel.HeightTable[higherIndex], lerpAmount);
            
            Vector2 pivot = _spriteRenderer.sprite.pivot;
            float pivotNormalizedY = pivot.y / _spriteRenderer.sprite.rect.height;
            
            float localY = (normalizedHeight - pivotNormalizedY) * _spriteBounds.size.y;
            
            return transform.position.y + localY * transform.localScale.y;
        }
    }
}
