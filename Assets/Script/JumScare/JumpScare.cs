using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpScare : MonoBehaviour
{
    public GameObject PanelJumpScare;

    void Start()
    {
        PanelJumpScare.SetActive(false);
    }

    public void TriggerJumpScare()
{
    if (PanelJumpScare != null)
    {
        PanelJumpScare.SetActive(true);
        StartCoroutine(DisableJumpScares());
    }
}


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            PanelJumpScare.SetActive(true);

            StartCoroutine(DisableJumpScares());
        }
    }
    
    IEnumerator DisableJumpScares()
    {
        yield return new WaitForSeconds(2);
        PanelJumpScare.SetActive(false);
    }

}
