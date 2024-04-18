using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using Cysharp.Threading.Tasks;

public class GameFlowManager : MonoBehaviour
{
    [SerializeField] private Text countDownText;
    [SerializeField] private GameObject gameUI;
    private IDisposable subscription;

    private void Start()
    {
        GameStateManager.Instance.GameStateObservable.Subscribe(state =>
        {
            HandleGameState(state);
        }).AddTo(this);
    }

    private async void HandleGameState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Initial:
                Init();
                break;
            case GameState.GameStart:
                await StartGame();
                break;
            case GameState.GamePlaying:
                break;
            case GameState.GameOver:
                break;
        }
    }

    private void Init()
    {
        gameUI.SetActive(false);
        countDownText.text = "Press Any Key!";
        countDownText.fontSize = 144;

        subscription = Observable.EveryUpdate()
                                .Where(_ => Input.anyKeyDown)
                                .FirstOrDefault()
                                .Subscribe(_ =>
                                {
                                    GameStateManager.ChangeGameState(GameState.GameStart);
                                });
    }
    private async UniTask StartGame()
    {
        countDownText.fontSize = 288;
        int remainingTime = 5;
        while (remainingTime > 0)
        {
            countDownText.text = remainingTime.ToString();
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            remainingTime--;
        }
        countDownText.text = "Game Start!";
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        countDownText.text = "";
        gameUI.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        GameStateManager.ChangeGameState(GameState.GamePlaying);
    }

    private void OnDestroy()
    {
        subscription?.Dispose();
    }
}
