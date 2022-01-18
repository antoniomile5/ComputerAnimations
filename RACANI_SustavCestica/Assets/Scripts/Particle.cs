using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    // Start is called before the first frame update
    public float lifetime = 10f;
    public float collisionLifetime = 1.5f;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.other.gameObject.name != "Plane")
        {
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Random.ColorHSV(0f, 1f, 1f, 1f, 0.2f, 0.8f));
            Destroy(gameObject, collisionLifetime);
        }
        // it hit something: create an explosion, and remove the projectile
    }
}
