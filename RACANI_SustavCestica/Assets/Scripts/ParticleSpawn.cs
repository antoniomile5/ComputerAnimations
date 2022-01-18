using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawn : MonoBehaviour
{
    public GameObject startingPosition;
    //public float distanceZ;
    public Rigidbody particle;
    public float speedOffset = 0.5f;
    public float speed = 1f;
    public float positionOffset = 1.0f;
    public float rotationOffset = 5.0f;
    public int particleCount = 250;
    public float interpolationPeriodOffset = 0.01f;

    private Plane plane;
    private float time = 0.0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if(Input.GetMouseButton(0) && time >= Random.Range(interpolationPeriodOffset, interpolationPeriodOffset+1f))
        {
            time = 0.0f;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(plane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                Debug.Log(hitPoint.ToString());
                startingPosition.transform.position = hitPoint;
            }

            if (GameObject.FindGameObjectsWithTag("Particle").Length <= particleCount)
            {
                Vector3 sp = startingPosition.transform.position;
                sp.y += 2f;
                sp.x += Random.Range(-positionOffset, positionOffset);
                sp.z += Random.Range(-positionOffset, positionOffset);
                Quaternion sr = startingPosition.transform.rotation;
                sr.eulerAngles = new Vector3(Random.Range(-rotationOffset, rotationOffset), sr.eulerAngles.y, Random.Range(-rotationOffset, rotationOffset));

                Rigidbody p = Instantiate(particle, sp, sr);
                p.velocity = sr.eulerAngles.normalized * Random.Range(speed - speedOffset, speed + speedOffset);
            }
        }
    }
}
