#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace LAS.Editor
{
    [InitializeOnLoad]
    public static class BootstrapLoader
    {
        static BootstrapLoader()
        {
            EditorApplication.playModeStateChanged += LoadBootstrapScene;
        }

        static void LoadBootstrapScene(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            }

            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                if (SceneManager.GetActiveScene().buildIndex != 0)
                    EditorSceneManager.LoadScene(0, LoadSceneMode.Additive);
            }
        }
    }
}
#endif