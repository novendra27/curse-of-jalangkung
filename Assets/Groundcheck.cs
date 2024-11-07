using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    PlayerLogic logicmovement;  

    private void Start(){

        logicmovement = this.GetComponentInParent<PlayerLogic>();
    }

    private void OnTriggerEnter(Collider other)
    {
        logicmovement.groundedchanger();
        Debug.Log("Touch The Ground");
    }

}
