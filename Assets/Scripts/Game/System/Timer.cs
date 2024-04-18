using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private ReactiveProperty<int> time = new ReactiveProperty<int>();

    [SerializeField] private Text counterText;

    CompositeDisposable disposables = new CompositeDisposable();

    private void Start()
    {
        SetTimerText(0);

        GameStateManager.Instance.GameStateObservable
            .Subscribe(state =>
            {
                switch (state)
                {
                    case GameState.GamePlaying:
                        Observable
                        .Interval(TimeSpan.FromSeconds(1))
                        .Do(x => time.Value = (int)x)
                        .Where(s => s % 5 == 0)
                        .Subscribe(s =>
                        {
                            SetScore((int)s);
                        }).AddTo(disposables);

                        time.Subscribe(t =>
                        {
                            SetTimerText(t);
                        }).AddTo(disposables);

                        break;
                        case GameState.GameOver:
                        disposables.Clear();
                        disposables.Dispose();
                        break;
                }
            }).AddTo(disposables);
    }

    private void SetTimerText(int time)
    {
        counterText.text = $"Time : {time}";
    }
    private void SetScore(int t)
    {
        Score.SetUpScore(t);
    }

    private void OnDisable()
    {
        disposables.Clear();
    }
    private void OnDestroy()
    {
        disposables.Dispose();
    }
}
