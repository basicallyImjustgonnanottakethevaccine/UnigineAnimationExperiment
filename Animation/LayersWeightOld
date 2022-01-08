using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "7d7c428d9e078288001ac44712ee34fa480d4011")]
public class LayersWeight : Component
{
	private ObjectMeshSkinned meshSkinned = null;
	private FirstPersonController Component;
	private float velocityX;
	private float velocityY;

	// Assign animations
	// Idle
	[ParameterFile(Filter = ".anim")]
	public string idle = "";
	[ParameterFile(Filter = ".anim")]
	public string crouchIdle = "";
	// Walk
	[ParameterFile(Filter = ".anim")]
	public string walkRight = "";
	[ParameterFile(Filter = ".anim")]
	public string walkFront = "";
	[ParameterFile(Filter = ".anim")]
	public string walkLeft = "";
	[ParameterFile(Filter = ".anim")]
	public string walkBack = "";
	// Run
	[ParameterFile(Filter = ".anim")]
	public string runRight = "";
	[ParameterFile(Filter = ".anim")]
	public string runFront = "";
	[ParameterFile(Filter = ".anim")]
	public string runLeft = "";
	[ParameterFile(Filter = ".anim")]
	public string runBack = "";
	// Crouch
	[ParameterFile(Filter = ".anim")]
	public string crouchRight = "";
	[ParameterFile(Filter = ".anim")]
	public string crouchFront = "";
	[ParameterFile(Filter = ".anim")]
	public string crouchLeft = "";
	[ParameterFile(Filter = ".anim")]
	public string crouchBack = "";
	// Jump
	[ParameterFile(Filter = ".anim")]
	public string jumpstart = "";
	[ParameterFile(Filter = ".anim")]
	public string jumpFall = "";
	[ParameterFile(Filter = ".anim")]
	public string jumpLand = "";
	
	// Layers
	// Idle
	private float idleWeight;
	private float crouchIdleWeight;
	// Walk
	private float walkRightWeight;
	private float walkFrontWeight;
	private float walkLeftWeight;
	private float walkBackWeight;
	// Run
	private float runRightWeight;
	private float runFrontWeight;
	private float runLeftWeight;
	private float runBackWeight;
	// Crouh
	private float crouchRightWeight;
	private float crouchFrontWeight;
	private float crouchLeftWeight;
	private float crouchBackWeight;
	// Jump
	private float jumpStartWeight;
	private float jumpFallWeight;
	private float jumpLandWeight;

	// Animation helpers
	public float transitionSpeed = 3.0f;
	private float currentTime;
	public float animationSpeed = 30.0f;
	private float timeInAir;
	public float timeInAirDeactivation = 0.15f; // past this time in air only jump animations can be played

	// Animation Layers
	private enum LAYERS
	{
		IDLE = 0,
		CROUCH_IDLE,
		WALK_RIGHT,
		WALK_FRONT,
		WALK_LEFT,
		WALK_BACK,
		RUN_RIGHT,
		RUN_FRONT,
		RUN_LEFT,
		RUN_BACK,
		CROUCH_RIGHT,
		CROUCH_FRONT,
		CROUCH_LEFT,
		CROUCH_BACK,	
		JUMP_START,
		JUMP_FALL,
		JUMP_LAND,
		COUNT
	}

