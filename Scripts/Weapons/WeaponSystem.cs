using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponSystem : MonoBehaviour
{
    public enum WeaponType { Normal, Missile, Beam };
    public bool[] UnlockedWeapon = new bool[] {true, false, false};
    public WeaponType CurrentWeapon = WeaponType.Normal; //Changes the function called from main control
    public float Cooldown, Charge = 0;
    public GameObject BulletPrefab;
    private bool WeaponEffectTriggered = false; //Used when weapons have different effects on press and hold

    private Transform WeaponEnd, Segments;
    private LineRenderer BeamLine;
    private Light WeaponTorch;
    private GameObject BeamEnd, BeamParticle;

    [SerializeField] GameObject SegmentPrefab;
    private float SegmentWidth;
    private TMP_Text WeaponName, WeaponAmmo;
    private SegmentBehaviour currentHighlight;

    private void Start()
    {
        WeaponEnd = PlayerHands.PH.WeaponEnd;
        WeaponTorch = WeaponEnd.GetChild(0).GetComponent<Light>();
        BeamLine = WeaponEnd.GetChild(4).GetComponent<LineRenderer>();
        BeamEnd = BeamLine.transform.GetChild(1).gameObject;
        BeamParticle = BeamEnd.transform.GetChild(0).gameObject;
        BeamParticle.SetActive(false);

        Transform WeaponSelect = GameObject.Find("Weapon Select").transform;
        Segments = WeaponSelect.GetChild(0);
        WeaponName = WeaponSelect.GetChild(1).GetChild(1).GetComponent<TMP_Text>();
        WeaponAmmo = WeaponSelect.GetChild(1).GetChild(2).GetComponent<TMP_Text>();

        //Eventually, settings and loading will call this with the unlocked list saved there
        SetUnlockedWeapons(new bool[] { true, true, true });
    }

    public void SetUnlockedWeapons(bool[] unlocks)
    {
        UnlockedWeapon = unlocks;

        //If the weapon that the player was using is now locked, iterate through and set them to the first unlocked weapon (usually Normal mode)
        if (!UnlockedWeapon[(int)CurrentWeapon])
        {
            for(int i = 0; i < UnlockedWeapon.Length; i++)
            {
                if (UnlockedWeapon[i])
                {
                    CurrentWeapon = (WeaponType)i;
                    break;
                }
            }
        }

        //Update weapon select segments to fit new arsenal
        UpdateSegments();
    }

    //Called at start and when obtaining a new weapon
    public void UpdateSegments()
    {
        if(Segments.childCount > 0)
        {
            foreach(Transform child in Segments)
            {
                Destroy(child.gameObject);
            }
        }

        //Count number of unlocked weapons
        int unlocked = 0;
        foreach(bool b in UnlockedWeapon)
        {
            unlocked += (b ? 1 : 0);
        }
        SegmentWidth = 360 / unlocked; //Width of each segment in degrees

        int SegmentsMade = 0;
        //Start at width/2, should be first weapon (Normal()) straight upward. Add width to each segment rotation
        for (int i = 0; i < UnlockedWeapon.Length; i++)
        {
            if (UnlockedWeapon[i])
            {
                GameObject seg = Instantiate(SegmentPrefab, Segments);
                SegmentBehaviour sb = seg.GetComponent<SegmentBehaviour>();
                sb.SetSegmentWidth(SegmentWidth);
                sb.SetRepresenting((WeaponType)i);
                seg.transform.localEulerAngles = new Vector3(0, 0, (SegmentWidth / 2) + (SegmentWidth * SegmentsMade));
                SegmentsMade++;
            }
        }
    }

    //Turn on torch and swap image on weapon wheel
    public void ToggleTorch()
    {
        WeaponTorch.enabled = !WeaponTorch.enabled;
    }

    public void HandleWeaponSelect(Vector2 stickDirection)
    {
        //Only update if stick is halfway out or more
        if(stickDirection.magnitude >= 0.5f)
        {
            //Convert input angle into 360-degree angle
            float angle = Vector2.SignedAngle(Vector2.right, stickDirection);
            if (angle < 0)
            {
                angle = 360 + angle;
            }

            bool inRange;

            //Iterate through each child object and test input angle against segment angle and width
            foreach(Transform child in Segments)
            {
                float segAngle = child.transform.localEulerAngles.z;

                inRange = segAngle - (SegmentWidth / 2) < angle && angle < segAngle + (SegmentWidth / 2);

                if (inRange)
                {
                    if(currentHighlight != null)
                    {
                        currentHighlight.ToggleHighlight();
                    }

                    currentHighlight = child.GetComponent<SegmentBehaviour>();
                    currentHighlight.ToggleHighlight();
                    CurrentWeapon = currentHighlight.Represents;

                    break;
                }
            }
        } else
        {
            //Reset selection animation
            if (currentHighlight != null)
            {
                currentHighlight.ToggleHighlight();
                currentHighlight = null;
            }
        }


        //Ammo is currently static text, replace with actual values once ammo tracking is implemented
        switch (CurrentWeapon)
        {
            case WeaponType.Normal:
                WeaponName.text = "Normal";
                WeaponAmmo.text = "--/--";
                break;
            case WeaponType.Missile:
                WeaponName.text = "Missile";
                WeaponAmmo.text = "20/20";
                break;
            case WeaponType.Beam:
                WeaponName.text = "Beam";
                WeaponAmmo.text = "--/--";
                break;
            default:
                WeaponName.text = "???";
                WeaponAmmo.text = "--/--";
                break;
        }
    }

    //Shoots a small bullet on tap or larger bullets based on charge time.
    public void Normal(ControllerMap.ButtonState Trigger)
    {
        if(Cooldown <= 0)
        {
            if (Trigger == ControllerMap.ButtonState.Down)
            {
                if (Charge < 1.5f)
                {
                    Charge += Time.deltaTime;
                }

                ActionParseInputs.Input.VibrateForTime(true, Charge / 3f, 0.1f);
            }
            else if (Trigger == ControllerMap.ButtonState.Released)
            {
                GameObject bullet = Instantiate(BulletPrefab, WeaponEnd.position, WeaponEnd.rotation);
                if (Charge < 0.5f)
                {
                    //Normal shot
                    ActionParseInputs.Input.VibrateForTime(true, 0.1f, 0.1f);
                }
                else if (Charge < 1.5f)
                {
                    //Semi-charge shot
                    ActionParseInputs.Input.VibrateForTime(true, 0.3f, 0.25f);
                    bullet.transform.localScale = new Vector3(2, 2, 2);
                    Cooldown = 0.5f;
                }
                else
                {
                    //Charge shot
                    ActionParseInputs.Input.VibrateForTime(true, 0.6f, 0.5f);
                    bullet.transform.localScale = new Vector3(3, 3, 3);
                    Cooldown = 1f;
                }

                Charge = 0;
            }
        }
    }


    //Shoots one missile on tap.
    public void Missile(ControllerMap.ButtonState Trigger)
    {
        if(Trigger == ControllerMap.ButtonState.Pressed && Cooldown <= 0)
        {
            //No projectile to spawn yet!
            ActionParseInputs.Input.VibrateForTime(true, 0.3f, 0.1f);
            Cooldown = 1f;
        }
    }

    //Shoots a wide beam forward when held down, but has a maximum time it can be used.
    public void Beam(ControllerMap.ButtonState Trigger)
    {
        if(Cooldown <= 0)
        {
            if (Trigger == ControllerMap.ButtonState.Pressed)
            {
                Charge = 3f;
                ActionParseInputs.Input.VibrateForTime(true, 0.3f, 0.1f);
                BeamParticle.SetActive(true);

                BeamLine.SetPositions(new Vector3[] { Vector3.down * 100f, Vector3.down * 100f });

                WeaponEffectTriggered = true;
            }
            else if (WeaponEffectTriggered)
            {
                if (Trigger == ControllerMap.ButtonState.Down)
                {
                    ActionParseInputs.Input.VibrateForTime(true, 0.1f, 0.1f);
                    Physics.Raycast(WeaponEnd.position, WeaponEnd.forward * 999f, out RaycastHit hit);

                    BeamLine.SetPosition(0, WeaponEnd.position);
                    BeamLine.SetPosition(1, hit.point != Vector3.zero ? hit.point : WeaponEnd.transform.forward * 999f);
                    BeamEnd.transform.eulerAngles = hit.normal;

                    Charge -= Time.deltaTime;

                    if(Charge < 0.7)
                    {
                        BeamLine.startWidth = Charge/10;
                        BeamLine.endWidth = Charge/10;
                    } else
                    {
                        BeamLine.startWidth = 0.07f;
                        BeamLine.endWidth = 0.07f;
                    }
                }

                //Charge is used as the weapon timer
                if (Trigger == ControllerMap.ButtonState.Released || Charge <= 0)
                {
                    Charge = 0f;
                    BeamLine.SetPositions(new Vector3[] { Vector3.down * 100f, Vector3.down * 100f });
                    BeamParticle.SetActive(false);
                    Cooldown = 1f;

                    WeaponEffectTriggered = false;
                }
            }

            //Always match beam end effect to the end of the beam
            BeamEnd.transform.position = BeamLine.GetPosition(1);
        }
    }
}
