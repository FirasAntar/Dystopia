using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
public class MatchMaking : MonoBehaviourPunCallbacks
{
    [SerializeField] private int playersAccepted;

    [SerializeField] private static MatchMaking _instance;

    [SerializeField] public int TeamId;
    public static MatchMaking instance
    {
        get => _instance;
        private set
        {
            if (_instance == null) _instance = value;
            else if (_instance != value)
            {
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        instance = this;
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        int playersInQueue = Launcher.instance.PlayersInQueueCount();

        int maxPlayerPerRoom = Launcher.instance.maxPlayerPerPvpRoom;

        if (playersInQueue == maxPlayerPerRoom)
        {
            MenuManager.instance.MatchFoundCheck();
        }
        else if (playersInQueue < maxPlayerPerRoom)
        {
            MenuManager.instance.PlayerInQueueUpdate(playersInQueue);
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        playersAccepted = 0;
        switch (GameManager.gameState)
        {
            case GameState.InMatchFound:

                MenuManager.instance.playerCancelQueue();
                break;

            case GameState.InQueue:

                MenuManager.instance.PlayerLeftQueueUpdate();
                break;

            case GameState.InCharacterSelect:

                MenuManager.instance.playerCancelQueue();
                break;

            case GameState.InGame:
                CharacterSelect.instance.PlayersDidntLockIn();
                break;

            default:
                break;
        }
    }
    public override void OnJoinedRoom()
    {
        PlayerManager.TeamAplayers.Clear();
        PlayerManager.TeamBplayers.Clear();
        if (PhotonNetwork.InRoom)
        {
            int playersInQueue = Launcher.instance.PlayersInQueueCount();

            int maxPlayerPerRoom = Launcher.instance.maxPlayerPerPvpRoom;

            MenuManager.instance.PvpQueueJoined(playersInQueue);

            if (playersInQueue == maxPlayerPerRoom)
            {
                MenuManager.instance.MatchFoundCheck();

                GameManager.gameState = GameState.InMatchFound;
            }
            else if (playersInQueue < maxPlayerPerRoom)
            {
                GameManager.gameState = GameState.InQueue;

                MenuManager.instance.PlayerInQueueUpdate(playersInQueue);
            }
        }
    }
    [PunRPC]
    private void PlayersAcceptedQueue()
    {
        playersAccepted++;

        if (playersAccepted == Launcher.instance.maxPlayerPerPvpRoom)
        {
            MenuManager.instance.CharacterSelectingStarted();

            playersAccepted = 0;

           

            PhotonNetwork.Instantiate("PlayerManager", Vector3.zero, Quaternion.identity);
        }
    }
}
