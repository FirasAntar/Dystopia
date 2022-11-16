using Cinemachine;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CharacterSelect : MonoBehaviourPun
{
    enum CharacterSelectState
    {
        LockingIn, StartingGame
    }
    [SerializeField] private CharacterSelectState characterSelectState;

    [SerializeField] private Hashtable Players = new Hashtable();

    [SerializeField] private int TeamA;

    [SerializeField] private int TeamB;

    [SerializeField] public Transform[] spawnPositions;

    [SerializeField] private int playersLockedIn;

    [SerializeField] private int lockInTime;

    [SerializeField] private int startGameTime;

    [SerializeField] private TMP_Text timer;

    [SerializeField] private PlayerIcon playerIconPrefab;

    [SerializeField] private Transform teamAList;

    [SerializeField] private Transform teamBList;

    [SerializeField] public List<PlayerIcon> playerIcons;

    public Transform[] TeamA_SpawnPositions;

    public Transform[] TeamB_SpawnPositions;

    [SerializeField] private CinemachineVirtualCamera cameraTeamA;

    [SerializeField] private CinemachineVirtualCamera cameraTeamB;

    [Header("Start Character Select")]
    public UnityEvent startAction;

    [Header("Stop character Select")]
    public UnityEvent stopAction;

    [Header("Start Game Event")]
    public UnityEvent startGameAction;

    private static CharacterSelect _instance;

    public static CharacterSelect instance
    {
        get => _instance
            ;
        private set
        {
            if (_instance == null) _instance = value;

            else if (_instance != null) Destroy(value);
        }
    }
    public PlayerManager player(int playerID)
    {
        return (PlayerManager)Players[playerID];
    }
    private void Awake()
    {
        instance = this;
    }
    public void CameraSet(int teamID, Transform player)
    {
        if (teamID == 0)
        {
            cameraTeamA.Priority = 1;
            cameraTeamB.Priority = 0;
            cameraTeamA.Follow = player;
            cameraTeamA.LookAt = player;
        }
        else if (teamID == 1)
        {
            cameraTeamA.Priority = 0;
            cameraTeamB.Priority = 1;
            cameraTeamB.Follow = player;
            cameraTeamB.LookAt = player;
        }
    }
    bool locked = false;
    public void CharacterLocked()
    {
        if (!locked)
        {
            photonView.RPC("PlayerLockedInCount", RpcTarget.AllViaServer);
            locked = true;
        }
    }
    public void CharacterSelecting(int characterID)
    {
        if (!locked)
        {
            int clientID = PhotonNetwork.LocalPlayer.ActorNumber;

            photonView.RPC("CharacterSelectingUpdate", RpcTarget.AllViaServer, (byte)clientID, (byte)characterID);
        }
    }
    [PunRPC]
    private void PlayerLockedInCount()
    {
        playersLockedIn++;

        if (playersLockedIn == Launcher.instance.maxPlayerPerPvpRoom && characterSelectState == CharacterSelectState.LockingIn)
        {
            StopAllCoroutines();

            characterSelectState = CharacterSelectState.StartingGame;

            if (PhotonNetwork.IsMasterClient)

                StartCoroutine(CountDown(startGameTime, StartButton));

            else if (!PhotonNetwork.IsMasterClient)

                StartCoroutine(CountDown(startGameTime, null));
        }
    }
    [PunRPC]
    private void CharacterSelectingUpdate(byte playerID, byte characterID)
    {
        PlayerManager player = (PlayerManager)Players[playerID];

        Character character = Instantiate(GameManager.instance.gameCharacters[characterID]);

        player.SetCharacter(character);

        foreach (var item in playerIcons)
        {
            if (item.playerID == playerID)
            {
                item.playerIcon.sprite = character.characterIcon;
                item.playerCharacter.text = character.characterName;
            }
        }

        character.gameObject.transform.SetParent(player.transform);

        character.gameObject.transform.localPosition = Vector3.zero;

    }

    public void AddPlayerIcon(int teamid, int playerID, string playername)
    {
        GameObject obj = Instantiate(playerIconPrefab.gameObject);

        PlayerIcon icon = obj.GetComponent<PlayerIcon>();

        icon.playerID = playerID;

        playerIcons.Add(icon);

        locked = false;

        if (teamid == 0)
        {
            obj.transform.SetParent(teamAList);

            obj.GetComponent<RectTransform>().localScale = Vector3.one;

            icon.playerName.text = playername;

        }
        else if (teamid == 1)
        {
            obj.transform.SetParent(teamBList);

            obj.GetComponent<RectTransform>().localScale = Vector3.one;

            icon.playerName.text = playername;
        }
    }
    bool onetimebool;
    [PunRPC]
    private void AddPlayerManager(byte playerID)
    {
        if(!onetimebool)
        {
            Players.Clear();
            onetimebool = true;
        }

        if (Players[playerID] != null)

            Players.Remove(playerID);

        Players.Add(playerID, PlayerManager.Players[playerID]);


        if (Players.Count == Launcher.instance.maxPlayerPerPvpRoom)
        {
            float x = ((float)Launcher.instance.maxPlayerPerPvpRoom / 2);

            if (PhotonNetwork.IsMasterClient)
            {
                foreach (PlayerManager player in Players.Values)
                {

                    if (TeamA < x)
                    {
                        player.photonView.RPC("SetPlayerTeam", RpcTarget.AllBufferedViaServer, (byte)0, (byte)TeamA);

                        TeamA++;
                    }
                    else if (TeamA >= x)
                    {
                        player.photonView.RPC("SetPlayerTeam", RpcTarget.AllBufferedViaServer, (byte)1, (byte)TeamB);
                        TeamB++;
                    }
                }
            }
        }

    }
    public void StartButton()
    {
        photonView.RPC("StartPvpGame", RpcTarget.AllViaServer);
    }
    [PunRPC]
    private void StartPvpGame()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)

            PhotonNetwork.CurrentRoom.IsOpen = false;

        MenuManager.instance.OpenMenu("GameMenu");

        startGameAction.Invoke();

        locked = false;

        foreach (PlayerManager player in Players.Values)
        {
            player.gameObject.AddComponent<PlayerController>();

            if (player.photonView.IsMine)
            {
                foreach (Spells spell in player.character.activeSpells)
                {
                    if(spell)
                    spell.GetAxis();
                }
                if (player.playerTeam == PlayerManager.ArenaTeam.TeamA)
                {
                    foreach (PlayerManager item in PlayerManager.TeamBplayers.Values)
                    {
                        item.gameObject.layer = 7;

                        item.GetComponentInChildren<PlayerCanva>().ChangeColor();

                        MenuManager.instance.targetButtons[item.playerTeamNumber].SetImage(item.character.characterIcon);
                        MenuManager.instance.targetButtons[item.playerTeamNumber].Hide();
                        MenuManager.instance.targetButtons[item.playerTeamNumber].playerID = item.playerID;
                    }
                }
                else if (player.playerTeam == PlayerManager.ArenaTeam.TeamB)
                {
                    foreach (PlayerManager item in PlayerManager.TeamAplayers.Values)
                    {
                        item.gameObject.layer = 7;

                        MenuManager.instance.targetButtons[item.playerTeamNumber].SetImage(item.character.characterIcon);

                        item.GetComponentInChildren<PlayerCanva>().ChangeColor();

                        MenuManager.instance.targetButtons[item.playerTeamNumber].Hide();

                        MenuManager.instance.targetButtons[item.playerTeamNumber].playerID = item.playerID;
                    }
                }
            }
        }
        MenuManager.instance.StartPvpGame();
        GameManager.gameState = GameState.InGame;
        GameMenuManager.instance.SetPlayers();
    }
    public void PlayersDidntLockIn()
    {


        foreach (PlayerIcon playericon in playerIcons)
        {
            Destroy(playericon.gameObject);
        }

        playerIcons.Clear();

        if(GameManager.gameState != GameState.InGame)
        MenuManager.instance.playerCancelQueue();

        stopAction.Invoke();
    }
    public void StartCharacterSelect()
    {
        playersLockedIn = 0;

        startAction.Invoke();

        characterSelectState = CharacterSelectState.LockingIn;

        TeamA = 0;

        TeamB = 0;

        onetimebool = false;

        StopAllCoroutines();

        StartCoroutine(CountDown(lockInTime, PlayersDidntLockIn));
    }
    public IEnumerator CountDown(int time, Action action)
    {
        int count = time;

        timer.text = count.ToString();

        while (count != 0)
        {
            // update Time visuals

            yield return new WaitForSeconds(1);

            count--;

            timer.text = count.ToString();
        }

        if (action != null)

            yield return new WaitForSeconds(2);
        if (action != null)
            action();
    }
}
