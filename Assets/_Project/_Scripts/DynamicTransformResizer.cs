using UnityEngine;

namespace LAS
{
    public class DynamicTransformResizer : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera;

        private Vector3 _defaultScale;
        private float _defaultLensSize;

        private void Awake()
        {
            _defaultScale = transform.localScale;
            _defaultLensSize = _mainCamera.orthographicSize;
        }

        private void FixedUpdate()
        {
            transform.localScale = _mainCamera.orthographicSize / _defaultLensSize * _defaultScale;
        }
    }
}
