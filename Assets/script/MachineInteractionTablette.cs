using UnityEngine;
using TMPro;

public class MachineInteractionTablette : MonoBehaviour
{
    [SerializeField] private float interactionRadius = 2f;
    [SerializeField] private GameObject interactionText;
    
    [Header("Configuration de la machine")]
    [SerializeField] private string machineType = "Distributeur";
    
    [Header("Configuration des étapes d'interaction")]
    [SerializeField] private Transform tabletteTransform; // Référence à la tablette
    [SerializeField] private string premiereCameraType = "PremierVue"; // Premier type de vue
    [SerializeField] private string deuxiemeCameraType = "TabletteVue"; // Deuxième type de vue
    [SerializeField] private GameObject instructionPanel; // Panel contenant le texte d'instruction
    [SerializeField] private TextMeshProUGUI instructionText; // Texte d'instruction
    
    private Transform playerTransform;
    private bool isPlayerInRange = false;
    private CameraController cameraController;
    private PlayerController playerController;
    
    // État de l'interaction avec la machine
    private enum InteractionState { None, PremiereVue, InstructionView, DeuxiemeVue }
    private InteractionState currentState = InteractionState.None;
    
    void Start()
    {
        interactionText.SetActive(false);
        
        if (instructionPanel != null)
            instructionPanel.SetActive(false);
            
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        cameraController = Camera.main.GetComponent<CameraController>();
        playerController = playerTransform.GetComponent<PlayerController>();
    }
    
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        
        if (distanceToPlayer <= interactionRadius)
        {
            if (!isPlayerInRange && currentState == InteractionState.None)
            {
                isPlayerInRange = true;
                interactionText.SetActive(true);
            }
            
            // Gestion des différents états d'interaction
            if (Input.GetKeyDown(KeyCode.Y))
            {
                switch (currentState)
                {
                    case InteractionState.None:
                        // Première interaction - Vue machine (PREMIÈRE VUE)
                        PremierInteraction();
                        break;
                        
                    case InteractionState.PremiereVue:
                        // Deuxième interaction - Afficher les instructions
                        ShowInstructions();
                        break;
                        
                    case InteractionState.InstructionView:
                        // Troisième interaction - Vue tablette (DEUXIÈME VUE)
                        DeuxiemeInteraction();
                        break;
                }
            }
            
            if (Input.GetKeyDown(KeyCode.T))
            {
                ExitAllViews();
            }
        }
        else if (isPlayerInRange)
        {
            isPlayerInRange = false;
            interactionText.SetActive(false);
            ExitAllViews();
        }
    }
    
    void PremierInteraction()
    {
        if (cameraController != null)
        {
            // Utilisez le premier type de vue
            cameraController.SetMachineView(transform, premiereCameraType, true);
            
            // Désactivez le mouvement du joueur
            if (playerController != null)
            {
                playerController.enabled = false;
            }
            
            currentState = InteractionState.PremiereVue;
        }
    }
    
    void ShowInstructions()
    {
        // Afficher le panneau d'instructions
        if (instructionPanel != null && instructionText != null)
        {
            instructionPanel.SetActive(true);
            instructionText.text = "Pour commander votre plat choisissez le sur la tablette. Pour continuer appuyez sur Y.";
            
            currentState = InteractionState.InstructionView;
        }
    }
    
    void DeuxiemeInteraction()
    {
        // Masquer les instructions
        if (instructionPanel != null)
            instructionPanel.SetActive(false);
            
        // Passer à la vue tablette avec le DEUXIÈME type de vue
        if (cameraController != null && tabletteTransform != null)
        {
            cameraController.SetMachineView(tabletteTransform, deuxiemeCameraType, true);
            currentState = InteractionState.DeuxiemeVue;
        }
    }
    
    void ExitAllViews()
    {
        // Masquer les instructions
        if (instructionPanel != null)
            instructionPanel.SetActive(false);
            
        // Réinitialiser la caméra
        if (cameraController != null)
        {
            cameraController.SetMachineView(null, "", false);
            
            // Réactivez le mouvement du joueur
            if (playerController != null)
            {
                playerController.enabled = true;
            }
            
            currentState = InteractionState.None;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}