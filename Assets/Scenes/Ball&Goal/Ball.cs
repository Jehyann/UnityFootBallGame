using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Ball : MonoBehaviour
{
    [SerializeField] private float scale = 10;
    [SerializeField] private float additionalForce = 100;
    [SerializeField] private Transform drone;

    private Rigidbody rb;
    public GameObject Particle;
    public bool followDrone = false;

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void Init()
    {
        rb.isKinematic = false;
        transform.localScale = Vector3.one * scale;
    }

    private void Update()
    {
        if (followDrone)
        {
            rb.isKinematic = true;
            float speed = drone.GetComponent<Drone>().speed;
            transform.rotation = Quaternion.LookRotation(new Vector3(0, 13, 0) - transform.position);
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Debug.Log("Collision with player");
            Debug.Log(collision.relativeVelocity.magnitude);
            followDrone = false;
            rb.isKinematic = false;
            GetComponent<Rigidbody>().AddForce((transform.position - collision.transform.position + Vector3.up * 13 * collision.gameObject.GetComponent<Rigidbody>().velocity.magnitude));

            if (collision.relativeVelocity.magnitude >= 18)
            {
            }
        }
    }

    private void ActivateParticle()
    {
        Particle.SetActive(true);
        StartCoroutine(DeactivateParticle());
    }

    private IEnumerator DeactivateParticle()
    {
        yield return new WaitForSeconds(2.0f);
        Particle.SetActive(false);
    }
}