	private void Init()
	{
		Unigine.Console.Run("console_onscreen 1");
		meshSkinned = node as ObjectMeshSkinned;
		if (meshSkinned == null)
			return;		
		Component = node.GetComponentInParent<FirstPersonController>();

		// Enable Layers
		// Idle
		meshSkinned.NumLayers = (int)LAYERS.COUNT;
		meshSkinned.SetAnimation((int)LAYERS.IDLE, idle);
		meshSkinned.SetLayerEnabled((int)LAYERS.IDLE, true);

		meshSkinned.SetAnimation((int)LAYERS.CROUCH_IDLE, crouchIdle);
		meshSkinned.SetLayerEnabled((int)LAYERS.CROUCH_IDLE, true);
		// Walk
		meshSkinned.SetAnimation((int)LAYERS.WALK_RIGHT, walkRight);
		meshSkinned.SetLayerEnabled((int)LAYERS.WALK_RIGHT, true);

		meshSkinned.SetAnimation((int)LAYERS.WALK_FRONT, walkFront);
		meshSkinned.SetLayerEnabled((int)LAYERS.WALK_FRONT, true);

		meshSkinned.SetAnimation((int)LAYERS.WALK_LEFT, walkLeft);
		meshSkinned.SetLayerEnabled((int)LAYERS.WALK_LEFT, true);

		meshSkinned.SetAnimation((int)LAYERS.WALK_BACK, walkBack);
		meshSkinned.SetLayerEnabled((int)LAYERS.WALK_BACK, true);	
		// Run	
		meshSkinned.SetAnimation((int)LAYERS.RUN_RIGHT, runRight);
		meshSkinned.SetLayerEnabled((int)LAYERS.RUN_RIGHT, true);

		meshSkinned.SetAnimation((int)LAYERS.RUN_FRONT, runFront);
		meshSkinned.SetLayerEnabled((int)LAYERS.RUN_FRONT, true);

		meshSkinned.SetAnimation((int)LAYERS.RUN_LEFT, runLeft);
		meshSkinned.SetLayerEnabled((int)LAYERS.RUN_LEFT, true);

		meshSkinned.SetAnimation((int)LAYERS.RUN_BACK, runBack);
		meshSkinned.SetLayerEnabled((int)LAYERS.RUN_BACK, true);
		// Crouch	
		meshSkinned.SetAnimation((int)LAYERS.CROUCH_RIGHT, crouchRight);
		meshSkinned.SetLayerEnabled((int)LAYERS.CROUCH_RIGHT, true);

		meshSkinned.SetAnimation((int)LAYERS.CROUCH_FRONT, crouchFront);
		meshSkinned.SetLayerEnabled((int)LAYERS.CROUCH_FRONT, true);

		meshSkinned.SetAnimation((int)LAYERS.CROUCH_LEFT, crouchLeft);
		meshSkinned.SetLayerEnabled((int)LAYERS.CROUCH_LEFT, true);

		meshSkinned.SetAnimation((int)LAYERS.CROUCH_BACK, crouchBack);
		meshSkinned.SetLayerEnabled((int)LAYERS.CROUCH_BACK, true);	
		// Jump
		meshSkinned.SetAnimation((int)LAYERS.JUMP_START, jumpstart);
		meshSkinned.SetLayerEnabled((int)LAYERS.JUMP_START, true);

		meshSkinned.SetAnimation((int)LAYERS.JUMP_FALL, jumpFall);
		meshSkinned.SetLayerEnabled((int)LAYERS.JUMP_FALL, true);

		meshSkinned.SetAnimation((int)LAYERS.JUMP_LAND, jumpLand);
		meshSkinned.SetLayerEnabled((int)LAYERS.JUMP_LAND, true);
	}
	
	private void Update()
	{
		vec3 result = Component.HorizontalVelocity * Component.node.GetWorldRotation(); // Local velocity
		velocityX = result.x;
		velocityY = result.y;
		Idle();
		Walk();
		Run();
		Crouch();
		Jump();
		UpdateAnimations();	
		//Log.Message("vel{0} \n", idleWeight);
	}

	private void Idle()
	{
		if ( MathLib.Abs(velocityX) < 0.1f && MathLib.Abs(velocityY) < 0.1f && !Component.IsCrouch && timeInAir < timeInAirDeactivation)
		{
			idleWeight = MathLib.Clamp(idleWeight + Game.IFps * transitionSpeed, 0.0f, 1.0f);			
		}
		else idleWeight = MathLib.Clamp(idleWeight - Game.IFps * transitionSpeed, 0.0f, 1.0f);

		if ( MathLib.Abs(velocityX) < 0.1f && MathLib.Abs(velocityY) < 0.1f && Component.IsCrouch && timeInAir < timeInAirDeactivation)
		{
			crouchIdleWeight = MathLib.Clamp(crouchIdleWeight + Game.IFps * transitionSpeed, 0.0f, 1.0f);			
		}
		else crouchIdleWeight = MathLib.Clamp(crouchIdleWeight - Game.IFps * transitionSpeed, 0.0f, 1.0f);
	}

