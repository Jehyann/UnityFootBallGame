using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Drone : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] public float flySpeed = 30;
    [SerializeField] public float chargeSpeed = 20;
    [SerializeField] public Transform ball;
    [SerializeField] private float cooldownTime = 5;

    private float timer;
    private bool chasing = false;
    private bool taking = false;

    // Start is called before the first frame update
    void Start()
    {
        float startRotation = Random.Range(0, 360);
        transform.rotation = Quaternion.Euler(0, startRotation, 0);
        timer = cooldownTime;
        speed = flySpeed;
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
        else if (taking)
        {
            TakeBall();
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
            Vector3 normale = hitInfo.normal;
            float nombreRandom = Random.Range(0, 2) == 0 ? -1 : 1;
            float rotation = Random.Range(20, 90) * nombreRandom;
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
                Debug.Log("Balle détectée");
                speed = chargeSpeed;
                chasing = true;
            }
        }
    }

    private void ChaseBall()
    {
        transform.rotation = Quaternion.LookRotation(ball.transform.position - transform.position);
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, ball.transform.position - transform.position, out hitInfo, 3))
        {
            if (hitInfo.collider.CompareTag("Ball"))
            {
                Debug.Log("Balle Attrappée");
                chasing = false;
                taking = true;
                ball.GetComponent<Ball>().followDrone = true;
                transform.rotation = Quaternion.LookRotation(new Vector3(0,13,0)-transform.position);
            }
        }
    }

    private void TakeBall()
    {
        if (transform.position.y >=13)
        {
            taking = false;
            ball.GetComponent<Ball>().followDrone = false;
            ball.GetComponent<Rigidbody>().isKinematic = false;
            Start();
        }
    }
    public void Reset()
    {
        Start();
    }
}
