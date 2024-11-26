using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance;
  public GameState State { get; private set; }
  public static event Action<GameState> GameStateChanged;

  private void Awake()
  {
    // Ensure only one instance of the GameManager exists
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);  // Keeps the GameManager between scenes
    }
    else
    {
      Destroy(gameObject);
    }
  }


  public void UpdateGameState(GameState newState)
  {
    State = newState;

    switch (newState)
    {
      case GameState.Paused:
        UnityEngine.Debug.Log("-------- Paused --------");
        break;
      case GameState.Running:
        UnityEngine.Debug.Log("-------- Running --------");
        break;
      default:
        break;
    }

    GameStateChanged?.Invoke(newState);
  }
}

public enum GameState
{
  Paused,
  Running,
}