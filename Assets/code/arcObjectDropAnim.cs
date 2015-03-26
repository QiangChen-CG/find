using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class arcObjectDropAnim : MonoBehaviour
{
	[Serializable]
	public class DropData
	{
		public GameObject go;
		public float speed = 1;
		public float currentTweenTime = 0;

		public Vector3 startpos = Vector3.zero;
		public Vector3 endpos = Vector3.zero;

		public bool isFinish()
		{
			return currentTweenTime >= 1;
		}

		public void Reset()
		{
			currentTweenTime = 0;
		}

		public bool Update(float deltatime)
		{
			bool finish = false;
			currentTweenTime += deltatime;
			if (currentTweenTime >= 1)
			{
				currentTweenTime = 1;
				finish = true;
			}
			Vector3 tweenpos = Vector3.zero;
			tweenpos.x = iTweenLite.Easing(startpos.x, endpos.x, currentTweenTime, iTweenLite.EaseType.easeOutElastic);
			tweenpos.y = iTweenLite.Easing(startpos.y, endpos.y, currentTweenTime, iTweenLite.EaseType.easeOutElastic);
			tweenpos.z = iTweenLite.Easing(startpos.z, endpos.z, currentTweenTime, iTweenLite.EaseType.easeOutElastic);

			go.transform.position = tweenpos;

			return finish;
		}
	}
	
	[Serializable]
	public class DropDataG
	{
		public GameObject gameobject;
		public float speed = 1;
		
		public float bunce = 0;
		public Vector3 startVelocity = Vector3.zero;
		public Vector3 startPos = Vector3.zero;

		public Vector3 velocity = Vector3.zero;
		public Vector3 velocityUpAdd = Vector3.zero;
		public Vector3 velocityDownAdd = Vector3.zero;
		
		public bool isLastGrounded = false;
		public bool isGrounded = true;	
		public bool isSleep = true;	

		//rand.		
		public Vector3 startVelocityMin = Vector3.zero;
		public Vector3 startVelocityMax = Vector3.zero;

		public bool isRand = false;

		public void inital()
		{
			startPos = gameobject.transform.position;
		}

		public bool isFinish()
		{
			return isSleep;
		}
		
		public void Reset()
		{
			isSleep = false;
			gameobject.transform.position = startPos;
			if (isRand)
			{
				velocity.x = UnityEngine.Random.Range(startVelocityMin.x, startVelocityMax.x);
				velocity.y = UnityEngine.Random.Range(startVelocityMin.y, startVelocityMax.y);
				velocity.z = UnityEngine.Random.Range(startVelocityMin.z, startVelocityMax.z);

			}
			else
			{				
				velocity = startVelocity;
			}



		}
		
		public bool Update(float deltatime)
		{
			if (isSleep)
			{
				return true;
			}

			float fixdeltatime = speed * deltatime;

			isGrounded = false;

			if (velocity.y > 0)
			{
				velocity += velocityUpAdd * fixdeltatime;
				if (velocity.y < 0)
				{
					velocity.y = 0;
				}
			}
			else
			{
				velocity += velocityDownAdd * fixdeltatime;
			}

			//位移.
			Vector3 pos = gameobject.transform.position;
			pos.y += velocity.y * fixdeltatime;
			pos.x += velocity.x * fixdeltatime;
			pos.z += velocity.z * fixdeltatime;

			//彈跳.
			if (pos.y < startPos.y)
			{
				pos.y = startPos.y;
				
				float b = velocity.y * bunce * -1;
				velocity.y = b;
			}

			//撞到地面.
			if (pos.y <= startPos.y)
			{
				isGrounded = true;
			}

			//event.
			if (isLastGrounded != isGrounded)
			{
				isLastGrounded = isGrounded;
				
				if (isGrounded)
				{
					//fightdata.onMonsterGround(this);
				}
				else
				{
					//fightdata.onMonsterOnSky(this);
				}
			}

			//彈不上去了. 就 sleep.
			if (pos.y <= startPos.y && velocity.y < Mathf.Abs(velocityUpAdd.y * deltatime))
			{
				isSleep = true;
			}
			
			gameobject.transform.position = pos;

			return false;
		}
	}
	public DropDataG[] drops;
	public bool startDrop = false;


	// Use this for initialization
	void Start ()
	{
		for (int i =0; i < drops.Length; i++)
		{
			drops[i].inital();
		}
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (startDrop)
		{
			float deltatime = Time.deltaTime;
			UpdateDrop(deltatime);
		}
	}

	void OnGUI()
	{
		if (GUILayout.Button("drop"))
		{
			StartDrop();
		}
	}

	void StartDrop()
	{
		startDrop = true;
		for (int i =0; i < drops.Length; i++)
		{
			drops[i].Reset();
		}

	}

	void UpdateDrop(float deltatime)
	{
		bool allfinish = true;
		for (int i =0; i < drops.Length; i++)
		{
			if (!drops[i].isFinish())
			{
				allfinish &= drops[i].Update(deltatime);
			}
		}

		if (allfinish)
		{
			startDrop = false;
		}
	}
}
