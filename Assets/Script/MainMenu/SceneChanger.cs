using UnityEngine;
using UnityEngine.SceneManagement;  // Untuk memuat scene
using UnityEngine.UI;  // Untuk RawImage
using UnityEngine.Video;  // Untuk VideoPlayer

public class SceneChanger : MonoBehaviour
{
    public RawImage rawImage;  // Referensi ke RawImage untuk video
    public VideoPlayer videoPlayer;  // Referensi ke VideoPlayer untuk memutar video
    public string nextSceneName;  // Nama scene yang ingin dipindah

    private float videoDuration = 30f;  // Durasi waktu dalam detik (30 detik)
    private float timer = 0f;  // Timer untuk menghitung waktu sejak scene dimulai

    void Start()
    {
        // Pastikan VideoPlayer dan RawImage sudah terhubung dengan benar
        if (videoPlayer != null)
        {
            // Menetapkan RawImage untuk menampilkan video
            videoPlayer.targetTexture = new RenderTexture(1920, 1080, 0);
            rawImage.texture = videoPlayer.targetTexture;

            videoPlayer.Play();  // Memulai video
        }
    }

    void Update()
    {
        // Tambahkan waktu yang telah berlalu ke timer
        timer += Time.deltaTime;

        // Cek jika tombol Space ditekan untuk berpindah scene
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadNextScene();
        }

        // Cek jika waktu sudah lebih dari 34 detik sejak scene dimulai
        if (timer >= videoDuration)
        {
            LoadNextScene();
        }
    }

    // Fungsi untuk memuat scene berikutnya
    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
