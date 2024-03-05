using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player1Controller : MonoBehaviour
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

    [Header("Runtime")]
    private float energy;
    private Vector3 newVelocity;
    private bool isGrounded = false;
    private bool isJumping = false;
    private float timeSinceLastSprint = 0f;
    private float currentFOV;

    bool canMove = false;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        energy = energySlider.maxValue;
        currentFOV = normalFOV;
    }

    // Update is called once per frame
    void Update()
    {
        // Gérer la rotation horizontale
        float rotationSpeed = 100f;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up * rotation);

        // Sprint
        if (Input.GetKey(KeyCode.LeftShift) && energy > 0)
        {
            float speed = runSpeed;

            energy -= sprintEnergyCost * Time.deltaTime;

            energySlider.value = energy;
            newVelocity.z = Input.GetAxis("Vertical") * speed;

            timeSinceLastSprint = 0f;

            currentFOV = sprintFOV;
        }
        else
        {
            float speed = walkSpeed;
            newVelocity.z = Input.GetAxis("Vertical") * speed;

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

        // Jetpack
        if (Input.GetKey(KeyCode.Q) && energy > 0)
        {
            // Appliquer la force du jetpack
            rb.AddForce(Vector3.up * jetpackForce * Time.deltaTime, ForceMode.Impulse);

            // Consommer de l'énergie
            energy -= jetpackEnergyCost * Time.deltaTime;
            energySlider.value = energy;
        }

        // Jump
        if (isGrounded && Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            Debug.Log("Player is jumping");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
        }

        arrow.rotation = Quaternion.LookRotation(ball.transform.position - transform.position) * Quaternion.Euler(-90,0,0);

    }

    void FixedUpdate()
    {
        // Mettre à jour l'état au sol
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

        // Appliquer la vélocité uniquement si le joueur est sur le sol ou en train de sauter
        if (isGrounded || isJumping)
        {
            rb.velocity = transform.TransformDirection(newVelocity);
        }

        // Réinitialiser le drapeau de saut si le joueur est au sol
        if (isGrounded)
        {
            isJumping = false;
        }
    }

    void LateUpdate()
    {

    }

    // Clamp the vertical head rotation (prevent bending backwards)
    public static float RestrictAngle(float angle, float angleMin, float angleMax)
    {
        if (angle > 180)
            angle -= 360;
        else if (angle < -180)
            angle += 360;
        if (angle > angleMax)
            angle = angleMax;
        if (angle < angleMin)
            angle = angleMin;
        return angle;
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
        canMove = true;
    }
    public void CantMove()
    {
        canMove = false;
    }

    public void FillEnergy()
    {
        energy = energySlider.maxValue;
        energySlider.value = energy;
    }
}