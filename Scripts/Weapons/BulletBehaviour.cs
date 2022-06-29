using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    Rigidbody RB;

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        RB.AddForce(transform.forward * 200f * (2+transform.localScale.x/2));
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer != 0)
        {
            //Projectiles can only interact with the enemy team or the ground - damage done here!
        }

        //In either case, destroy the bullet
        Destroy(gameObject);
    }
}
