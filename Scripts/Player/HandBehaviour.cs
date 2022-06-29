using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandBehaviour : MonoBehaviour
{
    Interactable target;
    float GrabRange = 5f;

    private void Update()
    {
        //Only update hand target if not carrying something
        if (!PlayerHands.PH.CarryingObject)
        {
            Physics.Raycast(transform.position, transform.forward * GrabRange, out RaycastHit hit);
            if (hit.collider != null && hit.collider.gameObject != null)
            {
                target = hit.collider.GetComponent<Interactable>();

                if(target != null)
                {
                    if(Vector3.Distance(transform.position, target.transform.position) <= target.MaxGrabDistance && PlayerHands.PH.InteractionTarget != target)
                    {
                        ActionParseInputs.Input.VibrateForTime(false, 0.1f, 0.1f);
                        PlayerHands.PH.InteractionTarget = target;
                    } else if(Vector3.Distance(transform.position, target.transform.position) > target.MaxGrabDistance)
                    {
                        PlayerHands.PH.InteractionTarget = null;
                    }
                } else
                {
                    PlayerHands.PH.InteractionTarget = null;
                }
            }
            else
            {
                PlayerHands.PH.InteractionTarget = null;
            }
        }        
    }

    private void OnCollisionStay(Collision collision)
    {
        if (PlayerHands.PH.HandVelocity.magnitude > 5f)
        {
            ActionParseInputs.Input.VibrateForTime(false, 0.025f, 0.1f);
        }
    }
}
