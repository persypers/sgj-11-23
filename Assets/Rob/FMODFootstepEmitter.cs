using UnityEngine;
using RigidFps;

[System.Serializable]
public class ParamRef
{
    public string paramName;
    public float paramValue;
}

[System.Serializable]
public class EmitterRef
{
    public FMODUnity.StudioEventEmitter Target;
    public ParamRef[] Params;
}

public class FMODFootstepEmitter : MonoBehaviour
{
    public EmitterRef[] footstepEmitters; // Assign StudioEventEmitters and their associated parameters in the Inspector
    private Player player; // Reference to the Player script to get grounded status

    private bool isFootstepPlaying = false;
    private bool hasRecentlyLanded = false;

    void Start()
    {
        // Get the Player script attached to the same GameObject
        player = GetComponent<Player>();
    }

    void Update()
    {
        // Check for player movement and grounded status from the Player script
        bool isMoving = (Input.GetAxis("Vertical") != 0f || Input.GetAxis("Horizontal") != 0f);
        bool isGrounded = player.IsGrounded;

        // Check for player movement and grounded status before playing footstep sounds
        if ((isMoving || hasRecentlyLanded) && isGrounded && !isFootstepPlaying)
        {
            foreach (var emitterRef in footstepEmitters)
            {
                // Start footstep sound event for each specified StudioEventEmitter
                if (emitterRef.Target != null)
                {
                    emitterRef.Target.Play();

                    // Set parameters if available
                    foreach (var param in emitterRef.Params)
                    {
                        emitterRef.Target.SetParameter(param.paramName, param.paramValue);
                    }

                    // Get the length of the footstep sound event in milliseconds
                    int length;
                    emitterRef.Target.EventDescription.getLength(out length);

                    isFootstepPlaying = true;

                    // Invoke a method to handle the end of the footstep sound and reset the flag
                    Invoke("ResetFootstepFlag", length * 0.001f); // Convert length from milliseconds to seconds
                }
            }

            // Reset the recently landed flag after triggering footsteps
            hasRecentlyLanded = false;
        }
    }

    // Method to reset the footstep playing flag after the footstep sound duration
    private void ResetFootstepFlag()
    {
        isFootstepPlaying = false;
    }

    // Call this method from the Player script to indicate a recent landing
    public void NotifyLanded()
    {
        hasRecentlyLanded = true;
        Debug.Log("Landed!");
    }
}
