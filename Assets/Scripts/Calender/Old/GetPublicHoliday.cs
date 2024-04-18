using UnityEngine;
using System.Net;
using System.IO;
using System;
using System.Collections.Generic;

[Serializable]
public class PublicHolidayData
{
    public string date;
    public string name;
}

public static class APIHelper
{
    public static List<PublicHolidayData> getPublicHolidayData()
    {
        List<PublicHolidayData> holidayList = new List<PublicHolidayData>();
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.national-holidays.jp/all");
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string json = reader.ReadToEnd();
        holidayList = JsonUtility.FromJson<Wrapper>("{ \"array\": " + json + " }").array;
        return holidayList;
    }

    private class Wrapper
    {
        public List<PublicHolidayData> array;
    }
}
