using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GenderuwoLogic2 : MonoBehaviour
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
    private float idleMoveTimer = 0f;
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
        MoveToRandomIdlePosition();
    }

    void Update()
    {
        if (isDead && !isSinking)
        {
            CheckDeathAnimation();
            return;
        }

        if (isSinking)
        {
            SinkIntoGround();
            return;
        }

        DistancetoTarget = Vector3.Distance(target.position, transform.position);

        if (DistancetoTarget <= ChaseRange && hitPoints > 0)
        {
            agent.speed = normalMoveSpeed;
            FaceTarget(target.position);
            if (DistancetoTarget > agent.stoppingDistance + 2f)
            {
                ChaseTarget();
            }
            else if (DistancetoTarget <= agent.stoppingDistance)
            {
                Attack();
            }
        }
        else
        {
            agent.speed = idleMoveSpeed;

            if (Vector3.Distance(transform.position, idleDestination) <= agent.stoppingDistance)
            {
                anim.SetBool("Run", false);

                if (!isLookingAround && !isWaitingToLookAround)
                {
                    isWaitingToLookAround = true;
                    waitTimer = 0f;
                }
            }
            else
            {
                anim.SetBool("Run", true);
            }

            if (isWaitingToLookAround)
            {
                waitTimer += Time.deltaTime;
                if (waitTimer >= waitBeforeLookAround)
                {
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
                LookAround();
            }

            if (!isLookingAround && !isWaitingToLookAround && Vector3.Distance(transform.position, idleDestination) <= agent.stoppingDistance)
            {
                MoveToRandomIdlePosition();
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
        Gizmos.DrawWireSphere(transform.position, idleMoveRadius);
    }
}