using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uintTester : MonoBehaviour
{
    //20035 RAM ADDR
    private const uint PART_ID_LPC18S57 = 0xF001D860;
    //20044 RAM ADDR
    private const uint PART_ID_LPC2468 = 0x1600FF35;

    // Start is called before the first frame update
    void Start()
    {

        Debug.Log(PART_ID_LPC18S57.ToString());
        Debug.Log(PART_ID_LPC2468.ToString());

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
