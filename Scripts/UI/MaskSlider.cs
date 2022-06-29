using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaskSlider : MonoBehaviour
{
    //An alternative to the Slider uses an Image's fill value as the bar, which can then mask its child objects.
    //Mainly used on labelled sliders like the Health bar or the hand scanners.

    public float CurrentValue, MaxValue;
    [SerializeField] Image SliderImage; //Fill amount 0-1 = 0-100% filled

    void UpdateSlider()
    {
        SliderImage.fillAmount = CurrentValue / MaxValue;
    }

    public void SetCurrentValue(float val)
    {
        if(val > MaxValue)
        {
            val = MaxValue;
        } else if(val < 0)
        {
            val = 0;
        }

        CurrentValue = val;
        UpdateSlider();
    }

    public void SetMaxValue(float val, bool keepRatio)
    {
        //KeepRatio true = keep same ratio of current value to max value
        //If false, current value remains as it was rather than being multiplied by the same amount

        MaxValue = val;

        if (keepRatio)
        {
            //current/max = ratio, so current = ratio*max
            CurrentValue = SliderImage.fillAmount * val;
        }

        UpdateSlider();
    }
}
