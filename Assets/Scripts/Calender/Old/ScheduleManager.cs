using UniRx;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public struct ScheduleInfo
{
    public string date;
    public string name;
    public string startTime;
    public string endTime;
    public bool isPowerSave;

    public ScheduleInfo(string d, string n, string s, string e)
    {
        date = d;
        name = n;
        startTime = s;
        endTime = e;
        isPowerSave = false;
    }
}
public class ScheduleManager : MonoBehaviour
{
    private static ScheduleManager instance;
    public static ScheduleManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ScheduleManager();
            }
            return instance;
        }
    }

    [Header ("UI")]
    [SerializeField] private DayInfoUI dayInfoUI;
    [SerializeField] private Calendar calendar;

    [SerializeField] private List<ScheduleInfo> selectingInfos = new List<ScheduleInfo>();
    [SerializeField] private ScheduleInfo currentInfo;

    public static Subject<List<ScheduleInfo>> OnSaveSchedule = new Subject<List<ScheduleInfo>>();

    private void Start()
    {
        dayInfoUI = GetComponent<DayInfoUI>();
        DayToggle.OnToggleSelected.Subscribe(DaySelected);
        PowerSaveToggle.OnToggleSelected.Subscribe(OnPowerModeSelected);
    }

    private void DaySelected(DayInfo info)
    {
        if (info.dateSelected)
        {
            currentInfo = calendar.GetCurrentInfo(info);
            selectingInfos.Add(currentInfo);
        }
        else
        {
            int index = selectingInfos.FindIndex(i => i.date == info.date);
            if(index <= -1) { return; }
            selectingInfos.RemoveAt(index);
        }
    }

    private void OnPowerModeSelected(bool isSelected)
    {
        currentInfo.isPowerSave = isSelected;
        if(!isSelected ) { currentInfo.startTime = currentInfo.endTime = ""; }
        dayInfoUI.OnPowerModeSelected(currentInfo);
    }
    public void OnTimeSettingPressed()
    {
        if(selectingInfos.Count > 0)
        dayInfoUI.OnDaySelected(currentInfo);
    }
    public void OnSaveButtonPressed(string startTime, string endTime)
    {
        currentInfo.startTime = startTime;
        currentInfo.endTime = endTime;

        var newInfoList = selectingInfos.ToList();

        newInfoList.ToList().ForEach(i => {
            int index = newInfoList.IndexOf(i);
            var newInfo = newInfoList[index];
            newInfo.startTime = currentInfo.startTime;
            newInfo.endTime = currentInfo.endTime;
            newInfo.isPowerSave = currentInfo.isPowerSave;
            newInfoList[index] = newInfo;
        });

        OnSaveSchedule.OnNext(newInfoList);
    }
    public void OnBackButtonPressed()
    {
        selectingInfos.Clear();
        currentInfo.date = currentInfo.name = currentInfo.startTime = currentInfo.endTime = string.Empty;
        currentInfo.isPowerSave = false;
    }
    public void SetCurrentInfoTime(string startTime, string endTime)
    {
        currentInfo.startTime = startTime;
        currentInfo.endTime = endTime;
    }
}
