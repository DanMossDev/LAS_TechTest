using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LAS
{
    public static class SceneTransitionManager
    {
        private const string k_MenuSceneName = "MenuScene";
        private const string k_LoadingSceneName = "LoadingScene";
        private const string k_GameplaySceneName = "GameScene";

        public static async Task LoadScene(string sceneName, Action callback = null)
        {
            await LoadSceneAsync(sceneName, callback);
        }

        public static void LoadMenuScene()
        {
            _ = LoadScene(k_MenuSceneName);
        }
        
        public static void LoadGameplayScene()
        {
            _ = LoadScene(k_GameplaySceneName);
        }

        private static bool _loadInProgress = false;
        private static async Task LoadSceneAsync(string newScene, Action callback = null)
        {
            while (_loadInProgress)
                await Task.Yield();

            string currentScene = SceneManager.GetActiveScene().name;
            
            _loadInProgress = true;
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(k_LoadingSceneName, LoadSceneMode.Additive);

            if (loadOperation == null)
            {
                LogNullScene(k_LoadingSceneName);
                _loadInProgress = false;
                return;
            }

            while (!loadOperation.isDone)
            {
                await Task.Yield();
            }
            
            LoadingScreen.Instance.FadeToBlack();
            
            loadOperation = SceneManager.UnloadSceneAsync(currentScene);
            
            if (loadOperation == null)
            {
                LogNullScene(currentScene, true);
                _loadInProgress = false;
                return;
            }

            while (!loadOperation.isDone)
            {
                await Task.Yield();
            }
            loadOperation = SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Additive);
            
            if (loadOperation == null)
            {
                LogNullScene(newScene);
                _loadInProgress = false;
                return;
            }

            while (!loadOperation.isDone)
            {
                await Task.Yield();
            }
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(newScene));
            await Task.Yield();
            
            LoadingScreen.Instance.FadeFromBlack();

            await Task.Delay(1050);

            AsyncOperation unLoadOperation = SceneManager.UnloadSceneAsync(k_LoadingSceneName);
            
            if (unLoadOperation == null)
            {
                Debug.LogError("Failed to remove loading scene");
                _loadInProgress = false;
                return;
            }

            while (!unLoadOperation.isDone)
            {
                await Task.Yield();
            }
            await Task.Yield();
            
            callback?.Invoke();
            _loadInProgress = false;
        }

        private static void LogNullScene(string sceneName, bool unload = false)
        {
            if (unload)
                Debug.LogError($"Failed to unload scene async: {sceneName} not found");
            else
                Debug.LogError($"Failed to load scene async: {sceneName} not found");
        }
    }
}
