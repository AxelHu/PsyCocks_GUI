using UnityEngine;
using System.Collections;

[System.Serializable]
public class ExConfigList
{
	public PsyEx.Mapper.ExConfig[] exConfigList;

	public static ExConfigList CreateFromJson(string jsonString)
	{
		return JsonUtility.FromJson<ExConfigList> (jsonString);
	}
}

