using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Flex : Character
{
    [SerializeField] private GameObject autoHitBox;

    [SerializeField] private GameObject passiveVFX;

    int passiveAttacks = 0;

    public override void Attack()
    {
        if (GetComponentInParent<PlayerManager>().inStealth)
            GetComponentInParent<PlayerManager>().photonView.RPC("Stealth", RpcTarget.All);

        if (passiveAttacks ==  0)
        {
            m_anim.SetTrigger("attackone");
            passiveAttacks++;
            if (m_player.photonView.IsMine)
            {
                GameObject obj = Instantiate(autoHitBox);
                obj.transform.position = transform.position;
                obj.transform.forward = transform.forward;
                obj.GetComponent<SpellHitBox>().SetInfo((int)autoAttackDamage, m_player);
                Destroy(obj, 0.3f);
            }
        }
        else if(passiveAttacks == 1)
        {
            m_anim.SetTrigger("attacktwo");
            passiveAttacks++;
            if (m_player.photonView.IsMine)
            {
                GameObject obj = Instantiate(autoHitBox);
                obj.transform.position = transform.position;
                obj.transform.forward = transform.forward;
                obj.GetComponent<SpellHitBox>().SetInfo((int)autoAttackDamage + 2, m_player);
                Destroy(obj, 0.3f);
            }
        } else if (passiveAttacks == 2)
        {
            m_anim.SetTrigger("attackthree");
            GameObject VFX = Instantiate(passiveVFX);
            VFX.transform.position = transform.position;
            VFX.transform.forward = transform.forward;
            Destroy(VFX, 1);
            passiveAttacks= 0;
            if (m_player.photonView.IsMine)
            {
                GameObject obj = Instantiate(autoHitBox);
                obj.transform.position = transform.position;
                obj.transform.forward = transform.forward;
                obj.GetComponent<SpellHitBox>().SetInfo((int)autoAttackDamage + 4, m_player);
                obj.GetComponent<SpellHitBox>().SetSlowInfo(2,10);
                Destroy(obj, 0.3f);
            }
        }
    }
}
