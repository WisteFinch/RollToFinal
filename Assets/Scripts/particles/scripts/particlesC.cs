using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particlesC : MonoBehaviour
{

    public GameObject Lightning;
    public GameObject Fireworks;
    public GameObject Fireball;


    public void getFirework(Vector3 spawnPos) {
        GameObject thisone = GameObject.Instantiate(Fireworks, spawnPos, new Quaternion(0, 0, 0, 0)) as GameObject;
        ParticleSystem exp = thisone.GetComponentInChildren<ParticleSystem>() as ParticleSystem;
        exp.Play();
        Destroy(thisone, exp.main.duration);
    }

    public void getLightning(Vector3 spawnPos)
    {
        GameObject thisone = GameObject.Instantiate(Lightning, spawnPos, new Quaternion(0, 0, 0, 0)) as GameObject;
        ParticleSystem exp = thisone.GetComponentInChildren<ParticleSystem>() as ParticleSystem;
        exp.Play();
        Destroy(thisone, exp.main.duration);
    }

    public void getFireball(Vector3 spawnPos) {
        GameObject thisone = GameObject.Instantiate(Fireball, spawnPos, new Quaternion(0, 0, 0, 0)) as GameObject;
        ParticleSystem exp = thisone.GetComponentInChildren<ParticleSystem>() as ParticleSystem;
        exp.Play();
    }

}
