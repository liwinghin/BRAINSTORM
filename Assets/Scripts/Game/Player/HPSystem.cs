using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class HPSystem : MonoBehaviour
{
    private ReactiveProperty<int> playerHP = new ReactiveProperty<int>();

    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider easeBar;
    private float lerpSpeed = 0.05f;

    void Start()
    {
        playerHP.Value = 100;
        hpBar.maxValue = playerHP.Value;
        easeBar.maxValue = playerHP.Value;
        easeBar.value = hpBar.value = playerHP.Value;

        CollisionManager.Instance.OnPlayerDamaged()
            .Subscribe(enemy =>
            {
                int damage = enemy.GetComponent<Enemy>().damage();
                playerHP.Value -= damage;
            })
            .AddTo(this);

        this.UpdateAsObservable()
            .Select(hp => playerHP.Value)
            .Subscribe(hp =>
            {
                hpBar.value = hp;
                easeBar.value = Mathf.Lerp(easeBar.value, hp, lerpSpeed);
            })
            .AddTo(this);

        playerHP.Where(hp => hp <= 0)
                .First()
                .Subscribe(_ =>
                {
                    GameStateManager.ChangeGameState(GameState.GameOver);
                })
                .AddTo(this);
    }

    public void Heal(int HealAmount)
    {
        playerHP.Value = Mathf.Clamp(playerHP.Value += HealAmount, 0, 100);
    }
}