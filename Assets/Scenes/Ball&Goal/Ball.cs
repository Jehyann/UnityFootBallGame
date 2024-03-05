using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Ball : MonoBehaviour
{
    [SerializeField] private float scale = 10;
    [SerializeField] private float additionalForce = 100;

    private Rigidbody rb;
    public GameObject Particle;

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

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Debug.Log("Collision with player");
            Debug.Log(collision.relativeVelocity.magnitude);
            
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