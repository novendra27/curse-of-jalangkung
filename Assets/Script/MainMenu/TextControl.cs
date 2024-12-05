using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class TextControl : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    private CanvasGroup canvasGroup;
    private float showTime = 6.5f;
    private float hideTime = 14.4f;
    private float fadeDuration = 2f;
    private float switchSceneTime = 15.5f;

    void Start()
    {
        canvasGroup = GetComponentInParent<CanvasGroup>();

        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup tidak ditemukan di Panel atau objek induk!");
            return;
        }

        canvasGroup.alpha = 0; 

        Invoke("FadeIn", showTime);
        Invoke("FadeOut", hideTime);
        Invoke("SwitchToMainMenu", switchSceneTime);  
    }

    void FadeIn()
    {
        StartCoroutine(FadeCanvasGroup(0, 1, fadeDuration));  // Fade from 0 to 1
    }

    void FadeOut()
    {
        StartCoroutine(FadeCanvasGroup(1, 0, fadeDuration));  // Fade from 1 to 0
    }

    IEnumerator FadeCanvasGroup(float start, float end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = end;
    }

    void SwitchToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); 
    }
}
