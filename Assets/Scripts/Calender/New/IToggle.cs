using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IToggle
{
    void Start();
    void HandleToggle(bool isOn);
    void ToggleOn();
    void ToggleOff();
}
