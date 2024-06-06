using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "CreateUnitData/EnemyData")]
public class EnemyDataList : ScriptableObject
{
    public List<EnemyData> dataList;
    
    [Serializable]
    public class EnemyData
    {
        public string enemyName;
        public int hp;
        public int atk;
        public int def;
    }
}