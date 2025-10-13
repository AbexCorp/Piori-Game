using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    protected virtual void Start()
    {
        if (_useLoadingScreenOnLoad)
            StartCoroutine(LoadingScreen());
    }

    #region >>> SceneLoad <<<

    [SerializeField]
    protected bool _useLoadingScreenOnLoad = true;
    [SerializeField]
    protected GameObject _screenBlock;
    [SerializeField]
    protected RectTransform _loadingScreen;

    protected void LoadScene(string scene)
    {
        StartCoroutine(StartSceneLoading(scene));
    }
    protected IEnumerator StartSceneLoading(string scene)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(scene);
        op.allowSceneActivation = false;
        yield return StartCoroutine(LoadingScreen(false));
        op.allowSceneActivation = true;

        int counter = 200;
        while (counter > 0)
        {
            yield return new WaitForSeconds(0.1f);
            counter--;
        }

        if (counter == 0)
        {
            Debug.LogWarning($"Failed To Load Scene {scene}");
            yield return null;
            Application.Quit();
        }
    }

    protected IEnumerator LoadingScreen(bool LoadIn = true)
    {
        _screenBlock.gameObject.SetActive(true);
        _loadingScreen.sizeDelta = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);
        Vector2 start;
        Vector2 end;
        float time = 0;
        float duration = 1.2f;

        if (LoadIn)
        {
            _loadingScreen.anchoredPosition = new Vector2(0, 0);
            start = _loadingScreen.anchoredPosition;
            end = new Vector2(-_loadingScreen.sizeDelta.x, 0);
        }
        else
        {
            _loadingScreen.anchoredPosition = new Vector2(_loadingScreen.sizeDelta.x, 0);

            start = _loadingScreen.anchoredPosition;
            end = Vector2.zero;
        }


        while (time < duration)
        {
            yield return null;
            time += Time.deltaTime;
            float t = time / duration;

            t = Mathf.SmoothStep(0f, 1f, t);
            _loadingScreen.anchoredPosition = Vector2.Lerp(start, end, t);
        }
        if (LoadIn)
        {
            _screenBlock.SetActive(false);
        }

        yield return null;
    }

    #endregion
}
