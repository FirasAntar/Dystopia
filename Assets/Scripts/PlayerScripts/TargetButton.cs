using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class TargetButton : MonoBehaviour
{
    public bool active;

    public string buttonAxis;

    public int playerID;

    public Color targetColor;

    public Color normalColor;

    public Image cadre;
    public void Show()
    {
        gameObject.SetActive(true);
        active = true;
    }
    public void Hide()
    {
        gameObject.SetActive(false);
        active = false;
    }
    public void ChangeColor(Color color)
    {
        cadre.color = color;
    }

    public void SetImage(Sprite icon)
    {
        GetComponent<Image>().sprite = icon;
        
    }
    private void Update()
    {
        if (SimpleInput.GetButtonDown(buttonAxis))
        {
            PlayerManager player = (PlayerManager)PlayerManager.Players[(byte)PhotonNetwork.LocalPlayer.ActorNumber];
            foreach (TargetButton item in MenuManager.instance.targetButtons)
            {
                if (item.active)
                {
                    item.ChangeColor(normalColor);
                }
            }
            ChangeColor(targetColor);
            player.SetTarget(playerID);
        }
    }
}
