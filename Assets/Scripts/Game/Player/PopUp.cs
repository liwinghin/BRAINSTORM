using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UniRx;
using System;

public class PopUpText
{
    public static string healText = "+30 HP!";
    public static string bounsText = "+200 PTs!";
    public static string speedText = "Speed Up!";
}
public class PopUp : MonoBehaviour 
{ 
    public static Subject<ItemType> doPopUp = new Subject<ItemType>();

    private Vector3 originalPos = Vector3.zero;
    private IDisposable popUpDisposable;

    [SerializeField] private Text popUp;

    public Dictionary<ItemType, (string text, Color color)> textMapping = new Dictionary<ItemType, (string, Color)>
    {
        { ItemType.Heal, (PopUpText.healText, new Color32(212, 255, 117, 255))},
        { ItemType.Bonus, (PopUpText.bounsText, new Color32(255, 242, 117, 255))},
        { ItemType.Speed, (PopUpText.speedText, new Color32(88, 192, 255, 255))},
    };

    // Start is called before the first frame update
    void Start()
    {
        originalPos = popUp.transform.localPosition;
        popUp.gameObject.SetActive(false);

        doPopUp.Subscribe(text =>
        {
            if (textMapping.TryGetValue(text, out var textInfo))
            {
                var (textValue, textColor) = textInfo;
                popUp.color = textColor;
                popUp.text = textValue;
                popUp.transform.localPosition = originalPos;
                popUpDisposable?.Dispose();
                popUp.gameObject.SetActive(true);
                PopUpAnimation(0.75f);
            }
        }).AddTo(this);
    }
    public void PopUpAnimation(float duration)
    {
        popUpDisposable = Observable
            .EveryUpdate()
            .TakeWhile(_ => duration > 0)
            .Subscribe(_ =>
            {
                popUp.transform.Translate(Vector3.up * Time.deltaTime * 3.0f);
                duration -= 0.02f;
            },
            () =>
            {
                popUp.gameObject.SetActive(false);
                popUp.transform.localPosition = originalPos;
                popUpDisposable.Dispose();
            });
    }
}