	private void Walk()
	{
		if ( velocityX > 0.1f && !Input.IsKeyPressed(Input.KEY.SHIFT) && !Component.IsCrouch && timeInAir < timeInAirDeactivation) 
		{
			walkRightWeight = MathLib.Clamp(walkRightWeight + Game.IFps * transitionSpeed, 0.0f, 1.0f);
		}
		else walkRightWeight = MathLib.Clamp(walkRightWeight - Game.IFps * transitionSpeed, 0.0f, 1.0f);

		if ( velocityY > 0.1f && !Input.IsKeyPressed(Input.KEY.SHIFT) && !Component.IsCrouch && timeInAir < timeInAirDeactivation)
		{
			walkFrontWeight = MathLib.Clamp(walkFrontWeight + Game.IFps * transitionSpeed, 0.0f, 1.0f);
		}
		else walkFrontWeight = MathLib.Clamp(walkFrontWeight - Game.IFps * transitionSpeed, 0.0f, 1.0f);

		if ( velocityX < -0.1f && !Input.IsKeyPressed(Input.KEY.SHIFT) && !Component.IsCrouch && timeInAir < timeInAirDeactivation)
		{
			walkLeftWeight = MathLib.Clamp(walkLeftWeight + Game.IFps * transitionSpeed, 0.0f, 1.0f);
		}
		else walkLeftWeight = MathLib.Clamp(walkLeftWeight - Game.IFps * transitionSpeed, 0.0f, 1.0f);

		if ( velocityY < -0.1f && !Input.IsKeyPressed(Input.KEY.SHIFT) && !Component.IsCrouch && timeInAir < timeInAirDeactivation)
		{
			walkBackWeight = MathLib.Clamp(walkBackWeight + Game.IFps * transitionSpeed, 0.0f, 1.0f);
		}
		else walkBackWeight = MathLib.Clamp(walkBackWeight - Game.IFps * transitionSpeed, 0.0f, 1.0f);
	}

	private void Run()
	{
		if (Input.IsKeyPressed(Input.KEY.SHIFT) && velocityX > 1 && timeInAir < timeInAirDeactivation) 
		{
			runRightWeight = MathLib.Clamp(runRightWeight + Game.IFps * transitionSpeed, 0.0f, 1.0f);
		}
		else runRightWeight = MathLib.Clamp(runRightWeight - Game.IFps * transitionSpeed, 0.0f, 1.0f);

		if (Input.IsKeyPressed(Input.KEY.SHIFT) && velocityY > 1 && timeInAir < timeInAirDeactivation)
		{
			runFrontWeight = MathLib.Clamp(runFrontWeight + Game.IFps * transitionSpeed, 0.0f, 1.0f);
		}
		else runFrontWeight = MathLib.Clamp(runFrontWeight - Game.IFps * transitionSpeed, 0.0f, 1.0f);

		if (Input.IsKeyPressed(Input.KEY.SHIFT) && velocityX < -1 && timeInAir < timeInAirDeactivation)
		{
			runLeftWeight = MathLib.Clamp(runLeftWeight + Game.IFps * transitionSpeed, 0.0f, 1.0f);
		}
		else runLeftWeight = MathLib.Clamp(runLeftWeight - Game.IFps * transitionSpeed, 0.0f, 1.0f);

		if (Input.IsKeyPressed(Input.KEY.SHIFT) && velocityY < -1 && timeInAir < timeInAirDeactivation)
		{
			runBackWeight = MathLib.Clamp(runBackWeight + Game.IFps * transitionSpeed, 0.0f, 1.0f);
		}
		else runBackWeight = MathLib.Clamp(runBackWeight - Game.IFps * transitionSpeed, 0.0f, 1.0f);
	}

