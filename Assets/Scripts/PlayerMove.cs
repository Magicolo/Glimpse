using UnityEngine;
using System.Collections;

//handles player movement, utilising the CharacterMotor class
[RequireComponent(typeof(CharacterMotor))]
//[RequireComponent(typeof(DealDamage))]
//[RequireComponent(typeof(AudioSource))]
public class PlayerMove : MonoBehaviour 
{
	//setup
	public Vector3 fakeGravity;
	public bool sidescroller;					//if true, won't apply vertical input
	public Transform mainCam, floorChecks;		//main camera, and floorChecks object. FloorChecks are raycasted down from to check the player is grounded.
	public Animator animator;					//object with animation controller on, which you want to animate
	//public AudioClip jumpSound;					//play when jumping
	//public AudioClip landSound;					//play when landing on ground
	//public AudioClip stepSound;
	
	//movement
	public float accel = 70f;					//acceleration/deceleration in air or on the ground
	public float airAccel = 18f;			
	public float decel = 7.6f;
	public float airDecel = 1.1f;
	[Range(0f, 5f)]
	public float rotateSpeed = 0.7f, airRotateSpeed = 0.4f;	//how fast to rotate on the ground, how fast to rotate in the air
	public float maxSpeed = 9;								//maximum speed of movement in X/Z axis
	public float slopeLimit = 40, slideAmount = 35;			//maximum angle of slopes you can walk on, how fast to slide down slopes you can't
	public float minimumSlide = 2;
	public float movingPlatformFriction = 7.7f;				//you'll need to tweak this to get the player to stay on moving platforms properly
	
	//jumping
	public Vector3 jumpForce =  new Vector3(0, 13, 0);		//normal jump force
	public Vector3 secondJumpForce = new Vector3(0, 13, 0); //the force of a 2nd consecutive jump
	public Vector3 thirdJumpForce = new Vector3(0, 13, 0);	//the force of a 3rd consecutive jump
	public float jumpDelay = 0.1f;							//how fast you need to jump after hitting the ground, to do the next type of jump
	public float jumpLeniancy = 0.17f;						//how early before hitting the ground you can press jump, and still have it work
	[HideInInspector]
	public int onEnemyBounce;
	[HideInInspector]
	public float groundDistance;

	public int onJump;
	public bool grounded;
	private Transform[] floorCheckers;
	private Quaternion screenMovementSpace;
	private float airPressTime, groundedCount, curAccel, curDecel, curRotateSpeed, slope;
	private Vector3 direction, moveDirection, screenMovementForward, screenMovementRight, movingObjSpeed;
	
	private CharacterMotor characterMotor;
	//private EnemyAI enemyAI;
	//private DealDamage dealDamage;

	public bool sliding = false;
	public bool controlsFrozen = false;
	//	private bool collSet = false;

	AudioItem landingSound;
	
	//setup
	void Awake()
	{
		//assign player tag if not already
		if(tag != "Player")
		{
			tag = "Player";
			Debug.LogWarning ("PlayerMove script assigned to object without the tag 'Player', tag has been assigned automatically", transform);
		}
		//usual setup
		mainCam = GameObject.FindGameObjectWithTag("MainCamera").transform;
	//	dealDamage = GetComponent<DealDamage>();
		characterMotor = GetComponent<CharacterMotor>();
		//gets child objects of floorcheckers, and puts them in an array
		//later these are used to raycast downward and see if we are on the ground
		floorCheckers = new Transform[floorChecks.childCount];
		for (int i=0; i < floorCheckers.Length; i++)
			floorCheckers[i] = floorChecks.GetChild(i);
	}
	
	//get state of player, values and input
	void Update()
	{	
		//JumpCalculations ();
	}
	
