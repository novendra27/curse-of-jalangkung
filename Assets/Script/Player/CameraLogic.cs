using System.Collections;
using UnityEngine;

public class CameraLogic : MonoBehaviour
{
    public Transform Player;
    public Transform ViewPoint;
    public Transform AIMViewPoint;
    public float RotationSpeed;
    public GameObject TPSCamera, AIMCamera;
    public GameObject CameraCanvas, FlashLight, FlashNormal;
    private Camera mainCamera;
    public Animator anim;

    private int originalPlayerLayer;
    private int aimLayer;

    private bool TPSMode = true, AimMode = false;
    private float flashDuration = 0.1f; // Durasi flash
    public WeaponLogic weaponLogic; // Hubungkan secara manual di Inspector

    public float fireRate; // Waktu cooldown antar tembakan (dalam detik)
    private float lastFireTime = 0f; // Waktu terakhir tembakan dilakukan

    [Header("Player SFX")]
    public AudioClip ShootAudio;
    AudioSource PlayerAudio;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Hapus GetComponent<WeaponLogic>() karena weaponLogic dihubungkan secara manual
        if (weaponLogic == null)
        {
            Debug.LogError("WeaponLogic not found! Please assign WeaponLogic in the Inspector.");
        }

        // Simpan layer asli player dan layer AIM
        originalPlayerLayer = LayerMask.NameToLayer("Player");
        aimLayer = LayerMask.NameToLayer("PlayerAIM");

        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found! Please ensure that there is a Main Camera in the scene.");
            return;
        }

        // Set culling mask kamera agar masing-masing kamera hanya menampilkan layer tertentu
        mainCamera.cullingMask = ~(1 << aimLayer);
        PlayerAudio = this.GetComponent<AudioSource>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 viewDir = Player.position - new Vector3(transform.position.x, Player.position.y, transform.position.z);
        ViewPoint.forward = viewDir.normalized;

        if (TPSMode)
        {
            Vector3 InputDir = ViewPoint.forward * verticalInput + ViewPoint.right * horizontalInput;
            if (InputDir != Vector3.zero)
            {
                Player.forward = Vector3.Slerp(Player.forward, InputDir.normalized, Time.deltaTime * RotationSpeed);
            }
        }
        else if (AimMode)
        {
            Vector3 dirToCombatLookAt = AIMViewPoint.position - new Vector3(transform.position.x, AIMViewPoint.position.y, transform.position.z);
            AIMViewPoint.forward = dirToCombatLookAt.normalized;

            Player.forward = Vector3.Slerp(Player.forward, dirToCombatLookAt.normalized, Time.deltaTime * RotationSpeed);
        }

        // Aktifkan flash effect dan serangan saat menembak (klik kiri) di mode AIM
        if (AimMode && Input.GetKeyDown(KeyCode.Mouse0))
        {
            // Cek apakah cukup waktu yang telah berlalu sejak tembakan terakhir
            if (Time.time - lastFireTime >= fireRate)
            {
                lastFireTime = Time.time; // Update waktu tembakan terakhir
                StartCoroutine(FlashEffect()); // Menyalakan FlashLight sementara
                PlayerAudio.clip = ShootAudio;
                PlayerAudio.Play();

                // Pastikan weaponLogic tidak null sebelum memanggil Shoot()
                if (weaponLogic != null)
                {
                    weaponLogic.Shoot(); // Memanggil fungsi shoot dari WeaponLogic
                }
                else
                {
                    Debug.LogError("weaponLogic is not assigned in CameraLogic.");
                }
            }
            else
            {
                Debug.Log("Fire rate too fast! Please wait...");
            }
        }
    }

    public void CameraModeChanger(bool TPS, bool AIM)
    {
        TPSMode = TPS;
        AimMode = AIM;

        // Sinkronkan isInCameraMode di WeaponLogic dengan AimMode
        if (weaponLogic != null)
        {
            weaponLogic.SetCameraMode(AimMode);
        }
        else
        {
            Debug.LogError("weaponLogic is not assigned in CameraLogic.");
        }

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera is not assigned.");
            return;
        }

        if (TPS)
        {
            anim.SetBool("AimMode", false);

            mainCamera.cullingMask = ~(1 << aimLayer);

            SetLayerRecursively(Player.gameObject, originalPlayerLayer);

            // Mengaktifkan TPS mode
            TPSCamera.SetActive(true);
            AIMCamera.SetActive(false);
            FlashLight.SetActive(false);
            CameraCanvas.SetActive(false);
            FlashNormal.SetActive(false);
        }
        else if (AIM)
        {
            anim.SetBool("AimMode", true);

            // Mengaktifkan AIM mode tanpa delay
            SetLayerRecursively(Player.gameObject, aimLayer);
            CameraCanvas.SetActive(true);
            TPSCamera.SetActive(false);
            AIMCamera.SetActive(true);
            FlashLight.SetActive(false); // Pastikan flash dimulai dari kondisi mati
            FlashNormal.SetActive(true);
        }
    }

    private IEnumerator FlashEffect()
    {
        // Aktifkan FlashLight
        FlashLight.SetActive(true);

        // Tunggu sebentar sesuai durasi flash
        yield return new WaitForSeconds(flashDuration);

        // Matikan FlashLight
        FlashLight.SetActive(false);
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
