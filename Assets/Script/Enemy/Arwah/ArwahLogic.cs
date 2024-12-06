using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArwahLogic : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent agent;
    public float idleMoveRadius = 5f;
    private Vector3 idleDestination;
    public float idleMoveSpeed = 1f;

    private CameraLogic cameraLogic; // Referensi ke CameraLogic
    private Renderer[] arwahRenderers; // Referensi ke semua Renderer Arwah

    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
        arwahRenderers = this.GetComponentsInChildren<Renderer>(); // Inisialisasi semua Renderer
        cameraLogic = FindObjectOfType<CameraLogic>(); // Temukan CameraLogic di scene

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
    }

    void MoveToRandomIdlePosition()
    {
        Vector2 randomPoint = Random.insideUnitCircle * idleMoveRadius;
        idleDestination = new Vector3(transform.position.x + randomPoint.x, transform.position.y, transform.position.z + randomPoint.y);

        agent.SetDestination(idleDestination);
        FaceTarget(idleDestination);
    }

    private void FaceTarget(Vector3 destination)
    {
        Vector3 direction = (destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed);
    }
}
