using TMPro;
using UnityEngine;

namespace LAS
{
    public class MenuScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        
        private void OnEnable()
        {
            Time.timeScale = 1;
            if (!PlayerPrefs.HasKey("highscore")) //Would prefer (hah) not to use player prefs but it's quick and dirty when under time constraints
                PlayerPrefs.SetFloat("highscore", 0);
            
            _scoreText.text = PlayerPrefs.GetFloat("highscore").ToString("0.0") + "m";
            
            InputManager.AnyInput += BeginGame;
        }

        private void OnDisable()
        {
            InputManager.AnyInput -= BeginGame;
        }

        private void BeginGame()
        {
            SceneTransitionManager.LoadGameplayScene();
        }
    }
}
