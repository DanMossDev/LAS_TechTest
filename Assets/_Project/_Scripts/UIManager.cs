using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace LAS
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _distanceTravelledText;
        [SerializeField] private RectTransform _endScreenPanel;

        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _highScoreText;

        private float _score;

        public void UpdateDistanceTravelled(float distance)
        {
            _score = distance / 5;
            _distanceTravelledText.text = _score.ToString("0.0") + "m";
        }

        public void ShowEndScreen()
        {
            Time.timeScale = 0;
            
            if (!PlayerPrefs.HasKey("highscore"))
                PlayerPrefs.SetFloat("highscore", 0);
            
            PlayerPrefs.SetFloat("highscore", Mathf.Max(PlayerPrefs.GetFloat("highscore"), _score));
            
            _scoreText.text = _score.ToString("0.0") + "m";
            _highScoreText.text = PlayerPrefs.GetFloat("highscore").ToString("0.0") + "m";
            
            _endScreenPanel.gameObject.SetActive(true);
            _endScreenPanel.DOAnchorPosY(0, 1.0f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                InputManager.SubmitInput += Submit;
            }).SetUpdate(true);
        }

        private void Submit()
        {
            
            InputManager.SubmitInput -= Submit;
            SceneTransitionManager.LoadMenuScene();
        }
    }
}
