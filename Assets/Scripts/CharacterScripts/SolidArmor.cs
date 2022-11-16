using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SolidArmor : Spells
{
    public GameObject startVFX;
    public int spellDuration;
    public int damageReductionAmount;
    public int bonusAttackDamage;
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
        m_anim.SetTrigger("Ultimate");
        GameObject obj = Instantiate(startVFX);
        obj.transform.position = transform.position;
        if (GetComponentInParent<PhotonView>().IsMine)
        {
            StartCoroutine(StartCooldown());
            StartCoroutine(ActiveBuff(spellDuration));
        }
    }
    IEnumerator ActiveBuff(int duration)
    {
        GetComponent<Hammer>().bonusAttackDamage = bonusAttackDamage;
        GetComponentInParent<PlayerManager>().photonView.RPC("SetReducedDamage", RpcTarget.AllViaServer, (byte)damageReductionAmount);
        yield return new WaitForSeconds(duration);
        GetComponent<Hammer>().bonusAttackDamage = 0;
        GetComponentInParent<PlayerManager>().photonView.RPC("SetReducedDamage", RpcTarget.AllViaServer, (byte)0);
    }
    public override void UseJoystick(float x, float z)
    {
        throw new System.NotImplementedException();
    }
}
