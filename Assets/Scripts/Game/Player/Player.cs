using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using System;

public class Player : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10.0f;
    [SerializeField] private Color[] playerColor;

    private HPSystem playerHp;
    private Rigidbody2D m_rigidbody;
    [SerializeField] private Image playerImage;

    private Vector2 _moveDirection = Vector2.zero;
    private bool canMove = false;

    private IDisposable speedBoostDisposable;
    private IDisposable colorDisposable;

    private void Start()
    {
        playerHp = GetComponent<HPSystem>();
        playerImage = GetComponent<Image>();
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_rigidbody.gravityScale = 0;

        this.UpdateAsObservable()
            .Where(_ => canMove)
            .Subscribe(_ => _moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")))
            .AddTo(this);

        this.FixedUpdateAsObservable()
            .Where(_=> canMove)
            .Subscribe(_ => Move())
            .AddTo(this);

        GameStateManager.Instance.GameStateObservable
                       .Subscribe(state =>
                       {
                           switch (state)
                           {
                               case GameState.GamePlaying:
                                   canMove = true;
                                   break;
                               case GameState.GameOver:
                                   canMove = false;
                                   m_rigidbody.velocity = Vector2.zero;
                                   break;
                           }
                       }).AddTo(this);

        CollisionManager.Instance.OnGetItem()
          .Subscribe(info =>
          {
              var item = info.GetComponent<Items>();
              PopUp.doPopUp.OnNext(item.itemType);

              switch (item.itemType)
              {
                  case ItemType.Heal:
                      playerHp.Heal((int)item.itemValue);
                      ApplyColorEffect(playerColor[0] ,0.5f);
                      break;
                  case ItemType.Bonus:
                      Score.SetUpScore((int)item.itemValue);
                      ApplyColorEffect(playerColor[1], 0.5f);
                      break;
                  case ItemType.Speed:
                      SpeedBoosted(item.itemValue);
                      ApplyColorEffect(playerColor[2], 0.5f);
                      break;
              }
          })
          .AddTo(this);

    }

    private void Move()
    {
        m_rigidbody.velocity = _moveDirection * _moveSpeed;
    }

    private void SpeedBoosted(float speed)
    {
        _moveSpeed *= speed;
        _moveSpeed = Mathf.Clamp(_moveSpeed, 10, 40);

        speedBoostDisposable?.Dispose();

        speedBoostDisposable = Observable.Timer(TimeSpan.FromSeconds(10))
            .Subscribe(__ =>
            {
                _moveSpeed = 10f;
            })
            .AddTo(this);
    }
    private void ApplyColorEffect(Color targetColor, float duration)
    {
        targetColor.a = 1;
        playerImage.color = targetColor;

        colorDisposable?.Dispose();

        speedBoostDisposable = Observable.Timer(TimeSpan.FromSeconds(1f))
             .Subscribe(__ =>
             {
                 playerImage.color = Color.white;
             })
             .AddTo(this);
    }
}
