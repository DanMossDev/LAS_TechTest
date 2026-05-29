using DG.Tweening;
using TMPro;
using UnityEngine;

namespace LAS
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _loadingCanvas;
        [SerializeField] private TextMeshProUGUI _loadingText;
        
        public static LoadingScreen Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void FadeToBlack()
        {
            _loadingText.alpha = 1;
            DOTween.To(() => _loadingCanvas.alpha, x => _loadingCanvas.alpha = x, 1, 0.5f).SetLink(gameObject);
        }

        public void FadeFromBlack()
        {
            _loadingText.DOFade(0, 0.3f).SetLink(gameObject);
            DOTween.To(() => _loadingCanvas.alpha, x => _loadingCanvas.alpha = x, 0, 0.5f).SetLink(gameObject).SetDelay(0.5f);
        }
    }
}
