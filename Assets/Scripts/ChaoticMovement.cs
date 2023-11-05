using UnityEngine;

public class ChaoticMovement : MonoBehaviour
{
    public float minForce = 200f;
    public float maxForce = 350f;
    public float changeInterval = 1.8f;
    public bool isChaoticMovementActive = true;

    private Rigidbody rb;
    private float timer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isChaoticMovementActive)
        {
            timer += Time.deltaTime;

            if (timer >= changeInterval)
            {
                ApplyRandomForce();
                timer = 0f;
            }
        }

        // Make the dog face the direction it moves
        if (rb.velocity != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(rb.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 10f); // Smoothly rotate towards the movement direction
        }
    }

    void ApplyRandomForce()
    {
        // Generate a random force in local space
        Vector3 randomForceLocal = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        float forceMagnitude = Random.Range(minForce, maxForce);

        // Convert local force to world space and apply it
        Vector3 randomForceWorld = transform.TransformDirection(randomForceLocal);
        rb.AddForce(randomForceWorld * forceMagnitude, ForceMode.Impulse);
    }
}
