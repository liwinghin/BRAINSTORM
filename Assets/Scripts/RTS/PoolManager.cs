using UnityEngine;
using UniRx.Toolkit;

namespace RTS
{
    public class PoolManager : ObjectPool<BaseUnit>
    {
        private readonly GameObject _prefab;
        private readonly Transform _parenTransform;
        public PoolManager(GameObject prefab)
        {
            _prefab = prefab;
        }
        public PoolManager(Transform parenTransform, GameObject prefab)
        {
            _parenTransform = parenTransform;
            _prefab = prefab;
        }
        protected override BaseUnit CreateInstance()
        {
            var e = GameObject.Instantiate(_prefab, _parenTransform, false);
            return e.GetComponent<BaseUnit>();
        }
        protected override void OnBeforeRent(BaseUnit instance)
        {
            instance.gameObject.SetActive(true);
        }
        protected override void OnBeforeReturn(BaseUnit instance)
        {
            instance.gameObject.SetActive(false);
        }
        protected override void OnClear(BaseUnit instance)
        {
            if (instance == null) return;
            if (instance.gameObject == null) return;

            Object.Destroy(instance.gameObject);
        }
    }
}
