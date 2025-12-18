using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeController : MonoBehaviour
{
    [SerializeField] Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    public static FadeController Instance { get; private set; } = null;

    void Awake() {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (fadeImage != null)
                DontDestroyOnLoad(fadeImage.gameObject.transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
    }

    public void Load(string scene) {
        StartCoroutine(LoadScene(scene));
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

    private IEnumerator LoadScene(string newScene)
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

        // �� ��ȯ �� �ٽ� FadeIn ȣ��
        StartCoroutine(FadeIn());
    }
}
