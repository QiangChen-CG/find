using UnityEngine;
using System.Collections;

public class StepChar : MonoBehaviour {
	
	public enum State
	{
		idle,
		move
	}
	
	public enum MoveDir
	{
		Forward,
		Back,
		Left,
		Right
	}

	public bool CanCtrl = true;

	public State state = State.idle;

	public float tweenspeed = 1;
	public float currenttweentime = 0;

	public float setp = 1.0f;
	public Rigidbody rb;
	public GameObject renderobject;
	public Animator anim;

	public Vector3 movedir = Vector3.zero;
	public Vector3 moveToPos = Vector3.zero;
	public Vector3 moveStartPos = Vector3.zero;
	public float hitStairHeight = 0;

	public bool touchBegin = false;
	public Vector2 beginTouchPos = Vector2.zero;
	public float rayLength = 3;
	public float rayHeight = 2;

	bool canWalkForward = true;
	bool canWalkBack = true;
	bool canWalkLeft = true;
	bool canWalkRight = true;


	void OnEnable()
	{
		GameVirtualInput.instance.RegButtonInputEvent(0, GameVirtualInput.ButtonInputEventType.UP, VILeft);
		GameVirtualInput.instance.RegButtonInputEvent(1, GameVirtualInput.ButtonInputEventType.UP, VIForward);
		GameVirtualInput.instance.RegButtonInputEvent(2, GameVirtualInput.ButtonInputEventType.UP, VIBack);
		GameVirtualInput.instance.RegButtonInputEvent(3, GameVirtualInput.ButtonInputEventType.UP, VIRight);

	}
	
	void OnDisable()
	{
		GameVirtualInput.instance.UnRegButtonInputEvent(0, GameVirtualInput.ButtonInputEventType.UP, VILeft);
		GameVirtualInput.instance.UnRegButtonInputEvent(1, GameVirtualInput.ButtonInputEventType.UP, VIForward);
		GameVirtualInput.instance.UnRegButtonInputEvent(2, GameVirtualInput.ButtonInputEventType.UP, VIBack);
		GameVirtualInput.instance.UnRegButtonInputEvent(3, GameVirtualInput.ButtonInputEventType.UP, VIRight);
		
	}
	
	void VILeft(Vector2 move)
	{
		if (CanCtrl)
		{
			MoveTest(MoveDir.Left, true);
		}
	}
	void VIRight(Vector2 move)
	{
		if (CanCtrl)
		{
			MoveTest(MoveDir.Right, true);
		}
	}
	void VIBack(Vector2 move)
	{
		if (CanCtrl)
		{
			MoveTest(MoveDir.Back, true);
		}
	}
	void VIForward(Vector2 move)
	{
		if (CanCtrl)
		{
			MoveTest(MoveDir.Forward, true);
		}
	}
	// Use this for initialization
	void Start ()
	{
		moveToPos = transform.position;
	}

	bool WayRayTest(Vector3 pos, Vector3 dir, bool getHitHeight = false)
	{		
		RaycastHit rhf = new RaycastHit();
		/*
		Debug.DrawLine(pos, pos+(dir*1.3f));
		if (Physics.Raycast(pos, dir, out rhf, 1.3f))
		{
			if (rhf.collider != null && rhf.collider.gameObject.tag == "wall")
			{
				return false;
			}
		}
		*/
		if (getHitHeight)
		{
			hitStairHeight = 0;
		}

		Vector3 raystart = pos + (dir*setp);
		raystart.y += rayHeight;
		Vector3 rayend = raystart;
		rayend.y -= rayLength;
		
		Debug.DrawLine(raystart, rayend, Color.yellow);
		if (Physics.Raycast(raystart, -Vector3.up, out rhf, rayLength))
		{
			//wall.
			if (rhf.collider != null && rhf.collider.gameObject.tag == "wall")
			{
				return false;
			}

			//樓梯.
			if (getHitHeight && rhf.collider != null && rhf.collider.gameObject.tag == "stair")
			{
				hitStairHeight = rhf.point.y - moveToPos.y;
			}

		}
		return true;
	}

