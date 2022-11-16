using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerIcon : MonoBehaviour
{
    public Image playerIcon;
    public TMP_Text playerName;
    public TMP_Text playerCharacter;
    public int playerID;
    public void SetNewCharacter(Character character)
    {
        playerIcon.sprite = character.characterIcon;

        playerName.text = character.characterName;
    }
}
