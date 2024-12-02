using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLogic : MonoBehaviour
{
    [SerializeField] Camera ShootCamera;
    [SerializeField] float range = 1000f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(ShootCamera.transform.position, ShootCamera.transform.forward, out hit, range))
        {
            Debug.Log("I hit this thing: " + hit.transform.name);
            if (hit.transform.tag.Equals("Genderuwo"))
            {
                GenderuwoLogic target = hit.transform.GetComponent<GenderuwoLogic>();
                target.TakeDamage(50);
            }

            if (hit.transform.tag.Equals("Kuntilanak"))
            {
                KuntilanakLogic target = hit.transform.GetComponent<KuntilanakLogic>();
                target.TakeDamage(50);
            }
        }
        else
        {
            return;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 direction = ShootCamera.transform.TransformDirection(Vector3.forward) * range;
        Gizmos.DrawRay(ShootCamera.transform.position, direction);
    }
}
