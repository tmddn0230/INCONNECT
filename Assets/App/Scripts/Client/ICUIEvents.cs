using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ICUIEvents : MonoBehaviour
{
    public static ICUIEvents Instance;
    public Image fadeImage; // 페이드 효과를 위한 이미지
    public float fadeDuration = 1f; // 페이드 인/아웃 지속 시간

    private void Awake()
    {
        // 싱글톤 패턴을 사용하여 인스턴스를 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void LoadScene(string sceneName)
    {
        // 씬 로드
        StartCoroutine(FadeOutAndLoadScene(sceneName));
    }

    private IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        // 페이드 아웃
        yield return StartCoroutine(Fade(0f, 1f));

        // 씬 로드
        SceneManager.LoadScene(sceneName);

        // 씬 로드 완료 대기
        yield return new WaitForSeconds(0.5f);

        // 페이드 인
        yield return StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator FadeIn()
    {
        // 페이드 인
        yield return StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        // 마지막 값을 정확히 설정
        color.a = endAlpha;
        fadeImage.color = color;
    }

    public void OnButtonClick()
    {
        // "ICPrototype_Cafe" 씬으로 전환
        LoadScene("ICPrototype_Cafe");
    }
}
