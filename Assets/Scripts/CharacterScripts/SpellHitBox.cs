using UnityEngine;

public class SpellHitBox : MonoBehaviour
{
    private int damage;

    private PlayerManager player;

    private bool isStun;

    private bool isSlow;

    int stunduration;

    int slowDuration;

    int slowPercentage;
    public void SetInfo(int dam, PlayerManager playermanager)
    {
        damage = dam;
        player = playermanager;
    }

    public void SetStunInfo(int duration)
    {
        isStun = true;
        stunduration = duration;
    }

    public void SetSlowInfo(int duration, int percentage)
    {
        slowDuration = duration;
        slowPercentage = percentage;
        isSlow = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            PlayerManager enemy = other.gameObject.GetComponent<PlayerManager>();
            if (!enemy.isDead)
            {
                enemy.Damage(damage, player.playerID);

                if (isStun && stunduration != 0)
                    enemy.Stun(stunduration);

                if (isSlow && slowDuration != 0)
                    enemy.Slow(slowDuration, slowPercentage);
            }
            
        }
    }
}
