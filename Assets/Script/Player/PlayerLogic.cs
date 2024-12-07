using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    [Header("Player Setting")]
    private Rigidbody rb;
    public float walkspeed, runspeed, fallspeed, airMultiplier, HitPoints = 100f;
    public Transform PlayerOrientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    bool grounded = true; //
    public Animator anim;
    public CameraLogic camlogic;
    bool TPSMode = true, AimMode = false;
    
    // Start is called before the first frame update

    [Header("Player SFX")]
    public AudioClip StepAudio;
    AudioSource PlayerAudio;
    public AudioClip DeathAudio;
    public AudioClip GetHitAudio;
    public AudioClip RunAudio;

    [Header("UI")]
    public UIGameplayLogic UIGameplay;
    float MaxHealth;


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
        
        Movement();
            if (!grounded)
    {
        // Tambahkan gaya jatuh secara manual
        rb.AddForce(Vector3.down * fallspeed * rb.mass, ForceMode.Force);  // Tambahkan gaya jatuh

        // Anda bisa mengubah nilai fallspeed untuk mempercepat atau memperlambat kecepatan jatuh karakter
    }//
        AimModeAdjuster();
        if(Input.GetKeyDown(KeyCode.F))
        {
            PlayerGetHit(100f);
        } 
        if (Input.GetKeyDown(KeyCode.P)){
            PlayerGetHit(10f);
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

public void AimModeAdjuster()
{
    // Cek jika tombol Mouse1 (klik kanan) ditekan
    if (Input.GetKeyDown(KeyCode.Mouse1))
    {
        Debug.Log("mouse1");

        // Cek apakah pemain sedang berlari atau berjalan
        if (Input.GetKey(KeyCode.LeftShift) || (horizontalInput != 0 || verticalInput != 0))
        {
            // Jangan izinkan transisi ke AIM mode saat berlari atau berjalan
            Debug.Log("Cannot switch to AIM mode while running or walking.");
            return;
        }

        // Jika tidak berlari atau berjalan, izinkan perubahan mode
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

        // Panggil CameraModeChanger dengan status mode baru
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