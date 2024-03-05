using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.ParticleSystem;

public class Goal : MonoBehaviour
{

    [SerializeField] private int playerID;
    [SerializeField] private Transform Ball;

    public GameObject Particle;
    public GameObject Scored;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Debug.Log("Goal");
            Ball.GetComponent<Rigidbody>().isKinematic = true;

            GameManager.GetInstance().ScorePoint(playerID);

            GameManager.GetInstance().Init();

            ActivateParticle();
            ActivateText();
        }
    }

    private void ActivateParticle()
    {
        Particle.SetActive(true);
        StartCoroutine(DeactivateParticle());
    }

    private void ActivateText()
    {
        Scored.SetActive(true);
        StartCoroutine(DeactivateText());
    }

    private IEnumerator DeactivateParticle()
    {
        yield return new WaitForSeconds(2.0f);
        Particle.SetActive(false);
    }

    private IEnumerator DeactivateText()
    {
        yield return new WaitForSeconds(3.0f);
        Scored.SetActive(false);
    }
}