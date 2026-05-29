using UnityEngine;

namespace LAS
{
    public class GameStarter : MonoBehaviour
    {
        [SerializeField] private GameObject _persistentServices;
        
        private void Start()
        {
            DontDestroyOnLoad(_persistentServices);
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0)
                SceneTransitionManager.LoadMenuScene();
            Destroy(gameObject);
        }
    }
}
