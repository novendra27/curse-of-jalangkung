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

    public GameObject panelPopUp;  // Referensi ke panel pop-up
    public float displayDuration = 6f;  // Durasi tampilan panel pop-up (dalam detik)

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

    void Start()
    {
        // Panggil metode untuk menampilkan panel pop-up saat scene dimulai
        ShowPopUp();
    }

    // Metode untuk menampilkan pop-up
    private void ShowPopUp()
    {
        if (panelPopUp != null)
        {
            panelPopUp.SetActive(true);  // Menampilkan panel pop-up
            StartCoroutine(HidePopUpAfterDelay());  // Mulai coroutine untuk menyembunyikan panel setelah delay
        }
    }

    // Coroutine untuk menyembunyikan panel setelah beberapa detik
    private IEnumerator HidePopUpAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);  // Tunggu selama `displayDuration` detik
        if (panelPopUp != null)
        {
            panelPopUp.SetActive(false);  // Menyembunyikan panel pop-up setelah 10 detik
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
