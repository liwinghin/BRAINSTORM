using UniRx;
using System;
using UnityEngine;

public enum CollisionType
{
    Damage,
    Item
}
public class PlayerCollisionInfo
{
    public CollisionType type { get; private set; }
    public GameObject obj { get; private set; }

    public PlayerCollisionInfo(CollisionType ctype, GameObject collidedObject)
    {
        type = ctype;
        obj = collidedObject;
    }
}
public class CollisionManager
{
    private static CollisionManager instance;
    public static CollisionManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CollisionManager();
            }
            return instance;
        }
    }

    private Subject<GameObject> playerDamagedSubject = new Subject<GameObject>();
    private Subject<GameObject> itemSubject = new Subject<GameObject>();

    public IObservable<GameObject> OnPlayerDamaged()
    {
        return playerDamagedSubject;
    }
    public IObservable<GameObject> OnGetItem()
    {
        return itemSubject;
    }
    public void NotifyPlayerCollision(CollisionType type, GameObject collidedObject)
    {
        var collisionInfo = new PlayerCollisionInfo(type, collidedObject);
        switch (type)
        {
            case CollisionType.Damage:
                playerDamagedSubject.OnNext(collidedObject);
                break;
            case CollisionType.Item:
                itemSubject.OnNext(collidedObject);
                break;
        }
    }
}