	//apply correct player movement (fixedUpdate for physics calculations)
	void FixedUpdate() 
	{
		//adjust movement values if we're in the air or on the ground
		curAccel = (grounded) ? accel : airAccel;
		curDecel = (grounded) ? decel : airDecel;
		curRotateSpeed = (grounded) ? rotateSpeed : airRotateSpeed;
		
		//get movement axis relative to camera
		screenMovementSpace = Quaternion.Euler (0, mainCam.eulerAngles.y, 0);
		screenMovementForward = screenMovementSpace * Vector3.forward;
		screenMovementRight = screenMovementSpace * Vector3.right;
		
		//get movement input, set direction to move in
		float h = 0;
		float v = 0;
		if (!sliding && !controlsFrozen) {
				h = Input.GetAxisRaw ("LeftStickHorizontal");
				v = Input.GetAxisRaw ("LeftStickVertical");
		}
		
		//only apply vertical input to movemement, if player is not sidescroller

		direction = (screenMovementForward * v) + (screenMovementRight * h);

		moveDirection = transform.position + direction;

		curAccel = curAccel * direction.magnitude;

		//are we grounded
		grounded = IsGrounded ();
		// Vector3 jumpDir = Vector3.zero;
		//move, rotate, manage speed
		//if (!sliding )
		//{
			characterMotor.MoveTo (moveDirection, curAccel, 0.15f, true);
			if( (animator.GetFloat("SoundWalk") > 0.1f || animator.GetFloat("SoundRun") > 0.1f ) && grounded )
			{
				//audio.volume = 0.1f;
				//audio.clip = stepSound;
				//audio.Play ();
			}

		 //}
		if (rotateSpeed != 0 && direction.magnitude >= 0.15f)
			characterMotor.RotateToDirection (moveDirection , curRotateSpeed * 5, true);
		characterMotor.ManageSpeed (curDecel, maxSpeed + movingObjSpeed.magnitude, true);

		//set animation values
		if(animator)
		{
			animator.SetFloat("DistanceToTarget", characterMotor.DistanceToTarget);
			animator.SetBool("Grounded", grounded);
			animator.SetFloat("YVelocity", rigidbody.velocity.y);
			animator.SetFloat("GroundDistance", groundDistance);
			animator.SetFloat("Speed", rigidbody.velocity.magnitude);

			animator.SetFloat("XAxe", Input.GetAxisRaw ("LeftStickVertical"));
			animator.SetFloat("ZAxe", Input.GetAxisRaw ("LeftStickHorizontal"));
			animator.SetBool("Sliding", sliding);

		}

		if (grounded && direction.magnitude < 0.15f && slope < slopeLimit && rigidbody.velocity.magnitude < minimumSlide) {
						//it's usually not a good idea to alter a rigidbodies velocity every frame
						//but this is the cleanest way i could think of, and we have a lot of checks beforehand, so it shou
						rigidbody.velocity = Vector3.zero;
						//	Debug.Log (rigidbody.velocity);
				} else
						rigidbody.AddForce (fakeGravity);

		JumpCalculations ();
		

		// set collider size smaller when jumping anim is playing
/*		if(!grounded && groundDistance >= 3 && !collSet){
			float coll = GetComponent<CapsuleCollider>().height / 1.6f;
			GetComponent<CapsuleCollider>().height = coll;
			collSet = true;
		}
		if( ((!grounded && groundDistance <= 3)|| grounded || animator.GetFloat("YVelocity") < 0) && collSet){
			float coll = GetComponent<CapsuleCollider>().height * 1.6f;
			GetComponent<CapsuleCollider>().height = coll;
			collSet = false;
		}   */
	}
	

	
	//returns whether we are on the ground or not
	//also: bouncing on enemies, keeping player on moving platforms and slope checking
	private bool IsGrounded() 
	{
		//get distance to ground, from centre of collider (where floorcheckers should be)
		float dist = collider.bounds.extents.y;
		//check whats at players feet, at each floorcheckers position
		foreach (Transform check in floorCheckers) {
						RaycastHit hit;
						if (Physics.Raycast (check.position, Vector3.down, out hit, dist + 0.05f)) {

								groundDistance = hit.distance;
								Debug.DrawLine (check.position, hit.point, Color.cyan);
								if (!hit.transform.collider.isTrigger) {       //slope control
										slope = Vector3.Angle (hit.normal, Vector3.up);
										//slide down slopes
						//				if (slope > slopeLimit && hit.transform.tag != "Pushable" && groundDistance <= 2.0f) {
										if (slope > slopeLimit && hit.transform.tag != "Pushable") {
												Vector3 slide = new Vector3 (0f, -slideAmount, 0f);
												rigidbody.AddForce (slide, ForceMode.Force);
												sliding = true;
										} else {
												sliding = false;
										}

										//moving platforms
										if (hit.transform.tag == "MovingPlatform" || hit.transform.tag == "Pushable") {
												movingObjSpeed = hit.transform.rigidbody.velocity;
												movingObjSpeed.y = 0f;
												//9.5f is a magic number, if youre not moving properly on platforms, experiment with this number
												rigidbody.AddForce (movingObjSpeed * movingPlatformFriction * Time.fixedDeltaTime, ForceMode.VelocityChange);
										} else {
												movingObjSpeed = Vector3.zero;
										}

					return true;
								}
						}
				}

		if (groundDistance <= 0.0f) 
		{

			//yes our feet are on something
			return true;

		} else 
		{	

			movingObjSpeed = Vector3.zero;
			//no none of the floorchecks hit anything, we must be in the air (or water)
			return false;
		}
	}
	
	//jumping
	private void JumpCalculations()
	{
		//keep how long we have been on the ground
		groundedCount = (grounded) ? groundedCount += Time.deltaTime : 0f;
		
		//play landing sound
		if(groundedCount < 0.25 && groundedCount != 0 && rigidbody.velocity.y < 1)
		{
			if (landingSound != null){
				landingSound.Stop();
			}
			landingSound = AudioPlayer.PlayContainer(string.Format("Player_Footstep_{0}_Land", TextureReader.GetMainTextureName()));
		}
		
		//if we press jump in the air, save the time
		if (Input.GetButtonDown ("Jump") && !grounded)
			airPressTime = Time.time;
		
		//if were on ground within slope limit
		if (grounded && slope < slopeLimit )
		{
			//and we press jump, or we pressed jump justt before hitting the ground
			if (Input.GetButtonDown ("Jump") || airPressTime + jumpLeniancy > Time.time)
			{	
				//increment our jump type if we haven't been on the ground for long
				onJump = (groundedCount < jumpDelay) ? Mathf.Min(2, onJump + 1) : 0;
				//execute the correct jump (like in mario64, jumping 3 times quickly will do higher jumps)
				if (onJump == 0)
						Jump (jumpForce);
				else if (onJump == 1)
						Jump (secondJumpForce);
				else if (onJump == 2)
						Jump (thirdJumpForce);
			}
		}
	}
	
	//push player at jump force
	public void Jump(Vector3 jumpVelocity)
	{
		//if(jumpSound)
		//{
			//audio.volume = 1;
			//audio.clip = jumpSound;
			//audio.Play ();
		//}
		rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);
		rigidbody.AddRelativeForce (jumpVelocity, ForceMode.Impulse);
		airPressTime = 0f;
	}
}