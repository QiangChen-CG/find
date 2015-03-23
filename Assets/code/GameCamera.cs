using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {

	public enum CamPlayMode
	{
		My = 0,
		ByOther
	}

	public GameObject target;
	public GameObject defaultRefPosObj;	
	Vector3 refDeltapos;
	//Vector3 lastPlayPos;
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
    Vector3 startOffset = new Vector3();
    public float MoveFactor = 5.0f;
    bool isFirstRunUpdate = true;

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
		if (target == null)
		{
			return;
		}

		lastpos = target.transform.position;
		startRotate = transform.rotation;
	}

	public void setTarget(GameObject go)
	{
		target = go;

		if (defaultRefPosObj != null)
		{
			refDeltapos = transform.position - defaultRefPosObj.transform.position;

			transform.position = refDeltapos;
		}


		lastpos = target.transform.position;

	}

	// Update is called once per frame
	void LateUpdate ()
	{
		if (target == null)
		{
			return;
		}

        if (isFirstRunUpdate)
        {
            isFirstRunUpdate = false;
            startOffset = target.transform.position - transform.position;
        }
		
		switch(camPlayMode)
		{
		case CamPlayMode.My:
		{
			if (follow)
			{   
				if (isDelayCam)
				{
					Vector3 MoveToPos = target.transform.position - startOffset;
					transform.position = Vector3.Lerp(transform.position, MoveToPos, Time.deltaTime * MoveFactor);
				}
				else
				{
					Vector3 pos = target.transform.position - lastpos;
					lastpos = target.transform.position;
					transform.Translate(pos, Space.World);
				}
				
			}
			
			if (lookat)
			{
				transform.LookAt(target.transform);
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
		//PoseInputScript.DebugDraw();
	}
}
