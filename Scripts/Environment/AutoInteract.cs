using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AutoInteract : MonoBehaviour
{
    [SerializeField] UnityEvent[] events;
    [SerializeField] bool SingleUse;
    [SerializeField] float ActivateTime;
    [SerializeField] MaskSlider Meter;
    bool PlayerNearby = false;
    bool Activated = false;
    public float HoldTime = 0f;

    private void Update()
    {
        if (ActivateTime != 0 && HoldTime < ActivateTime)
        {
            if (PlayerNearby)
            {
                HoldTime += Time.deltaTime * Time.timeScale;
            } else
            {
                if(HoldTime > 0)
                {
                    HoldTime -= Time.deltaTime * Time.timeScale * 2f;
                }
            }

        } else if(ActivateTime != 0  && HoldTime >= ActivateTime && !Activated)
        {
            Activate();
        }

        if(Meter != null)
        {
            Meter.SetCurrentValue(HoldTime); //Assumes meter is already set up to be the same time to fill
        }
    }

    void Activate()
    {
        Activated = true;
        foreach (UnityEvent e in events)
        {
            e.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ActivateTime == 0)
        {
            Activate();
        } else if (other.gameObject.name == "Hand Object")
        {
            PlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Hand Object")
        {
            PlayerNearby = false;

            if (!SingleUse)
            {
                Activated = false;

                if(HoldTime >= ActivateTime)
                {
                    HoldTime = 0f;
                }
            }
        }
    }
}
