using UnityEngine;

public class GestionnaireJeu : MonoBehaviour
{
    public static GestionnaireJeu Instance { get; private set; }

    public bool peutJouer = false;

    private void Awake()
    {
        // Verifier qu'il n'existe qu'un seul gestionnaire
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        CommencerJeu();
    }

    public void CommencerJeu()
    {
        peutJouer = true;
        VerouillerCurseur();
    }

    public void VerouillerCurseur()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void DeverouillerCurseur()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}