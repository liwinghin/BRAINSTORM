using UnityEngine;
using TMPro;
using UniRx;
using UnityEngine.UI;

public struct DayInfo
{
    public string date;
    public string name;
    public bool dateSelected;

    public DayInfo(string d, string n, bool ds)
    {
        date = d;
        name = n;
        dateSelected = ds;
    }
}
public class DayToggle : ToggleHandler<DayInfo>
{
    [SerializeField] private string date = "";
    [SerializeField] private string h_name = "";

    [SerializeField] private Image startImage;
    [SerializeField] private Image endImage;

    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI startText;
    [SerializeField] private TextMeshProUGUI endText;

    public bool isScheduled;

    protected override void Start()
    {
        toggle.OnValueChangedAsObservable()
            .Skip(1)
            .Subscribe(isSelected =>
            {
                DayInfo info = new DayInfo(date, h_name, isSelected);
                OnToggleSelected.OnNext(info);
            })
            .AddTo(this);
    }
    public void ResetToggle()
    {
        isScheduled = false;
        toggle.isOn = false;
        startText.text = "";
        endText.text = "";
        startImage.gameObject.SetActive(false);
        endImage.gameObject.SetActive(false);
    }
    public string GetDate
    {
        get { return date; }
    }
    public bool isSelected
    {
        get { return toggle.isOn; }
    }
    public void Setup(string d, string h)
    {
        date = d;
        h_name = h;
    }
    public void SetScheduled(string s, string e)
    {
        startText.text = s;
        endText.text = e;
        startImage.gameObject.SetActive(true);
        endImage.gameObject.SetActive(true);
        isScheduled = true;
    }
    public void SetDayText(string text)
    {
        dayText.text = text;
    }
    public void SetToggle(bool b)
    {
        toggle.isOn = b;
    }
}
