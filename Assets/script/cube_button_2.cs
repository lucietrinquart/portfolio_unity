using UnityEngine;
using TMPro;
using UnityEngine.Video;
using UnityEngine.UI; // Ajouté pour accéder aux composants UI

public class CubeButton2 : MonoBehaviour 
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject detailPanel;
    public GameObject videoPanel;
    
    [Header("Video Settings")]
    public VideoPlayer videoPlayer;
    public RawImage videoRawImage;
    public RenderTexture videoRenderTexture;
    
    [Header("UI Elements")]
    public TMP_Text detailText;
    
    private bool videoPlaying = false;
    
    void Start()
    {
        // Désactiver le videoPanel et detailPanel au démarrage
        if (videoPanel != null)
            videoPanel.SetActive(false);
        
        if (detailPanel != null)
            detailPanel.SetActive(false);
        
        // Vérifier et configurer les éléments vidéo
        if (videoPlayer != null)
        {
            // S'assurer que la vidéo utilise le bon RenderTexture
            videoPlayer.targetTexture = videoRenderTexture;
            
            // S'abonner à l'événement de fin de vidéo
            videoPlayer.loopPointReached += OnVideoFinished;
            
            // Précharger la vidéo
            videoPlayer.Prepare();
        }
        else
        {
            Debug.LogError("VideoPlayer n'est pas assigné dans l'inspecteur!");
        }
        
        // Vérifier que le RawImage a bien le RenderTexture
        if (videoRawImage != null && videoRenderTexture != null)
        {
            videoRawImage.texture = videoRenderTexture;
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
            
            // S'assurer que la vidéo est visible en plein écran
            if (videoRawImage != null)
            {
                RectTransform rectTransform = videoRawImage.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    // Configurer le RawImage pour qu'il occupe tout l'espace disponible
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.one;
                    rectTransform.sizeDelta = Vector2.zero;
                    rectTransform.anchoredPosition = Vector2.zero;
                }
            }
            
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
        // Utiliser le thread principal pour manipuler l'UI
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            if (videoPlaying)
            {
                // Désactiver le panneau vidéo
                if (videoPanel != null)
                    videoPanel.SetActive(false);
                
                videoPlaying = false;
                
                // Afficher le panneau de détail
                ShowDetailPanel();
            }
        });
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