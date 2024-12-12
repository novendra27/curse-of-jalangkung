using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        StartCoroutine(PlayGameWithDelay());
    }

    private IEnumerator PlayGameWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadSceneAsync(2);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
