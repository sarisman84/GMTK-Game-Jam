using UnityEngine.SceneManagement;

namespace Utility
{
    public static class SceneExtention
    {
        public static void SetSceneActive(this Scene scene, bool value)
        {
            foreach (var rootObjects in scene.GetRootGameObjects())
            {
                rootObjects.SetActive(value);
            }

        }
    }
}