using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysiqueBalle : MonoBehaviour
{
    [SerializeField] float linearDecel = 0.4f;
    [SerializeField] float angularDecel = 4f;
    [SerializeField] float stopSpeed = 0.03f;
    [SerializeField] float stopAngular = 0.3f;

    Rigidbody rb;
    void Awake() => rb = GetComponent<Rigidbody>();

    void FixedUpdate()
    {
        Vector3 v = rb.linearVelocity;
        Vector3 w = rb.angularVelocity;

        if (v.magnitude < stopSpeed && w.magnitude < stopAngular)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        float newSpeed = Mathf.Max(0f, v.magnitude - linearDecel * Time.fixedDeltaTime);
        rb.linearVelocity = v.normalized * newSpeed;
        rb.angularVelocity = Vector3.MoveTowards(w, Vector3.zero, angularDecel * Time.fixedDeltaTime);
    }
}