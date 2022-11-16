using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SelfHeal : Spells
{
    public GameObject spellVFX;
    public int healAmount;
    public override void JoystickAxis(float z, float x)
    {
        throw new System.NotImplementedException();
    }

    public override void SpellClicked()
    {
        throw new System.NotImplementedException();
    }

    public override void UseButton()
    {
        m_anim.SetTrigger("abilityThree");
        GetComponentInParent<PhotonView>().RPC("HealRPC", RpcTarget.AllViaServer, (byte)healAmount);
        StartCoroutine(StartCooldown());
    }

    public override void UseJoystick(float x, float z)
    {
        throw new System.NotImplementedException();
    }
}
