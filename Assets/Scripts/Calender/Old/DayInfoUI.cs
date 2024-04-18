using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using DanielLochner.Assets.SimpleScrollSnap;

public class DayInfoUI : MonoBehaviour
{
    [SerializeField] private ScheduleManager scheduleManager;
    [SerializeField] private Calendar calendar;

    [Header("UI - Info")]
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI hourTimeText;
    [SerializeField] private TextMeshProUGUI minutiesTimeText;
    [SerializeField] private Toggle powerSaveToggle;
    private ReactiveProperty<string> info_Date = new();
    private ReactiveProperty<string> info_Name = new();

    [Header("UI - ScrollView")]
    [SerializeField] private TimeMode timeMode = TimeMode.Start;
    [SerializeField] private ReactiveProperty<string> startTime = new();
    [SerializeField] private ReactiveProperty<string> endTime = new();
    [SerializeField] private SimpleScrollSnap h_ScrollView;
    [SerializeField] private SimpleScrollSnap m_ScrollView;

    private string getCurrentTime => $"{h_ScrollView.CenteredPanel:00}:{m_ScrollView.CenteredPanel:00}";

    [Header("UI - Common Button")]
    [SerializeField] private Button timeSettingButton;
    [SerializeField] private Button allSelectButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button resetButton;

    // Start is called before the first frame update
    void Start()
    {
        scheduleManager = GetComponent<ScheduleManager>();

        //Init GameInfo
        infoPanel.SetActive(false);
        info_Date.Subscribe(t => dateText.text = t);
        info_Name.Subscribe(t => nameText.text = t);

        //Init Common
        saveButton.OnClickAsObservable().Subscribe(t => OnSaveButtonPressed()).AddTo(this);
        backButton.OnClickAsObservable().Subscribe(t => OnBackButtonPressed()).AddTo(this);
        resetButton.OnClickAsObservable().Subscribe(t => OnResetButtonPressed()).AddTo(this);
        timeSettingButton.OnClickAsObservable().Subscribe(t => OnTimeSettingPressed()).AddTo(this);
        allSelectButton.OnClickAsObservable().Subscribe(t => OnAllSelectPressed()).AddTo(this);

        //Init ScrollView
        h_ScrollView.OnPanelCentered.AsObservable().Subscribe(_ => OnTimeValueChanged()).AddTo(this);
        m_ScrollView.OnPanelCentered.AsObservable().Subscribe(_ => OnTimeValueChanged()).AddTo(this);

        Observable.EveryLateUpdate()
                .Subscribe(_ =>
                {
                    saveButton.interactable = (!powerSaveToggle.isOn) ? true : 
                                            (startTime.Value != "" && endTime.Value != "") ? true : false;
                });

        TimeModeToggle.OnToggleSelected.Subscribe(m => timeMode = m);
    }

    public void OnDaySelected(ScheduleInfo currentInfo)
    {
        info_Date.Value = currentInfo.date;
        info_Name.Value = $"{currentInfo.name}";
        hourTimeText.text = $"時間：{currentInfo.startTime}";
        minutiesTimeText.text = $"~ {currentInfo.endTime}";
        powerSaveToggle.isOn = currentInfo.isPowerSave;
        infoPanel.SetActive(true);
    }
    public void OnPowerModeSelected(ScheduleInfo currentInfo)
    {
        startTime.Value = currentInfo.isPowerSave && timeMode == TimeMode.Start ? getCurrentTime : currentInfo.startTime;
        endTime.Value = currentInfo.isPowerSave && timeMode == TimeMode.End ? getCurrentTime : currentInfo.endTime;

        hourTimeText.text = $"時間：{startTime.Value}";
        minutiesTimeText.text = $"~  {endTime.Value}";

        scheduleManager.SetCurrentInfoTime(startTime.Value, endTime.Value);
    }
    private void OnSaveButtonPressed()
    {
        scheduleManager.OnSaveButtonPressed(startTime.Value, endTime.Value);
    }
    private void OnResetButtonPressed()
    {
        h_ScrollView.GoToPanel(0);
        m_ScrollView.GoToPanel(0);
        powerSaveToggle.isOn = false;
        startTime.Value = "";
        endTime.Value = "";
    }
    private void OnBackButtonPressed()
    {
        scheduleManager.OnBackButtonPressed();
        calendar.OnBackButtonPressed();
        info_Date.Value = info_Name.Value = hourTimeText.text = minutiesTimeText.text = "";
        powerSaveToggle.isOn = false;
        infoPanel.SetActive(false);
    }
    private void OnTimeSettingPressed()
    {
        scheduleManager.OnTimeSettingPressed();
    }
    private void OnAllSelectPressed()
    {
        calendar.OnAllSelect();
    }
    private void OnTimeValueChanged()
    {
        startTime.Value = timeMode == TimeMode.Start ? getCurrentTime : startTime.Value;
        endTime.Value = timeMode == TimeMode.End ? getCurrentTime : endTime.Value;

        if (!powerSaveToggle.isOn) { return; }
        hourTimeText.text = timeMode == TimeMode.Start ? $"時間：{getCurrentTime}" : hourTimeText.text;
        minutiesTimeText.text = timeMode == TimeMode.End ? $"~  {getCurrentTime}" : minutiesTimeText.text;
        scheduleManager.SetCurrentInfoTime(startTime.Value, endTime.Value);
    }
}
