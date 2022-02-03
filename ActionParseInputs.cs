using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class ActionParseInputs : MonoBehaviour
{
    public static ActionParseInputs Input;

    public bool RightHanded = true;
    [SerializeField] InputActionAsset ActionAsset;
    private InputActionMap LeftMap, RightMap;

    public ControllerMap HandCon, GunCon;
    private UnityEngine.XR.InputDevice LeftCon, RightCon;

    private Coroutine LeftVibrate, RightVibrate;

    private void OnEnable()
    {
        if(Input == null)
        {
            DontDestroyOnLoad(gameObject);
            Input = this;

            if (ActionAsset != null)
            {
                //Enable all maps in our input mapper
                ActionAsset.Enable();

                //Track inputs from left and right controllers
                LeftMap = ActionAsset.FindActionMap("LeftHand");
                RightMap = ActionAsset.FindActionMap("RightHand");

                ResetHandState();
            }
        } else
        {
            Destroy(gameObject);
        }        
    }

    private void Start()
    {
        StartCoroutine(FindDevices());
    }

    //Capture each device being used for vibration
    private IEnumerator FindDevices()
    {
        List<UnityEngine.XR.InputDevice> devices = new List<UnityEngine.XR.InputDevice>();

        InputDeviceCharacteristics ConChars = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left;
        while (devices.Count == 0)
        {
            InputDevices.GetDevicesWithCharacteristics(ConChars, devices);

            if (devices.Count > 0)
            {
                LeftCon = devices[0];
                break;
            }

            yield return null;
        }

        devices.Clear();
        ConChars = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right;
        while (devices.Count == 0)
        {
            InputDevices.GetDevicesWithCharacteristics(ConChars, devices);
            if (devices.Count > 0)
            {
                RightCon = devices[0];
                break;
            }

            yield return null;
        }

        PlayerHands.PH.SetRightHanded(true);
    }

    public void VibrateForTime(bool CannonHand, float intensity, float duration)
    {
        UnityEngine.XR.InputDevice device;

        //Determine which controller to use based on if the request is for the cannon or not
        if (CannonHand)
        {
            device = RightHanded ? RightCon : LeftCon;
        }
        else
        {
            device = RightHanded ? LeftCon : RightCon;
        }

        if (device == RightCon)
        {
            if(RightVibrate != null)
            {
                StopCoroutine(RightVibrate);
            }            
            RightVibrate = StartCoroutine(Vibrate(device, intensity, duration));
        } else if(device == LeftCon)
        {
            if (LeftVibrate != null)
            {
                StopCoroutine(LeftVibrate);
            }
            LeftVibrate = StartCoroutine(Vibrate(device, intensity, duration));
        }
    }

    IEnumerator Vibrate(UnityEngine.XR.InputDevice device, float intensity, float duration)
    {
        
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;

            device.SendHapticImpulse(0, intensity);
            yield return null;
        }

        //Stop impulse afterwards
        device.StopHaptics();

        if(device == RightCon)
        {
            RightVibrate = null;
        } else
        {
            LeftVibrate = null;
        }
    }

    public void ToggleRightHanded()
    {
        RightHanded = !RightHanded;
        PlayerHands.PH.SetRightHanded(RightHanded);
        ResetHandState();
    }

    //Used when swapping hands
    void ResetHandState()
    {
        HandCon = new ControllerMap();
        GunCon = new ControllerMap();
    }

    public void Stick(InputAction.CallbackContext context)
    {
        Vector2 val = context.ReadValue<Vector2>();

        if ((context.action.actionMap == LeftMap && RightHanded) || (context.action.actionMap == RightMap && !RightHanded))
        {
            //Hand controller
            HandCon.StickVector = val;
        }
        else
        {
            //Cannon controller
            GunCon.StickVector = val;
        }
    }


    public ControllerMap.ButtonState ParseAxis(ControllerMap.ButtonState currentState, float value)
    {
        if(value >= 0.5f)
        {
            //Down
            if(currentState == ControllerMap.ButtonState.Up || currentState == ControllerMap.ButtonState.Released)
            {
                return ControllerMap.ButtonState.Pressed;
            } else
            {
                return ControllerMap.ButtonState.Down;
            }
        } else
        {
            if(currentState == ControllerMap.ButtonState.Down || currentState == ControllerMap.ButtonState.Pressed)
            {
                return ControllerMap.ButtonState.Released;
            } else
            {
                return ControllerMap.ButtonState.Up;
            }
        }
    }


    //Set an enum state to replicate GetButtonDown, etc.
    public void Trigger(InputAction.CallbackContext context)
    {
        float val = context.ReadValue<float>();

        if((context.action.actionMap == LeftMap && RightHanded) || (context.action.actionMap == RightMap && !RightHanded))
        {
            //Hand controller
            HandCon.Trigger = ParseAxis(HandCon.Trigger, val);

        } else
        {
            //Cannon controller
            GunCon.Trigger = ParseAxis(GunCon.Trigger, val);
        }
    }

    public void Grip(InputAction.CallbackContext context)
    {
        float val = context.ReadValue<float>();

        if ((context.action.actionMap == LeftMap && RightHanded) || (context.action.actionMap == RightMap && !RightHanded))
        {
            //Hand controller
            HandCon.Grip = ParseAxis(HandCon.Grip, val);

        }
        else
        {
            //Cannon controller
            GunCon.Grip = ParseAxis(GunCon.Grip, val);
        }
    }
}
