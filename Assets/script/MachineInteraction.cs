using UnityEngine;
using TMPro;

public class MachineInteraction : MonoBehaviour
{
    [SerializeField] private float interactionRadius = 2f;
    [SerializeField] private GameObject interactionText;
    private Transform playerTransform;
    private bool isPlayerInRange = false;
    private CameraController cameraController;

     private PlayerController playerController; 

    void Start()
    {
        interactionText.SetActive(false);
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        cameraController = Camera.main.GetComponent<CameraController>();
        playerController = playerTransform.GetComponent<PlayerController>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= interactionRadius)
        {
            if (!isPlayerInRange)
            {
                isPlayerInRange = true;
                interactionText.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                InteractWithMachine();
            }
            
            if (Input.GetKeyDown(KeyCode.T))
            {
                ExitMachineView();
            }
        }
        else if (isPlayerInRange)
        {
            isPlayerInRange = false;
            interactionText.SetActive(false);
            ExitMachineView();
        }
    }

    void InteractWithMachine()
    {
        if (cameraController != null)
        {
            cameraController.SetDistributeurView(transform, true);
             // Désactivez le mouvement du joueur
            if (playerController != null)
            {
                playerController.enabled = false; // Ou utilisez une méthode dédiée
            }
        }
    }

    void ExitMachineView()
    {
        if (cameraController != null)
        {
            cameraController.SetDistributeurView(null, false);
            // Réactivez le mouvement du joueur
            if (playerController != null)
            {
                playerController.enabled = true; // Ou utilisez une méthode dédiée
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}