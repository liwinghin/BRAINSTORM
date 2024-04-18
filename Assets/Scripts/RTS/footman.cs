using UnityEngine;
using UniRx;

namespace RTS
{
    public class footman : BaseUnit
    {
        [SerializeField] private Vector3 initPosition;
        [SerializeField] private Transform basePosition;
        [SerializeField] private float moveSpeed;

        // Start is called before the first frame update
        void Start()
        {
            initPosition = transform.position;
            SetPlayerBasePosition();
            // Subscribe to the UniRx Update event
            Observable.EveryUpdate()
                .Subscribe(_ => MoveTowardsPlayerBase())
                .AddTo(this); // Add the disposable to the GameObject to automatically unsubscribe when it's destroyed
        }

        private void MoveTowardsPlayerBase()
        {
            if (basePosition == null) return;

            Vector3 direction = basePosition.position - transform.position;
            float distanceToPlayerBase = direction.magnitude;

            if (distanceToPlayerBase > 0.1f) // Adjust this threshold as needed
            {
                transform.Translate(direction.normalized * Time.deltaTime * moveSpeed);
            }
            else
            {
                Debug.Log("Enemy has arrived at the player base!");
                transform.position = initPosition;
                switch (team)
                {
                    case Team.enemy:
                        UnitPool.enemySubject.OnNext(this);
                        break;
                    case Team.player:
                        UnitPool.playerSubject.OnNext(this);
                        break;
                }
            }
        }
        // Find the player base position dynamically
        private void SetPlayerBasePosition()
        {
            string target = team switch
            {
                Team.player => "EnemyBase",
                Team.enemy => "PlayerBase",
                _ => throw new System.NotImplementedException()
            };

            GameObject playerBaseObject = GameObject.FindGameObjectWithTag(target);

            if (playerBaseObject != null)
            {
                basePosition = playerBaseObject.transform;
            }
        }
    }
}
