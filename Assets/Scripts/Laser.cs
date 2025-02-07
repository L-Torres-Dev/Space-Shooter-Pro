using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] float laserSpeed = 8;

    void Update()
    {
        transform.Translate(Vector3.up *laserSpeed * Time.deltaTime);

        if(transform.position.y > 8)
            Destroy(gameObject);
    }
}
