using UnityEngine;
using TMPro;
using UnityEngine.Video;

public class CubeButton2 : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject detailPanel;
    public GameObject videoPanel;  // Votre Quad
    public VideoPlayer videoPlayer;
    public TMP_Text detailText;
    
    private bool videoPlaying = false;
    
    void Start()
    {
        // S'assurer que le videoPanel (Quad) est désactivé au départ
        if (videoPanel != null)
            videoPanel.SetActive(false);
            
        // Configurer l'événement de fin de vidéo
        if (videoPlayer != null)
        {
            // S'assurer que le VideoPlayer a une référence au renderer du Quad
            if (videoPlayer.renderMode == VideoRenderMode.MaterialOverride && videoPlayer.targetMaterialRenderer == null)
            {
                Renderer quadRenderer = videoPanel.GetComponent<Renderer>();
                if (quadRenderer != null)
                {
                    videoPlayer.targetMaterialRenderer = quadRenderer;
                    videoPlayer.targetMaterialProperty = "_MainTex";
                }
            }
            
            videoPlayer.loopPointReached += OnVideoFinished;
        }
    }
    
    void OnMouseDown()
    {
        Debug.Log("Bouton cliqué - Lancement de la séquence vidéo");
        
        // Cacher le panneau principal
        mainPanel.SetActive(false);
        
        // Activer le panneau vidéo et lancer la vidéo
        if (videoPanel != null && videoPlayer != null)
        {
            videoPanel.SetActive(true);
            
            // Assurer que la vidéo est prête à jouer
            videoPlayer.Prepare();
            videoPlayer.Play();
            videoPlaying = true;
            
            Debug.Log("Vidéo lancée");
        }
        else
        {
            Debug.LogWarning("Configuration incomplète : videoPanel ou videoPlayer manquant");
            // Si pas de vidéo configurée, aller directement au panneau de détail
            ShowDetailPanel();
        }
    }
    
    void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Vidéo terminée");
        
        // Cette méthode est appelée quand la vidéo se termine
        if (videoPlaying)
        {
            // Désactiver le panneau vidéo
            videoPanel.SetActive(false);
            videoPlaying = false;
            
            // Afficher le panneau de détail
            ShowDetailPanel();
        }
    }
    
    void ShowDetailPanel()
    {
        // Activer le panneau de détail
        detailPanel.SetActive(true);
    }
    
    void OnDestroy()
    {
        // Nettoyer l'événement pour éviter les fuites de mémoire
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;
    }
}