using ExitGames.Client.Photon;
using Michsky.UI.Shift;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField] private int _maxPlayerPerPvpRoom;

    [SerializeField] private TimedEvent timerEvent;

    public ModalWindowManager disconnectedWindow;

    public ModalWindowManager LeaveGameWindow;

    public TMP_InputField userName;

    public TMP_Text profileName;

    public TMP_Text welcomeName;

    public string clientName;

    public void MaxPlayerPerRoomUpdate(int x)
    {
        _maxPlayerPerPvpRoom = x;
    }

    public int maxPlayerPerPvpRoom
    {
        get => _maxPlayerPerPvpRoom;
    }

    private static Launcher _instance;

    public static Launcher instance
    {
        get => _instance;
        private set
        {
            if (_instance == null) _instance = value;

            else if (_instance != value)

                Destroy(value);
        }
    }
    private void Awake()
    {
        instance = this;

        Application.targetFrameRate = 400;
    }
    private void Start()
    {
        GameManager.gameState = GameState.SpashScreen;
    }
    public void ConnectToMaster()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            timerEvent.StartIEnumerator();
        }
    }
    public int PlayersInQueueCount()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount;
    }
    public override void OnConnectedToMaster()
    {
        if (GameManager.gameState != GameState.SpashScreen)
        {
            timerEvent.reconnectedAction.Invoke();
        }
        PhotonNetwork.JoinLobby();

    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected");

        disconnectedWindow.ModalWindowIn();

        if (GameManager.gameState == GameState.InGame)
        {
            MenuManager.instance.LeaveGame();
        }

        CharacterSelect.instance.PlayersDidntLockIn();
        GameManager.gameState = GameState.InLobby;
    }
    public override void OnJoinedLobby()
    {
        if (userName.text != "")
            PhotonNetwork.NickName = userName.text;
        else
            PhotonNetwork.NickName = "Guest";

        welcomeName.text = PhotonNetwork.NickName;
        profileName.text = PhotonNetwork.NickName;

        foreach (PlayerIcon item in CharacterSelect.instance.playerIcons)
        {
            Destroy(item.gameObject);
        }

        CharacterSelect.instance.playerIcons.Clear();

        timerEvent.StopAllCoroutines();
        timerEvent.timerAction.Invoke();
        GameManager.gameState = GameState.InLobby;
    }
    public TMP_Text queueUpdate;
    public void JoinRandomPvpRoom()
    {

        if (PhotonNetwork.IsConnected && !PhotonNetwork.InRoom)
        {

            Hashtable expectedCustomRoomProperties = new Hashtable { { "PVP", 1 } };

            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, (byte)maxPlayerPerPvpRoom);

            queueUpdate.text = "LOADING";

        }
        else if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            queueUpdate.text = "QUICK PLAY";
        }

    }
    public void LeaveGame()
    {
        LeaveGameWindow.ModalWindowIn();
    }
    public void LeaveGameAccepted()
    {
        if (GameManager.gameState == GameState.InGame)
        {
            MenuManager.instance.LeaveGame();
        }

    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreatePvpRoom();
    }
    public override void OnLeftRoom()
    {
        queueUpdate.text = "QUICK PLAY";
    }
    private void CreatePvpRoom()
    {
        RoomOptions roomOptions = new RoomOptions();

        roomOptions.IsOpen = true;

        roomOptions.IsVisible = true;

        roomOptions.MaxPlayers = (byte)maxPlayerPerPvpRoom;

        roomOptions.CustomRoomPropertiesForLobby = new string[1] { "PVP" };

        roomOptions.CustomRoomProperties = new Hashtable { { "PVP", 1 } };
        if ((PhotonNetwork.IsConnected && !PhotonNetwork.InRoom))
        {
            PhotonNetwork.CreateRoom(null, roomOptions, TypedLobby.Default);

            queueUpdate.text = "IN QUEUE";
        }
    }
}

