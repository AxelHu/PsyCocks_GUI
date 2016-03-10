using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Utils
{
	public static float GetDeltaTime()
	{
		return Time.deltaTime;
	}
	
	public  static List<string> StringMulti(List<List<string>> strLiLi, string conSymbol = "_")
	{
		int strListNum = strLiLi.Count;
		int maxNum = 1;
		foreach (List<string> strLi in strLiLi) 
		{
			if(strLiLi.Count != 0)
				maxNum *= strLi.Count;
		}

		List<string> res = new List<string> (maxNum);
		for(int i = 0; i < maxNum; i++)
		{
			string str = "";
			int j = 0;
			int index = i;
			foreach (List<string> li in strLiLi) 
			{
				int c = index % strLiLi [j].Count;
				index /= strLiLi [j].Count;
				str = str + strLiLi[j][c] + conSymbol;
				j++;
			}
			str = str.Remove (str.Length-1);
			res.Add(str);
		}
		//Debug.Log (maxNum);
		return res;
	}

	/*
	// test code
	List<string> a1 = new List<string>{"a", "b"};
	List<string> a2 = new List<string>{"1", "2", "3"};
	List<string> a3 = new List<string>{"x", "y", "z", "w"};

	List<List<string>> aaa = new List<List<string>> ();
	List<string> res;
	public void Start()
	{
		aaa.Add (a1);
		aaa.Add (a2);
		aaa.Add (a3);

		res = StringMulti (aaa);
		foreach(string a in res)
		{
			Debug.Log (a);
		}
	}
	*/

}

