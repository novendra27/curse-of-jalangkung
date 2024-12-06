using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GenderuwoLogic : MonoBehaviour
{
    public float hitPoints = 100f;
    public float turnSpeed = 2f;
    public Transform target;
    public float ChaseRange;
    private NavMeshAgent agent;
    private float DistancetoTarget;
    private Animator anim;

    // Variabel untuk idle movement
    public float idleMoveRadius = 5f;
    private Vector3 idleDestination;
    public float idleMoveSpeed = 1f;
    public float normalMoveSpeed = 3.5f;

    // Variabel untuk melihat kanan dan kiri
    private bool isLookingAround = false;
    private bool isWaitingToLookAround = false;
    private float lookAroundTimer = 0f;
    public float lookAroundDuration = 3f;
    public float lookAroundSpeed = 0.5f;
    public float maxLookAngle = 45f;
    private Quaternion originalRotation;

    // Variabel untuk jeda sebelum look around
    public float waitBeforeLookAround = 2f;
    private float waitTimer = 0f;

    // Variabel untuk kontrol fase putaran ke kiri dan kanan
    private bool isTurningLeft = true;
    private float turnPhaseDuration = 1f;
    private float turnPhaseTimer = 0f;
    private Quaternion targetRotation;

    // Variabel untuk efek tenggelam
    private bool isSinking = false;
    private bool isDead = false; // Menandakan apakah musuh sudah mati
    public float sinkSpeed = 1f; // Kecepatan tenggelam
    public float sinkDepth = -1f; // Kedalaman tenggelam sebelum dihapus

    // Tambahan variabel untuk mengontrol transisi dari chase ke idle
    private bool wasChasing = false;
    public float idleTransitionDelay = 1f; // Waktu jeda sebelum memulai idle movement setelah chase
    private float idleTransitionTimer = 0f;


    public float maxIdleMoveTime = 5f; // Waktu maksimal yang diizinkan untuk mencapai tujuan idle
    private float idleMoveTimer = 0f; // Timer untuk melacak waktu menuju titik idle

    [Header("Genderuwo Sound")]
    public AudioClip GenderuwoAttack;
    public AudioClip GenderuwoDeath;
    public AudioClip GenderuwoStep;
    public AudioClip GenderuwoHurt;
    public AudioClip GenderuwoIdleYawn;
    AudioSource GenderuwoAudio;


    public void TakeDamage(float damage)
    {
        hitPoints -= damage;
        anim.SetTrigger("GetHit");
        anim.SetFloat("HitPoints", hitPoints);
        if (hitPoints <= 0 && !isDead)
        {
            Die();
        }
    }

    void Start()
    {
        target = FindAnyObjectByType<PlayerLogic>().transform;
        agent = this.GetComponent<NavMeshAgent>();
        anim = this.GetComponentInChildren<Animator>();
        anim.SetFloat("HitPoints", hitPoints);

        // Tentukan tujuan idle pertama kali
        MoveToRandomIdlePosition(); // Menambahkan pemanggilan ini di Start untuk memastikan idle langsung terjadi

        GenderuwoAudio = this.GetComponent<AudioSource>();
        GenderuwoAudio.spatialBlend = 1.0f; // Set to 3D sound
        GenderuwoAudio.maxDistance = ChaseRange; // Set max distance to ChaseRange

    }


    void Update()
    {
        if (isDead && !isSinking)
        {
            Debug.Log("Enemy is dead and not sinking. Checking death animation.");
            CheckDeathAnimation();
            return;
        }

        if (isSinking)
        {
            Debug.Log("Enemy is sinking.");
            SinkIntoGround();
            return;
        }

        DistancetoTarget = Vector3.Distance(target.position, transform.position);
        //Debug.Log("Distance to target: " + DistancetoTarget);

        if (DistancetoTarget <= ChaseRange && hitPoints > 0)
        {
            Debug.Log("Target in chase range. DistancetoTarget: " + DistancetoTarget);
            // Reset idle transition jika sedang mengejar target
            wasChasing = true;
            idleTransitionTimer = 0f;

            agent.speed = normalMoveSpeed;
            FaceTarget(target.position);

            if (DistancetoTarget > agent.stoppingDistance)
            {
                Debug.Log("Chasing target. Distance is greater than stopping distance.");
                ChaseTarget();
            }
            else if (DistancetoTarget <= agent.stoppingDistance)
            {
                Debug.Log("Target within attack range. Attacking.");
                Attack();
            }
        }
        else
        {
            // Reset jika target sudah tidak dalam jangkauan pengejaran
            if (wasChasing)
            {
                Debug.Log("Target lost. Transitioning to idle mode.");
                wasChasing = false;
                idleTransitionTimer = 0f;  // Reset idle timer

                // Setelah kehilangan target, langsung ke mode idle
                agent.speed = idleMoveSpeed;
                MoveToRandomIdlePosition();  // Mengatur posisi idle secara acak
                anim.SetBool("Run", false);  // Stop running animation
            }

            // Memperbarui timer untuk waktu menuju idle
            idleMoveTimer += Time.deltaTime;

            // Jika tidak sampai ke titik idle dalam waktu tertentu, pilih titik idle baru
            if (Vector3.Distance(transform.position, idleDestination) <= agent.stoppingDistance)
            {
                Debug.Log("Arrived at idle destination.");
                anim.SetBool("Run", false);

                if (!isLookingAround && !isWaitingToLookAround)
                {
                    Debug.Log("Waiting to look around.");
                    isWaitingToLookAround = true;
                    waitTimer = 0f;
                }

                // Reset timer jika mencapai titik idle
                idleMoveTimer = 0f;
            }
            else
            {
                Debug.Log("Moving towards idle destination.");
                anim.SetBool("Run", true);
            }

            // Cek apakah waktu yang dihabiskan untuk mencapai idle terlalu lama
            if (idleMoveTimer >= maxIdleMoveTime)
            {
                Debug.Log("Not reaching idle destination in time. Selecting new idle position.");
                MoveToRandomIdlePosition();  // Pilih titik idle baru
                idleMoveTimer = 0f;  // Reset timer setelah memilih titik baru
            }

            if (isWaitingToLookAround)
            {
                waitTimer += Time.deltaTime;
                //Debug.Log("Waiting to look around. Wait timer: " + waitTimer);
                if (waitTimer >= waitBeforeLookAround)
                {
                    Debug.Log("Ready to start looking around.");
                    isWaitingToLookAround = false;
                    isLookingAround = true;
                    lookAroundTimer = 0f;
                    turnPhaseTimer = 0f;
                    originalRotation = transform.rotation;
                    SetTargetRotation();
                }
            }

            if (isLookingAround)
            {
                Debug.Log("Looking around.");
                LookAround();

                // Setelah selesai looking around, kembali ke mode idle
                if (lookAroundTimer >= lookAroundDuration)
                {
                    isLookingAround = false;
                    MoveToRandomIdlePosition();  // Pindah ke posisi idle acak setelah selesai looking around
                }
            }
        }
    }


    void MoveToRandomIdlePosition()
    {
        Vector2 randomPoint = Random.insideUnitCircle * idleMoveRadius;
        idleDestination = new Vector3(transform.position.x + randomPoint.x, transform.position.y, transform.position.z + randomPoint.y);

        agent.SetDestination(idleDestination);
        FaceTarget(idleDestination);
    }

    private void SetTargetRotation()
    {
        float angle = isTurningLeft ? -maxLookAngle : maxLookAngle;
        targetRotation = originalRotation * Quaternion.Euler(0, angle, 0);
    }

    private void LookAround()
    {
        lookAroundTimer += Time.deltaTime;
        turnPhaseTimer += Time.deltaTime;

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookAroundSpeed * Time.deltaTime);

        if (turnPhaseTimer >= turnPhaseDuration)
        {
            isTurningLeft = !isTurningLeft;
            turnPhaseTimer = 0f;
            SetTargetRotation();
        }

        if (lookAroundTimer >= lookAroundDuration)
        {
            isLookingAround = false;
        }
    }

    private void Die()
    {
        isDead = true;
        anim.SetBool("isDead", true); // Pastikan animasi death dipicu dengan parameter "isDead"
        agent.enabled = false; // Hentikan pergerakan
    }

    private void CheckDeathAnimation()
    {
        // Periksa apakah animasi "death" sudah selesai
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Death") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            StartSinking();
        }
    }

    private void StartSinking()
    {
        isSinking = true;
        anim.enabled = false; // Nonaktifkan animasi untuk memulai efek tenggelam
    }

    private void SinkIntoGround()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, sinkDepth, transform.position.z), Time.deltaTime * sinkSpeed);

        if (transform.position.y <= sinkDepth + 0.1f)
        {
            Destroy(gameObject);
        }
    }

    public void HitConnect()
    {
        if (DistancetoTarget <= agent.stoppingDistance)
        {
            target.GetComponent<PlayerLogic>().PlayerGetHit(50f);
        }
    }

    private void FaceTarget(Vector3 destination)
    {
        Vector3 direction = (destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }

    public void Attack()
    {
        anim.SetBool("Run", false);
        anim.SetBool("Attack", true);
    }

    public void ChaseTarget()
    {
        agent.SetDestination(target.position);
        anim.SetBool("Run", true);
        anim.SetBool("Attack", false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, ChaseRange);
    }

    public void PlayGenderuwoAttack()
    {
        GenderuwoAudio.clip = GenderuwoAttack;
        GenderuwoAudio.Play();
    }

    public void PlayGenderuwoDeath()
    {
        GenderuwoAudio.clip = GenderuwoDeath;
        GenderuwoAudio.Play();
    }

    public void PlayGenderuwoStep()
    {
        if (Vector3.Distance(transform.position, target.position) <= ChaseRange)
        {
            GenderuwoAudio.clip = GenderuwoStep;
            GenderuwoAudio.Play();
        }
    }

    public void PlayGenderuwoHurt()
    {
        GenderuwoAudio.clip = GenderuwoHurt;
        GenderuwoAudio.Play();
    }

    public void PlayGenderuwoIdleYawn()
    {
        if (Vector3.Distance(transform.position, target.position) <= ChaseRange)
        {
            GenderuwoAudio.clip = GenderuwoIdleYawn;
            GenderuwoAudio.Play();
        }
    }
}