	private void Crouch()
	{
		if (Component.IsCrouch && velocityX > 0.1f) 
		{
			crouchRightWeight = MathLib.Clamp(crouchRightWeight + Game.IFps * transitionSpeed, 0.0f, 1.0f);
		}
		else crouchRightWeight = MathLib.Clamp(crouchRightWeight - Game.IFps * transitionSpeed, 0.0f, 1.0f);

		if (Component.IsCrouch && velocityY > 0.1f)
		{
			crouchFrontWeight = MathLib.Clamp(crouchFrontWeight + Game.IFps * transitionSpeed, 0.0f, 1.0f);
		}
		else crouchFrontWeight = MathLib.Clamp(crouchFrontWeight - Game.IFps * transitionSpeed, 0.0f, 1.0f);

		if (Component.IsCrouch && velocityX < -0.1f)
		{
			crouchLeftWeight = MathLib.Clamp(crouchLeftWeight + Game.IFps * transitionSpeed, 0.0f, 1.0f);
		}
		else crouchLeftWeight = MathLib.Clamp(crouchLeftWeight - Game.IFps * transitionSpeed, 0.0f, 1.0f);

		if (Component.IsCrouch && velocityY < -0.1f)
		{
			crouchBackWeight = MathLib.Clamp(crouchBackWeight + Game.IFps * transitionSpeed, 0.0f, 1.0f);
		}
		else crouchBackWeight = MathLib.Clamp(crouchBackWeight - Game.IFps * transitionSpeed, 0.0f, 1.0f);
	}

	private void Jump()
	{
		if (!Component.IsGround)
		{
			timeInAir = timeInAir + Game.IFps; // Use timeInAir because Component.IsGround isn't consistent, need a little buffer time ,like 1/10 sec
			if (timeInAir > 0.2f)
			{
				jumpStartWeight = 1.0f;
				//jumpStartWeight = MathLib.Clamp(jumpStartWeight + Game.IFps * transitionSpeed, 0.0f, 1.0f); // mean player just started jumping
			} 
			else jumpStartWeight = MathLib.Clamp(jumpStartWeight - Game.IFps * transitionSpeed, 0.0f, 1.0f);

			if (timeInAir > 1f) // Start falling animation after 1
			{
				jumpFallWeight = MathLib.Clamp(jumpFallWeight + Game.IFps * transitionSpeed, 0.0f, 1.0f); // mean player just started jumping
			} 
		}
		else
		{
			//float jumpLandTime; 
			if ( timeInAir > 0.25 ) // If player has spend some time in air
			{
				currentTime = 0.0f; // Not a loop animation, so make animation start to frame 0.
				jumpLandWeight = 1.0f;
			}
			//if (meshSkinned.GetNumFrames)
			timeInAir = 0.0f;
			jumpStartWeight = MathLib.Clamp(jumpStartWeight - Game.IFps * transitionSpeed, 0.0f, 1.0f);
			jumpFallWeight = MathLib.Clamp(jumpFallWeight - Game.IFps * transitionSpeed, 0.0f, 1.0f);
			// stop landing animation at the end of it
			if (meshSkinned.GetFrame((int)LAYERS.JUMP_LAND) > meshSkinned.GetNumFrames((int)LAYERS.JUMP_LAND) - 15 ) // fade out landing animation 10 frame before it end
			{
				jumpLandWeight = MathLib.Clamp(jumpLandWeight - Game.IFps * transitionSpeed, 0.0f, 1.0f);
			} 
			Log.Message("{0} \n", meshSkinned.GetFrame((int)LAYERS.JUMP_LAND));
			

		}
		
	}

