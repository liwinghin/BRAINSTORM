using UniRx;

public class PowerSaveToggle : ToggleHandler<bool>
{
    protected override void Start()
    {
        toggle.OnValueChangedAsObservable()
            .Skip(1)
            .Subscribe(isSelected =>
            {
                OnToggleSelected.OnNext(isSelected);
            })
            .AddTo(this);
    }
}
