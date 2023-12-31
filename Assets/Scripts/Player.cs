﻿using UnityEngine;
using UnityEngine.Events;
using Fancy;
using System.Net;
//using UnityEditor.ShaderGraph.Internal;

namespace RigidFps
{
	[RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody), typeof(PlayerInputHandler) )]
	public class Player : Fancy.MonoSingleton< Player >
	{
		[Header("References")] 
		public Transform rotationRoot;
		[Tooltip("Reference to the main camera used for the player")]
		public Camera PlayerCamera;

		public HandScript hand;
		public LayerMask handLayerMask;

		[Tooltip("Audio source for footsteps, jump, etc...")]
		public AudioSource AudioSource;

		[Tooltip("Height at which the player dies instantly when falling off the map")]
		public float KillHeight = -50f;

		[Header("Controls")] [Tooltip("Rotation speed for moving the camera")]
		public float RotationSpeed = 200f;
		public float handRaycastDistance = 1.0f;
		public float interactDistance = 2.0f;
		public LayerMask interactableLayerMask;

		[Header("Audio")] [Tooltip("Amount of footstep sounds played when moving one meter")]
		public float FootstepSfxFrequency = 1f;

		[Tooltip("Sound played for footsteps")]
		public AudioClip FootstepSfx;

		[Tooltip("Sound played when jumping")] public AudioClip JumpSfx;
		[Tooltip("Sound played when landing")] public AudioClip LandSfx;

		[Tooltip("Sound played when taking damage froma fall")]
		public AudioClip FallDamageSfx;

		[Header("Physics movement")]
		public float maxMovementSpeed = 10.0f;
		public float movementAccel = 20.0f;
		public float airAccel = 5.0f;
		public float jumpForce = 40.0f;

		public float minReduceSpeedDistanceX = 80.0f;
		public float maxReducedSpeedDistanceX = 95.0f;

		public float minReduceSpeedDistanceZ = 200.0f;
		public float maxReducedSpeedDistanceZ = 250.0f;

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
		public delegate void FootstepSoundEvent(Vector3 position);
		//public static event FootstepSoundEvent OnFootstepSound;
		
		PlayerInputHandler inputHandler;
		new CapsuleCollider collider;
		new Rigidbody rigidbody;
		GroundCheck groundCheck;
		Vector3 groundNormal;
		Vector3 characterVelocity;
		Vector3 latestImpactSpeed;
		//float lastTimeJumped = 0f;
		float cameraVerticalAngle = 0f;
		float handRaycastHeight;
		float footstepDistanceCounter;

		const float maxYSpeedForJump = 2.5f;
		const float k_JumpGroundingPreventionTime = 0.2f;
		const float k_GroundCheckDistanceInAir = 0.07f;

		void Start()
		{
			// fetch components on the same gameObject
			inputHandler = GetComponent<PlayerInputHandler>();
			collider = GetComponent<CapsuleCollider>();
			rigidbody = GetComponent<Rigidbody>();
			groundCheck = GetComponentInChildren< GroundCheck >();

			handRaycastHeight = hand.transform.localPosition.y;

			//PlayerCamera.transform.parent = null;
			hand.transform.parent = null;
			//hand.gameObject.SetActive( !hand.IsEmpty );
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
				// land SFX
				//AudioSource.PlayOneShot(LandSfx);
				FMODFootstepEmitter footstepEmitter = GetComponent<FMODFootstepEmitter>();
            	if (footstepEmitter != null)
            	{
                	footstepEmitter.NotifyLanded();
            	}
			}

			if( jumpTimeout > 0.0f )
				jumpTimeout -= Time.deltaTime;
			if( groundCheck.IsGrounded && inputHandler.GetJumpInputDown() && jumpTimeout <= 0.0f && rigidbody.velocity.y < maxYSpeedForJump )
				doJump = true;

			if( IsGrounded )
			{
				// footsteps sound
				float chosenFootstepSfxFrequency = FootstepSfxFrequency;
				if (footstepDistanceCounter >= 1f / chosenFootstepSfxFrequency)
				{
					footstepDistanceCounter = 0f;
					//AudioSource.PlayOneShot(FootstepSfx);
				}

				// keep track of distance traveled for footsteps sound
				footstepDistanceCounter += rigidbody.velocity.magnitude * Time.deltaTime;
			}

			HandleHand( Time.deltaTime );

			//HandleCharacterRotation( Time.deltaTime );

			// converts move input to a worldspace vector based on our character's transform orientation
			worldspaceMoveInput = rotationRoot.TransformVector(inputHandler.GetMoveInput());
		}
		
		void FixedUpdate()
		{
			HandleCharacterMovement(Time.fixedDeltaTime);
		}

		void LateUpdate()
		{
			HandleCharacterRotation( Time.deltaTime );
		}

		Vector3 worldspaceMoveInput;
		bool doJump;
		float jumpTimeout;
		Vector3 desiredVelocity;
		Vector3 horizontalAccel;
		Vector3 velDiff;
		float maxAccelMagnitude;


		public float followSmoothTime = 0.2f;
		public float cameraHeight = 1.5f;
		Vector3 cameraVelocity;
		void HandleCharacterRotation( float dt )
		{
			float bearing;
			// horizontal character rotation
			{
				// rotate the transform with the input speed around its local Y axis
				rotationRoot.Rotate(
					new Vector3(0f, inputHandler.GetLookInputsHorizontal() * RotationSpeed * RotationMultiplier, 0f)
					, Space.Self);
				bearing = rotationRoot.eulerAngles.y;
			}

			// vertical camera rotation
			{
				// add vertical inputs to the camera's vertical angle
				cameraVerticalAngle += inputHandler.GetLookInputsVertical() * RotationSpeed * RotationMultiplier;

				// limit the camera's vertical angle to min/max
				cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -89f, 89f);

				// apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
				//PlayerCamera.transform.localEulerAngles = new Vector3(cameraVerticalAngle, bearing, 0);
				PlayerCamera.transform.localEulerAngles = new Vector3(cameraVerticalAngle, 0.0f, 0);

/*
				PlayerCamera.transform.position = Vector3.SmoothDamp(
					PlayerCamera.transform.position,
					rotationRoot.position + new Vector3( 0.0f, cameraHeight, 0.0f),
					ref cameraVelocity,
					followSmoothTime,
					Mathf.Infinity,
					dt
				);
*/
			}

			if( hand != null )
			{
				hand.SetTargetRotation( new Vector3( cameraVerticalAngle, bearing, 0) );
			}
		}

		void HandleCharacterMovement( float dt )
		{
			// figure out horizontal acceleration to be applied
			var horizontalVelocity = Vector3.ProjectOnPlane( rigidbody.velocity, Vector3.up );

			desiredVelocity = worldspaceMoveInput * maxMovementSpeed;

			if( groundCheck.IsGroundedOnTrain )
				desiredVelocity += TrainScript.Instance.currentVelocity;

			var desiredVelocityHor = Vector3.ProjectOnPlane( desiredVelocity, Vector3.up );

			{
				Vector3 toTrain = TrainScript.Instance.transform.position - transform.position;
				Vector3 toTrainHor = new Vector3( Mathf.Abs( toTrain.x ), 0.0f, Mathf.Abs( toTrain.z ) );
				if( toTrainHor.x >= minReduceSpeedDistanceX || toTrainHor.z >= minReduceSpeedDistanceZ )
				{
					Vector2 factor = new Vector2(
						Mathf.Clamp01( Mathf.InverseLerp( maxReducedSpeedDistanceX, minReduceSpeedDistanceX, toTrainHor.x ) ),
						Mathf.Clamp01( Mathf.InverseLerp( maxReducedSpeedDistanceZ, minReduceSpeedDistanceZ, toTrainHor.z ) ) );
				
					if( desiredVelocityHor.x * toTrain.x < 0.0f )
						desiredVelocityHor.x *= factor.x;
					if( desiredVelocityHor.z * toTrain.z < 0.0f )
						desiredVelocityHor.z *= factor.y;
				}
			}

			velDiff = desiredVelocityHor - horizontalVelocity;

			maxAccelMagnitude = Mathf.Min( velDiff.magnitude / dt, IsGrounded ? movementAccel : airAccel );
			horizontalAccel = velDiff.normalized * maxAccelMagnitude;

			rigidbody.AddForce( horizontalAccel, ForceMode.Acceleration );

			// jumping
			if ( doJump)
			{
				doJump = false;
				jumpTimeout = k_JumpGroundingPreventionTime;
				rigidbody.AddForce( Vector3.up * jumpForce, ForceMode.VelocityChange );

				// play sound
				//AudioSource.PlayOneShot(JumpSfx);

				// Force grounding to false
				IsGrounded = false;
				groundNormal = Vector3.up;
			}
		}

		void HandleHand( float dt )
		{
			if( inputHandler.GetFireInputDown() )
			{
				if( hand.IsEmpty )
				{

					Vector3 origin = rigidbody.position + ( PlayerCamera.transform.position - transform.position );
					if( Physics.Raycast(
						origin, //transform.position + new Vector3( 0.0f, handRaycastHeight, 0.0f ),
						PlayerCamera.transform.forward, out RaycastHit hit, handRaycastDistance, handLayerMask ) )
					{
						float armDistance = Vector3.Distance( rigidbody.position + new Vector3( 0.0f, handRaycastHeight, 0.0f ), hit.rigidbody.transform.position );
						Debug.Log("PickUp hit: " + hit.rigidbody.gameObject.name );
						var item = hit.rigidbody.GetComponent< Item >();
						if( item != null )
							hand.PickUp( item, hit.rigidbody, armDistance );
					}
				} else 
				{
					var item = hand.Drop();
				}
			}

			if( inputHandler.GetReloadButtonDown() )
			{
				if( Physics.Raycast( PlayerCamera.transform.position,
					PlayerCamera.transform.forward, out RaycastHit hit, handRaycastDistance, interactableLayerMask ) )
				{
					Debug.Log("Interact hit: " + hit.collider.gameObject.name );
					var interactable = hit.collider.GetComponent< Interactable >();
					if( interactable != null )
					{
						interactable.DoAction( gameObject );
					}
				}
			}
		}

		public void Warp( Vector3 move )
		{
			//PlayerCamera.transform.position += move;
			var body = hand.GetComponent< Rigidbody >();
			body.position += move;
			body.WakeUp();
			hand.transform.position += move;
			hand.itemDummy.position += move;
			hand.itemDummy.transform.position += move;
			hand.itemDummy.WakeUp();
		}

		void OnDie()
		{
			IsDead = true;
		}
	}
}