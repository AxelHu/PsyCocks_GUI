using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PsyEx.Mapper
{
	[System.Serializable]
	public class ExConfig
    {
		public string exId;//实验id
		public string exName;//实验名
		public int sortId;//排序序号
		public bool setFlag;//设置状态
		public SettingConfig1 config1;
		public SettingConfig2 config2;
		public SettingConfig3 config3;
		public SettingConfig4 config4;
		public SettingConfig5 config5;

		public static ExConfig CreateFromJson(string jsonStr)
		{
			return JsonUtility.FromJson<ExConfig> (jsonStr);
		}
    }
}
