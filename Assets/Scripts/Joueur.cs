
using UnityEngine;
using UnityEngine.InputSystem;

public class Joueur : MonoBehaviour
{
    [Header("Visée")]
    [SerializeField] private float vitesseRotation = 120f;
    [SerializeField] private Vector3 decalageRotationQueue = Vector3.zero;
    [SerializeField] private float decalagePositionQueue = 0.85f;

    [Header("Puissance du tir")]
    [SerializeField] private float vitesseChargement = 8f;
    [SerializeField] private float forceMaximale = 10f;

    [Header("Physique")]
    [SerializeField] private float seuilArret = 0.01f;

    [Header("Commandes")]
    [SerializeField] private InputActionReference actionRotation;
    [SerializeField] private InputActionReference actionTir;

    [Header("References")]
    [SerializeField] private GameObject balleBlanche;
    [SerializeField] private GameObject pivotQueue;
    [SerializeField] private GameObject meshQueue;

    private Rigidbody corpsBalle;
    private float angleVisée;
    private float angleInitial;
    private float forceTir;
    private bool chargementEnCours;

    private void OnEnable()
    {
        if (actionRotation != null)
            actionRotation.action.Enable();

        if (actionTir != null)
            actionTir.action.Enable();
    }

    private void OnDisable()
    {
        if (actionRotation != null)
            actionRotation.action.Disable();

        if (actionTir != null)
            actionTir.action.Disable();
    }

    private void Awake()
    {
        angleInitial = transform.eulerAngles.y;

        if (balleBlanche != null)
            corpsBalle = balleBlanche.GetComponent<Rigidbody>();

        if (corpsBalle == null)
            Debug.LogError("Joueur : Rigidbody de la balle blanche introuvable.", this);

        if (pivotQueue == null)
            Debug.LogError("Joueur : pivot de la queue non assigné.", this);
    }

    private void Update()
    {
        if (corpsBalle == null || pivotQueue == null)
            return;

        ActualiserEtatJeu();

        if (corpsBalle.linearVelocity.sqrMagnitude > seuilArret * seuilArret)
            return;

        if (GestionnaireJeu.Instance != null &&
            !GestionnaireJeu.Instance.peutJouer)
            return;

        GererVisée();
        GererChargementTir();
        ActualiserTransformQueue();
    }

    private void ActualiserEtatJeu()
    {
        bool balleEnMouvement =
            corpsBalle.linearVelocity.sqrMagnitude >
            seuilArret * seuilArret;

        pivotQueue.SetActive(!balleEnMouvement);

        if (GestionnaireJeu.Instance != null)
            GestionnaireJeu.Instance.peutJouer = !balleEnMouvement;

        if (balleEnMouvement)
        {
            chargementEnCours = false;
            forceTir = 0f;
        }
    }

    private void GererVisée()
    {
        if (actionRotation == null)
            return;

        float directionRotation =
            actionRotation.action.ReadValue<float>();

        angleVisée = Mathf.Repeat(
            angleVisée + directionRotation * vitesseRotation * Time.deltaTime,
            360f
        );
    }

    private void GererChargementTir()
    {
        if (actionTir == null)
            return;

        if (actionTir.action.WasPressedThisFrame())
        {
            chargementEnCours = true;
            forceTir = 0f;
        }

        if (chargementEnCours && actionTir.action.IsPressed())
        {
            forceTir = Mathf.Min(
                forceTir + vitesseChargement * Time.deltaTime,
                forceMaximale
            );
        }

        if (chargementEnCours && actionTir.action.WasReleasedThisFrame())
        {
            ExecuterTir();
            chargementEnCours = false;
        }
    }

    private Vector3 CalculerDirectionTir()
    {
        float angleTotal = angleInitial + angleVisée;

        return Quaternion.AngleAxis(angleTotal, Vector3.up) * Vector3.right;
    }

    private void ExecuterTir()
    {
        Vector3 direction = CalculerDirectionTir();

        corpsBalle.AddForce(
            direction * forceTir,
            ForceMode.Impulse
        );

        forceTir = 0f;
    }

    private void ActualiserTransformQueue()
    {
        Vector3 direction = CalculerDirectionTir();

        Quaternion rotationVisée =
            Quaternion.LookRotation(direction, Vector3.up);

        pivotQueue.transform.rotation =
            rotationVisée * Quaternion.Euler(decalageRotationQueue);

        pivotQueue.transform.position =
            balleBlanche.transform.position;

        meshQueue.transform.localPosition = new Vector3(0, 0, decalagePositionQueue + (forceTir * 0.015f));

    }
}