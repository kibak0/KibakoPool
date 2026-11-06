using Unity.VisualScripting;
using UnityEngine;

public class DetectionCorner : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Balle"))
        {
            other.gameObject.SetActive(false);
        }
    }
}