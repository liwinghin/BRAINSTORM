using UnityEngine;
using UniRx;
using UniRx.Triggers;

public enum ItemType
{
    Speed,
    Heal,
    Bonus
}
public class ItemValues
{
    public static float heal = 30;
    public static float bouns = 200;
    public static float speed = 1.25f;
}

public class Items : MonoBehaviour
{
    public ItemType itemType;
    public float itemValue;

    private void OnEnable()
    {
        itemType = (ItemType)Random.Range(0, System.Enum.GetValues(typeof(ItemType)).Length);

        itemValue = itemType switch
        {
            ItemType.Speed => ItemValues.speed,
            ItemType.Heal => ItemValues.heal,
            ItemType.Bonus => ItemValues.bouns,
            _ => 0
        };

    }
    void Start()
    {
        this.OnTriggerEnter2DAsObservable()
            .Subscribe(col =>
            {
                if (col.gameObject.CompareTag("Player"))
                {
                    CollisionManager.Instance.NotifyPlayerCollision(CollisionType.Item, gameObject);
                    ItemsPool.itemsPool.OnNext(transform);
                }
            }).AddTo(this);
    }
}
