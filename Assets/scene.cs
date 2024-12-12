using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  // Untuk memuat scene


public class scene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadGamePlayScene()
    {
        StartCoroutine(LoadSceneWithDelay(3));
    }

    public void LoadMainMenuScene()
    {
        StartCoroutine(LoadSceneWithDelay(1));
    }

    private IEnumerator LoadSceneWithDelay(int sceneIndex)
    {
        // Delay selama 0.5 detik
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadSceneAsync(sceneIndex);
    }
}
