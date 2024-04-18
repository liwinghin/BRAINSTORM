using UnityEngine;
using UniRx.Toolkit;

public class ObjectPoolManager : ObjectPool<Transform>
{
    private readonly GameObject _prefab;
    private readonly Transform _parenTransform;
    public ObjectPoolManager(GameObject prefab)
    {
        _prefab = prefab;
    }
    public ObjectPoolManager(Transform parenTransform, GameObject prefab)
    {
        _parenTransform = parenTransform;
        _prefab = prefab;
    }
    protected override Transform CreateInstance()
    {
        var e = GameObject.Instantiate(_prefab, _parenTransform, false);
        return e.transform;
    }
    protected override void OnBeforeRent(Transform instance)
    {
        instance.gameObject.SetActive(true);
        instance.transform.position = new Vector3
        (
            x: Random.Range(-8.9f, 8.9f),
            y: Random.Range(-5f, 5f),
            z: 1f
        );
    }
    protected override void OnBeforeReturn(Transform instance)
    {
        instance.gameObject.SetActive(false);
    }
    protected override void OnClear(Transform instance)
    {
        if (instance == null) return;
        if (instance.gameObject == null) return;

        Object.Destroy(instance.gameObject);
    }
}