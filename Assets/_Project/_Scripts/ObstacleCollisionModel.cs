using UnityEngine;

namespace LAS
{
    [CreateAssetMenu(fileName = "ObstacleCollisionModel", menuName = "LAS/ObstacleCollisionModel")]
    public class ObstacleCollisionModel : ScriptableObject
    {
        [SerializeField] private float[] _heightTable;
        [SerializeField] private int _resolution = 25;

        [SerializeField] private Sprite _sprite;
        
        public float[] HeightTable => _heightTable;
        public int Resolution => _resolution;
        
#if UNITY_EDITOR
        [ContextMenu("Generate Height Table")]
        public void GenerateHeightTable()
        {
            var rect = _sprite.rect;
            var texture = _sprite.texture;
            
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
    }
}
