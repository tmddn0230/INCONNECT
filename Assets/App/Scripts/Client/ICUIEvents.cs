using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ICUIEvents : MonoBehaviour
{
    public static ICUIEvents Instance;
    public Image fadeImage; // ���̵� ȿ���� ���� �̹���
    public float fadeDuration = 1f; // ���̵� ��/�ƿ� ���� �ð�

    private void Awake()
    {
        // �̱��� ������ ����Ͽ� �ν��Ͻ��� ����
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
        // �� �ε�
        StartCoroutine(FadeOutAndLoadScene(sceneName));
    }

    private IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        // ���̵� �ƿ�
        yield return StartCoroutine(Fade(0f, 1f));

        // �� �ε�
        SceneManager.LoadScene(sceneName);

        // �� �ε� �Ϸ� ���
        yield return new WaitForSeconds(0.5f);

        // ���̵� ��
        yield return StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator FadeIn()
    {
        // ���̵� ��
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

        // ������ ���� ��Ȯ�� ����
        color.a = endAlpha;
        fadeImage.color = color;
    }

    public void OnButtonClick()
    {
        // "ICPrototype_Cafe" ������ ��ȯ
        LoadScene("ICPrototype_Cafe");
    }
}