	private void UpdateAnimations()
	{
		// Set weight //
		meshSkinned.SetLayerWeight((int)LAYERS.IDLE, idleWeight);
		meshSkinned.SetLayerWeight((int)LAYERS.CROUCH_IDLE, crouchIdleWeight);
		// Walk
		meshSkinned.SetLayerWeight((int)LAYERS.WALK_RIGHT, walkRightWeight);
		meshSkinned.SetLayerWeight((int)LAYERS.WALK_FRONT, walkFrontWeight);		
		meshSkinned.SetLayerWeight((int)LAYERS.WALK_LEFT, walkLeftWeight);
		meshSkinned.SetLayerWeight((int)LAYERS.WALK_BACK, walkBackWeight);
		// Run
		meshSkinned.SetLayerWeight((int)LAYERS.RUN_RIGHT, runRightWeight);
		meshSkinned.SetLayerWeight((int)LAYERS.RUN_FRONT, runFrontWeight);		
		meshSkinned.SetLayerWeight((int)LAYERS.RUN_LEFT, runLeftWeight);
		meshSkinned.SetLayerWeight((int)LAYERS.RUN_BACK, runBackWeight);
		// crouch
		meshSkinned.SetLayerWeight((int)LAYERS.CROUCH_RIGHT, crouchRightWeight);
		meshSkinned.SetLayerWeight((int)LAYERS.CROUCH_FRONT, crouchFrontWeight);		
		meshSkinned.SetLayerWeight((int)LAYERS.CROUCH_LEFT, crouchLeftWeight);
		meshSkinned.SetLayerWeight((int)LAYERS.CROUCH_BACK, crouchBackWeight);
		// Jump
		meshSkinned.SetLayerWeight((int)LAYERS.JUMP_START, jumpStartWeight);
		meshSkinned.SetLayerWeight((int)LAYERS.JUMP_FALL, jumpFallWeight);
		meshSkinned.SetLayerWeight((int)LAYERS.JUMP_LAND, jumpLandWeight);

		// Set frame //
		meshSkinned.SetFrame((int)LAYERS.IDLE, currentTime * animationSpeed);
		meshSkinned.SetFrame((int)LAYERS.CROUCH_IDLE, currentTime * animationSpeed);
		// Walk
		meshSkinned.SetFrame((int)LAYERS.WALK_RIGHT, currentTime * animationSpeed);
		meshSkinned.SetFrame((int)LAYERS.WALK_FRONT, currentTime * animationSpeed);
		meshSkinned.SetFrame((int)LAYERS.WALK_LEFT, currentTime * animationSpeed);
		meshSkinned.SetFrame((int)LAYERS.WALK_BACK, currentTime * animationSpeed);
		// Run
		meshSkinned.SetFrame((int)LAYERS.RUN_RIGHT, currentTime * animationSpeed);
		meshSkinned.SetFrame((int)LAYERS.RUN_FRONT, currentTime * animationSpeed);
		meshSkinned.SetFrame((int)LAYERS.RUN_LEFT, currentTime * animationSpeed);
		meshSkinned.SetFrame((int)LAYERS.RUN_BACK, currentTime * animationSpeed);	
		// Crouch
		meshSkinned.SetFrame((int)LAYERS.CROUCH_RIGHT, currentTime * animationSpeed);
		meshSkinned.SetFrame((int)LAYERS.CROUCH_FRONT, currentTime * animationSpeed);
		meshSkinned.SetFrame((int)LAYERS.CROUCH_LEFT, currentTime * animationSpeed);
		meshSkinned.SetFrame((int)LAYERS.CROUCH_BACK, currentTime * animationSpeed);
		// Jump
		meshSkinned.SetFrame((int)LAYERS.JUMP_START, currentTime * animationSpeed);
		meshSkinned.SetFrame((int)LAYERS.JUMP_FALL, currentTime * animationSpeed);
		meshSkinned.SetFrame((int)LAYERS.JUMP_LAND, currentTime * animationSpeed);
	
		currentTime += Game.IFps;
	}

}