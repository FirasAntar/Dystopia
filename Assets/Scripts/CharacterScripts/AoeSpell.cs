using Photon.Pun;
using System.Collections;
using UnityEngine;

public class AoeSpell : Spells
{
    public GameObject spellVFX;

    public SpellHitBox hitBox;

    public int spellDamage;

    public bool slow;

    public int slowDuration;

    public int slowPercentage;

    public int channelingTime;

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
        if (channelingTime == 0)
        {
            m_anim.SetTrigger("abilityThree");
            GameObject obj = Instantiate(spellVFX);
            obj.transform.position = transform.position;
            Destroy(obj, 3);
            if (GetComponentInParent<PhotonView>().IsMine)
            {
                obj = Instantiate(hitBox.gameObject);
                obj.transform.position = transform.position;
                if (slow) obj.GetComponent<SpellHitBox>().SetSlowInfo(slowDuration, slowPercentage);
                obj.GetComponent<SpellHitBox>().SetInfo(spellDamage, GetComponentInParent<PlayerManager>());
                StartCoroutine(StartCooldown());
                Destroy(obj, 1.5f);
            }
        }
        else if (channelingTime != 0)
        {
            m_anim.SetTrigger("FlexUltimate");
            GameObject obj = Instantiate(spellVFX);
            obj.transform.position = transform.position + Vector3.up/3;
            obj.transform.SetParent(transform);
            Destroy(obj, 2);
            if (GetComponentInParent<PhotonView>().IsMine)
            {
                StartCoroutine(channeling());
            }
        }
    }
    IEnumerator channeling()
    {
        Character flex = GetComponent<Character>();

        foreach (Spells item in flex.activeSpells)
        {
            if (item)
                if (!item.inCooldown)
                    item.StopCoroutine(item.Active());
        }
        float x = 3;
        float damage = spellDamage / 3;
        while (x > 0)
        {
            GameObject obj = Instantiate(hitBox.gameObject);
            obj.transform.position = transform.position ;
            if (slow) obj.GetComponent<SpellHitBox>().SetSlowInfo(slowDuration, slowPercentage);
            obj.GetComponent<SpellHitBox>().SetInfo((int)damage, GetComponentInParent<PlayerManager>());
            StartCoroutine(StartCooldown());
            Destroy(obj, 0.3f);
            yield return new WaitForSeconds(channelingTime / 3);
            x--;
            if (x == 0)
            {
                foreach (Spells item in flex.activeSpells)
                {
                    if (item)
                        if (!item.inCooldown)
                        {
                            item.StopCoroutine(item.Active());
                            item.StartCoroutine(item.Active());
                        }
                }
            }
        }
    }
    public override void UseJoystick(float x, float z)
    {
        throw new System.NotImplementedException();
    }
}
