using TMPro;
using UnityEngine;

public class PlayerKilledPlayer : MonoBehaviour
{
    public TMP_Text killer;
    public TMP_Text killed;

    public Color allyColor;
    public Color enemyColor;

    public void SetNames(string killerName,string killedName,bool allyDied)
    {
        killer.text = killerName;
        killed.text = killedName;

        if (allyDied)
        {
            killer.color = enemyColor;
            killed.color = allyColor;
        }else if(!allyDied)
        {
            killer.color = allyColor;
            killed.color = enemyColor;
        }
        Destroy(this.gameObject, 2);
    }
}
