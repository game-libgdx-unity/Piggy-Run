using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

public class ChangeScene : MonoBehaviour
{
    bool Sceneloaded = false;
    CanvasGroup cg;
    void Start()
    {
        cg = GameObject.Find("UI").GetComponent<CanvasGroup>();
    }
    // Use this for initialization
    public void OnBtnPlayClicked()
    {
        if (!Sceneloaded)
            StartCoroutine(LoadPlayScene());
    }

    private IEnumerator LoadPlayScene()
    {
        Sceneloaded = true;
        Destroy(GameObject.Find("EventSystem"));

        //Camera.main.clearFlags = CameraClearFlags.Depth;
        cg.DOFade(0f, 1f).OnComplete(() => SceneManager.UnloadScene("start"));
        yield return null;
        SceneManager.LoadScene("main", LoadSceneMode.Additive);
        //Destroy(Camera.main.gameObject);

    }
}
