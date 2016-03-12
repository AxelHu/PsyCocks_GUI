using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;


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
			if(str.Length > 0)
				str = str.Remove (str.Length-1);
			res.Add(str);
		}
		//Debug.Log (maxNum);
		return res;
	}

	public static List<string> SplitString(string str, string conSymbol)
	{
		List<string> res = new List<string>();
		string res1 = "";
		string res2 = "";
		int index = str.IndexOf (conSymbol);
		res1 = str.Substring (0, index);
		res2 = str.Substring (index+1);
		res.Add (res1);
		res.Add (res2);
		return res;
	}
		

	public static Vector2 GetV2FromV3(Vector3 v3)
	{
		return new Vector2 (v3.x, v3.y);
	}


    public static bool DoFileOutput(string path,string fileName,List<string> textList)
    {
        bool flag = false;
        FileStream fs = null;
        StreamWriter sw = null;

        try
        {
            if(File.Exists(path + "/" + fileName))
            {
                fs = new FileStream(path + "/" + fileName, FileMode.Truncate, FileAccess.Write);
            }
            else
            {
                fs = new FileStream(path + "/" + fileName, FileMode.Append, FileAccess.Write);
            }

            sw = new StreamWriter(fs, Encoding.UTF8);
            foreach (string item in textList)
            {
                sw.WriteLine(item);
            }
            flag = true;
        }
        catch (Exception)
        {
        }
        finally
        {
            sw.BaseStream.Flush();
            fs.Flush();
            sw.Close();
            fs.Close();
            
        }

        return flag;
    }
		
    public static List<Dictionary<string,string>> DoFileInput(string fileName)
    {
        List<Dictionary<string, string>> DataList = new List<Dictionary<String, String>>();
        FileStream fs = null;
        StreamReader sr = null;

        try
        {
            fs = new FileStream(fileName, FileMode.Open);
            sr = new StreamReader(fs);
            Dictionary<string, string> data = new Dictionary<String, String>();
            while (sr.Peek()>= 0)
            {
                string str, key, value;
                str = sr.ReadLine();
                key = str.Substring(0, str.IndexOf("="));
                value = str.Substring(str.IndexOf("=") + 1);
                data.Add(key, value);
            }
            DataList.Add(data);
        }
        catch (Exception)
        {
        }
        finally
        {
            sr.BaseStream.Flush();
            fs.Flush();
            sr.Close();
            fs.Close();                
        }

        return DataList;
    }
		
    public static string DoFileJsonInput(string fileName)
    {
        string jsoninput = "";
        FileStream fs = null;
        StreamReader sr = null;

        try
        {
            fs = new FileStream(fileName, FileMode.Open);
            sr = new StreamReader(fs);

            jsoninput = sr.ReadLine();
        }
        catch (Exception)
        {
        }
        finally
        {
            sr.BaseStream.Flush();
            fs.Flush();
            sr.Close();
            fs.Close();
        }
        return jsoninput;
    }

    public static bool DoFileOutputLine(string path, string fileName, List<string> textList)
    {
        bool flag = false;
        FileStream fs = null;
        StreamWriter sw = null;
        try
        {
            fs = new FileStream(path + "/" + fileName, FileMode.Append, FileAccess.Write);
            sw = new StreamWriter(fs, Encoding.UTF8);
            string data = "";
            foreach(string str in textList)
            {
                string convert = str;
                if (str.Contains(','))
                {
                    convert = string.Format("\"{0}\"", str);
                }
                data += convert + ",";
            }
            data = data.Substring(0, data.Length - 1);
            sw.WriteLine(data);
            sw.WriteLine();
            flag = true;           
        }
        catch (Exception)
        {
        }
        finally
        {
            sw.BaseStream.Flush();
            fs.Flush();
            sw.Close();
            fs.Close();
        }
        return flag;
    }

    public static string MakeDirectoy(string foldername)
    {

        string sPath = Directory.GetCurrentDirectory() + "\\" + foldername;
        if (!Directory.Exists(sPath))
        {
            Directory.CreateDirectory(sPath);
        }
        return sPath;
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

