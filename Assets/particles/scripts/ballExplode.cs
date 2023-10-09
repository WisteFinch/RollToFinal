using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballExplode : MonoBehaviour
{
    public GameObject exp;

    private void OnCollisionEnter(Collision collision)
    {

        Vector3 here = this.transform.position;
        Destroy(this.gameObject);
        GameObject.Instantiate(exp, here, new Quaternion(0, 0, 0, 0));

    }

}
