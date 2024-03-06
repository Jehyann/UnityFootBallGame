using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player2Controller : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public Transform head;
    public new Camera camera;
    public Slider energySlider;
    public Transform ball;
    public Transform arrow;

    [Header("Configurations")]
    public float walkSpeed;
    public float runSpeed;
    public float jumpForce;
    public float sprintEnergyCost;
    public float jetpackEnergyCost;
    public float jetpackForce;
    public float energyRegenRate;
    public float sprintFOV = 90f;
    public float normalFOV = 60f;
    public float fovChangeSpeed = 5f;
    public float throwDistanceThreshold;

    [Header("Runtime")]
    private float energy;
    private Vector3 newVelocity;
    private bool isGrounded = false;
    private bool isJumping = false;
    private float timeSinceLastSprint = 0f;
    private float currentFOV;

    public float speed;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        energy = energySlider.maxValue;
        currentFOV = normalFOV;
    }

    void Update()
    {
        // Gérer la rotation horizontale
        float rotationSpeed = 100f;
        float rotation = Input.GetAxis("Horizontal_P2") * rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up * rotation);

        // Sprint
        if (Input.GetKey(KeyCode.H) && energy > 0)
        {
            float speed = runSpeed;

            energy -= sprintEnergyCost * Time.deltaTime;

            energySlider.value = energy;
            newVelocity.z = Input.GetAxis("Vertical_P2") * speed;

            timeSinceLastSprint = 0f;

            currentFOV = sprintFOV;
        }
        // Walk
        else
        {
            float speed = walkSpeed;
            newVelocity.z = Input.GetAxis("Vertical_P2") * speed;

            timeSinceLastSprint += Time.deltaTime;

            if (timeSinceLastSprint >= 3f && energy < energySlider.maxValue)
            {
                energy += energyRegenRate * Time.deltaTime;
                energy = Mathf.Clamp(energy, 0f, energySlider.maxValue);
                energySlider.value = energy;
            }

            currentFOV = normalFOV;
        }

        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, currentFOV, fovChangeSpeed * Time.deltaTime);

        // Lancer balle proportionnel énergie
        if (Input.GetKeyDown(KeyCode.O))
        {
            ThrowBall();
        }

        // Jetpack
        if (Input.GetKey(KeyCode.U) && energy > 0)
        {
            rb.AddForce(Vector3.up * jetpackForce * Time.deltaTime, ForceMode.Impulse);

            energy -= jetpackEnergyCost * Time.deltaTime;
            energySlider.value = energy;
        }

        // Jump
        if (isGrounded && Input.GetKeyDown(KeyCode.RightControl) && !isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
        }

        arrow.rotation = Quaternion.LookRotation(ball.transform.position - transform.position) * Quaternion.Euler(-90, 0, 0);

    }

    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.500005f))
        {
            isGrounded = true;
            rb.drag = 1.5f;
        }
        else
        {
            isGrounded = false;
            rb.drag = 0.1f;
        }

        if (isGrounded || isJumping)
        {
            rb.velocity = transform.TransformDirection(newVelocity);
        }

        if (isGrounded)
        {
            isJumping = false;
        }
    }

    public void ThrowBall()
    {
        if (Vector3.Distance(transform.position, ball.position) < throwDistanceThreshold)
        {
            if (energy > 0.25)
            {
                float throwForce = energy * 50;

                Rigidbody ballRb = ball.GetComponent<Rigidbody>();
                ballRb.velocity = camera.transform.forward * throwForce;

                energy = 0f;
                energySlider.value = energy;
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
        isJumping = false;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    public void CanMove()
    {
        rb.isKinematic = false;
    }
    public void CantMove()
    {
        rb.isKinematic = true;
    }
    public void FillEnergy()
    {
        energy = energySlider.maxValue;
        energySlider.value = energy;
    }
}