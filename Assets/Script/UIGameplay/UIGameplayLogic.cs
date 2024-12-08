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
    public Button buttonMenu;
    public GameObject panelMenu; // Panel yang ingin dibuka/tutup

    public void Update()
    {
        // Cek jika tombol ESC ditekan
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (panelMenu != null)
            {
                // Jika panel aktif, tutup panel
                if (panelMenu.activeSelf)
                {
                    panelMenu.SetActive(false);
                }
                else
                {
                    // Jika panel tidak aktif, panggil metode onClick pada button
                    if (buttonMenu != null)
                    {
                        buttonMenu.onClick.Invoke();
                    }
                }
            }
        }
    }


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
