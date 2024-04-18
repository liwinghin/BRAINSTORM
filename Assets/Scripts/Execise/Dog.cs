using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : Animal
{
    public Dog(string name, int age) : base(name, age) { }

    // Start is called before the first frame update
    void Start()
    {
        base.Scream();
    }

    public override void Scream()
    {
        Debug.Log("woof");
    }
}
