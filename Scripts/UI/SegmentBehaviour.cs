using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SegmentBehaviour : MonoBehaviour
{
    public WeaponSystem.WeaponType Represents; //Which weapon is this for?
    private Animator ani;
    private Image MainImage, IconImage;

    private void OnEnable()
    {
        ani = GetComponent<Animator>();
        MainImage = GetComponent<Image>();
        IconImage = transform.GetChild(0).GetChild(0).GetComponent<Image>();
    }

    public void SetSegmentWidth(float width)
    {
        //Segment sprite width is normalised, so width/360 for value
        //Segment icon should be -width/2

        MainImage.fillAmount = width / 360;
        transform.GetChild(0).localEulerAngles = new Vector3(0, 0, -(width / 2));
    }

    public void SetRepresenting(WeaponSystem.WeaponType type)
    {
        Represents = type;
        
        //switch for which to use?
    }

    public void ToggleHighlight()
    {
        ani.SetBool("Selected", !ani.GetBool("Selected"));
    }
}
