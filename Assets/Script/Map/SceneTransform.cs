using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneTransform : MonoBehaviour {
    public GameObject Loading;
    public Text Loading_text;
    public CanvasGroup Fade_img;
    float fadeDuration = 1f;

    public static SceneTransform Instance {
        get {
            return instance;
        }
    }
    private static SceneTransform instance;

    void Start() {
        if(instance != null) {
            DestroyImmediate(this.gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(this.gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Fade_img.DOFade(0, fadeDuration)
        .OnStart(() => {
            Loading.SetActive(false);
        })
        .OnComplete(() => {
            Fade_img.blocksRaycasts = false;
        });
    }

    public void ChangeScene(string sceneName, GameObject Player, Vector2 SpawnPosition) {
        Fade_img.DOFade(1, fadeDuration / 2)
        .OnStart(() => {
            Fade_img.blocksRaycasts = true;
        })
        .OnComplete(() => {
            StartCoroutine("LoadScene", sceneName);
        });
    }

    IEnumerator LoadScene(string sceneName) {
        Loading.SetActive(true);

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;

        float past_time = 0;
        float percentage = 0;

        while(!(async.isDone)) {
            yield return null;

            past_time += Time.deltaTime;

            if(percentage >= 90) {
                percentage = Mathf.Lerp(percentage, 100, past_time);

                if(percentage == 100) {
                    async.allowSceneActivation = true;
                }
            }
            else {
                percentage = Mathf.Lerp(percentage, async.progress * 100f, past_time);
                if(percentage >= 90) past_time = 0;
            }
            Loading_text.text = percentage.ToString("0") + "%s";
        }
    }
}