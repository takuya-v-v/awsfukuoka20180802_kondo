using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

// singleton model
public class SceneManager : SingletonMonoBehaviour<SceneManager>
{
    [SerializeField] private bool IsLoadingUIScene;
 
    // UIScene Manegement
    [SerializeField] private List<string> uiSceneNameList = new List<string>();
    [SerializeField] private string currentUISceneName;
    [SerializeField] private SceneBase currentSceneBase;
    [SerializeField] private UnityEngine.SceneManagement.Scene currentUIScene;
    [SerializeField] private string beforeUiSceneName;
    public int UISceneCount
    {
        get
        {
            return uiSceneNameList.Count;
        }
    }
    public string CurrentUISceneName
    {
        get
        {
            return currentUISceneName;
        }
    }
    public SceneBase CurrentSceneBase
    {
        get
        {
            return currentSceneBase;
        }
    }
    public string BeforeUiSceneName
    {
        get
        {
            return beforeUiSceneName;
        }
    }

    public void ReplaceUIScene(string sceneName)
    {
        if (IsLoadingUIScene)
        {
            return;
        }
        IsLoadingUIScene = true;
        StartCoroutine(_ReplaceUIScene(sceneName));
    }

    private IEnumerator _UnloadCurrentUIScene()
    {
        if (currentUIScene.IsValid() && currentSceneBase != null)
        {
            yield return currentSceneBase.Out();
        }

    }

    public IEnumerator _ReplaceUIScene(string sceneName)
    {
        UnityEngine.SceneManagement.Scene beforeScene = currentUIScene;
        SceneBase beforeSceneBase = currentSceneBase;
        if (currentUIScene.IsValid() && beforeSceneBase != null)
        {
            beforeSceneBase.SetSortingOrderCanvas(12);
        }
        currentUIScene = new UnityEngine.SceneManagement.Scene();
        currentSceneBase = null;
        uiSceneNameList.Clear();
        if (beforeScene.IsValid() && beforeSceneBase != null)
        {
            beforeSceneBase.SetSortingOrderCanvas(11);
            yield return beforeSceneBase.Out();
        }
        if (beforeScene.IsValid())
        {
            AsyncOperation oparation2 = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(beforeScene);
            do
            {
                yield return null;
            } while (oparation2.isDone == false);
        }
        beforeUiSceneName = null;
        AsyncOperation oparation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        do
        {
            yield return null;
        } while (oparation.isDone == false);
        yield return null;
        currentUIScene = GetSceneBySceneName(sceneName);
        if (currentUIScene.IsValid())
        {
            currentUISceneName = sceneName;
            currentSceneBase = GetSceneBase(currentUIScene);
            uiSceneNameList.Add(sceneName);
            if (currentSceneBase != null)
            {
                currentSceneBase.SetSortingOrderCanvas(12);
                yield return currentSceneBase.In();
            }
        }
        yield return null;

        IsLoadingUIScene = false;
    }


    public void PushUIScene(string sceneName)
    {
        if (currentUISceneName == sceneName)
        {
            return;
        }
        if (IsLoadingUIScene)
        {
            return;
        }
        IsLoadingUIScene = true;
        StartCoroutine(_PushUIScene(sceneName));
    }

