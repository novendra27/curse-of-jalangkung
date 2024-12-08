using System.Collections;
using UnityEngine;

public class WeaponLogic : MonoBehaviour
{
    [SerializeField] private Transform AIMViewPoint;    // Titik pandang kamera (AIMViewPoint)
    [SerializeField] private float range = 10f;       // Jarak serangan
    [SerializeField] private int photoDamage = 50;      // Damage yang diberikan saat memotret

    private bool isInCameraMode = false;

    // Method untuk mengatur mode kamera dari CameraLogic
    public void SetCameraMode(bool isInCameraMode)
    {
        this.isInCameraMode = isInCameraMode;
    }

    // Method untuk melakukan serangan foto saat klik kiri ditekan dalam mode AIM
    public void Shoot()
    {
        if (isInCameraMode)
        {
            StartCoroutine(TakePhoto());
        }
    }

    private IEnumerator TakePhoto()
    {
        // Melakukan Raycast untuk mendeteksi musuh
        RaycastHit hit;
        if (Physics.Raycast(AIMViewPoint.position, AIMViewPoint.forward, out hit, range))
        {
            Debug.Log("I hit this thing: " + hit.transform.name);
              if (hit.transform.CompareTag("Genderuwo"))
              {
                  // Mengambil komponen EnemyLogic pada musuh dan memberikan damage
                  GenderuwoLogic target = hit.transform.GetComponent<GenderuwoLogic>();
                  if (target != null)
                  {
                      target.TakeDamage(photoDamage);
                  }
              }
        }

        yield return null;
    }

    void OnDrawGizmos()
    {
        // Menampilkan garis ray dalam mode Gizmo untuk debugging
        Gizmos.color = Color.red;
        Vector3 direction = AIMViewPoint.TransformDirection(Vector3.forward) * range;
        Gizmos.DrawRay(AIMViewPoint.position, direction);
    }
}