	void FindWay()
	{
		//Vector3 raystart = transform.position;
		//raystart.y += rayYfix;
		//canWalkForward = WayRayTest(raystart, transform.forward);
		//canWalkBack = WayRayTest(raystart, -transform.forward);
		//canWalkLeft = WayRayTest(raystart, -transform.right);
		//canWalkRight = WayRayTest(raystart, transform.right);

	}

	void MoveValue(Vector3 moveval)
	{
		//校正.
		transform.position = moveToPos;

		movedir = moveval;
		moveToPos = transform.position + movedir * setp;
		moveToPos.y += hitStairHeight;
		moveStartPos = transform.position;
		renderobject.transform.LookAt(renderobject.transform.position + movedir);
		anim.Play("move", 0, 0);
		state = State.move;
		currenttweentime = 0;
		arcGameSoundPlayer.instance.PlayFX(3);

		GameData.instance.AddFood(-1);

	}

	public void Dead()
	{
		CanCtrl = false;
		anim.Play("die");
		
		arcGameSoundPlayer.instance.PlayFX(6);
	}
	
	public void Alive()
	{
		CanCtrl = true;
		anim.Play("move");
	}
	void HandleKeyCtrl(float deltatime)
	{
		if (state == State.idle)
		{
			if (Input.GetKeyDown(KeyCode.W))
			{
				MoveTest(MoveDir.Forward, true);
			}
			else if (Input.GetKeyDown(KeyCode.S))
			{
				MoveTest(MoveDir.Back, true);
			}
			else if (Input.GetKeyDown(KeyCode.A))
			{
				MoveTest(MoveDir.Left, true);
			}
			else if (Input.GetKeyDown(KeyCode.D))
			{
				MoveTest(MoveDir.Right, true);
			}
		}
	}

	void UpdateMovment(float deltatime)
	{
		if (state != State.move)
		{
			return;
		}

		currenttweentime += deltatime * tweenspeed;

		if (currenttweentime > 1)
		{
			currenttweentime = 1;
		}

		Vector3 tweenpos = Vector3.zero;
		tweenpos.x = iTweenLite.Easing(moveStartPos.x, moveToPos.x, currenttweentime, iTweenLite.EaseType.linear);
		tweenpos.y = iTweenLite.Easing(moveStartPos.y, moveToPos.y, currenttweentime, iTweenLite.EaseType.linear);
		tweenpos.z = iTweenLite.Easing(moveStartPos.z, moveToPos.z, currenttweentime, iTweenLite.EaseType.linear);
		
		rb.MovePosition(tweenpos);
		//rb.position = tweenpos;

		if (currenttweentime >= 1)
		{
			currenttweentime = 0;
			state = State.idle;
		}
	}

	void HandleToucehDragInput()
	{
		
		//Vector2 screenCharPos = Camera.main.WorldToScreenPoint (transform.position);
		
		//Vector2 touchpos = screenCharPos;

		/*
		if (Input.GetMouseButtonDown (0)) 
		{
			beginTouchPos = Input.mousePosition;
			gettouch = true;
		}*/
		if (Input.touchCount > 0)
		{
			Touch t = Input.GetTouch(0);
			if (t.phase == TouchPhase.Began)
			{
				beginTouchPos = t.position;
				touchBegin = true;
				Debug.Log("t b");
			}
		}

		if (touchBegin) 
		{
			bool getend = false;
			Vector2 endpos = Vector2.zero;
			if (Input.touchCount > 0)
			{
				Touch t = Input.GetTouch(0);
				if (t.phase == TouchPhase.Ended)
				{
					endpos = t.position;
					getend = true;
					touchBegin = false;
					Debug.Log("t end");
				}
			}

			if (getend) 
			{
				Vector2 dir = endpos - beginTouchPos;
				dir.Normalize ();
				
				float du = Vector2.Dot (dir, Vector2.up);
				float dr = Vector2.Dot (dir, Vector2.right);
				
				//上.
				if (du >= 0) {
					//右.
					if (dr >= 0) {
						if (canWalkForward) {
							MoveValue (transform.forward);
						}
						
					} else {
						//左.
						if (canWalkLeft) {			
							MoveValue (-transform.right);
						}
						
					}
				} else {
					//下.
					//右.
					if (dr >= 0) {
						if (canWalkRight) {
							MoveValue (transform.right);
						}
						
					} else {
						//左.						
						if (canWalkBack) {
							MoveValue (-transform.forward);
						}
						
					}					
				}				
			}
		}
	}