    public IEnumerator _PushUIScene(string sceneName)
    {
        UnityEngine.SceneManagement.Scene beforeScene = currentUIScene;
        SceneBase beforeSceneBase = currentSceneBase;
        if (currentUIScene.IsValid() && beforeSceneBase != null)
        {
            beforeSceneBase.SetSortingOrderCanvas(12);
        }
        currentUIScene = new UnityEngine.SceneManagement.Scene();
        currentSceneBase = null;
        beforeUiSceneName = beforeScene.path;
        AsyncOperation oparation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        do
        {
            yield return null;
        } while (oparation.isDone == false);
        currentUIScene = GetSceneBySceneName(sceneName);
        if (beforeScene.IsValid() && beforeSceneBase != null)
        {
            beforeSceneBase.SetSortingOrderCanvas(11);
            StartCoroutine(beforeSceneBase.PushOut());
        }
        if (currentUIScene.IsValid())
        {
            currentUISceneName = sceneName;
            currentSceneBase = GetSceneBase(currentUIScene);
            uiSceneNameList.Add(sceneName);
            if (currentSceneBase != null)
            {
                currentSceneBase.SetSortingOrderCanvas(12);
                yield return currentSceneBase.PushIn();
            }
        }
        yield return null;
        if (beforeScene.IsValid())
        {
            AsyncOperation oparation2 = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(beforeScene);
            do
            {
                yield return null;
            } while (oparation2.isDone == false);
        }
        IsLoadingUIScene = false;
    }

    public void PopUIScene(int depth = 1)
    {
        if (IsLoadingUIScene)
        {
            return;
        }
        if (uiSceneNameList.Count <= 1)
        {
            return;
        }
        if (uiSceneNameList.Count <= depth)
        {
            depth = uiSceneNameList.Count - 1;
        }
        IsLoadingUIScene = true;
        StartCoroutine(_PopUIScene(depth));
    }

    public IEnumerator _PopUIScene(int depth = 1)
    {
        UnityEngine.SceneManagement.Scene beforeScene = currentUIScene;
        SceneBase beforeSceneBase = currentSceneBase;
        if (currentUIScene.IsValid() && beforeSceneBase != null)
        {
            beforeSceneBase.SetSortingOrderCanvas(12);
        }
        currentUIScene = new UnityEngine.SceneManagement.Scene();
        currentSceneBase = null;
        for (int i = 0; i < depth; i++)
        {
            uiSceneNameList.RemoveAt(uiSceneNameList.Count - 1);
        }
        string sceneName = uiSceneNameList[uiSceneNameList.Count - 1];
        AsyncOperation oparation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        do
        {
            yield return null;
        } while (oparation.isDone == false);
        if (beforeScene.IsValid() && beforeSceneBase != null)
        {
            beforeSceneBase.SetSortingOrderCanvas(11);
            StartCoroutine(beforeSceneBase.PopOut());
        }
        currentUIScene = GetSceneBySceneName(sceneName);
        if (currentUIScene.IsValid())
        {
            currentUISceneName = sceneName;
            currentSceneBase = GetSceneBase(currentUIScene);
            if (currentSceneBase != null)
            {
                currentSceneBase.SetSortingOrderCanvas(12);
                yield return currentSceneBase.PopIn();
            }
        }
        yield return null;
        if (beforeScene.IsValid())
        {
            AsyncOperation oparation2 = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(beforeScene);
            do
            {
                yield return null;
            } while (oparation2.isDone == false);
        }
        IsLoadingUIScene = false;
    }

    private SceneBase GetSceneBase(UnityEngine.SceneManagement.Scene scene)
    {
        SceneBase uiSceneBase = null;
        foreach (GameObject obj in scene.GetRootGameObjects())
        {
            uiSceneBase = obj.GetComponent<SceneBase>();
            if (uiSceneBase != null)
            {
                return uiSceneBase;
            }
        }
        foreach (GameObject obj in scene.GetRootGameObjects())
        {
            uiSceneBase = obj.GetComponentInChildren<SceneBase>();
            if (uiSceneBase != null)
            {
                return uiSceneBase;
            }
        }
        return null;
    }

    private UnityEngine.SceneManagement.Scene GetSceneBySceneName(string sceneName)
    {
        return UnityEngine.SceneManagement.SceneManager.GetSceneByPath("Assets/" + sceneName + ".unity");
    }

    private string GetSceneName(UnityEngine.SceneManagement.Scene scene)
    {
        return scene.path.Substring(7, scene.path.Length - 13);
    }
}
