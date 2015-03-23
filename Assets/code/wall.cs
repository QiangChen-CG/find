using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class wall : MonoBehaviour
{
	public Color color;

	public Material clonemateril = null;
	void OnDestroy()
	{
		if (clonemateril != null)
		{
			if (Application.isEditor)
			{
				GameObject.DestroyImmediate(clonemateril);
				clonemateril = null;
			}
			else
			{
				GameObject.Destroy(clonemateril);
				clonemateril = null;
			}
		}
	}

	// Use this for initialization
	void Start ()
	{
		if (clonemateril == null)
		{
			if (Application.isEditor)
			{
				clonemateril = new Material(GetComponent<Renderer>().sharedMaterial);
				clonemateril.color = color;
				GetComponent<Renderer>().sharedMaterial = clonemateril;
			}
			else
			{
				clonemateril = new Material(GetComponent<Renderer>().material);
				clonemateril.color = color;
				GetComponent<Renderer>().material = clonemateril;
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Application.isEditor)
		{
			if (clonemateril != null)
			{
				GetComponent<Renderer>().sharedMaterial.color = color;
			}
		}
	}
}
