using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Paramètres de Base")]
    public float moveSpeed = 5f;
    public float gravityStrength = 10f;
    public Transform planet;
    public Transform cameraTransform; // Référence à la caméra
    
    [Header("Paramètres de Contrôle")]
    public float rotationSpeed = 15f;        // Augmenté pour une rotation plus rapide
    public float movementSmoothTime = 0.1f;  // Pour un démarrage/arrêt plus fluide
    public float jumpForce = 8f;
    public float groundCheckDistance = 0.3f;
    public LayerMask groundLayer;
    
    // Variables privées
    private Rigidbody rb;
    private Vector3 gravityUp;
    private Animator animator;
    private bool isMoving = false;
    private bool isGrounded = true;
    
    // Variables pour le lissage du mouvement
    private Vector3 currentVelocity;         // Pour SmoothDamp
    private Vector3 targetMoveDirection;     // Direction cible
    private Vector3 smoothedMoveDirection;   // Direction lissée
    private Vector3 lastMoveDirection;       // Dernière direction significative
    
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
            Debug.LogWarning("Animator manquant sur le joueur.");
        }
        
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
            Debug.Log("Référence caméra auto-assignée à la caméra principale");
        }
        
        // Initialiser les valeurs
        lastMoveDirection = transform.forward;
    }

    void Update()
    {
        // Calcul de la direction "haut" par rapport à la planète
        gravityUp = (transform.position - planet.position).normalized;
        
        // Vérification du sol
        CheckGrounded();
        
        // Gestion des entrées et animation
        CheckMovementInput();
        
        // Gestion du saut
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        ApplyGravity();
        HandleMovement();
    }

    void CheckMovementInput()
    {
        // Lecture des entrées et création d'un vecteur de direction
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
        
        // Vérifier si le joueur appuie sur une touche de déplacement
        isMoving = moveDirection.magnitude > 0.1f;
        
        // Convertir le mouvement en fonction de la caméra (style Mario Galaxy)
        if (isMoving && cameraTransform != null)
        {
            // Récupérer les axes de la caméra projetés sur le plan local
            Vector3 cameraForward = Vector3.ProjectOnPlane(cameraTransform.forward, gravityUp).normalized;
            Vector3 cameraRight = Vector3.ProjectOnPlane(cameraTransform.right, gravityUp).normalized;
            
            // Créer un vecteur de direction relatif à la caméra
            targetMoveDirection = (cameraForward * moveDirection.z + cameraRight * moveDirection.x).normalized;
        }
        else
        {
            targetMoveDirection = Vector3.zero;
        }
        
        // Lisser la direction du mouvement
        smoothedMoveDirection = Vector3.SmoothDamp(
            smoothedMoveDirection, 
            targetMoveDirection, 
            ref currentVelocity, 
            movementSmoothTime
        );
        
        // Mettre à jour l'animation
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
        
        // Rotation du personnage dans la direction du mouvement
        if (smoothedMoveDirection.magnitude > 0.1f)
        {
            // Enregistrer la dernière direction significative
            lastMoveDirection = smoothedMoveDirection;
            
            // Rotation progressive pour se tourner dans la direction du mouvement
            Quaternion targetRotation = Quaternion.LookRotation(smoothedMoveDirection, gravityUp);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void ApplyGravity()
    {
        // Application de la force de gravité vers le centre de la planète
        rb.AddForce(-gravityUp * gravityStrength, ForceMode.Acceleration);
        
        // Orientation du personnage par rapport à la surface
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.deltaTime);
    }

    void HandleMovement()
    {
        // Application du mouvement
        if (smoothedMoveDirection.magnitude > 0.01f)
        {
            rb.MovePosition(rb.position + smoothedMoveDirection * moveSpeed * Time.fixedDeltaTime);
        }
    }
    
    void CheckGrounded()
    {
        RaycastHit hit;
        
        // Lance un rayon vers le bas (en fonction de gravityUp)
        if (Physics.Raycast(transform.position, -gravityUp, out hit, groundCheckDistance, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
    
    void Jump()
    {
        // Appliquer une force dans la direction opposée à la gravité
        rb.AddForce(gravityUp * jumpForce, ForceMode.Impulse);
        
        // Mettre à jour l'état
        isGrounded = false;
    }
    
    // Fonction utile pour visualiser les rayons dans l'éditeur
    void OnDrawGizmosSelected()
    {
        if (planet != null)
        {
            // Calculer la direction "haut" quand les gizmos sont dessinés
            Vector3 up = (transform.position - planet.position).normalized;
            
            // Dessiner un rayon pour le groundCheck
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position, -up * groundCheckDistance);
            
            // Dessiner un rayon pour la direction du mouvement
            if (Application.isPlaying && smoothedMoveDirection.magnitude > 0.1f)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position, smoothedMoveDirection * 2f);
            }
        }
    }
}