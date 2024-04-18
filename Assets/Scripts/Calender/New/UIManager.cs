using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class UIManager : MonoBehaviour
{
    public Toggle toggleA;
    public Toggle toggleB;

    // Start is called before the first frame update
    void Start()
    {
        toggleA.OnValueChangedAsObservable()
            .Subscribe(on => HandleToggle(toggleA, on));

        toggleB.OnValueChangedAsObservable()
            .Subscribe(on => HandleToggle(toggleB, on));
    }
    private void HandleToggle(Toggle toggle, bool isOn)
    {
        IToggle t = toggle.GetComponent<IToggle>();
        t?.HandleToggle(isOn);
    }
}
