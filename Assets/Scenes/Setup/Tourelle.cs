using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tourelle : MonoBehaviour
{
    [SerializeField] float cooldownTimer = 5f;
    [SerializeField] private Transform obstaclePrefab;

    private float timer = 5f;
    private Transform currentMissile;
    private bool missileMoving = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer-=Time.deltaTime;
        if (timer <= 0)
        { 
            currentMissile = CheckBall();
        }
        if (missileMoving)
        {
            MoveMissile(currentMissile);
        }
    }

    private Transform CheckBall()
    {
        RaycastHit infoHit;
        Vector3 pos = transform.position + new Vector3(0, 3, 0);

        if (Physics.Raycast(pos, transform.forward,out infoHit, 50, LayerMask.GetMask("Ball")))
        {
            if (infoHit.collider.CompareTag("Ball")) 
            {
                Debug.Log("LA BALLE");
                Transform missile = Shoot(pos);
                missileMoving = true;
                return missile;
            }
        }
        return null;
    }

    private Transform Shoot(Vector3 pos)
    {
        Transform missile = Instantiate(obstaclePrefab, pos, transform.rotation);
        timer = cooldownTimer;
        return missile;
    }
    private void MoveMissile(Transform missile)
    {
        if (missile != null)
        {
            float missileSpeed = 75.0f;
            RaycastHit infoHit;
            if (!Physics.Raycast(missile.position, missile.forward, out infoHit, 1f, LayerMask.GetMask("Ball")))
            {
                missile.Translate(Vector3.forward * missileSpeed * Time.deltaTime);
            }
            else
            {
                if (infoHit.collider.CompareTag("Ball"))
                {
                    infoHit.collider.GetComponent<Rigidbody>().AddForce(missile.forward * missileSpeed * 2, ForceMode.Impulse);
                }
                missileMoving = false;
                missile.GetComponent<Rigidbody>().isKinematic = false;
                StartCoroutine(DeleteMissile());
            }
        }
    }

    private IEnumerator DeleteMissile()
    {
        yield return new WaitForSeconds(3);
        GameObject.Destroy(currentMissile.gameObject);
    }
}
