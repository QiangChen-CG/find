using UnityEngine;
using System.Collections;

public class CarCtrl : MonoBehaviour {


	public enum DriveWhellType
	{
		RearWheelDrive,
		FrontWheelDrive
	}

	public Rigidbody frontWheel;
	public Rigidbody rearWheel;

	public DriveWhellType driveWhellType = DriveWhellType.FrontWheelDrive;

	public float power = 30;
	public float rot = 30;

	//油門. (0~1).
	public float Accelerator = 0;
	public float keyboardAcceleratorAdd = 1.0f;

	//方向. (-1~1).
	public float SteeringWheel = 0;
	public float keyboardSteeringWheelAdd = 1.0f;

	public float groundPowerF = 1;
	public float groundRotF = 1;

	
	public float forwardpower = 0;	
	public float rotatepower = 0;
	public Vector3 v;

	public Vector3 moveto;
	public Quaternion deltaRotation;
	public enum shiftGear
	{
		N,
		D,
		R
	}
	
	public enum rotateHandle
	{
		N,
		L,
		R
	}


	public shiftGear shift = shiftGear.N;
	public rotateHandle rothandle = rotateHandle.N;
	// Use this for initialization
	void Start ()
	{
	
	}

	void SwitchDriveType(DriveWhellType dt)
	{
		driveWhellType = dt;
		if (driveWhellType == DriveWhellType.FrontWheelDrive)
		{
			frontWheel.isKinematic = true;
			rearWheel.isKinematic = false;
		}
		else
		{
			frontWheel.isKinematic = false;
			rearWheel.isKinematic = true;
		}
	}

	void UpdateAccelerator(float deltatime)
	{
		Vector3 dir = frontWheel.transform.position - rearWheel.transform.position;
		dir.Normalize();
		
		if (Input.GetKey(KeyCode.W))
		{
			forwardpower = power;
			shift = shiftGear.D;
		}
		else if (Input.GetKey(KeyCode.S))
		{
			forwardpower = power;
			shift = shiftGear.R;
		}
		
		if (Input.GetKeyUp(KeyCode.W))
		{
		}
		if (Input.GetKeyUp(KeyCode.S))
		{
		}
		
		//break.
		if (Input.GetKey(KeyCode.Space))
		{
			forwardpower = 0;
		}
		
		//防負.
		forwardpower -= groundPowerF * deltatime;
		forwardpower = Mathf.Max(0, forwardpower);
		
		float f = 1;
		if (shift == shiftGear.D)
		{
		}
		else if (shift == shiftGear.R)
		{
			f = -1;
		}

		
		if (driveWhellType == DriveWhellType.FrontWheelDrive)
		{
			moveto = frontWheel.transform.position + (dir * forwardpower * deltatime * f);
		}
		else
		{
			moveto = rearWheel.transform.position + (dir * forwardpower * deltatime * f);
			
		}
	}
	
	void UpdateSteeringWheel(float deltatime)
	{
		float l = 1;
		if (Input.GetKey(KeyCode.A))
		{
			rothandle = rotateHandle.L;
			rotatepower = rot * forwardpower;
		}
		else if (Input.GetKey(KeyCode.D))
		{
			rothandle = rotateHandle.R;
			rotatepower = rot * forwardpower;
		}
		
//		if (rothandle == rotateHandle.L)
//		{
//			l = -1;
//		}
//		else if (rothandle == rotateHandle.R)
//		{
//			
//		}

		if (shift == shiftGear.R)
		{
			l *= -1;
		}

		rotatepower -= groundRotF * deltatime;		
		rotatepower = Mathf.Max(0, rotatepower);

		
		if (driveWhellType == DriveWhellType.FrontWheelDrive)
		{
			deltaRotation = Quaternion.Euler(frontWheel.transform.up * rotatepower * deltatime * SteeringWheel * l);
		}
		else
		{
			deltaRotation = Quaternion.Euler(rearWheel.transform.up * rotatepower * deltatime * SteeringWheel * l);
			
		}

		
	}

	void UpdateSteeringWheelCtrl(float deltatime)
	{
		if (Input.GetKey(KeyCode.A))
		{
			rothandle = rotateHandle.L;
			SteeringWheel -= keyboardAcceleratorAdd * deltatime;
			SteeringWheel = Mathf.Max(-1, SteeringWheel);
		}
		else if (Input.GetKey(KeyCode.D))
		{
			rothandle = rotateHandle.R;
			SteeringWheel += keyboardAcceleratorAdd * deltatime;
			SteeringWheel = Mathf.Min(1, SteeringWheel);
		}
		
		if (Input.GetKeyUp(KeyCode.A))
		{
			SteeringWheel = 0;
		}
		else if (Input.GetKeyUp(KeyCode.D))
		{
			SteeringWheel = 0;
		}
	}

	// Update is called once per frame
	void Update ()
	{
		float deltatime = Time.deltaTime;

		if (Input.GetKey(KeyCode.R))
		{
			SwitchDriveType(DriveWhellType.FrontWheelDrive);
		}
		else if (Input.GetKey(KeyCode.F))
		{
			SwitchDriveType(DriveWhellType.RearWheelDrive);
		}



		UpdateAccelerator(deltatime);

		UpdateSteeringWheelCtrl(deltatime);
		UpdateSteeringWheel(deltatime);
		//frontWheel.MovePosition(moveto);
		//v = frontWheel.velocity;


	}

	void FixedUpdate()
	{
		
		if (driveWhellType == DriveWhellType.FrontWheelDrive)
		{
			frontWheel.MovePosition(moveto);
			frontWheel.MoveRotation(frontWheel.transform.rotation * deltaRotation);
		}
		else
		{
			rearWheel.MovePosition(moveto);
			rearWheel.MoveRotation(rearWheel.transform.rotation * deltaRotation);

		}
	}
}

