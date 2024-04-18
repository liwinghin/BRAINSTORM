using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class EnemyPool : MonoBehaviour
{
    public Transform parent;
    public GameObject prefab;
    private ObjectPoolManager pool;
    private bool canSpawn = false;

    public static Subject<Transform> enemyPool = new Subject<Transform>();

    void Start()
    {
        pool = new ObjectPoolManager(parent, prefab);
        enemyPool.Subscribe(_obj => pool.Return(_obj)).AddTo(this);

        Observable
            .Interval(TimeSpan.FromSeconds(3))
            .Where(_=> canSpawn)
            .Subscribe(prefab =>
            {
                var newPrefab = pool.Rent();
            }).AddTo(this);

        pool.ObserveEveryValueChanged(x => x.Count)
            .Where(count => count > 10)
            .Subscribe(_ =>
            {
                pool.Shrink(0.5f, 1);
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

        this.OnDestroyAsObservable().Subscribe(_ => pool.Dispose());
    }
}
