using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPlayerControl : MonoBehaviour
{
    CharacterController CC;
    ActionParseInputs Input;
    WeaponSystem Weapons;

    public float MoveSpeed, TurnAngle;
    bool SelectingWeapon, CanTurn;
    Animator WeaponAni;

    // Start is called before the first frame update
    void Start()
    {
        CC = GetComponent<CharacterController>();
        Input = ActionParseInputs.Input;
        Weapons = GetComponent<WeaponSystem>();

        WeaponAni = transform.GetChild(3).GetComponent<Animator>();
    }

    void GunControls()
    {
        if (Weapons.Cooldown > 0)
        {
            Weapons.Cooldown -= Time.deltaTime;
        }

        if (Weapons.Cooldown <= 0 && Weapons.Charge <= 0)
        {
            //Allow Weapon Change if weapons have cooled off and are not currently being used
            SelectingWeapon = (Input.GunCon.Grip == ControllerMap.ButtonState.Down || Input.GunCon.Grip == ControllerMap.ButtonState.Pressed);
        } else
        {
            //Automatically false if using a weapon or waiting for it to cool down
            SelectingWeapon = false;
        }        

        if (SelectingWeapon)
        {
            Weapons.HandleWeaponSelect(Input.GunCon.StickVector);

            if(Input.GunCon.Trigger == ControllerMap.ButtonState.Pressed)
            {
                Weapons.ToggleTorch();
            }
        }
        else
        {
            //Player turns with the gun controller, so don't handle turning if stick is being used for weapon select
            if (CanTurn)
            {
                //Turn in direction of stick if past 0.5 in either direction
                float TurnDirection = Input.GunCon.StickVector.x;
                if (TurnDirection < -0.5f || TurnDirection > 0.5f)
                {
                    gameObject.transform.Rotate(new Vector3(0, TurnAngle * (TurnDirection < 0 ? -1 : 1)));
                    CanTurn = false;
                }
            }
            else
            {
                //Re-enable turning after returning stick to the middle
                if (Input.GunCon.StickVector.magnitude < 0.5f)
                {
                    CanTurn = true;
                }
            }

            //Call weapon behaviours
            if (Weapons.Cooldown <= 0)
            {
                switch (Weapons.CurrentWeapon)
                {
                    case WeaponSystem.WeaponType.Normal:
                        Weapons.Normal(Input.GunCon.Trigger);
                        break;
                    case WeaponSystem.WeaponType.Missile:
                        Weapons.Missile(Input.GunCon.Trigger);
                        break;
                    case WeaponSystem.WeaponType.Beam:
                        Weapons.Beam(Input.GunCon.Trigger);
                        break;
                    default:
                        print("No behaviour ready for this weapon!");
                        break;
                }
            }
        }
    }

    void HandControls()
    {

        //Move in joystick direction relative to camera direction
        CC.Move(Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * new Vector3(Input.HandCon.StickVector.x * MoveSpeed, -9.81f, Input.HandCon.StickVector.y * MoveSpeed) * Time.deltaTime);
        
        if(PlayerHands.PH.InteractionTarget != null)
        {
            if (!PlayerHands.PH.CarryingObject && Input.HandCon.Grip == ControllerMap.ButtonState.Pressed)
            {
                PlayerHands.PH.TryPickUp();
            }
            else if (PlayerHands.PH.CarryingObject && Input.HandCon.Grip == ControllerMap.ButtonState.Released)
            {
                PlayerHands.PH.DropItem();
            }
        }
    }

    void UpdateAnimations()
    {
        WeaponAni.SetBool("SelectingWeapon", SelectingWeapon);
        WeaponAni.SetInteger("CurrentWeapon", (int)Weapons.CurrentWeapon);
        if(Weapons.Charge != 0)
        {
            WeaponAni.SetFloat("WeaponCharge", Weapons.Charge);
        }
        WeaponAni.SetFloat("WeaponCooldown", Weapons.Cooldown);

        WeaponAni.SetBool("TriggerDown", Input.GunCon.Trigger == ControllerMap.ButtonState.Pressed);
        WeaponAni.SetBool("TriggerHeld", Input.GunCon.Trigger == ControllerMap.ButtonState.Pressed || Input.GunCon.Trigger == ControllerMap.ButtonState.Down);
    }

    private void Update()
    {
        GunControls();
        HandControls();
        UpdateAnimations();
    }
}
