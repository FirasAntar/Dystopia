using Photon.Pun;
using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviourPun, IDamagable<float>
{
    public enum ArenaTeam
    {
        TeamA, TeamB
    }

    public bool isDead = false;

    public int kills;

    public int deaths;

    public ArenaTeam playerTeam;

    public static Hashtable Players = new Hashtable();

    public static Hashtable TeamAplayers = new Hashtable();

    public static Hashtable TeamBplayers = new Hashtable();

    public PlayerCanva canva;

    public GameObject enemyTargetSprite;

    GameObject activeTargetSprite;

    public bool inStealth;

    bool autoAttackReset;

    IEnumerator AutoReset()
    {
        autoAttackReset = true;
        yield return new WaitForSeconds(1);
        autoAttackReset = false;
    }
    public string playerName
    {
        get; private set;
    }

    public int playerTeamNumber
    {
        get; private set;
    }

    public float health
    {
        get; private set;
    }

    private float reducedDamage;

    public int playerID
    {
        get; private set;
    }

    public PlayerManager target;

    [SerializeField]
    public Character character
    {
        get;
        private set;
    }
    public void Update()
    {
        if (photonView.IsMine)
        {
            PlayerInput();
        }
    }
    public void SetTarget(int id)
    {
        switch (playerTeam)
        {
            case ArenaTeam.TeamA:
                PlayerManager enemyB = (PlayerManager)TeamBplayers[(byte)id];
                target = enemyB;
                break;
            case ArenaTeam.TeamB:
                PlayerManager enemy = (PlayerManager)TeamAplayers[(byte)id];
                target = enemy;
                break;
            default:
                break;
        }
        if (!activeTargetSprite)
        {
            activeTargetSprite = Instantiate(enemyTargetSprite);
            activeTargetSprite.transform.SetParent(target.transform);
            activeTargetSprite.transform.localPosition = Vector3.zero;
        }
    }
    public void RemoveTarget()
    {
        if (target && photonView.IsMine)
        {
            Destroy(activeTargetSprite);
            MenuManager.instance.targetButtons[target.playerTeamNumber].ChangeColor(MenuManager.instance.targetButtons[target.playerTeamNumber].normalColor);
            target = null;
        }
    }
    public void PlayerInput()
    {
        if (SimpleInput.GetButtonDown("Attack") && photonView.IsMine && !isDead)
        {
            if (!autoAttackReset)
            {
                if (target)
                {
                    if (Math.distanceAB(transform.position, target.transform.position) <= character.maxRange + 0.5f)
                    {
                        Vector3 ab = Math.vectorABXZ(transform.position, target.transform.position);
                        transform.forward = ab;
                    }

                }
                GetComponent<PlayerController>().StopMovement(true);
                photonView.RPC("Attack", RpcTarget.All);
                StartCoroutine(AutoReset());
            }
        }
    }

    public void SetCharacter(Character _character)
    {
        if (character)
            Destroy(character.gameObject);

        character = _character;
        health = _character.maxHealth;
        canva.SetUp(character, gameObject.layer);
        character.transform.forward = transform.forward;
    }
    private void OnEnable()
    {
        if (photonView.IsMine)
        {
            int playerid = PhotonNetwork.LocalPlayer.ActorNumber;
            transform.position = Vector3.zero;
            playerName = PhotonNetwork.NickName;
            photonView.RPC("SetPlayerID", RpcTarget.AllViaServer, (byte)playerid, playerName);
            CharacterSelect.instance.GetComponent<PhotonView>().RPC("AddPlayerManager", RpcTarget.AllViaServer, (byte)playerid);
            StartCoroutine(TargetDetector());
            TeamAplayers.Clear();
            TeamBplayers.Clear();
        }
    }
    [PunRPC]
    private void SetPlayerTeam(byte TeamID, byte playerNumber)
    {
        if (TeamID == 0)
        {
            playerTeam = ArenaTeam.TeamA;

            Transform pos = CharacterSelect.instance.TeamA_SpawnPositions[playerNumber];

            transform.position = pos.position;

            transform.forward = pos.forward;

            if (TeamAplayers[(byte)playerID] != null)
                TeamAplayers.Remove((byte)playerID);

            TeamAplayers.Add((byte)playerID, this);
        }
        else if (TeamID == 1)
        {

            Transform pos = CharacterSelect.instance.TeamB_SpawnPositions[playerNumber];

            transform.position = pos.position;

            transform.forward = pos.forward;

            if (TeamBplayers[(byte)playerID] != null)
                TeamBplayers.Remove((byte)playerID);

            TeamBplayers.Add((byte)playerID, this);

            playerTeam = ArenaTeam.TeamB;
        }

        playerTeamNumber = playerNumber;

        CharacterSelect.instance.AddPlayerIcon((int)playerTeam, playerID, "Player" + playerID);
    }
    [PunRPC]
    public void SetPlayerID(byte id, string name)
    {
        if (Players[id] != null)
            Players.Remove(id);
        Players.Add(id, this);
        playerID = id;
        playerName = name;
    }
    [PunRPC]
    private void Attack()
    {
        character.Attack();
    }
    [PunRPC]
    private void SpellButton(byte id)
    {
        character.UseAbility(id);

    }
    [PunRPC]
    private void SpellJoystick(byte id, float x, float z)
    {
        character.UseAbility(id, x, z);
    }
    [PunRPC]
    private void HealRPC(byte healAmount)
    {
        health = health + healAmount;
        canva.SmoothSync(health);
    }
    [PunRPC]
    private void DamageRPC(byte damageTaken, float enemyID)
    {
        float x = (damageTaken - (damageTaken * reducedDamage / 100));
        health = health - x;
        if (health <= 0)
        {
            PlayerManager enemy = (PlayerManager)Players[(byte)enemyID];

            enemy.kills++;

            deaths++;

            isDead = true;

            if (photonView.IsMine)
                foreach (Spells item in character.activeSpells)
                {
                    if (item)
                        item.StopAllCoroutines();
                }

            ArenaTeam playerteam = ArenaTeam.TeamA;

            foreach (PlayerManager item in Players.Values)
            {
                if (item.photonView.IsMine)
                {
                    playerteam = item.playerTeam;
                }
            }

            if (playerteam == playerTeam)
            {
                MenuManager.instance.KilledUpdates(enemy.playerName, playerName, true);

            }
            else if (playerteam != playerTeam)
            {
                MenuManager.instance.KilledUpdates(enemy.playerName, playerName, false);
            }

            health = character.maxHealth;
            character.m_anim.SetBool("Death", true);
            health = 0;

            GameMenuManager.instance.PlayerDied(this);
        }
        canva.SpawnDamage((int)x);
        canva.SmoothSync(health);
    }
    public void AddDeathKills(int k, int d)
    {
        kills += k;
        deaths += d;
    }
    public void Damage(float damageTaken, float enemyID)
    {
        photonView.RPC("DamageRPC", RpcTarget.AllBufferedViaServer, (byte)damageTaken, enemyID);
    }
    [PunRPC]
    private void ReduceDamageBuff(byte reduceddamagepercentage, byte time)
    {
        reducedDamage = (float)reduceddamagepercentage;
        StartCoroutine(ReducedDamage((float)time));
    }
    [PunRPC]
    private void SetReducedDamage(byte percentage)
    {
        reducedDamage = percentage;
    }
    [PunRPC]
    private void Stealth()
    {
        if (inStealth)
        {
            inStealth = false;
            if (!photonView.IsMine)
            {
                character.transform.GetChild(0).gameObject.SetActive(true);

                canva.healthbar.GetComponent<CanvasGroup>().alpha = 1;
            }

        }
        else if (!inStealth)
        {
            inStealth = true;

            if (!photonView.IsMine)
            {
                character.transform.GetChild(0).gameObject.SetActive(false);

                canva.healthbar.GetComponent<CanvasGroup>().alpha = 0;
            }
            switch (playerTeam)
            {
                case ArenaTeam.TeamA:
                    foreach (PlayerManager player in TeamBplayers.Values)
                    {
                        if (player.target)

                            if (player.target.playerID == GetComponentInParent<PlayerManager>().playerID)
                            {
                                player.RemoveTarget();

                                MenuManager.instance.targetButtons[player.playerTeamNumber].Hide();

                            }
                    }
                    break;
                case ArenaTeam.TeamB:
                    foreach (PlayerManager player in TeamAplayers.Values)
                    {
                        if (player.target)

                            if (player.target.playerID == GetComponentInParent<PlayerManager>().playerID)
                            {
                                player.RemoveTarget();
                                MenuManager.instance.targetButtons[player.playerTeamNumber].Hide();
                            }
                    }
                    break;
                default:
                    break;
            }
        }
    }
    IEnumerator TargetDetector()
    {
        while (!isDead)
        {

            switch (playerTeam)
            {
                case ArenaTeam.TeamA:
                    foreach (PlayerManager item in TeamBplayers.Values)
                    {
                        if (item)
                        {
                            if (Math.distanceAB(item.transform.position, transform.position) <= 15 && !item.inStealth)
                            {
                                if (!MenuManager.instance.targetButtons[item.playerTeamNumber].active)
                                {
                                    if (!target)
                                    {
                                        target = item;
                                        activeTargetSprite = Instantiate(enemyTargetSprite);
                                        activeTargetSprite.transform.SetParent(target.transform);
                                        activeTargetSprite.transform.localPosition = Vector3.zero;
                                        MenuManager.instance.targetButtons[item.playerTeamNumber].ChangeColor(MenuManager.instance.targetButtons[item.playerTeamNumber].targetColor);
                                    }

                                    MenuManager.instance.targetButtons[item.playerTeamNumber].Show();
                                }
                            }
                            else if (Math.distanceAB(item.transform.position, transform.position) > 15)
                            {
                                if (MenuManager.instance.targetButtons[item.playerTeamNumber].active)
                                {
                                    if (item == target)
                                    {
                                        target = null;
                                        Destroy(activeTargetSprite);
                                        MenuManager.instance.targetButtons[item.playerTeamNumber].ChangeColor(MenuManager.instance.targetButtons[item.playerTeamNumber].normalColor);
                                    }
                                    MenuManager.instance.targetButtons[item.playerTeamNumber].Hide();
                                }
                            }
                        }
                    }
                    break;
                case ArenaTeam.TeamB:
                    foreach (PlayerManager item in TeamAplayers.Values)
                    {
                        if (item)
                        {

                            if (Math.distanceAB(item.transform.position, transform.position) <= 15 && !item.inStealth)
                            {
                                if (!MenuManager.instance.targetButtons[item.playerTeamNumber].active)
                                {
                                    if (!target)
                                    {
                                        target = item;
                                        activeTargetSprite = Instantiate(enemyTargetSprite);
                                        activeTargetSprite.transform.SetParent(target.transform);
                                        activeTargetSprite.transform.localPosition = Vector3.zero;
                                        MenuManager.instance.targetButtons[item.playerTeamNumber].ChangeColor(MenuManager.instance.targetButtons[item.playerTeamNumber].targetColor);
                                    }
                                    MenuManager.instance.targetButtons[item.playerTeamNumber].Show();
                                }
                            }
                            else if (Math.distanceAB(item.transform.position, transform.position) > 15)
                            {
                                if (MenuManager.instance.targetButtons[item.playerTeamNumber].active)
                                {
                                    if (item == target)
                                    {
                                        target = null;
                                        Destroy(activeTargetSprite);
                                        MenuManager.instance.targetButtons[item.playerTeamNumber].ChangeColor(MenuManager.instance.targetButtons[item.playerTeamNumber].normalColor);
                                    }
                                    MenuManager.instance.targetButtons[item.playerTeamNumber].Hide();
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(1);
        }
    }
    IEnumerator ReducedDamage(float time)
    {
        yield return new WaitForSeconds(time);
        reducedDamage = 0;
    }
    [PunRPC]
    private void Stunned(float duration)
    {
        StartCoroutine(InStun(duration));
    }
    IEnumerator InStun(float duration)
    {
        foreach (Spells spell in character.activeSpells)
        {
            spell.StopCoroutine(spell.Active());
        }
        yield return new WaitForSeconds(duration);
        foreach (Spells spell in character.activeSpells)
        {
            spell.StartCoroutine(spell.Active());
        }
    }
    public void Stun(float duration)
    {
        photonView.RPC("Stunned", RpcTarget.AllViaServer, duration);
    }

    [PunRPC]
    private void Slowed(float duration, float percentage)
    {
        StartCoroutine(InSlow(duration, percentage));
    }
    IEnumerator InSlow(float duration, float percentage)
    {
        GetComponent<PlayerController>().SlowPlayer((int)percentage);
        yield return new WaitForSeconds(duration);
        GetComponent<PlayerController>().ResetSlow();
    }
    public void Slow(float duration, float percentage)
    {
        photonView.RPC("Slowed", RpcTarget.AllViaServer, duration, percentage);
    }


}
interface IDamagable<T>
{
    public void Damage(T damageTaken, T enemyID);

    public void Stun(T duration);

    public void Slow(T duration, T percentage);


}