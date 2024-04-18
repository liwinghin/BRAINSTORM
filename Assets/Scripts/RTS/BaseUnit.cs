using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    enemy,
    player
}
public class BaseUnit : MonoBehaviour
{
    [SerializeField] protected Team team;
    public void SetTeam(Team t)
    {
        team = t;
    }
}
