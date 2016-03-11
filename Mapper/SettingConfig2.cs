using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PsyEx.Mapper
{
	[System.Serializable]
    public class SettingConfig2
    {
		public bool mainTest;//主任务

		public string moveTrail;//运动轨迹
		public int direction;//运动方向
		public int speedMode;//速度模式0匀速1匀加速
		public double speed;//起始速度
		public double fspeed;//加速度起始速度
		public double minSpeed;//最小速度
		public double maxSpeed;//最大速度
		public double minASpeed;//最小加速度
		public double maxASpeed;//最大加速度

		public int ctrlDirection;//控制方向0正向1反向
		public string backgroundColor;//背景颜色
		public bool feedback;//反馈

		public int estimateNum;//待估次数
		public double estimate1;
		public double estimate2;
		public double estimate3;
		public double estimate4;
		public double estimate5;
		public double estimate6;
		public double estimate7;
		public double estimate8;
		public double estimate9;
		public double estimate10;
		public double estimate11;
		public double estimate12;
		public int loop;//循环次数
    }
}
