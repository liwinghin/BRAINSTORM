using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace RTS
{
    public class UnitPool : MonoBehaviour
    {
        public Transform enemySpawnPoint;
        public GameObject enemyPrefab;
        private PoolManager enemyPool;

        public Transform playerSpawnPoint;
        public GameObject playerPrefab;
        private PoolManager playerPool;

        //[SerializeField] private bool canSpawn = false;

        public static Subject<BaseUnit> enemySubject = new Subject<BaseUnit>();
        public static Subject<BaseUnit> playerSubject = new Subject<BaseUnit>();

        void Start()
        {
            SetUpPool(enemySpawnPoint, enemyPrefab, enemyPool, enemySubject, Team.enemy);
            SetUpPool(playerSpawnPoint, playerPrefab, playerPool, playerSubject, Team.player);

            //GameStateManager.Instance.GameStateObservable
            //              .Subscribe(state =>
            //              {
            //                  switch (state)
            //                  {
            //                      case GameState.GamePlaying:
            //                          canSpawn = true;
            //                          break;
            //                      case GameState.GameOver:
            //                          canSpawn = false;
            //                          break;
            //                  }

            //              }).AddTo(this);

        }

        private void SetUpPool(Transform sp, GameObject prefab, PoolManager pool, Subject<BaseUnit> s, Team t)
        {
            pool = new PoolManager(sp, prefab);

            s.Subscribe(obj => pool.Return(obj)).AddTo(this);

            Observable
                .Interval(TimeSpan.FromSeconds(3))
                .Subscribe(prefab =>
                {
                    var newPrefab = pool.Rent();
                    newPrefab.SetTeam(t);
                }).AddTo(this);

            pool.ObserveEveryValueChanged(x => x.Count)
                .Where(count => count > 10)
                .Subscribe(_ =>
                {
                    pool.Shrink(0.5f, 1);
                }).AddTo(this);

            this.OnDestroyAsObservable().Subscribe(_ => pool.Dispose());
        }
    }
}
