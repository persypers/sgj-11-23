using UnityEngine;
using UnityEngine.Events;
using Fancy;
using System.Net;

namespace RigidFps
{
	[RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody), typeof(PlayerInputHandler) )]
	public class Player : MonoBehaviour
	{
		[Header("References")] [Tooltip("Reference to the main camera used for the player")]
		public Camera PlayerCamera;

		[Tooltip("Audio source for footsteps, jump, etc...")]
		public AudioSource AudioSource;

		[Header("General")] [Tooltip("Force applied downward when in the air")]
		public float GravityDownForce = 20f;

		[Tooltip("Physic layers checked to consider the player grounded")]
		public LayerMask GroundCheckLayers = -1;

		[Tooltip("distance from the bottom of the character controller capsule to test for grounded")]
		public float GroundCheckDistance = 0.05f;

		[Header("Movement")] [Tooltip("Max movement speed when grounded (when not sprinting)")]
		public float MaxSpeedOnGround = 10f;

		[Tooltip(
			"Sharpness for the movement when grounded, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite")]
		public float MovementSharpnessOnGround = 15;

		[Tooltip("Max movement speed when not grounded")]
		public float MaxSpeedInAir = 10f;

		[Tooltip("Acceleration speed when in the air")]
		public float AccelerationSpeedInAir = 25f;

		[Tooltip("Multiplicator for the sprint speed (based on grounded speed)")]
		public float SprintSpeedModifier = 2f;

		[Tooltip("Height at which the player dies instantly when falling off the map")]
		public float KillHeight = -50f;

		[Header("Rotation")] [Tooltip("Rotation speed for moving the camera")]
		public float RotationSpeed = 200f;

		[Range(0.1f, 1f)] [Tooltip("Rotation speed multiplier when aiming")]
		public float AimingRotationMultiplier = 0.4f;

		[Header("Jump")] [Tooltip("Force applied upward when jumping")]
		public float JumpForce = 9f;

		[Header("Stance")] [Tooltip("Ratio (0-1) of the character height where the camera will be at")]
		public float CameraHeightRatio = 0.9f;

		[Tooltip("Height of character when standing")]
		public float CapsuleHeightStanding = 1.8f;

		[Header("Audio")] [Tooltip("Amount of footstep sounds played when moving one meter")]
		public float FootstepSfxFrequency = 1f;

		[Tooltip("Amount of footstep sounds played when moving one meter while sprinting")]
		public float FootstepSfxFrequencyWhileSprinting = 1f;

		[Tooltip("Sound played for footsteps")]
		public AudioClip FootstepSfx;

		[Tooltip("Sound played when jumping")] public AudioClip JumpSfx;
		[Tooltip("Sound played when landing")] public AudioClip LandSfx;

		[Tooltip("Sound played when taking damage froma fall")]
		public AudioClip FallDamageSfx;

		[Header("Fall Damage")]
		[Tooltip("Whether the player will recieve damage when hitting the ground at high speed")]
		public bool RecievesFallDamage;

		[Tooltip("Minimun fall speed for recieving fall damage")]
		public float MinSpeedForFallDamage = 10f;

		[Tooltip("Fall speed for recieving th emaximum amount of fall damage")]
		public float MaxSpeedForFallDamage = 30f;

		[Tooltip("Damage recieved when falling at the mimimum speed")]
		public float FallDamageAtMinSpeed = 10f;

		[Tooltip("Damage recieved when falling at the maximum speed")]
		public float FallDamageAtMaxSpeed = 50f;

		[Tooltip("CharacterController.slopeLimit")]
		public float slopeLimit = 45f;
		
		[Header("Physics movement")]
		public float maxMovementSpeed = 10.0f;
		public float movementAccel = 20.0f;
		public float airAccel = 5.0f;
		public float jumpForce = 40.0f;

		public UnityAction<bool> OnStanceChanged;

		public Vector3 CharacterVelocity { get; set; }
		public bool IsGrounded { get; private set; }
		public bool HasJumpedThisFrame { get; private set; }
		public bool IsDead { get; private set; }
		public float RotationMultiplier
		{
			get
			{
				return 1f;
			}
		}

		PlayerInputHandler inputHandler;
		new CapsuleCollider collider;
		new Rigidbody rigidbody;
		GroundCheck groundCheck;
		Vector3 groundNormal;
		Vector3 characterVelocity;
		Vector3 latestImpactSpeed;
		float lastTimeJumped = 0f;
		float cameraVerticalAngle = 0f;
		float footstepDistanceCounter;

		const float k_JumpGroundingPreventionTime = 0.2f;
		const float k_GroundCheckDistanceInAir = 0.07f;

		void Awake()
		{
		}

		void Start()
		{
			// fetch components on the same gameObject
			inputHandler = GetComponent<PlayerInputHandler>();
			collider = GetComponent<CapsuleCollider>();
			rigidbody = GetComponent<Rigidbody>();
			groundCheck = GetComponentInChildren< GroundCheck >();
		}

		void Update()
		{
			// check for Y kill
			if (!IsDead && transform.position.y < KillHeight)
			{
			}

			HasJumpedThisFrame = false;

			bool wasGrounded = IsGrounded;
			IsGrounded = groundCheck.IsGrounded;
			// landing
			if (IsGrounded && !wasGrounded)
			{
				// Fall damage
				float fallSpeed = -Mathf.Min(CharacterVelocity.y, latestImpactSpeed.y);
				float fallSpeedRatio = (fallSpeed - MinSpeedForFallDamage) /
									   (MaxSpeedForFallDamage - MinSpeedForFallDamage);
				if (RecievesFallDamage && fallSpeedRatio > 0f)
				{
					float dmgFromFall = Mathf.Lerp(FallDamageAtMinSpeed, FallDamageAtMaxSpeed, fallSpeedRatio);
					//m_Health.TakeDamage(dmgFromFall, null);

					// fall damage SFX
					AudioSource.PlayOneShot(FallDamageSfx);
				}
				else
				{
					// land SFX
					AudioSource.PlayOneShot(LandSfx);
				}
			}

			HandleCharacterMovement( Time.deltaTime );
		}

		Vector3 desiredVelocity;
		Vector3 horizontalAccel;
		Vector3 velDiff;
		float maxAccelMagnitude;

		void HandleCharacterRotation( float dt )
		{
			// horizontal character rotation
			{
				// rotate the transform with the input speed around its local Y axis
				transform.Rotate(
					new Vector3(0f, (inputHandler.GetLookInputsHorizontal() * RotationSpeed * RotationMultiplier),
						0f), Space.Self);
			}

			// vertical camera rotation
			{
				// add vertical inputs to the camera's vertical angle
				cameraVerticalAngle += inputHandler.GetLookInputsVertical() * RotationSpeed * RotationMultiplier;

				// limit the camera's vertical angle to min/max
				cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -89f, 89f);

				// apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
				PlayerCamera.transform.localEulerAngles = new Vector3(cameraVerticalAngle, 0, 0);
			}
		}

		void HandleCharacterMovement( float dt )
		{
			// character movement handling
			bool isSprinting = inputHandler.GetSprintInputHeld();
			{
				float speedModifier = isSprinting ? SprintSpeedModifier : 1f;

				// converts move input to a worldspace vector based on our character's transform orientation
				Vector3 worldspaceMoveInput = transform.TransformVector(inputHandler.GetMoveInput());

				// figure out horizontal acceleration to be applied
				var velocity = rigidbody.velocity;
				var horizontalVelocity = Vector3.ProjectOnPlane( rigidbody.velocity, Vector3.up );

				desiredVelocity = worldspaceMoveInput * maxMovementSpeed;
				velDiff = desiredVelocity - horizontalVelocity;

				maxAccelMagnitude = Mathf.Min( velDiff.magnitude / dt, IsGrounded ? movementAccel : airAccel );
				horizontalAccel = velDiff.normalized * maxAccelMagnitude;

				rigidbody.AddForce( horizontalAccel, ForceMode.Acceleration );

				// jumping
				if (IsGrounded && inputHandler.GetJumpInputDown())
				{
					rigidbody.AddForce( Vector3.up * jumpForce, ForceMode.VelocityChange );

					// play sound
					AudioSource.PlayOneShot(JumpSfx);

					// remember last time we jumped because we need to prevent snapping to ground for a short time
					lastTimeJumped = Time.time;
					HasJumpedThisFrame = true;

					// Force grounding to false
					IsGrounded = false;
					groundNormal = Vector3.up;
				}

				if( IsGrounded )
				{
					// footsteps sound
					float chosenFootstepSfxFrequency =
						isSprinting ? FootstepSfxFrequencyWhileSprinting : FootstepSfxFrequency;
					if (footstepDistanceCounter >= 1f / chosenFootstepSfxFrequency)
					{
						footstepDistanceCounter = 0f;
						AudioSource.PlayOneShot(FootstepSfx);
					}

					// keep track of distance traveled for footsteps sound
					footstepDistanceCounter += CharacterVelocity.magnitude * Time.deltaTime;
				}
			}
/*
			// apply the final calculated velocity value as a character movement
			Vector3 capsuleBottomBeforeMove = GetCapsuleBottomHemisphere();
			Vector3 capsuleTopBeforeMove = GetCapsuleTopHemisphere();
			//controller.Move(CharacterVelocity * Time.deltaTime);

			// detect obstructions to adjust velocity accordingly
			latestImpactSpeed = Vector3.zero;
			if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, collider.radius,
				CharacterVelocity.normalized, out RaycastHit hit, CharacterVelocity.magnitude * Time.deltaTime, -1,
				QueryTriggerInteraction.Ignore))
			{
				// We remember the last impact speed because the fall damage logic might need it
				latestImpactSpeed = CharacterVelocity;

				CharacterVelocity = Vector3.ProjectOnPlane(CharacterVelocity, hit.normal);
			}
*/
		}

		void OnDie()
		{
			IsDead = true;
		}
	}
}