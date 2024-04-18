using System;
using UnityEngine;
using UniRx;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float intensity = 3f;
    [SerializeField] private float time = 0.3f;

    private Vector3 initialPosition;
    private IDisposable shakeDisposable;

    private void Start()
    {
        CollisionManager.Instance.OnPlayerDamaged()
           .Subscribe(_ =>
           {
               ShakeCamera(time, intensity);
           })
           .AddTo(this);
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        initialPosition = transform.localPosition;
        shakeDisposable?.Dispose();

        shakeDisposable = Observable
            .Interval(TimeSpan.FromSeconds(0.02))
            .TakeWhile(_ => duration > 0)
            .Subscribe(_ =>
            {
                transform.localPosition = initialPosition + UnityEngine.Random.insideUnitSphere * magnitude;
                duration -= 0.02f;
            },
            () =>
            {
                transform.localPosition = initialPosition;
                shakeDisposable.Dispose();
            });
    }
}