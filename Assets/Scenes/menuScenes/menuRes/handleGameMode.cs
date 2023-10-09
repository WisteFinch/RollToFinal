using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handleGameMode : MonoBehaviour
{
    public GameObject startB;
    public void handler(int v)
    {
        startB.GetComponent<StartGame>().gamemode = v;
    }
}
