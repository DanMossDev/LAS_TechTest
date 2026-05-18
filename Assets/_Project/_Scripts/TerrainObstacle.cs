using UnityEngine;

namespace LAS
{
    public class TerrainObstacle : MonoBehaviour
    {
        [SerializeField] private float[] _heightTable;
        [SerializeField] private int _resolution = 25;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private Bounds _spriteBounds => _spriteRenderer.sprite.bounds;
        
#if UNITY_EDITOR
        [ContextMenu("Generate Height Table")]
        public void GenerateHeightTable()
        {
            var sprite = _spriteRenderer.sprite;
            var rect = sprite.rect;
            var texture = sprite.texture;
            
            _heightTable = new float[_resolution];
            
            for (int i = 0; i < _resolution; i++)
            {
                float normalizedX = i / (float)(_resolution - 1);
                int pixelX = (int)(rect.x + normalizedX * rect.width);
                pixelX = Mathf.Clamp(pixelX, 0, texture.width - 1);
                
                float topY = 0;
                for (int y = (int)(rect.y + rect.height - 1); y >= rect.y; y--)
                {
                    if (y >= texture.height) continue;
                    if (texture.GetPixel(pixelX, y).a > 0.1f)
                    {
                        topY = (y - rect.y) / rect.height;
                        break;
                    }
                }
                
                _heightTable[i] = topY;
            }
        }
#endif

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
            
            float floatIndex = t * (_resolution - 1);
            int lowerIndex = Mathf.FloorToInt(floatIndex);
            int higherIndex = Mathf.Min(lowerIndex + 1, _resolution - 1);
            float lerpAmount = floatIndex - lowerIndex;
            
            float normalizedHeight = Mathf.Lerp(_heightTable[lowerIndex], _heightTable[higherIndex], lerpAmount);
            
            Vector2 pivot = _spriteRenderer.sprite.pivot;
            float pivotNormalizedY = pivot.y / _spriteRenderer.sprite.rect.height;
            
            float localY = (normalizedHeight - pivotNormalizedY) * _spriteBounds.size.y;
            
            return transform.position.y + localY * transform.localScale.y;
        }
    }
}
