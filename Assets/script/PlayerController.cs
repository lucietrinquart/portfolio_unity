using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gravityStrength = 10f;
    public Transform planet;
    
    // Ajoutons un paramètre pour la rotation
    public float rotationSpeed = 10f;

    private Rigidbody rb;
    private Vector3 gravityUp;
    private Animator animator;
    private bool isMoving = false;
    private Vector3 lastMoveDirection = Vector3.zero;

    // Constantes pour les animations
    private const string ANIM_IS_WALKING = "isWalking";

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        
        // Récupérer le component Animator
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on player!");
        }
    }

    void Update()
    {
        // Gestion des animations
        CheckMovementInput();
    }

    void FixedUpdate()
    {
        ApplyGravity();
        HandleMovement();
    }

    void CheckMovementInput()
    {
        // Vérifier si le joueur appuie sur une touche de déplacement
        isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || 
                   Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
        
        // Mettre à jour les paramètres d'animation
        if (animator != null)
        {
            animator.SetBool(ANIM_IS_WALKING, isMoving);
            
            // Ajuster la vitesse globale de l'animation
            if (isMoving)
            {
                // On utilise une vitesse d'animation constante pour éviter les tremblements
                animator.speed = 1.0f;
            }
            else
            {
                animator.speed = 1.0f; // Vitesse normale pour l'animation idle
            }
        }
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

        // Lecture des entrées et création d'un vecteur de direction
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
            // Normaliser la direction pour une vitesse constante dans toutes les directions
            Vector3 offset = Vector3.ProjectOnPlane(moveDirection, gravityUp).normalized;
            
            // Déplacer le personnage
            rb.MovePosition(rb.position + offset * moveSpeed * Time.fixedDeltaTime);
            
            // Tourner le personnage vers la direction du mouvement, uniquement si on ne recule pas
            if (moveDirection.z >= 0)
            {
                // Rotation vers la direction du mouvement
                Quaternion newRotation = Quaternion.LookRotation(offset, gravityUp);
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
            }
            else
            {
                // Si on recule, on ne change pas la rotation du personnage
                // Cela évite le tremblement car le personnage continue à regarder devant lui
            }
            
            // Sauvegarder la dernière direction de mouvement
            lastMoveDirection = offset;
        }
    }
}