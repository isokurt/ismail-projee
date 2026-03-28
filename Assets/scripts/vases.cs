using UnityEngine;

public class Vases : MonoBehaviour
{
    [SerializeField] float breakForce = 5f;
    [SerializeField] GameObject crackedVasePrefab;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > breakForce)
        {
            Instantiate(crackedVasePrefab, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
