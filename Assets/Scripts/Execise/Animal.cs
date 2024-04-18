using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    protected string aname;
    protected int age;

    public Animal(string n, int a)
    {
        aname = n;
        age = a;
    }

    public virtual void Scream() { }
}
