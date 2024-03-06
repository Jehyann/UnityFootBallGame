using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Drone : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] public float flySpeed;
    [SerializeField] public float chargeSpeed;
    [SerializeField] public Transform ball;
    [SerializeField] private float cooldownTime = 5;

    private float timer;
    private bool chasing = false;
    // Start is called before the first frame update
    void Start()
    {
        float startRotation = Random.Range(0, 360);
        transform.rotation = Quaternion.Euler(0, startRotation, 0);
        timer = cooldownTime;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        CheckCollision();
        Move();
        if (chasing)
        {
            ChaseBall();
        }
        else if (timer < 0)
        {
            DetectBall();
        }
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

    private void DetectBall()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, 20))
        {
            if (hitInfo.collider.CompareTag("Ball")) 
            {
                speed = chargeSpeed;
                chasing = true;
            }
        }
    }

    private void ChaseBall()
    {
        transform.rotation = Quaternion.LookRotation(ball.transform.position - transform.position);
    }
}
