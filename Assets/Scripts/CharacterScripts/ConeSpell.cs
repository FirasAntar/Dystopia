using Photon.Pun;
using UnityEngine;
using System.Collections;
public class ConeSpell : Spells
{
    [SerializeField] private string animationName;
    private GameObject activeIndicator;
    private Transform cam;
    public float hitboxDelay;
    public int slowPercentage;
    public int slowDuration;
    public override void UseButton()
    {
        m_anim.SetTrigger(animationName);
        GameObject obj = Instantiate(spellVFX);
        obj.transform.position = transform.position;
        obj.transform.forward = transform.forward;
        Destroy(obj, 6);
        if (GetComponentInParent<PhotonView>().IsMine)
        {
            StartCoroutine(StartCooldown());
            StartCoroutine(delay(transform.forward));
        }
    }

    public override void UseJoystick(float x, float z)
    {
        m_anim.SetTrigger(animationName);
        GameObject obj = Instantiate(spellVFX);
        obj.transform.position = transform.position;
        Vector3 dir = new Vector3(z, 0, x);
        obj.transform.forward = dir;
        Destroy(obj, 6);
        if (GetComponentInParent<PhotonView>().IsMine)
        {
            StartCoroutine(StartCooldown());
            StartCoroutine(delay(dir));
        }
    }
    IEnumerator delay(Vector3 dir)
    {
        yield return new WaitForSeconds(hitboxDelay);
        GameObject obj = Instantiate(hitBox.gameObject);
        obj.GetComponent<SpellHitBox>().SetInfo(spellDamage, GetComponentInParent<PlayerManager>());
        if (slowDuration != 0)
        {
            obj.GetComponent<SpellHitBox>().SetSlowInfo(slowDuration, slowPercentage);
        }
        obj.transform.position = transform.position;
        obj.transform.forward = dir;
        Destroy(obj, 0.2f);
    }
    public override void JoystickAxis(float z, float x)
    {
        if (!activeIndicator)
        {
            activeIndicator = Instantiate(spellIndicator);
            activeIndicator.transform.SetParent(transform);
            activeIndicator.transform.localPosition = Vector3.zero;
        }else if(activeIndicator)
        {

            if(!cam)
            cam = Camera.main.transform;

            Vector3 direction = cam.transform.forward * z + cam.transform.right* x;

            direction.y = 0;

            activeIndicator.transform.forward = Math.dampVector3(activeIndicator.transform.forward,direction,20,Time.deltaTime);
        }

    }

    public override void SpellClicked()
    {
        if (activeIndicator)
            Destroy(activeIndicator);
    }

    public int spellDamage;

    public SpellHitBox hitBox;

    public GameObject spellIndicator;

    public GameObject spellVFX;

}
