using UnityEngine;
using System.Collections;

public class StepChar : MonoBehaviour {
	
	public enum State
	{
		idle,
		move
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

	public bool touchBegin = false;
	public Vector2 beginTouchPos = Vector2.zero;
	public float rayYfix = 0;

	bool canWalkForward = true;
	bool canWalkBack = true;
	bool canWalkLeft = true;
	bool canWalkRight = true;

	// Use this for initialization
	void Start ()
	{
		moveToPos = transform.position;
	}

	void FindWay()
	{
		canWalkForward = true;
		canWalkBack = true;
		canWalkLeft = true;
		canWalkRight = true;

		Vector3 raystart = transform.position;
		raystart.y += rayYfix;

		RaycastHit rhf = new RaycastHit();
		if (Physics.Raycast(raystart, transform.forward, out rhf, 1.3f))
		{
			if (rhf.collider != null && rhf.collider.gameObject.tag == "wall")
			{
				canWalkForward = false;
			}
		}

		if (Physics.Raycast(raystart, -transform.forward, out rhf, 1.3f))
		{
			if (rhf.collider != null && rhf.collider.gameObject.tag == "wall")
			{
				canWalkBack = false;
			}
		}
		
		if (Physics.Raycast(raystart, transform.right, out rhf, 1.3f))
		{
			if (rhf.collider != null && rhf.collider.gameObject.tag == "wall")
			{
				canWalkRight = false;
			}
		}
		if (Physics.Raycast(raystart, -transform.right, out rhf, 1.3f))
		{
			if (rhf.collider != null && rhf.collider.gameObject.tag == "wall")
			{
				canWalkLeft = false;
			}
		}
	}

	void MoveValue(Vector3 moveval)
	{
		//校正.
		transform.position = moveToPos;

		movedir = moveval;
		moveToPos = transform.position + movedir * setp;
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
			if (Input.GetKeyDown(KeyCode.W) && canWalkForward)
			{
				MoveValue(transform.forward);
			}
			else if (Input.GetKeyDown(KeyCode.S) && canWalkBack)
			{
				MoveValue(-transform.forward);
			}
			else if (Input.GetKeyDown(KeyCode.A) && canWalkLeft)
			{			
				MoveValue(-transform.right);
			}
			else if (Input.GetKeyDown(KeyCode.D)&& canWalkRight)
			{
				MoveValue(transform.right);
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
					else if (canWalkRight)
					{
						MoveValue(transform.right);
					}
					else if (canWalkLeft)
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
					else if (canWalkBack)
					{			
						MoveValue(-transform.forward);
					}
					else if (canWalkForward)
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
					else if (canWalkForward)
					{
						MoveValue(transform.forward);
					}							
					else if (canWalkBack)
					{
						MoveValue(-transform.forward);
					}		
				}
				else
				{
					//左.						
					if (canWalkBack)
					{
						MoveValue(-transform.forward);
					}
					else if (canWalkLeft)
					{			
						MoveValue(-transform.right);
					}
					else if (canWalkRight)
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
			FindWay();
			//HandleTouchInput();
			//HandleToucehDragInput();
			HandleTouchInputSmart();
			HandleKeyCtrl(deltatime);
		}


	}
		
	void FixedUpdate ()
	{
		float deltatime = arcGamePlayTimeMgr.GetDeltaTime("player");
		UpdateMovment(deltatime);
	}


}
