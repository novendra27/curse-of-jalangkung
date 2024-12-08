using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class UIGameplayLogic : MonoBehaviour
{
    public Image HealthBar;
    public TextMeshProUGUI HealthText;
    //public GameObject PanelGameResult;
    //public TextMeshProUGUI GameResultText;

    public void UpdateHealthBar(float CurrentHealth, float MaxHealth)
    {
        HealthBar.fillAmount = CurrentHealth / MaxHealth;
        HealthText.text = CurrentHealth.ToString();

        //if (CurrentHealth <= 0) GameResult(false);
    }

    //public void GameResult(bool win)
    //{
    //    PanelGameResult.SetActive(true);
    //    Cursor.lockState = CursorLockMode.None;
    //    Cursor.visible = true;
    //    if (win)
    //    {
    //        GameResultText.color = Color.green;
    //        GameResultText.text = "You Win!";
    //    }
    //    else
    //    {
    //        GameResultText.color = Color.red;
    //        GameResultText.text = "You Lose!";
    //    }
    //}

    //public void GameResultDecision(bool TryAgain)
    //{
    //    if (TryAgain)
    //    {
    //        SceneManager.LoadScene("Maze");
    //    }
    //    else
    //    {
    //        SceneManager.LoadScene("MainMenu");
    //    }
    //}
}
