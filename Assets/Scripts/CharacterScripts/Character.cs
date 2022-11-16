using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public string characterName;

    public int maxHealth;

    public float maxRange;

    public float autoAttackDamage;

    public float movementSpeed;
        
    public Spells[] activeSpells;

    public Sprite characterIcon;

    public Animator m_anim
    {
        get;private set;
    }
    public PlayerManager m_player
    {
        get;private set;
    }
    public abstract void Attack();

    public void RestartMovement()
    {
        GetComponentInParent<PlayerController>().StopMovement(false);
    }
    public void StopMovement()
    {
        GetComponentInParent<PlayerController>().StopMovement(true);
    }
    public void UseAbility(int index)
    {
        if (activeSpells[index])
            activeSpells[index].UseButton();
    }
    public void UseAbility(int index,float x,float z)
    {
        if (activeSpells[index])
            activeSpells[index].UseJoystick(x,z);
    }
    private void Start()
    {
        m_player = GetComponentInParent<PlayerManager>();
        m_anim = GetComponent<Animator>();
    }
}
