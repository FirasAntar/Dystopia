using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Menu[] menus;

    public GameObject playersKilled;

    public GameObject playerKilledPlayerPrefab;

    public TargetButton[] targetButtons;

    public Menu activeMenu;

    [Header("MATCH FOUND EVENT")]
    public UnityEvent matchFoundEvent;

    [Header("Leave Game Action")]
    public UnityEvent leaveGameAction;



    private static MenuManager _instance;
    public static MenuManager instance
    {
        get => _instance;
        private set
        {
            if (_instance == null) _instance = value;
            else if (_instance != value)

                Destroy(value);

        }
    }
    public void KilledUpdates(string killerName,string killedName,bool allyDied)
    {
        GameObject obj = Instantiate(playerKilledPlayerPrefab);
        obj.GetComponent<PlayerKilledPlayer>().SetNames(killerName, killedName, allyDied);
        obj.transform.SetParent(playersKilled.transform);
    }

    public void LeavingGame()
    {
        leaveGameAction.Invoke();
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            GameManager.gameState = GameState.InLobby;
        }
    }

    public void SettingsButtonClicked(Animator anim)
    {

        anim.SetTrigger("clicked");
    }
    private void Awake()
    {
        instance = this;
    }
    public void OpenMenu(string menuName)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].menuName == menuName)

                OpenMenu(menus[i]);

            else if (menus[i].opened)

                CloseMenu(menus[i]);

        }
    }

    public void OpenMenu(Menu menu)
    {
        if (activeMenu != null) CloseMenu(activeMenu);

        activeMenu = menu;

        menu.Open();
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }

    public void JoinPvpQueue()
    {
        Launcher.instance.JoinRandomPvpRoom();
    }

    public void PvpQueueJoined(int playersInQ)
    {
        Debug.Log("Current Client In Pvp Queue");

        PlayerInQueueUpdate(playersInQ);
    }

    public void PlayerInQueueUpdate(int playersInQ)
    {
       // Debug.Log(playersInQ + " players in queue");
       // playersInQueue.text = "iN QUEUE (" + playersInQ + "/" + Launcher.instance.maxPlayerPerPvpRoom + ")";
    }
    public void MatchFoundCheck()
    {
        matchFoundEvent.Invoke();

        GameManager.gameState = GameState.InMatchFound;
    }

    public void CharacterSelectingStarted()
    {
        if (GameManager.gameState == GameState.InMatchFound)
        {
            GameManager.gameState = GameState.InCharacterSelect;

            CharacterSelect.instance.StartCharacterSelect();

            StartCoroutine(CharacterSelectStarted());
        }
        // Visual Transition
    }

    public void PlayerAcceptedQueue()
    {
        GetComponent<PhotonView>().RPC("PlayersAcceptedQueue", RpcTarget.AllViaServer);
    }
    public void playerCancelQueue()
    {
       // playersInQueue.text = "QUICK PLAY";

       // acceptMatchFoundButton.interactable = true;

        StopCoroutine(LeaveRoom());

        StartCoroutine(LeaveRoom());
    }

    public void PlayerLeftQueueUpdate()
    {

        PlayerInQueueUpdate(Launcher.instance.PlayersInQueueCount());
    }
    IEnumerator CharacterSelectStarted()
    {
        yield return new WaitForSeconds(0.3f);
    }
    IEnumerator LeaveRoom()
    {
        yield return new WaitForSeconds(0.3f);
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
    }

    internal void StartPvpGame()
    {
        
    }

    public void LeaveGame()
    {
        PhotonNetwork.LeaveRoom();
        leaveGameAction.Invoke();
    }
}
