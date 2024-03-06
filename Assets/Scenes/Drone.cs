using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    [SerializeField] public float speed;

    // Start is called before the first frame update
    void Start()
    {
        float startRotation = Random.Range(0, 360);
        transform.rotation = Quaternion.Euler(0, startRotation, 0);
    }

    // Update is called once per frame
    void Update()
    {
        CheckCollision();
        Move();
    }
    private void Move()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void CheckCollision()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, 2, LayerMask.GetMask("Ground")))
        {
            Debug.Log("Cocorico");
            Vector3 normale = hitInfo.normal;
            Debug.Log(normale.x);
            Debug.Log(normale.z);
            float rotation = Random.Range(-60, 60);
            Debug.Log(rotation);
            transform.rotation = Quaternion.LookRotation(normale) * Quaternion.Euler(0,rotation,0);
        }
    }
}
