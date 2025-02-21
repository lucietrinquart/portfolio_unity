using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gravityStrength = 10f;
    public Transform planet;

    private Rigidbody rb;
    private Vector3 gravityUp;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        ApplyGravity();
        HandleMovement();
    }

    void ApplyGravity()
    {
        gravityUp = (transform.position - planet.position).normalized;
        Vector3 gravityForce = -gravityUp * gravityStrength;
        rb.AddForce(gravityForce, ForceMode.Acceleration);
        
        // Rotation du joueur pour qu'il soit toujours orienté par rapport à la planète
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.deltaTime);
    }

    void HandleMovement()
    {
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) // Avant
            moveDirection += transform.forward;
        if (Input.GetKey(KeyCode.S)) // Arrière
            moveDirection -= transform.forward;
        if (Input.GetKey(KeyCode.A)) // Gauche
            moveDirection -= transform.right;
        if (Input.GetKey(KeyCode.D)) // Droite
            moveDirection += transform.right;

        if (moveDirection != Vector3.zero)
        {
            Vector3 offset = Vector3.ProjectOnPlane(moveDirection, gravityUp).normalized;
            rb.MovePosition(rb.position + offset * moveSpeed * Time.fixedDeltaTime);
        }
    }
}