using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDoor : MonoBehaviour
{
    Animator doorAni;

    private void Start()
    {
        doorAni = GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        doorAni.SetBool("character_nearby", !doorAni.GetBool("character_nearby"));
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 3 && other.gameObject.name == "Player")
        {
            doorAni.SetBool("character_nearby",true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 3 && other.gameObject.name == "Player")
        {
            doorAni.SetBool("character_nearby", false);
        }
    }*/
}
