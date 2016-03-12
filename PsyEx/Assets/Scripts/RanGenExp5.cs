using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RanGenExp5 : RanGen
{
	public override string GetStringFromCoord (System.Collections.Generic.List<int> coord)
	{
		string res = "";
		int i = 0;
		foreach (int c in coord) 
		{
			res = res + strLibrary [i] [c] + conSym;
			i++;
		}
		if (res.Length > 0)
			res = res.Remove (res.Length-1);
		return res;
	}

	public void InitExp5(string num, string xz)
	{
		List<string> strLi1_1 = new List<string>{num};
		List<string> strLi1_2 = new List<string>{xz};
		List<string> strLi1_3 = new List<string>{"15"};
		List<string> strLi1_4 = new List<string>{"a", "b"};
		List<List<string>> strLiLi1 = new List<List<string>> ();
		strLiLi1.Add (strLi1_1);
		strLiLi1.Add (strLi1_2);
		strLiLi1.Add (strLi1_3);
		strLiLi1.Add (strLi1_4);
		List<string> resLi1 = Utils.StringMulti (strLiLi1);
		
		
		List<string> strLi2_1 = new List<string>{num};
		List<string> strLi2_2 = new List<string>{xz};
		List<string> strLi2_3 = new List<string>{"15", "75", "135", "195"};
		List<string> strLi2_4 = new List<string>{"a", "b"};
		List<List<string>> strLiLi2 = new List<List<string>> ();
		strLiLi2.Add (strLi2_1);
		strLiLi2.Add (strLi2_2);
		strLiLi2.Add (strLi2_3);
		strLiLi2.Add (strLi2_4);
		List<string> resLi2 = Utils.StringMulti (strLiLi2);
		
		List<List<string>> strLiLi = new List<List<string>> ();
		strLiLi.Add (resLi1);
		strLiLi.Add (resLi2);
		base.Init (strLiLi, ",");
	}
}

