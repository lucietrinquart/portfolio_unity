using UnityEngine;
using TMPro;
using UnityEngine.Video;

public class CubeButton2 : MonoBehaviour 
{
    public GameObject mainPanel;
    public GameObject detailPanel;
    public GameObject videoPanel;
    public VideoPlayer videoPlayer;
    public TMP_Text detailText;
    
    private bool videoPlaying = false;
    public RenderTexture videoRenderTexture;
    
    void Start()
    {
        // Désactiver le videoPanel au démarrage
        if (videoPanel != null)
            videoPanel.SetActive(false);
        
        // Désactiver le detailPanel au démarrage
        if (detailPanel != null)
            detailPanel.SetActive(false);
        
        // Configuration de l'événement de fin de vidéo
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
            // Précharger la vidéo
            videoPlayer.Prepare();
        }
        if (videoPlayer != null)
    {
        videoPlayer.targetTexture = videoRenderTexture;
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.Prepare();
    }
        else
        {
            Debug.LogError("VideoPlayer n'est pas assigné dans l'inspecteur!");
        }
    }
    
    void OnMouseDown()
    {
        Debug.Log("Bouton cliqué - Lancement de la séquence vidéo");
        
        // Vérifier que tout est bien configuré
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer n'est pas assigné!");
            ShowDetailPanel();
            return;
        }
        
        // Cacher le panneau principal
        if (mainPanel != null)
            mainPanel.SetActive(false);
        
        // Activer le panneau vidéo et lancer la vidéo
        if (videoPanel != null)
        {
            videoPanel.SetActive(true);
            
            // Attendre que la vidéo soit prête avant de la jouer
            if (videoPlayer.isPrepared)
            {
                PlayVideo();
            }
            else
            {
                videoPlayer.prepareCompleted += PrepareCompleted;
                videoPlayer.Prepare();
            }
        }
        else
        {
            Debug.LogError("videoPanel n'est pas assigné!");
            ShowDetailPanel();
        }
    }
    
    private void PrepareCompleted(VideoPlayer vp)
    {
        // La vidéo est prête, on peut la jouer
        videoPlayer.prepareCompleted -= PrepareCompleted;
        PlayVideo();
    }
    
    private void PlayVideo()
    {
        videoPlayer.Play();
        videoPlaying = true;
        Debug.Log("Vidéo lancée");
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
        if (detailPanel != null)
            detailPanel.SetActive(true);
        else
            Debug.LogError("detailPanel n'est pas assigné!");
    }
    
    void OnDestroy()
    {
        // Nettoyer les événements
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
            videoPlayer.prepareCompleted -= PrepareCompleted;
        }
    }
}