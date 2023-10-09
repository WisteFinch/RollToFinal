using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class suicide : MonoBehaviour
{
    void Start()
    {
        Invoke("Isuicide",5f);
    }
    private void Isuicide() { Destroy(this.gameObject); }
}
