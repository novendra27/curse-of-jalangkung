using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    [Header("Player Setting")]
    private Rigidbody rb;
    public float walkspeed = 6f, runspeed = 12f, fallspeed, airMultiplier, HitPoints = 100f;
    public Transform PlayerOrientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    bool grounded = true; //
    public Animator anim;
    public CameraLogic camlogic;
    bool TPSMode = true, AimMode = false;
        private float channelingTimer;
    private const float maxChannelingDuration = 5f;
    private bool isChanneling;

    // Start is called before the first frame update

    [Header("Player SFX")]
    public AudioClip StepAudio;
    AudioSource PlayerAudio;
    public AudioClip DeathAudio;
    public AudioClip GetHitAudio;
    public AudioClip RunAudio;

    [Header("Summon Jalangkung")]

    [Header("UI")]
    public UIGameplayLogic UIGameplay;
    float MaxHealth;

    public GameObject jalangkungPrefab; // Prefab jalangkung yang akan di-summon
    public Transform summonPoint;       // Lokasi tempat jalangkung muncul relatif terhadap player
    public KeyCode summonKey = KeyCode.J; // Tombol untuk mensummon jalangkung
    void Start()
    {
         rb = this.GetComponent<Rigidbody>();
      //  PlayerOrientation = this.GetComponent<Transform>();
        PlayerAudio = this.GetComponent<AudioSource>();
        MaxHealth = HitPoints;
        UIGameplay.UpdateHealthBar(HitPoints, MaxHealth);

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0f) return; // Tambahkan pengecekan status pause

        Movement();
        if (!grounded)
        {
            rb.AddForce(Vector3.down * fallspeed * rb.mass, ForceMode.Force);
        }
        AimModeAdjuster();
        if (Input.GetKeyDown(KeyCode.L))
        {
            PlayerGetHit(100f);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerGetHit(10f);
        }

        if (Input.GetKeyDown(summonKey))
        {
            SummonJalangkung();
        }

        HandleChannelingAnimation();
    }
    private void HandleChannelingAnimation()
    {
        if (Time.timeScale == 0f) return; // Tambahkan pengecekan status pause

        if (Input.GetKeyDown(KeyCode.F))
        {
            anim.SetBool("Channeling", true);
            isChanneling = true;
            channelingTimer = 0f;
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            anim.SetBool("Channeling", false);
            isChanneling = false;
        }

        if (isChanneling)
        {
            channelingTimer += Time.deltaTime;
            if (channelingTimer >= maxChannelingDuration)
            {
                anim.SetBool("Channeling", false);
                isChanneling = false;
            }
        }
    }


    private void SummonJalangkung()
{
    // Pastikan prefab dan summonPoint tidak null
    if (jalangkungPrefab != null && summonPoint != null)
    {
        Instantiate(jalangkungPrefab, summonPoint.position, summonPoint.rotation);
        Debug.Log("Jalangkung summoned!");
    }
    else
    {
        Debug.LogWarning("Jalangkung prefab or summon point is not assigned!");
    }
}
private void step(){
    Debug.Log("step");
    PlayerAudio.clip = StepAudio;
    PlayerAudio.Play();
}
private void run(){
    Debug.Log("run");
    PlayerAudio.clip = RunAudio;
    PlayerAudio.Play();
}
private void Movement()
{
    horizontalInput = Input.GetAxisRaw("Horizontal");
    verticalInput = Input.GetAxisRaw("Vertical");
    moveDirection = PlayerOrientation.forward * verticalInput + PlayerOrientation.right * horizontalInput;
    
    if(grounded && moveDirection != Vector3.zero)
    {
        if (!AimMode && Input.GetKey(KeyCode.LeftShift))
        {
            anim.SetBool("Run",true);
            anim.SetBool("Walk",false);
            rb.AddForce(moveDirection.normalized * runspeed * 10f, ForceMode.Force);
        }
        else
        {
            anim.SetBool("Walk", true);
            anim.SetBool("Run",false);
            rb.AddForce(moveDirection.normalized * walkspeed * 10f, ForceMode.Force);
        }
    }
    else
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Run",false);
    }
}

    public void groundedchanger()
    {
        grounded = true;
    }

    private void AimModeAdjuster()
    {
        if (Time.timeScale == 0f) return; // Tambahkan pengecekan status pause

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (Input.GetKey(KeyCode.LeftShift) || (horizontalInput != 0 || verticalInput != 0))
            {
                return;
            }

            if (AimMode)
            {
                TPSMode = true;
                AimMode = false;
            }
            else if (TPSMode)
            {
                TPSMode = false;
                AimMode = true;
            }

            camlogic.CameraModeChanger(TPSMode, AimMode);
        }
    }


public void PlayerGetHit(float damage)
{
    Debug.Log("Player Receive Damage - " + damage);
    HitPoints = HitPoints - damage;
        UIGameplay.UpdateHealthBar(HitPoints, MaxHealth);

        // Mainkan audio get hit
        PlayerAudio.clip = GetHitAudio;
    PlayerAudio.Play();
    anim.SetTrigger("GetHit");

    if (HitPoints == 0f)
    {
        // Mainkan audio death
        PlayerAudio.clip = DeathAudio;
        PlayerAudio.Play();
        anim.SetBool("Death", true);
        rb.isKinematic = true;  // Menonaktifkan fisika pada Rigidbody

    }
}


}