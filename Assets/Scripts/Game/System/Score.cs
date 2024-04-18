using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class Score : MonoBehaviour
{
    private static ReactiveProperty<int> score = new ReactiveProperty<int>();

    [SerializeField] private Text scoreText;

    private float currentScore = 0;

    private void Start()
    {
        scoreText.text = $"Score : {currentScore}";

        score.Skip(1)
            .Subscribe(score =>
            {
                var newScore = score * 15f;
                currentScore += newScore;
                scoreText.text = $"Score : {currentScore}";
            }).AddTo(this);
    }
    public static void SetUpScore(int value)
    {
        score.Value = value;
    }
}
