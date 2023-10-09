using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Random;


public class fireworkInit : MonoBehaviour
{

    void Start()
    {
        Material mats = this.GetComponent<ParticleSystemRenderer>().materials[1];
        mats.SetColor("_EmissionColor", new Color(Random.value, Random.value, Random.value));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
