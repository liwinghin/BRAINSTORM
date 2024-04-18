using UnityEngine;
using UniRx;
using System;
using UnityEngine.UI;

[Serializable]
public enum TimeMode
{
    Start,
    End
}
public class TimeModeToggle : ToggleHandler<TimeMode>
{
    [SerializeField] private TimeMode timeMode = TimeMode.Start;

    protected override void Start()
    {
        toggle.OnValueChangedAsObservable()
            .Where(t => t)
            .Subscribe(_ => OnToggleSelected.OnNext(timeMode))
            .AddTo(this);
    }
}
