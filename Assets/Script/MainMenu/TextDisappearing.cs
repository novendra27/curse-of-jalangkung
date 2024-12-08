using UnityEngine;
using TMPro;  // Namespace untuk TextMeshPro

public class TextDisappearing : MonoBehaviour
{
    public TMP_Text textObject;  // Referensi ke objek TextMeshPro yang ingin dihilangkan
    private float timer = 0f;  // Timer untuk menghitung waktu sejak scene dimulai
    private float disappearTime = 5f;  // Waktu dalam detik untuk menghilangkan teks (5 detik)

    void Start()
    {
        // Pastikan teks dimulai dalam keadaan terlihat
        if (textObject != null)
        {
            textObject.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        // Tambahkan waktu yang telah berlalu ke timer
        timer += Time.deltaTime;

        // Cek jika waktu sudah lebih dari 5 detik
        if (timer >= disappearTime)
        {
            // Sembunyikan teks setelah 5 detik
            if (textObject != null)
            {
                textObject.gameObject.SetActive(false);
            }
        }
    }
}
