using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameCameraMT : MonoBehaviour {

	public enum CamPlayMode
	{
		My = 0,
		ByOther
	}

	public class CamData
	{

	}

	//鏡頭起始位置.
	public GameObject defaultRefPosObj;
	Vector3 refDeltapos;

	public GameObject currentTarget;
	public GameObject target1;
	public GameObject target2;
	public GameObject targetArvg;//多 target 時的平均位置點.

	Quaternion startRotate;
	public CamPlayMode camPlayMode = CamPlayMode.My;

	// Use this for initialization
	public Vector3 lastpos;
	public bool follow = false;
	public bool Follow
	{
		get	{return follow;}
		set {follow = value;}
	}
	public bool lookat = false;
	public bool Lookat
	{
		get	{return lookat;}
		set {lookat = value;}
	}

    public bool isDelayCam = true;
    public float MoveFactor = 5.0f;

	//
	float olddist = 0.0f;
	float oldOthsize = 0.0f;
	public float othSizeAdd = 0.0f;

	public void SwitchPlayMode(CamPlayMode pm)
	{
		camPlayMode = pm;
		
		switch(camPlayMode)
		{
		case CamPlayMode.My:
		{
			//transform.position = lastPlayPos;
			transform.rotation = startRotate;
			break;
		}
		case CamPlayMode.ByOther:
		{
			//lastPlayPos = transform.position;
			//startRotate = transform.rotation;
			break;
		}
		}
	}

	void Start ()
	{
		oldOthsize = Camera.main.orthographicSize;
		startRotate = transform.rotation;

		//算出偏移.
		if (defaultRefPosObj != null)
		{
			refDeltapos = transform.position - defaultRefPosObj.transform.position;
		}

		//建出多 target 時的平均位置點.
		if (targetArvg == null)
		{
			targetArvg = new GameObject("targetArvg");
			targetArvg.transform.parent = gameObject.transform;
		}

		//至少要有一個 tartet.
		if (target1 == null)
		{
			return;
		}

		CaluteCam();

	}

	public void CaluteCam()
	{
		currentTarget = target1;
		
		if (target1 != null && target2 != null)
		{
			targetArvg.transform.position = Vector3.Lerp(target1.transform.position, target2.transform.position, 0.5f);
			currentTarget = targetArvg;
			float dist = Vector3.Distance(target1.transform.position, target2.transform.position);
			olddist = dist;
		}

	}


	public void setTarget1(GameObject go)
	{
		target1 = go;
		CaluteCam();
	}
	public void setTarget2(GameObject go)
	{
		target2 = go;
		CaluteCam();
	}

	// Update is called once per frame
	void LateUpdate ()
	{
		if (currentTarget == null)
		{
			return;
		}
		
		if (target1 != null && target2 != null)
		{
			//中間的位置.
			targetArvg.transform.position = Vector3.Lerp(target1.transform.position, target2.transform.position, 0.5f);
			float dist = Vector3.Distance(target1.transform.position, target2.transform.position);

			//防止為0.
			if (dist == 0)
			{
				Camera.main.orthographicSize = oldOthsize;
			}
			else
			{
				float o = (dist * oldOthsize) / olddist;
				Camera.main.orthographicSize = o + othSizeAdd;
			}
		}

		
		switch(camPlayMode)
		{
		case CamPlayMode.My:
		{
			if (follow)
			{   
				if (isDelayCam)
				{
					Vector3 MoveToPos = currentTarget.transform.position + refDeltapos;
					transform.position = Vector3.Lerp(transform.position, MoveToPos, Time.deltaTime * MoveFactor);			

				}
				else
				{
					transform.position = currentTarget.transform.position + refDeltapos;
				}				
			}
			
			if (lookat)
			{
				transform.LookAt(currentTarget.transform);
			}
			break;
		}
		case CamPlayMode.ByOther:
		{
			break;
		}
		}

	}

	
	void OnPostRender()
	{
	}
}
