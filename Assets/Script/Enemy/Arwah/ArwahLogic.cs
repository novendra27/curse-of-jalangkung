using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArwahLogic : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent agent;
    
    // Idle movement variables
    public float idleMoveRadius = 5f;
    public float idleMoveSpeed = 1f;
    private Vector3 idleDestination;
    private float idleMoveTimer = 0f;
    public float maxIdleMoveTime = 5f;
    public float turnSpeed = 2f;
    
    public float slowRadius = 5f; // Radius dalam meter di mana efek slow diberikan
    public float walkSlowFactor = 10f; // Faktor pengurangan kecepatan berjalan
    public float runSlowFactor = 20f; // Faktor pengurangan kecepatan berlari
    private PlayerLogic playerLogic; // Referensi ke PlayerLogic

    private CameraLogic cameraLogic; // Referensi ke CameraLogic
    private Renderer[] arwahRenderers; // Referensi ke semua Renderer Arwah

    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
        arwahRenderers = this.GetComponentsInChildren<Renderer>(); // Inisialisasi semua Renderer
        cameraLogic = FindObjectOfType<CameraLogic>(); // Temukan CameraLogic di scene
        playerLogic = FindObjectOfType<PlayerLogic>(); // Temukan PlayerLogic di scene
    if (playerLogic == null)
    {
        Debug.LogError("PlayerLogic not found in the scene!");
    }
        MoveToRandomIdlePosition();
    }

    // Update is called once per frame
    void Update()
    {
        // Atur visibilitas berdasarkan mode kamera
        if (cameraLogic != null)
        {
            foreach (var renderer in arwahRenderers)
            {
                renderer.enabled = cameraLogic.AimMode;
            }
        }

        // Call MoveToRandomIdlePosition if needed
        //MoveToRandomIdlePosition();
        // Update timer for idle movement
        idleMoveTimer += Time.deltaTime;

        // Check if arrived at current idle destination
        if (Vector3.Distance(transform.position, idleDestination) <= agent.stoppingDistance)
        {
            // Reset timer and get new destination
            idleMoveTimer = 0f;
            MoveToRandomIdlePosition();
        }

        // If taking too long to reach destination, pick a new one
        if (idleMoveTimer >= maxIdleMoveTime)
        {
            idleMoveTimer = 0f;
            MoveToRandomIdlePosition();
        }
        
        if (playerLogic != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerLogic.transform.position);

            if (distanceToPlayer <= slowRadius)
            {
                ApplySlowEffect(); // Terapkan efek slow ke player
            }
            else
            {
                RemoveSlowEffect(); // Hapus efek slow jika player keluar radius
            }
        }
    }

    void MoveToRandomIdlePosition()
    {
        // Generate random point within radius
        Vector2 randomPoint = Random.insideUnitCircle * idleMoveRadius;
        idleDestination = new Vector3(
            transform.position.x + randomPoint.x,
            transform.position.y,
            transform.position.z + randomPoint.y
        );

        // Set new destination and face it
        agent.SetDestination(idleDestination);
        FaceTarget(idleDestination);
    }

    private void FaceTarget(Vector3 destination)
    {
        Vector3 direction = (destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }

    void OnDrawGizmosSelected()
    {
        // Visualize the idle movement radius in the editor
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, idleMoveRadius);
    }

    void ApplySlowEffect()
    {
        if (playerLogic != null)
        {
            playerLogic.walkspeed = Mathf.Max(0, playerLogic.walkspeed - walkSlowFactor);
            playerLogic.runspeed = Mathf.Max(0, playerLogic.runspeed - runSlowFactor);
            Debug.Log("Player slowed by Arwah.");
        }
    }

    void RemoveSlowEffect()
    {
        if (playerLogic != null)
        {
            // Reset kecepatan player ke nilai default (sesuaikan dengan nilai awal di PlayerLogic)
            playerLogic.walkspeed = 20f; // Ganti dengan nilai awal walkspeed
            playerLogic.runspeed = 40f; // Ganti dengan nilai awal runspeed
            Debug.Log("Player speed restored.");
        }
    }

}
