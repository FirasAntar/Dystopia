using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    private static GameLogic _instance;

    public static GameLogic instance
    {
        get => _instance; set
        {
            if (_instance == null)
                _instance = value;
            else if (_instance != null)
                Destroy(value);
        }
    }
    private void Awake()
    {
        instance = this;
    }
}
