using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;

public enum GameState
{
    Initial,
    GameStart,
    GamePlaying,
    GameOver
}
public class GameStateManager : MonoBehaviour
{
    private static GameStateManager _instance;
    public static GameStateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameStateManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("GameFlowManager");
                    _instance = singletonObject.AddComponent<GameStateManager>();
                }
            }
            return _instance;
        }
    }
    private static ReactiveProperty<GameState> gameState = new ReactiveProperty<GameState>(GameState.Initial);
    public IObservable<GameState> GameStateObservable => gameState;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }

        _instance = this;

        DontDestroyOnLoad(this);
        gameState.Value = GameState.Initial;
    }
    private void Start()
    {
        gameState.Subscribe(state =>
        {
            switch (state)
            {
                case GameState.Initial:
                    Debug.Log("init");
                    break;
                case GameState.GameStart:
                    Debug.Log("GameStart");
                    break;
                case GameState.GamePlaying:
                    Debug.Log("GamePlaying");
                    break;
                case GameState.GameOver:
                    Debug.Log("GameOver");
                    break;
            }
        }).AddTo(this);
    }
    public static void ChangeGameState(GameState newState)
    {
        gameState.Value = newState;
    }
}