using System.Collections;
using UnityEngine;

public class JalangkungLogic : MonoBehaviour
{
    public float destructionTime = 5f; // Waktu channeling sebelum jalangkung hancur
    private bool playerInRange = false;
    private bool isBeingDestroyed = false;
    private float channelingTimer = 0f;

    [Header("Player Reference")]
    public GameObject player; // Referensi untuk objek player

    [Header("Effects and SFX")]
    public GameObject destructionEffect; // Efek saat jalangkung dihancurkan
   // public AudioClip channelingAudio;
 //   public AudioClip destroyAudio;

  //  private AudioSource audioSource;

    void Start()
    {
     //   audioSource = GetComponent<AudioSource>();

        // Pastikan player tidak null
        if (player == null)
        {
            Debug.LogWarning("Player reference is not assigned in JalangkungLogic!");
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKey(KeyCode.F))
        {
            if (!isBeingDestroyed)
            {
                StartChanneling();
            }
            else
            {
                channelingTimer += Time.deltaTime;

                if (channelingTimer >= destructionTime)
                {
                    DestroyJalangkung();
                }
            }
        }
        else if (isBeingDestroyed)
        {
            // Reset jika player berhenti menekan F
            StopChanneling();
        }
    }

    private void StartChanneling()
    {
        isBeingDestroyed = true;
        channelingTimer = 0f;

        // Mainkan audio channeling
     /*   if (channelingAudio != null)
        {
            audioSource.clip = channelingAudio;
            audioSource.Play();
        }*/

        Debug.Log("Channeling started...");
    }

    private void StopChanneling()
    {
        isBeingDestroyed = false;
        channelingTimer = 0f;

        // Hentikan audio channeling
      /*  if (audioSource.isPlaying && audioSource.clip == channelingAudio)
        {
            audioSource.Stop();
        }
*/
        Debug.Log("Channeling stopped.");
    }

    private void DestroyJalangkung()
    {
        Debug.Log("Jalangkung destroyed!");

        // Mainkan efek penghancuran
        if (destructionEffect != null)
        {
            Instantiate(destructionEffect, transform.position, Quaternion.identity);
        }

        // Mainkan audio penghancuran
      /*  if (destroyAudio != null)
        {
            audioSource.clip = destroyAudio;
            audioSource.Play();
        }
*/
        // Hancurkan objek jalangkung setelah sedikit waktu (agar audio sempat diputar)
        //Destroy(gameObject, destroyAudio != null ? destroyAudio.length : 0f);
        Destroy(gameObject);

    }

    private void OnTriggerEnter(Collider other)
    {
        // Cek apakah objek yang masuk adalah player
        if (other.gameObject == player)
        {
            playerInRange = true;
            Debug.Log("Player entered Jalangkung area.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Cek apakah objek yang keluar adalah player
        if (other.gameObject == player)
        {
            playerInRange = false;
            StopChanneling();
            Debug.Log("Player exited Jalangkung area.");
        }
    }
}
