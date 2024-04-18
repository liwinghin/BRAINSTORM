using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections.Generic;

public class ItemsPool : MonoBehaviour
{
    public Transform parent;
    public GameObject prefab;
    private ObjectPoolManager pool;

    private bool canSpawn = false;
    private int maxCount = 3;
    public List<Transform> itemsList = new List<Transform>();

    public static Subject<Transform> itemsPool = new Subject<Transform>();

    void Start()
    {
        pool = new ObjectPoolManager(parent, prefab);
        itemsPool.Subscribe(obj =>
        {
            pool.Return(obj);
            itemsList.Remove(obj);
        }).AddTo(this);

        Observable
            .Interval(TimeSpan.FromSeconds(5))
            .Where(_ => canSpawn && itemsList.Count < maxCount)
            .Subscribe(prefab =>
            {
                var newPrefab = pool.Rent();
                itemsList.Add(newPrefab);
            }).AddTo(this);

        GameStateManager.Instance.GameStateObservable
                      .Subscribe(state =>
                      {
                          switch (state)
                          {
                              case GameState.GamePlaying:
                                  canSpawn = true;
                                  break;
                              case GameState.GameOver:
                                  canSpawn = false;
                                  break;
                          }

                      }).AddTo(this);

        this.OnDestroyAsObservable().Subscribe(_ =>
        { 
            pool.Dispose();
            itemsList.Clear();
        });
    }
}