	bool CanNextWalkTo(Vector3 dir, Vector3 dirNext)
	{
		Vector3 raystart = transform.position;
		raystart += dir;
		raystart.y += rayHeight;

		return WayRayTest(raystart, dirNext);
	}

	void MoveTest(MoveDir dir, bool smart)
	{
		Vector3 raystart = transform.position;
		raystart.y += rayHeight;
		switch (dir)
		{				
			case MoveDir.Forward:
			{
				if (WayRayTest(raystart, transform.forward, true))
				{
					MoveValue(transform.forward);
				}
				else if (smart)
				{
					if (WayRayTest(raystart, transform.right, true) && CanNextWalkTo(transform.right, transform.forward))
					{
						MoveValue(transform.right);
					}
					else if (WayRayTest(raystart, -transform.right, true) && CanNextWalkTo(-transform.right, transform.forward))
					{			
						MoveValue(-transform.right);
					}
				}
				break;

			}
			case MoveDir.Back:
			{					
				if (WayRayTest(raystart, -transform.forward, true))
				{
					MoveValue(-transform.forward);
				}
				else if (smart)
				{
					if (WayRayTest(raystart, -transform.right, true) && CanNextWalkTo(-transform.right, -transform.forward))
					{			
						MoveValue(-transform.right);
					}
					else if (WayRayTest(raystart, transform.right, true) && CanNextWalkTo(transform.right, -transform.forward))
					{
						MoveValue(transform.right);
					}	
				}
				break;
			}
			case MoveDir.Left:
			{
				if (WayRayTest(raystart, -transform.right, true))
				{			
					MoveValue(-transform.right);
				}
				else if (smart)
				{
					if (WayRayTest(raystart, -transform.forward, true) && CanNextWalkTo(-transform.forward, -transform.right))
					{			
						MoveValue(-transform.forward);
					}
					else if (WayRayTest(raystart, transform.forward, true) && CanNextWalkTo(transform.forward, -transform.right))
					{
						MoveValue(transform.forward);
					}
				}
				break;
			}
			case MoveDir.Right:
			{
				if (WayRayTest(raystart, transform.right, true))
				{
					MoveValue(transform.right);
				}
				else if (smart)
				{
					if (WayRayTest(raystart, transform.forward, true) && CanNextWalkTo(transform.forward, transform.right))
					{
						MoveValue(transform.forward);
					}							
					else if (WayRayTest(raystart, -transform.forward, true) && CanNextWalkTo(-transform.forward, transform.right))
					{
						MoveValue(-transform.forward);
					}
				}
				break;
			}
		}
	}

