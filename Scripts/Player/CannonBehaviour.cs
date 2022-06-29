using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBehaviour : MonoBehaviour
{
    //If cannon is touching something, vibrate
    private void OnCollisionStay(Collision collision)
    {
        if(PlayerHands.PH.CannonVelocity.magnitude > 5f)
        {
            ActionParseInputs.Input.VibrateForTime(true, 0.025f, 0.1f);
        }
        
    }
}
