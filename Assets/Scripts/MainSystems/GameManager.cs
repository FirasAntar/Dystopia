using UnityEngine;
public enum GameState
{
    SpashScreen,InLobby, InQueue,InMatchFound, InCharacterSelect, InGame
}
public class GameManager : MonoBehaviour
{
    public Character[] gameCharacters;

    private static GameManager _instance;

    public static GameManager instance
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

    private static GameState _gameState;

    public static GameState gameState
    {
        get => _gameState;
        set
        {
            _gameState = value;
        }
    }

}
