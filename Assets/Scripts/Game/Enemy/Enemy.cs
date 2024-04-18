using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class Enemy : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int m_damage;

    [Header("Speed")]
    [SerializeField] private float currentSpeed;

    private Rigidbody2D rb;
    private Vector2 lastVelocity;
    private Vector2 direction;

    [Header("Color")]
    [SerializeField] Color minColor;
    [SerializeField] Color maxColor;

    private Image image;

    public int damage()
    {
        return m_damage;
    }
    private void Awake()
    {
        rb = transform.GetComponent<Rigidbody2D>();
        image = GetComponent<Image>();
    }
    private void OnEnable()
    {
        var randomDirection = Random.insideUnitCircle.normalized;
        var randomSize = Random.Range(0.5f, 4f);
        GetComponent<RectTransform>().localScale = new Vector3(randomSize, randomSize, 1);

        float speedMultiplier = 5.0f / randomSize;
        rb.velocity = randomDirection * Random.Range(1, 10) * speedMultiplier;

        m_damage = Mathf.RoundToInt(randomSize * 5);
        SetColor(randomSize);
    }
    void Start()
    {
        this.LateUpdateAsObservable()
            .Subscribe(_ => lastVelocity = rb.velocity)
            .AddTo(this);

        this.OnCollisionEnter2DAsObservable()
            .Subscribe(col =>
            {
                if (col.gameObject.CompareTag("Player"))
                {
                    CollisionManager.Instance.NotifyPlayerCollision(CollisionType.Damage, gameObject);
                    EnemyPool.enemyPool.OnNext(transform);
                }
                else
                {
                    currentSpeed = lastVelocity.magnitude;
                    direction = Vector3.Reflect(lastVelocity.normalized, col.contacts[0].normal);
                    rb.velocity = direction * Mathf.Max(currentSpeed, 0);
                    rb.velocity = currentSpeed > 30 ? Vector2.ClampMagnitude(rb.velocity, 30) : rb.velocity;
                }
            }).AddTo(this);

        GameStateManager.Instance.GameStateObservable
               .Subscribe(state =>
               {
                   if (state == GameState.GameOver)
                   {
                       EnemyPool.enemyPool.OnNext(transform);
                   }
               }).AddTo(this);
    }

    private void SetColor(float targetColor)
    {
        float t = (targetColor - 0.5f) / (4f - 0.5f);
        Color lerpedColor = Color.Lerp(minColor, maxColor, t);
        lerpedColor.a = 1;
        image.color = lerpedColor;
    }
}
