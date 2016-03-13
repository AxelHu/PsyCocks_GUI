using UnityEngine;
using System.Collections;

// derive from MonoBehaviour since it's related to a visual gameObject
public class UIPanel : MonoBehaviour
{
	public UIPanel parentPanel;
	public int depth;
	public int layer;
	public float posX;
	public float posY;
	protected float height;
	protected float width;
	public GameObject panel;
	Transform trans;

	public virtual void Init(PanelData data, UIPanel parentPanel = null)
	{
		//Debug.Log (this.gameObject);
		//Object prefab = Resources.Load ("Prefabs/UIPrototype");
		//panel = GameObject.Instantiate (prefab) as GameObject;
		//trans = panel.transform.Find ("Plane");
		trans = panel.transform;
		height = data.height;
		width = data.width;
		if(this.parentPanel == null)
			this.parentPanel = parentPanel;
		// if parent panel is 0, make this the depth 0 panel
		if(this.parentPanel == null)
		{
			depth = 0;
		}
		else
		{
			depth = this.parentPanel.depth + 1;
			this.gameObject.transform.parent = this.parentPanel.transform;
		}
		this.layer = data.layer;
		this.posX = data.positionX;
		this.posY = data.positionY;
		this.Reshape ();
		UIManager.instance.AddOnUIUpdate (this);
	}

	public virtual void OnUIUpdate()
	{
		Reshape ();
	}

	public virtual void OnDestroy()
	{
		if(this.gameObject != null)
		UIManager.instance.DelOnUIUpdate (this);
	}

	public void Reshape()
	{
		Rescale ();
		RePosition ();
		ReLayer ();
	}

	public void Rescale()
	{
		trans.localScale = new Vector3(width / 1000f, trans.localScale.y, height / 1000f);
	}

	public void RePosition()
	{
		trans.position = new Vector3 (posX/100, posY/100, trans.position.z);
	}

	public void ReLayer()
	{
		trans.position = new Vector3 (trans.position.x, trans.position.y, GetZFromLayer ());
	}

	protected float GetZFromLayer()
	{
		float res;
		if (parentPanel != null)
			res = parentPanel.trans.position.z + layer / Mathf.Pow (10, depth);
		else
			res = layer;
		//if(parentPanel != null)
		//Debug.Log (parentPanel.trans.position.z);
		//Debug.Log (res);
		return res;
	}

	public void SetLayer(int layer)
	{
		this.layer = layer;
	}

	public void SetPosition(float x, float y)
	{
		posX = x;
		posY = y;
	}

	public void SetScale(float width, float height)
	{
		this.width = width;
		this.height = height;
	}
}



public struct PanelData
{
	public void Init(float height, float width, float positionX, float positionY, int layer)
	{
		this.height = height;
		this.width = width;
		this.positionX = positionX;
		this.positionY = positionY;
		this.layer = layer;
	}

	public float height;
	public float width;
	public float positionX;
	public float positionY;
	public int layer;
}