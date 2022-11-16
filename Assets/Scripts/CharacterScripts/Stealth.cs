using QFX.IFX;
using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEngine.Rendering;

public class Stealth : Spells
{
    public int stealthDuration;

    private IFX_MaterialAdder _materialAdder;

    [SerializeField] private float MaterialLifeTime;

    [SerializeField] private GameObject[] MaterialTargets;

    public Transform CloneSpecificTarget;

    public IFX_AnimationModule CloneOpacityAnimation;

    public float CloneLifeTime;

    public Material CloneMaterial;

    public Component[] CloneComponentsWithRequirements;

    public Material Material;

    public GameObject startVFX;

    public GameObject targetGraphic;

    public Volume gameVolume;
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
        
        m_anim.SetTrigger("abilityTwo");

        if (_materialAdder != null)
        {
            _materialAdder.Run();
        }
        else if (_materialAdder == null)
        {
            _materialAdder = gameObject.AddComponent<IFX_MaterialAdder>();
            _materialAdder.LifeTime = MaterialLifeTime;
            _materialAdder.Targets = MaterialTargets;
            _materialAdder.Material = Material;
            _materialAdder.Setup();
            _materialAdder.Run();
        }


        /*  var target = CloneSpecificTarget != null ? CloneSpecificTarget.gameObject : gameObject;

          IFX_Cloner.MakeClone(target, CloneLifeTime, CloneMaterial, CloneOpacityAnimation, CloneComponentsWithRequirements);
        */
        // GameObject obj = Instantiate(startVFX);


         if (GetComponentInParent<PhotonView>().IsMine)
        {
            StartCoroutine(StealthStarted());
            StartCoroutine(StartCooldown());
        }

       // obj.transform.position = transform.position;

       // obj.GetComponent<ParticleSystem>().Play();
    }
    
    public override void UseJoystick(float x, float z)
    {
        throw new System.NotImplementedException();
    }

    IEnumerator StealthStarted()
    {
        GetComponentInParent<PhotonView>().RPC("Stealth", RpcTarget.AllViaServer);
        
        yield return new WaitForSeconds(stealthDuration);

        GetComponentInParent<PhotonView>().RPC("Stealth", RpcTarget.AllViaServer);
    }
}
