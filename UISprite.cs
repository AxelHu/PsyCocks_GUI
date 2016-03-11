using UnityEngine;
using System.Collections;

public class UISprite : UIPanel
{
	protected Material mat;
	public override void Init (PanelData data, UIPanel parentPanel)
	{
		base.Init (data, parentPanel);
	}

	public virtual void InitWithPic(PanelData data, UIPanel parentPanel, string path)
	{
		Init (data, parentPanel);
		SetPic (path, data.width, data.height);
	}

	public virtual void SetPic(string path, float width = 0, float height = 0)
	{
		MeshRenderer meshRen = panel.GetComponent<MeshRenderer> ();
		mat = meshRen.material;
		Texture tex = Resources.Load(path) as Texture;
		if (width == 0 && height == 0)
		{
			this.width = tex.width;
			this.height = tex.height;
		}
		else
		{
			this.width = width;
			this.height = height;
		}
		mat.mainTexture = tex;
	}

	public void SetPicAndRefresh(string path)
	{
		SetPic (path);
		Reshape ();
	}
}

