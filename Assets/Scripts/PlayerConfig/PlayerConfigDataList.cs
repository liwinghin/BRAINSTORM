using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfigDataList", menuName = "PlayerConfig/ConfigData")]
public class PlayerConfigDataList : ScriptableObject
{
    public List<PlayerConfigData> dataList;

    [Serializable]
    public class PlayerConfigData
    {
        public string playerConfigName;
    }
}
