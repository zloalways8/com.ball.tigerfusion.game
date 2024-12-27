using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DimensionalPassageOrchestrator : MonoBehaviour
{
    private void Start()
    {
        BeginSceneStreaming(EntityFormCoordinator.CoreExistencePlane);
    }

    private void BeginSceneStreaming(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        var loadOperation = SceneManager.LoadSceneAsync(sceneName);
        loadOperation.allowSceneActivation = false;

        while (!loadOperation.isDone)
        {
            if (loadOperation.progress >= 0.9f)
            {
                loadOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}