using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class UIGameplayLogic : MonoBehaviour
{
    // Variabel yang sudah ada
    public Image HealthBar;
    public TextMeshProUGUI HealthText;
    public TextMeshProUGUI TotemCountText;
    public Button buttonMenu;
    public GameObject panelMenu;
    public GameObject panelPopUp;
    public float displayDuration = 6f;
    public GameObject PanelGameResult;
    public GameObject PanelJumpScare;
    public TextMeshProUGUI GameResultText;
    public GameObject PressF;

    private int totalTotems = 0;
    private int destroyedTotems = 0;

    // Variabel untuk timer
    public TextMeshProUGUI TimerText;
    public float timerDuration; // Durasi timer dalam detik (misalnya 5 menit)
    private float timer;

    void Start()
{
    if (PanelJumpScare == null)
    {
        Debug.LogError("PanelJumpScare is not assigned! Please assign it in the Inspector.");
    }

    ShowPopUp();
    StartTimer(timerDuration);
}


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

        // Perbarui timer
        UpdateTimer();
    }

    public void ShowPressF(bool show)
    {
        if (PressF != null)
        {
            PressF.SetActive(show);
        }
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

        if (CurrentHealth <= 0)
        {
            GameResult(false);
        }
    }

    public void SetTotalTotems(int total)
    {
        totalTotems = total;
        UpdateTotemCountText();
    }

    public void IncrementDestroyedTotems()
    {
        destroyedTotems++;
        UpdateTotemCountText();

        if (destroyedTotems >= totalTotems)
        {
            GameResult(true);
        }
    }

    private void UpdateTotemCountText()
    {
        if (TotemCountText != null)
        {
            TotemCountText.text = $"{destroyedTotems}/{totalTotems}";
        }
    }

    private void GameResult(bool win)
    {
        if (PanelGameResult != null && GameResultText != null)
        {
            PanelGameResult.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (win)
            {
                GameResultText.color = Color.green;
                GameResultText.text = "You Win!";
            }
            else
            {
                GameResultText.color = Color.red;
                GameResultText.text = "You Lose!";
            }
        }
    }

    // Metode untuk memulai timer
    public void StartTimer(float duration)
    {
        timerDuration = duration;
        timer = timerDuration;
    }

    // Metode untuk memperbarui timer
    private void UpdateTimer()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);
            TimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            // Timer selesai, panggil metode GameResult dengan parameter false
            GameResult(false);
        }
    }
}
