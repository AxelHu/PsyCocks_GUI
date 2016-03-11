using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RanGen
{
	// random result without repeat
	// remove monobehaviour when finished test
	protected List<List<string>> strLibrary;
	protected List<int> strCapacity;
	protected List<bool> drawCheck1;
	protected List<int> drawCheck2;
	protected int maxIndex;

	protected int listNum;
	protected int drawCount;
	protected string conSym = "_";

	public virtual void Init(List<List<string>> ranStringListList, string conSym = "_")
	{
		strLibrary = new List<List<string>> ();
		strCapacity = new List<int> ();
		this.strLibrary = ranStringListList;
		this.listNum = ranStringListList.Count;
		this.drawCount = 0;
		this.conSym = conSym;
		maxIndex = 1;
		foreach (List<string> strL in ranStringListList) 
		{
			if (strL.Count != 0)
				maxIndex *= strL.Count;
			strCapacity.Add (strL.Count);
		}
		strCapacity.Reverse ();
		drawCheck1 = new List<bool> (maxIndex);
		for (int i = 0; i < maxIndex; i++) 
		{
			drawCheck1.Add (false);
		}
		Debug.Log (drawCheck1.Count);

		drawCheck2 = new List<int> (maxIndex);
		for (int i = 0; i < maxIndex; i++) 
		{
			drawCheck2.Add (i);
		}
	}

	string outputString;

	int i = 0;

	// map from index number to specific string-combination
	// l1: a l2: b l3:c l4:d ...  d*c*b*a
	public List<int> GetCoordFromIndex(int index)
	{
		int i = 0;
		if (index >= maxIndex || index < 0)
			return null;
		List<int> res = new List<int> ();
		foreach (List<string> li in strLibrary) 
		{
			res.Add (index % strLibrary [i].Count);
			index /= strLibrary [i].Count;
			i++;
		}

		return res;
	}

	public virtual string GetNextRanVal()
	{
		string res = "";
		if (drawCount >= maxIndex)
			return res;
		int ranInt = Random.Range (0, maxIndex - drawCount);
		drawCount++;
		res = GetStringFromCoord(GetCoordFromIndex(drawCheck2[ranInt]));
		drawCheck2.Remove (drawCheck2 [ranInt]);
		return res;
	}

	public virtual string GetStringFromCoord(List<int> coord)
	{
		string res = "";
		int i = 0;
		foreach (int c in coord) 
		{
			res = res + strLibrary [i] [c] + conSym;
			i++;
		}
		if (res.Length > 0)
			res = res.Remove (res.Length - 1);
		return res;
	}

	public bool IsFinished()
	{
		return drawCount >= maxIndex;
	}
}
