using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHands : MonoBehaviour
{
    public static PlayerHands PH;
    public float HandMoveSpeed = 200f;

    private Transform CannonObject, HandObject, LeftObject, RightObject, HandModel, CannonUI, HandUI, CannonTarget, HandTarget;
    public Transform WeaponEnd, WeaponSelect;
    private Light Torch;
    private Rigidbody CannonRB, HandRB;
    public Vector3 CannonVelocity, HandVelocity;

    public Interactable InteractionTarget;
    public bool CarryingObject;

    private void OnEnable()
    {
        if(PH == null)
        {
            DontDestroyOnLoad(gameObject);
            PH = this;

            LeftObject = transform.Find("Left Controller");
            RightObject = transform.Find("Right Controller");
            CannonObject = transform.Find("Weapon Object");
            
            WeaponEnd = CannonObject.GetChild(1).transform;
            Torch = WeaponEnd.GetChild(0).GetComponent<Light>();
            HandObject = transform.Find("Hand Object");
            CannonUI = CannonObject.GetChild(0).GetChild(0);
            WeaponSelect = CannonUI.GetChild(0);
            HandModel = HandObject.GetChild(0); //Model transform needs to be offset differently based on handedness
            HandUI = HandModel.GetChild(0);

            CannonRB = CannonObject.GetComponent<Rigidbody>();
            HandRB = HandObject.GetComponent<Rigidbody>();

        } else
        {
            Destroy(gameObject);
        }
    }

    public void ToggleTorch()
    {
        Torch.enabled = !Torch.enabled;
    }

    public void TryPickUp()
    {
        if (InteractionTarget.CanPickUp)
        {
            InteractionTarget.SetFollow(HandObject);
            CarryingObject = true;
        } else
        {
            //If the object can't be picked up, activate its function right away
            InteractionTarget.Interact();
        }
    }

    public void DropItem()
    {
        if (InteractionTarget.CanPickUp)
        {
            InteractionTarget.GetComponent<Rigidbody>().AddForce(HandVelocity * 2);
            InteractionTarget.SetFollow(null);
            InteractionTarget = null;
            CarryingObject = false;
        }
    }

    private void Update()
    {
        if(CannonTarget != null)
        {
            CannonVelocity = HandMoveSpeed * (CannonTarget.position - CannonObject.position);
            CannonRB.AddForce(CannonVelocity);
            CannonObject.localRotation = Quaternion.Lerp(CannonObject.localRotation, CannonTarget.localRotation, HandMoveSpeed * Time.deltaTime);
        }

        if(HandTarget != null)
        {
            HandVelocity = HandMoveSpeed * (HandTarget.position - HandObject.position);
            HandRB.AddForce(HandVelocity);
            HandObject.localRotation = Quaternion.Lerp(HandObject.localRotation, HandTarget.localRotation, HandMoveSpeed * Time.deltaTime);
        }

        CannonRB.velocity = Vector3.zero;
        HandRB.velocity = Vector3.zero;

        
        WeaponSelect.GetChild(0).LookAt(Camera.main.transform);
    }

    public void SetRightHanded(bool RightHanded)
    {
        if (RightHanded)
        {
            CannonTarget = RightObject;
            HandTarget = LeftObject;

            HandModel.localPosition = new Vector3(0.02f, 0.04f, 0);
            HandModel.localScale = new Vector3(1, 1, 1);
            HandUI.localRotation = Quaternion.Euler(new Vector3(90, 0, 180));
        } else
        {
            CannonTarget = LeftObject;
            HandTarget = RightObject;

            HandModel.localPosition = new Vector3(-0.02f, 0.04f, 0);
            HandModel.localScale = new Vector3(1, -1, 1);
            HandUI.localRotation = Quaternion.Euler(new Vector3(-90, 0, 180));
        }
    }
}
