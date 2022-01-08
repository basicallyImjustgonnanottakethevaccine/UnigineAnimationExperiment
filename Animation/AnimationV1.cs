using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "bc33a5dd91b96d98385c1417bf5c228a0959b061")]
public class AnimationV1 : Component
{
	private ObjectMeshSkinned meshSkinned = null;
	public ObjectMeshSkinned animManBase = null;
	public ObjectMeshSkinned animManAimoffset = null;


	[ParameterFile(Filter = ".anim")]
	public string[] baseAnimations = new string [5];
	[ParameterFile(Filter = ".anim")]
	public string[] additionalAnimations = new string [1];

	// Animation helpers
	private float currentTime;
	public float animationSpeed = 30.0f;
	public float blendSpeed = 3.0f;
	public float rotationSpeed = 3.0f;

	// Layer Weight array
	private float[] BaselayersWeight;

	// Animation Layers
	private enum BASE
	{
		IDLE = 0,
		WALK_FRONT,
		WALK_RIGHT,	
		WALK_LEFT,
		WALK_BACK,/*
		RUN_RIGHT,
		RUN_FRONT,
		RUN_LEFT,
		RUN_BACK,
		CROUCH_IDLE,
		CROUCH_RIGHT,
		CROUCH_FRONT,
		CROUCH_LEFT,
		CROUCH_BACK,	
		JUMP_START,
		JUMP_FALL,
		JUMP_LAND,*/
		COUNT
	}

	// AIM OFFSET
	public Node lookAtNode;
	private Node LookAtTarget;
	public float aimSpeedY = 1f;
	public PlayerDummy camera;
	private float AimOffsetFrame;

	// Interpolate
	// public List<string> interpolatedBones = new List<string>(12); // Use name to define witch bone are the legs.
	private List<int> upperBodyBonesNumbers = new List<int>();	// Dont forget to assign a value to the list : new List<int>();
	private List<int> legsBonesNumbers = new List<int>();
	private List<int> splinesBonesNumbers = new List<int>();
	
	//test
	private float aim = 0f;

	private	enum AIMOFFSET
	{
		RIFLE = 0,
		COUNT
	}

	private enum FINAL
	{
		FINAL = 0,
		COUNT
	}

	private void Init() // test
	{
		Unigine.Console.Run("console_onscreen 1");
		meshSkinned = node as ObjectMeshSkinned;

		// LAYERS //
		animManBase.NumLayers = (int)BASE.COUNT;
		animManAimoffset.NumLayers = (int)AIMOFFSET.COUNT;
		meshSkinned.NumLayers = (int)FINAL.COUNT;

		// weight
		BaselayersWeight = new float[animManBase.NumLayers];

		// Setup base animations
		for (int i = 0; i < (int)BASE.COUNT; i++)
		{	
			animManBase.SetAnimation(i, baseAnimations[i]);
			animManBase.SetLayerEnabled((i), true);
		}

		// Setup aimoffset animations
		animManAimoffset.SetAnimation((int)AIMOFFSET.RIFLE, additionalAnimations[0]);
		animManAimoffset.SetLayerEnabled((int)AIMOFFSET.RIFLE, true);
		animManAimoffset.SetLayerWeight((int)AIMOFFSET.RIFLE,1);
		//meshSkinned.SetFrame((int)AIMOFFSET.RIFLE,0, 0, 30 );

		// Setup final layer
		meshSkinned.SetLayerEnabled((int)FINAL.FINAL, true);
		meshSkinned.SetLayerWeight((int)FINAL.FINAL,1);

		// Bones //
		// Setup upper body bones
		// Legs are bone from number 49 to 60
		for (int i = 2; i < 49; i++)
		{
			upperBodyBonesNumbers.Add(i); 
		}
		// Setup legs bones
		for (int i = 49; i <= 60; i++)
		{
			legsBonesNumbers.Add(i);
			legsBonesNumbers.Add(1);
		}
		// Setup splines bones
		for (int i = 2; i <= 4; i++)
		{
			splinesBonesNumbers.Add(i); 
		}
		//legsBonesNumbers.Add(0);
		//legsBonesNumbers.Add(1);


		// Setup lookAt Node
		int num = lookAtNode.FindChild("LookAtTarget");
		LookAtTarget = lookAtNode.GetChild(num);

		/*
		// find bones in mesh and save their numbers
		upperBodyBonesNumbers = new List<int>();
			for (int i = 0; i < meshSkinned.NumBones; i++)
			{
				string name = meshSkinned.GetBoneName(i);
				if (interpolatedBones.Contains(name))
					upperBodyBonesNumbers.Add(i);
			}
		*/
	}
	
	private void Update()
	{
		//Log.Message("{0}\n",Input.MouseCoordDelta.y);
		FirstPersonController Component = node.GetComponentInParent<FirstPersonController>();
		Idle(Component);
		Walk();
		RotatePlayer();
		UpdateAnimations();
		BlendAnimations();
		RifleAimOffset();
	}

