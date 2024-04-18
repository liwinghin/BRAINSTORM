using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class ToggleHandler<T> : MonoBehaviour
{
    [SerializeField] protected Toggle toggle;
    [SerializeField] protected T value;

    public static Subject<T> OnToggleSelected = new Subject<T>();
    protected virtual void Awake()
    {
        toggle = GetComponent<Toggle>();
    }
    protected virtual void Start()
    {
        toggle
            .OnValueChangedAsObservable()
            .Subscribe(_ => OnToggleSelected.OnNext(value))
            .AddTo(this);
    }
}