	void HandleTouchInputSmart()
	{
		Vector2 screenCharPos = Camera.main.WorldToScreenPoint(transform.position);
		
		Vector2 touchpos = screenCharPos;
		bool gettouch = false;
		#if UNITY_WEBPLAYER
		if (Input.GetMouseButtonDown(0))
		{
			touchpos = Input.mousePosition;
			gettouch = true;
		}
		#else		
		if (Input.touchCount > 0)
		{
			Touch t = Input.GetTouch(0);
			if (t.phase == TouchPhase.Began)
			{
				touchpos = t.position;
				gettouch = true;
			}
		}
		#endif
		if (gettouch)
		{
			Vector2 dir = touchpos - screenCharPos;
			dir.Normalize();
			
			float du = Vector3.Dot(dir, Vector2.up);
			float dr = Vector3.Dot(dir, Vector2.right);
			
			//上.
			if (du >= 0)
			{
				//右.
				if (dr >= 0)
				{
					if (canWalkForward)
					{
						MoveValue(transform.forward);
					}
					else if (canWalkRight && CanNextWalkTo(transform.right, transform.forward))
					{
						MoveValue(transform.right);
					}
					else if (canWalkLeft && CanNextWalkTo(-transform.right, transform.forward))
					{			
						MoveValue(-transform.right);
					}
				}
				else
				{
					//左.
					if (canWalkLeft)
					{			
						MoveValue(-transform.right);
					}
					else if (canWalkBack && CanNextWalkTo(-transform.forward, -transform.right))
					{			
						MoveValue(-transform.forward);
					}
					else if (canWalkForward && CanNextWalkTo(transform.forward, -transform.right))
					{
						MoveValue(transform.forward);
					}
					
				}
			}
			else
			{
				//下.
				//右.
				if (dr >= 0)
				{
					if (canWalkRight)
					{
						MoveValue(transform.right);
					}
					else if (canWalkForward && CanNextWalkTo(transform.forward, transform.right))
					{
						MoveValue(transform.forward);
					}							
					else if (canWalkBack && CanNextWalkTo(-transform.forward, transform.right))
					{
						MoveValue(-transform.forward);
					}		
				}
				else
				{
					//下.						
					if (canWalkBack)
					{
						MoveValue(-transform.forward);
					}
					else if (canWalkLeft && CanNextWalkTo(-transform.right, -transform.forward))
					{			
						MoveValue(-transform.right);
					}
					else if (canWalkRight && CanNextWalkTo(transform.right, -transform.forward))
					{
						MoveValue(transform.right);
					}					
				}				
			}			
		}
	}

	

	void HandleTouchInput()
	{
		Vector2 screenCharPos = Camera.main.WorldToScreenPoint(transform.position);

		Vector2 touchpos = screenCharPos;
		bool gettouch = false;
#if UNITY_WEBPLAYER
		if (Input.GetMouseButtonDown(0))
		{
			touchpos = Input.mousePosition;
			gettouch = true;
		}
#else		
		if (Input.touchCount > 0)
		{
			Touch t = Input.GetTouch(0);
			if (t.phase == TouchPhase.Began)
			{
				touchpos = t.position;
				gettouch = true;
			}
		}
#endif
		if (gettouch)
		{
			Vector2 dir = touchpos - screenCharPos;
			dir.Normalize();
			
			float du = Vector3.Dot(dir, Vector2.up);
			float dr = Vector3.Dot(dir, Vector2.right);
			
			//上.
			if (du >= 0)
			{
				//右.
				if (dr >= 0)
				{
					if (canWalkForward)
					{
						MoveValue(transform.forward);
					}
					
				}
				else
				{
					//左.
					if (canWalkLeft)
					{			
						MoveValue(-transform.right);
					}
					
				}
			}
			else
			{
				//下.
				//右.
				if (dr >= 0)
				{
					if (canWalkRight)
					{
						MoveValue(transform.right);
					}
					
				}
				else
				{
					//左.						
					if (canWalkBack)
					{
						MoveValue(-transform.forward);
					}
					
				}
				
			}
			
		}
	}

	// Update is called once per frame
	void Update ()
	{
		if (CanCtrl)
		{
			float deltatime = arcGamePlayTimeMgr.GetDeltaTime("player");
			//FindWay();
			//HandleTouchInput();
			//HandleToucehDragInput();
			//HandleTouchInputSmart();
			HandleKeyCtrl(deltatime);
		}


	}
		
	void FixedUpdate ()
	{
		float deltatime = arcGamePlayTimeMgr.GetDeltaTime("player");
		UpdateMovment(deltatime);
	}


}