	private void RotatePlayer()
	{
		quat rotation = animManBase.GetRotation();
		
		// Rotate the legs left - right
		if (Input.IsKeyPressed(Input.KEY.W))
		{
			if (Input.IsKeyPressed(Input.KEY.D))
			if ( rotation.z > -0.3f)
			animManBase.Rotate(new quat (0,0,-50 * rotationSpeed * Game.IFps));	
			if (Input.IsKeyPressed(Input.KEY.A))
			if ( rotation.z < 0.3f)
			animManBase.Rotate(new quat (0,0,50 * rotationSpeed * Game.IFps));
					
		}
		if (Input.IsKeyPressed(Input.KEY.S))
		{
			if (Input.IsKeyPressed(Input.KEY.D))
			if ( rotation.z < 0.3f)
			animManBase.Rotate(new quat (0,0,50 * rotationSpeed * Game.IFps));	
			if (Input.IsKeyPressed(Input.KEY.A))
			if ( rotation.z > -0.3f)
			animManBase.Rotate(new quat (0,0,-50 * rotationSpeed * Game.IFps));		
		}
		// Unrotate legs
		if (Input.IsKeyPressed(Input.KEY.W))
		{
			if (!Input.IsKeyPressed(Input.KEY.A) && !Input.IsKeyPressed(Input.KEY.D))
			UnrotateLegs(rotation.z);
		}
		if (Input.IsKeyPressed(Input.KEY.D))
		{
			if (!Input.IsKeyPressed(Input.KEY.W) && !Input.IsKeyPressed(Input.KEY.S))
			UnrotateLegs(rotation.z);
		}
		if (Input.IsKeyPressed(Input.KEY.A))
		{
			if (!Input.IsKeyPressed(Input.KEY.W) && !Input.IsKeyPressed(Input.KEY.S))
			UnrotateLegs(rotation.z);
		}
		if (Input.IsKeyPressed(Input.KEY.S))
		{
			if (!Input.IsKeyPressed(Input.KEY.A) && !Input.IsKeyPressed(Input.KEY.D))
			UnrotateLegs(rotation.z);
		}
		// Unrotate when player isn't moving at all
		if (!Input.IsKeyPressed(Input.KEY.W) && !Input.IsKeyPressed(Input.KEY.D) && !Input.IsKeyPressed(Input.KEY.A) && !Input.IsKeyPressed(Input.KEY.S) && (MathLib.Abs(rotation.z) > 0.01f))
		{
			if (!Input.IsKeyPressed(Input.KEY.A) && !Input.IsKeyPressed(Input.KEY.D))
			UnrotateLegs(rotation.z);
		}
	}

	private void UnrotateLegs(float rotationZ)
	{
		if (MathLib.Abs(rotationZ) > 0.01f) // Not using 0 otherwise the rotation will just ping pong between -0.1/0.1 and create shake
			animManBase.Rotate(new quat (0,0,-rotationZ * (1/MathLib.Abs(rotationZ)))); // *(1/MathLib.Abs(rotation.z)) keep rotation speed constant as rotation.z lower in value
	}

	private void RifleAimOffset()
	{	
		// What is the rotation of the lookAtNode ?	
		quat rotation = lookAtNode.GetRotation();
		float rotationX = rotation.EulerX; // Looking front is 0, up is 1.6 and down -1.6

		// Rotate Up
		if ( Input.MouseCoordDelta.y < 0 && rotationX < 1.5f ) // negative delta mean moving mouse up
		{
			lookAtNode.Rotate(-Input.MouseCoordDelta.y * Game.IFps * aimSpeedY,0,0);
		}
		// Rotate Down
		if ( Input.MouseCoordDelta.y > 0 && rotationX > -1.5f ) // positive delta mean moving mouse down
		{
			lookAtNode.Rotate(-Input.MouseCoordDelta.y * Game.IFps * aimSpeedY,0,0);
		}

		// Convert rotationX into animation frame
		// Go to https://rechneronline.de/function-graphs/ and enter -x*9.3+15, -1.6 give frame 30, 0 give frame 15, and 1.6 gve frame 0
		// It is a trial and error to get the numbers right, for other aim offset setup it might not work and need another fonction
		AimOffsetFrame = -rotationX * 9.3f + 15f;	
		
		// Camera Look at target
		camera.WorldLookAt(LookAtTarget.WorldPosition);
	}

	private void Idle(FirstPersonController Component)
	{
		
		if ( MathLib.Abs(Component.HorizontalVelocity.x) < 0.5f && MathLib.Abs(Component.HorizontalVelocity.y) < 0.5f)
		{
			BaselayersWeight[0] = MathLib.Clamp(BaselayersWeight[0] + blendSpeed * Game.IFps,0,1f) ;
		}
		else BaselayersWeight[0] = MathLib.Clamp(BaselayersWeight[0] - blendSpeed * Game.IFps,0,1f) ;
	}

