using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleB : MonoBehaviour, IToggle
{
    // Start is called before the first frame update
    public void Start()
    {

    }

    public void ToggleOn()
    {
        print(name + ": On");
    }

    public void ToggleOff()
    {
        print(name + ": Off");
    }
    public void HandleToggle(bool isOn)
    {
        switch (isOn)
        {
            case true:
                ToggleOn(); break;
            case false:
                ToggleOff(); break;
        }
    }
}
