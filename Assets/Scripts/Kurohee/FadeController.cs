using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeController : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;
    public string sceneToLoad = "SecondScene";

    private static FadeController instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // 페이드 이미지가 있는 캔버스도 함께 유지
            if (fadeImage != null)
                DontDestroyOnLoad(fadeImage.gameObject.transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
    }

    private IEnumerator FadeIn()
    {
        Color tempColor = fadeImage.color;
        tempColor.a = 1f;
        fadeImage.color = tempColor;

        while (tempColor.a > 0f)
        {
            tempColor.a -= Time.deltaTime / fadeDuration;
            fadeImage.color = tempColor;
            yield return null;
        }
    }

    public IEnumerator FadeOutAndLoadScene(string newScene)
    {
        Color tempColor = fadeImage.color;
        tempColor.a = 0f;
        fadeImage.color = tempColor;

        while (tempColor.a < 1f)
        {
            tempColor.a += Time.deltaTime / fadeDuration;
            fadeImage.color = tempColor;
            yield return null;
        }
        var task  = SceneManager.LoadSceneAsync(newScene);
        task.allowSceneActivation = false;
        while (task.isDone) {
            if(task.progress >= 0.9) {
                task.allowSceneActivation = true;
            }
            yield return null;
        }
        task.allowSceneActivation = true;

        // 씬 전환 후 다시 FadeIn 호출
        StartCoroutine(FadeIn());
    }

    public void OnClickStart()
    {
        StartCoroutine(FadeOutAndLoadScene(sceneToLoad));
    }
}