	private void Walk()
	{
		if (Input.IsKeyPressed(Input.KEY.W))
		{
			BaselayersWeight[1] = MathLib.Clamp(BaselayersWeight[1] + blendSpeed * Game.IFps,0,1f) ;
		}
		else BaselayersWeight[1] = MathLib.Clamp(BaselayersWeight[1] - blendSpeed * Game.IFps,0,1f) ;

		if (Input.IsKeyPressed(Input.KEY.D) && !Input.IsKeyPressed(Input.KEY.W) && !Input.IsKeyPressed(Input.KEY.S))
		{
			BaselayersWeight[2] = MathLib.Clamp(BaselayersWeight[2] + blendSpeed * Game.IFps,0,1f) ;
		}
		else BaselayersWeight[2] = MathLib.Clamp(BaselayersWeight[2] - blendSpeed * Game.IFps,0,1f) ;

		if (Input.IsKeyPressed(Input.KEY.A) && !Input.IsKeyPressed(Input.KEY.W) && !Input.IsKeyPressed(Input.KEY.S))
		{
			BaselayersWeight[3] = MathLib.Clamp(BaselayersWeight[3] + blendSpeed * Game.IFps,0,1f) ;
		}
		else BaselayersWeight[3] = MathLib.Clamp(BaselayersWeight[3] - blendSpeed * Game.IFps,0,1f) ;

		if (Input.IsKeyPressed(Input.KEY.S))
		{
			BaselayersWeight[4] = MathLib.Clamp(BaselayersWeight[4] + blendSpeed * Game.IFps,0,1f) ;
		}
		else BaselayersWeight[4] = MathLib.Clamp(BaselayersWeight[4] - blendSpeed * Game.IFps,0,1f) ;
	}

	private void BlendAnimations()
	{
		foreach (int bone in legsBonesNumbers)
		// You can remove offset if you place animManBase and animManAimoffset at 0.0.0 // I had to place AnimManBase to 0,0,0 because of the rotation of the lower part mess up with bone coordinate.
		{
			//mat4 offset = meshSkinned.WorldTransform - animManBase.WorldTransform;
			meshSkinned.SetBoneWorldTransform(bone, animManBase.GetBoneWorldTransform(bone) /*+ offset*/);
		}
		
		foreach (int bone in upperBodyBonesNumbers) // Keep offset for debuging purpose with AnimManAimoffset meshskinned.
		{
			mat4 offset = meshSkinned.WorldTransform - animManAimoffset.WorldTransform;
			meshSkinned.SetBoneWorldTransform(bone, animManAimoffset.GetBoneWorldTransform(bone) + offset);
		}

		// Blend upper and lower body
		// Splines
		meshSkinned.SetBoneWorldTransform(2, MathLib.Lerp(meshSkinned.GetBoneWorldTransform(2), animManBase.GetBoneWorldTransform(2), 0.7f));
		meshSkinned.SetBoneWorldTransform(3, MathLib.Lerp(meshSkinned.GetBoneWorldTransform(3), animManBase.GetBoneWorldTransform(3), 0.4f));
		meshSkinned.SetBoneWorldTransform(4, MathLib.Lerp(meshSkinned.GetBoneWorldTransform(4), animManBase.GetBoneWorldTransform(4), 0.2f));
		// Arms
		meshSkinned.SetBoneWorldTransform(5, MathLib.Lerp(meshSkinned.GetBoneWorldTransform(5), animManBase.GetBoneWorldTransform(5), 0.2f));
		meshSkinned.SetBoneWorldTransform(26, MathLib.Lerp(meshSkinned.GetBoneWorldTransform(26), animManBase.GetBoneWorldTransform(26), 0.2f)); 
		meshSkinned.SetBoneWorldTransform(6, MathLib.Lerp(meshSkinned.GetBoneWorldTransform(6), animManBase.GetBoneWorldTransform(6), 0.1f)); 
		meshSkinned.SetBoneWorldTransform(27, MathLib.Lerp(meshSkinned.GetBoneWorldTransform(27), animManBase.GetBoneWorldTransform(27), 0.1f));
		// Neck and head
		meshSkinned.SetBoneWorldTransform(47, MathLib.Lerp(meshSkinned.GetBoneWorldTransform(47), animManBase.GetBoneWorldTransform(47), 0.1f));
		meshSkinned.SetBoneWorldTransform(48, MathLib.Lerp(meshSkinned.GetBoneWorldTransform(48), animManBase.GetBoneWorldTransform(48), 0.1f));
	}

	private void UpdateAnimations()
	{
		// Update aim offset
		animManAimoffset.SetFrame((int)AIMOFFSET.RIFLE, AimOffsetFrame, 0, 30 );
		// Update base animations
		for (int i = 0; i < (int)BASE.COUNT; i++)
		{	
			animManBase.SetLayerWeight((i), BaselayersWeight[i]);
			animManBase.SetFrame((i), currentTime * animationSpeed);
		}
		currentTime += Game.IFps;
	}
}