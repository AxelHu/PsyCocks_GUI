using UnityEngine;
using System.Collections;

public class OrthoCam : MonoBehaviour
{
	Camera cam;
	// Use this for initialization
	void Start ()
	{
		cam = this.gameObject.GetComponent<Camera> ();
	}

	public void Init()
	{
		cam.orthographic = true;
	}

	// Update is called once per frame
	void Update ()
	{
		float size = (cam.rect.yMax - cam.rect.yMin) * Screen.height * 0.5f/100f;
		if (!Mathf.Approximately(cam.orthographicSize, size)) cam.orthographicSize = size;
	}
}

