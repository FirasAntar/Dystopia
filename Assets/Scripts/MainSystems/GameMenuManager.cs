using UnityEngine;

public class GameMenuManager : MonoBehaviour
{
    public Animator victory;
    public Animator defeat;

    private static GameMenuManager _instance;

    public int teamA;

    public int teamB;

    bool Winning = true;

    public static GameMenuManager instance
    {
        get => _instance;
        set
        {
            if (_instance == null) _instance = value;
            else if (_instance != null) Destroy(value);
        }
    }
    private void Awake()
    {
        instance = this;
    }

    public void SetPlayers()
    {
        teamA = 0;
        teamB = 0;
        victory.Play("Window Out");
        defeat.Play("Window Out");
        if (PlayerManager.TeamAplayers.Count != 0)
            foreach (PlayerManager teamAplayers in PlayerManager.TeamAplayers.Values)
            {
                if (teamAplayers) teamA++;
            }
        if (PlayerManager.TeamBplayers.Count != 0)
            foreach (PlayerManager teamBplayers in PlayerManager.TeamBplayers.Values)
            {
                if (teamBplayers) teamB++;
            }
    }
    public void PlayerDied(PlayerManager player)
    {
        if (player.photonView.IsMine)
        {
            Winning = false;
        }
        else
        {
            Winning = true;
        }
        switch (player.playerTeam)
        {
            case PlayerManager.ArenaTeam.TeamA:
                teamA--;
                if (teamA == 0)
                {
                    Victory(Winning);
                }
                break;
            case PlayerManager.ArenaTeam.TeamB:
                teamB--;
                if (teamB == 0)
                {
                    Victory(Winning);
                }
                break;
            default:
                break;
        }
    }
    public void Victory(bool gamewon)
    {
        if (gamewon)
        {
            victory.Play("Window In");
        }
        else if (!gamewon)
        {
            defeat.Play("Window In");
        }
    }
}
