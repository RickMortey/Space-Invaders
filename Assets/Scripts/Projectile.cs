using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public Vector3 direction;

    public float speed;

    public System.Action Destroyed;
    
    // Update is called once per frame
    void Update()
    {
        this.transform.position += this.speed * Time.deltaTime * this.direction;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (this.Destroyed != null)
        {
            this.Destroyed.Invoke();

        }
        Destroy(this.gameObject);
    }
}
