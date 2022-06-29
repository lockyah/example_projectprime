using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public bool CanPickUp;
    [SerializeField] UnityEvent[] Events;
    public Transform FollowTarget;
    public float MaxGrabDistance = 5f; //From how far away can this object be grabbed?
    public float MoveSpeed = 600f;
    private Collider Coll;
    private Rigidbody RB;

    private void Start()
    {
        Coll = GetComponent<Collider>();
        RB = GetComponent<Rigidbody>();
    }

    public void SetFollow(Transform follow)
    {
        if (follow != null)
        {
            gameObject.layer = 7; //PlayerHidden layer, stops collision with player objects
        }
        else
        {
            StartCoroutine(GrabCooldown());
        }

        FollowTarget = follow;
        RB.angularDrag = follow == null ? 0 : 0.8f * RB.mass;
        RB.constraints = follow == null ? RigidbodyConstraints.None : RigidbodyConstraints.FreezeRotation;
        RB.useGravity = follow == null;
        MoveSpeed = 0.5f;
    }

    private void Update()
    {
        if(FollowTarget != null)
        {
            float dist = Vector3.Distance(transform.position, FollowTarget.position);
            if(dist < 1f)
            {
                MoveSpeed = 0.5f;
            }

            RB.MovePosition(Vector3.Lerp(transform.position, FollowTarget.position, MoveSpeed));
            transform.rotation = Quaternion.Lerp(transform.rotation, FollowTarget.rotation, 100f * Time.deltaTime);
            RB.velocity = Vector3.zero;
        }
    }

    public void TogglePhysics()
    {
        Coll.enabled = !Coll.enabled;

        if (RB.constraints == RigidbodyConstraints.None)
        {
            RB.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
            RB.velocity = Vector3.zero;
        }
        else
        {
            RB.constraints = RigidbodyConstraints.None;
        }
    }

    //Wait until there's some distance between the hand and the object to swap back to normal layer
    IEnumerator GrabCooldown()
    {
        Transform HandRef = FollowTarget;

        while(Vector3.Distance(gameObject.transform.position, HandRef.transform.position) < 0.25f)
        {
            yield return null;
        }

        gameObject.layer = 10; //Return to Interact layer
    }

    public void Interact()
    {
        //Called from the hand when grabbed if !CanPickUp, or by using trigger while holding this object.
        foreach(UnityEvent e in Events)
        {
            e.Invoke();
        }
    }

}
