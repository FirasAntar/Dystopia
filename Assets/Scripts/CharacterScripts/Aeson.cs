using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aeson : Character
{
    [SerializeField] private GameObject autoHitBox;
    public override void Attack()
    {
        m_anim.SetTrigger("attack");
        if (m_player.photonView.IsMine)
        {
            GameObject obj = Instantiate(autoHitBox);
            obj.transform.position = transform.position;
            obj.transform.forward = transform.forward;
            obj.GetComponent<SpellHitBox>().SetInfo((int)(autoAttackDamage), m_player);
            Destroy(obj, 0.3f);
        }
    }
}
