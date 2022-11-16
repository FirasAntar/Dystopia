using Photon.Pun;
using System.Collections;
using UnityEngine;

public abstract class Spells : MonoBehaviour
{
    public enum SpellButton
    {
        One, Two, Three, Four
    }
    public enum InputType
    {
        Button, Joystick
    }

    public string spellName;

    public int cooldown;


    public Sprite icon;

    public InputType inputType;

    public SpellButton spellButton;

    public bool inCooldown;

    public Animator m_anim;

    public abstract void UseButton();

    public abstract void UseJoystick(float x, float z);

    public abstract void JoystickAxis(float z, float x);

    public abstract void SpellClicked();

    public static bool globalCooldown;

    public static IEnumerator GlobalCooldown()
    {
        globalCooldown = true;
        yield return new WaitForSeconds(0.7f);
        globalCooldown = false;
    }

    public IEnumerator StartCooldown()
    {
        float x = cooldown;
        inCooldown = true;
        AbilityIcon abilityicon = AbilityIcon.abilityIcons[(int)spellButton];
        abilityicon.cooldownValue.color = Color.white;
        abilityicon.UpdateCooldown(x);
        while (inCooldown)
        {
            if (x == 0)
            {
                inCooldown = false;
                if(!GetComponentInParent<PlayerManager>().isDead)
                StartCoroutine(Active());
                abilityicon.cooldownValue.color = Color.clear;
                abilityicon.UpdateCooldown(x);
                StopCoroutine(StartCooldown());
                break;
            }

            yield return new WaitForSeconds(1);

            x--;

            abilityicon.UpdateCooldown(x);
        }
    }

    string buttonAxis;
    string Zaxis;
    string Xaxis;

    public void GetAxis()
    {
        AbilityIcon abilityicon = AbilityIcon.abilityIcons[(int)spellButton];
        buttonAxis = abilityicon.ButtonAxis;
        Zaxis = abilityicon.Zaxis;
        Xaxis = abilityicon.Xaxis;
        abilityicon.icon.sprite = icon;
        StartCoroutine(Active());
    }
    public IEnumerator Active()
    {
        bool clicked = false;
        Vector3 directon = Vector3.zero;
        while (!inCooldown)
        {
            switch (inputType)
            {
                case InputType.Button:

                    if (SimpleInput.GetButton(buttonAxis))
                    {
                        PhotonView pv = GetComponentInParent<PhotonView>();
                        if (GetComponentInParent<PlayerManager>().inStealth)
                            pv.RPC("Stealth", RpcTarget.All);

                        pv.RPC("SpellButton", RpcTarget.All, (byte)spellButton);
                        GetComponentInParent<PlayerController>().StopMovement(true);
                        inCooldown = true;
                        StopCoroutine(Active());
                    }

                    break;
                case InputType.Joystick:

                    float z = SimpleInput.GetAxisRaw(Zaxis);
                    float x = SimpleInput.GetAxisRaw(Xaxis);
                    Vector2 axis = new Vector2(x, z);


                    if (SimpleInput.GetButtonUp(buttonAxis) && axis == Vector2.zero && !clicked)
                    {
                        clicked = true;
                        PlayerController player = GetComponentInParent<PlayerController>();
                        PlayerManager playermanager = player.playerManager;
                        PlayerManager targetmanager = null;
                        if (playermanager.target)
                        {
                            targetmanager = playermanager.target;
                            if (Math.distanceAB(transform.position, targetmanager.transform.position) <= playermanager.character.maxRange)
                            {
                                Vector3 dir = Math.vectorABXZ(transform.position, targetmanager.transform.position);

                                player.transform.forward = dir;
                                GetComponentInParent<PhotonView>().RPC("SpellJoystick", RpcTarget.All, (byte)spellButton, dir.z, dir.x);
                                if (playermanager.inStealth)
                                    playermanager.photonView.RPC("Stealth", RpcTarget.All);
                                player.StopMovement(true);
                                inCooldown = true;
                                SpellClicked();
                                StopCoroutine(Active());
                                clicked = false;
                                break;
                            }
                            if (playermanager.inStealth)
                                playermanager.photonView.RPC("Stealth", RpcTarget.All);
                            GetComponentInParent<PhotonView>().RPC("SpellJoystick", RpcTarget.All, (byte)spellButton, transform.forward.z, transform.forward.x);
                            player.StopMovement(true);
                            inCooldown = true;
                            SpellClicked();
                            StopCoroutine(Active());
                            clicked = false;
                            break;
                        }
                        else if (!playermanager.target)
                        {
                            if (playermanager.inStealth)
                                playermanager.photonView.RPC("Stealth", RpcTarget.All);
                            GetComponentInParent<PhotonView>().RPC("SpellJoystick", RpcTarget.All, (byte)spellButton, transform.forward.z, transform.forward.x);
                            player.StopMovement(true);
                            inCooldown = true;
                            SpellClicked();
                            StopCoroutine(Active());
                            clicked = false;
                            break;
                        }
                    }
                    else
                    {
                        if (axis != Vector2.zero && !clicked)
                        {
                            JoystickAxis(x, z);
                            clicked = true;
                        }
                        else if (clicked && axis != Vector2.zero)
                        {
                            JoystickAxis(x, z);
                            directon = Camera.main.transform.forward * x + Camera.main.transform.right * z;
                            directon.y = 0;
                        }
                        else if (clicked && axis == Vector2.zero)
                        {
                            PlayerController player = GetComponentInParent<PlayerController>();
                            GetComponentInParent<PhotonView>().RPC("SpellJoystick", RpcTarget.All, (byte)spellButton, directon.z, directon.x);
                            if (player.playerManager.inStealth)
                                player.playerManager.photonView.RPC("Stealth", RpcTarget.All);
                            player.StopMovement(true);
                            player.transform.forward = directon;
                            inCooldown = true;
                            SpellClicked();
                            StopCoroutine(Active());
                            clicked = false;
                            break;
                        }
                    }


                    break;
            }
            yield return null;
        }
    }
}
