using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;
public class AbilityIcon : MonoBehaviour
{
    public Spells.SpellButton spellButton;
    public static AbilityIcon[] abilityIcons = new AbilityIcon[4];
    public TMP_Text cooldownValue;
    public Image icon;
    public string ButtonAxis;
    public string Zaxis;
    public string Xaxis;
    public Image cooldown;
    bool incooldown;
    public Animator anim;
    private void OnEnable()
    {
        abilityIcons[(int)spellButton] = this;

        switch (spellButton)
        {
            case Spells.SpellButton.One:
                ButtonAxis = "AbilityOne";
                Zaxis = "OneForward";
                Xaxis = "OneRight";
                break;
            case Spells.SpellButton.Two:
                ButtonAxis = "AbilityTwo";
                Zaxis = "TwoForward";
                Xaxis = "TwoRight";
                break;
            case Spells.SpellButton.Three:
                ButtonAxis = "AbilityThree";
                Zaxis = "ThreeForward";
                Xaxis = "ThreeRight";
                break;
            case Spells.SpellButton.Four:
                ButtonAxis = "Ultimate";
                Zaxis = "UltForward";
                Xaxis = "UltRight";
                break;
            default:
                break;
        }
    }
    public void UpdateIcon(Sprite sprite)
    {
        icon.sprite = sprite;  
    }
    public void UpdateCooldown(float timeLeft)
    {
        cooldownValue.text = timeLeft.ToString();
        if(!incooldown)
        {
            incooldown = true;
            anim.Play("abilityIconCoolodwn");
            cooldown.fillAmount = 1;
            StartCoroutine(activeCooldown((int)timeLeft));
        }
    }
    IEnumerator activeCooldown(int time)
    {
        float x = time;
        while (incooldown)
        {
            if (x == 0)
            {
                incooldown = false;
                cooldown.fillAmount = 0;
                anim.Play("OffCooldown");
                break;
            }
            float z = x/time;
          //  cooldown.fillAmount = z;

            yield return new WaitForSeconds(1);
            x--;
        }
    }
}
