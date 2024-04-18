using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.IO;

[Serializable]
public class PublicHoildayDataSet
{
    public List<PublicHolidayData> dataSet = new List<PublicHolidayData>();
}

public class Calendar : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI monthYearText;
    [SerializeField] private Transform dayGrid;
    [SerializeField] private GameObject dayPrefab;
    [SerializeField] private Button nextMonthButton;
    [SerializeField] private Button prevMonthButton;
    [SerializeField] private Color currentDayColor;
    [SerializeField] private Color holidayDayColor;

    [Header("Data")]
    [SerializeField] private List<DayToggle> dayList = new List<DayToggle>();
    [SerializeField] private List<PublicHoildayDataSet> holidaysByYearList = new List<PublicHoildayDataSet>();
    [SerializeField] private List<ScheduleInfo> scheduleList = new List<ScheduleInfo>();

    private ReactiveProperty<DateTime> currentDate = new ReactiveProperty<DateTime>();

    private void Start()
    {
        currentDate.Value = DateTime.Now;
        currentDate.Subscribe(_ => UpdateCalendar()).AddTo(this);
        nextMonthButton.OnClickAsObservable().Subscribe(_ => NextMonth()).AddTo(this);
        prevMonthButton.OnClickAsObservable().Subscribe(_ => PreviousMonth()).AddTo(this);
        ScheduleManager.OnSaveSchedule.Subscribe(OnSaveSchedule);
    }
    private void NextMonth()
    {
        currentDate.Value = currentDate.Value.AddMonths(1);
    }
    private void PreviousMonth()
    {
        currentDate.Value = currentDate.Value.AddMonths(-1);
    }
    private void GetPublicHoliday()
    {

        var data = APIHelper.getPublicHolidayData();
        var filteredData = data
       .Where(holiday =>
       {
           int year = int.Parse(holiday.date.Split('-')[0]);
           return year == 2024;
       })
       .ToList();

        foreach (PublicHolidayData holiday in filteredData)
        {
            int year = int.Parse(holiday.date.Split('-')[0]);
            var yearDataSet = holidaysByYearList.FirstOrDefault(dataSet => dataSet.dataSet[0].date.StartsWith(year.ToString()));

            if (yearDataSet != null)
            {
                yearDataSet.dataSet.Add(holiday);
            }
            else
            {
                PublicHoildayDataSet newYearData = new PublicHoildayDataSet();
                newYearData.dataSet = new List<PublicHolidayData>{holiday};
                holidaysByYearList.Add(newYearData);
            }
        }
    }
    private void UpdateCalendar()
    {
        GetPublicHoliday();
        monthYearText.text = currentDate.Value.ToString("yyyy MMMM");

        foreach (Transform child in dayGrid)
        {
            Destroy(child.gameObject);
        }

        DateTime firstDayOfMonth = new DateTime(currentDate.Value.Year, currentDate.Value.Month, 1);
        int daysInMonth = DateTime.DaysInMonth(currentDate.Value.Year, currentDate.Value.Month);
        int startDayOfWeek = (int)firstDayOfMonth.DayOfWeek;

        Vector2 dayPrefabSize = dayPrefab.GetComponent<RectTransform>().sizeDelta;
        float spacingX = dayPrefabSize.x;
        float spacingY = dayPrefabSize.y;

        dayList = new List<DayToggle>();

        for (int i = 0; i < daysInMonth; i++)
        {
            GameObject dayObject = Instantiate(dayPrefab, dayGrid);
            dayObject.name = $"Day {i + 1}";
            RectTransform rectTransform = dayObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2((i + startDayOfWeek) % 7 * spacingX, -(i + startDayOfWeek) / 7 * spacingY);
            DayToggle day = dayObject.GetComponent<DayToggle>();
            day.SetDayText((i + 1).ToString());
            DateTime date = new DateTime(currentDate.Value.Year, currentDate.Value.Month, i + 1);
            day.Setup(date.ToString("yyyy-MM-dd"), "");
            dayList.Add(day);

            if (i + 1 == DateTime.Now.Day && currentDate.Value.Month == DateTime.Now.Month && currentDate.Value.Year == DateTime.Now.Year)
            {
                dayObject.GetComponent<Image>().color = currentDayColor;
            }
        }

        if (holidaysByYearList.Count <= 0) { return; }

        dayList.ForEach(d =>
        {
            var matchingHoliday = holidaysByYearList
                   .SelectMany(h => h.dataSet)
                   .FirstOrDefault(holiday => holiday.date == d.GetDate);

            if (matchingHoliday != null)
            {
                d.Setup(matchingHoliday.date, matchingHoliday.name);
                d.GetComponent<Image>().color = holidayDayColor;
            }
        });

        if(scheduleList.Count <= 0) { return; }

        dayList
            .Where(day => scheduleList.Select(schedule => schedule.date).Contains(day.GetDate))
            .ToList()
            .ForEach(d =>
            {
                ScheduleInfo schedule = scheduleList.Find(s => s.date == d.GetDate);
                d.SetScheduled(schedule.startTime, schedule.endTime);
            });
    }
    public void OnSaveSchedule(List<ScheduleInfo> list)
    {
        var itemsToAdd = list
        .Where(si => si.isPowerSave && !scheduleList.Any(s => s.date == si.date)).ToList();

        var itemsToRemove = scheduleList
        .Where(s => list.Any(si => si.date == s.date && si.isPowerSave == false)).ToList();

        scheduleList.AddRange(itemsToAdd);
        scheduleList.RemoveAll(item => itemsToRemove.Contains(item));

        SaveScheduleToJson();
        UpdateCalendar();
    }
    public void OnAllSelect()
    {
        bool isOn = dayList.Any(d => !d.isSelected);
        dayList.ForEach (d => d.SetToggle(isOn));
    }
    public void OnBackButtonPressed()
    {
        dayList.Where(d => d.isScheduled).ToList().ForEach(t => t.ResetToggle());
        UpdateCalendar();
    }
    public ScheduleInfo GetCurrentInfo(DayInfo dayInfo)
    {      
        ScheduleInfo info = new (dayInfo.date, dayInfo.name, "", "");
        if (scheduleList.Any(i => i.date == dayInfo.date))
        {
            return scheduleList.Find(i => i.date == dayInfo.date);
        }
        else
            return info;        
    }

    public void SaveScheduleToJson()
    {
        string json = JsonUtility.ToJson(scheduleList, prettyPrint: true);
        string filePath = Application.dataPath + "/scheduleData.json";

        File.WriteAllText(filePath, json);
        Debug.Log("Schedule data saved to " + filePath);
    }
}