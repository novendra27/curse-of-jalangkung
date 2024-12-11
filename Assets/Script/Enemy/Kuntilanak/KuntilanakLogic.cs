using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KuntilanakLogic : MonoBehaviour
{
    [Header("Enemy Setting")]
    public float turnSpeed = 15f;
    public Transform target;
    public float ChaseRange;
    private NavMeshAgent agent;
    private float DistancetoTarget;
    private float DistancetoDefault;
    private Animator anim;
    public UIGameplayLogic GamePlayLogic;
    public UIGameplayLogic PanelJumpScare;
    Vector3 DefaultPosition;

    void Start()
{
    target = FindAnyObjectByType<PlayerLogic>().transform;
    agent = this.GetComponent<NavMeshAgent>();
    anim = this.GetComponentInChildren<Animator>();
    DefaultPosition = this.transform.position;

    if (GamePlayLogic == null)
    {
        GamePlayLogic = FindObjectOfType<UIGameplayLogic>();
        if (GamePlayLogic == null)
        {
            Debug.LogError("UIGameplayLogic not found in the scene!");
        }
    }

    // if (GamePlayLogic != null && GamePlayLogic.PanelJumpScare == null)
    // {
    //     Debug.LogError("PanelJumpScare is not assigned in UIGameplayLogic! Please assign it in the Inspector.");
    // }
}


    void Update()
{
    DistancetoTarget = Vector3.Distance(target.position, transform.position);
    DistancetoDefault = Vector3.Distance(DefaultPosition, transform.position);

    if (DistancetoTarget <= ChaseRange)
    {
        if (DistancetoTarget <= ChaseRange / 2) // Chase when distance is less than half of chase range
        {
            if (DistancetoTarget > agent.stoppingDistance + 2f)
            {
                ChaseTarget();
            }
            else if (DistancetoTarget <= agent.stoppingDistance)
            {
                Attack();
                
                if (GamePlayLogic != null && GamePlayLogic.PanelJumpScare != null)
                {
                    GamePlayLogic.PanelJumpScare.SetActive(true);
                }
                else
                {
                    GamePlayLogic.PanelJumpScare.SetActive(false);
                    Debug.LogError("PanelJumpScare is not assigned or GamePlayLogic is missing!");
                }
            }
        }
        else // Just face the target when in range but further than half chase range
        {
            FaceTarget(target.position);
            anim.SetBool("Run", false);
            anim.SetBool("Attack", false);
        }
    }
    else // Return to default position when out of range
    {
        agent.SetDestination(DefaultPosition);
        FaceTarget(DefaultPosition);
        if (DistancetoDefault <= agent.stoppingDistance)
        {
            anim.SetBool("Run", false);
            anim.SetBool("Attack", false);
        }
    }
}


    public void JumpScareConnect()
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

    // Tampilkan JumpScare dengan durasi 3 detik, lalu hancurkan Kuntilanak
    if (GamePlayLogic != null && GamePlayLogic.PanelJumpScare != null)
    {
        StartCoroutine(ShowJumpScareAndDestroy());
    }
    else
    {
        Debug.LogError("PanelJumpScare is not assigned or GamePlayLogic is missing!");
    }
}


    private IEnumerator ShowJumpScareAndDestroy()
{
    // Tampilkan PanelJumpScare
    GamePlayLogic.PanelJumpScare.SetActive(true);

    // Tunggu selama 3 detik
    yield return new WaitForSeconds(3f);

    // Sembunyikan PanelJumpScare
    GamePlayLogic.PanelJumpScare.SetActive(false);

    // Hancurkan enemy Kuntilanak
    Destroy(gameObject);
}


    public void ChaseTarget()
    {
        agent.SetDestination(target.position);
        anim.SetBool("Run", true);
        anim.SetBool("Attack", false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, ChaseRange);
        // Drawing inner circle to show chase threshold
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, ChaseRange/2);
    }

//     void OnCollisionEnter(Collision collision)
// {
//     if (collision.gameObject.CompareTag("Player"))
//     {
//         if (GamePlayLogic != null && GamePlayLogic.PanelJumpScare != null)
//         {
//             GamePlayLogic.PanelJumpScare.SetActive(true);
//         }
//         else
//         {
//             Debug.LogError("PanelJumpScare is not assigned in UIGameplayLogic!");
//         }
//     }
// }

// void OnCollisionExit(Collision collision)
// {
//     if (collision.gameObject.CompareTag("Player"))
//     {
//         if (GamePlayLogic != null && GamePlayLogic.PanelJumpScare != null)
//         {
//             GamePlayLogic.PanelJumpScare.SetActive(false);
//         }
//     }
// }

}