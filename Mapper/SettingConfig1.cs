using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PsyEx.Mapper
{
	[System.Serializable]
    public class SettingConfig1
    {
        public string backgroundColor;//背景颜色
		public int testTime;//测试时间
		public int testNum;//测试次数

		public string particle;//运动轨迹
		public int direction;//运动方向：0顺时针1逆时针
		public bool pause;//暂停点
		public int pauseRate;//暂停频率
		public int moveMode;//运动方式：1平移+滚转0平移

		public int ctrlDirection;//控制方向0正向1反向
		public int speedMode;//速度模式0匀速1匀加速
		public double speed;//起始速度
		public double minSpeed;//最小速度
		public double maxSpeed;//最大速度
		public double minASpeed;//最小加速度
		public double maxASpeed;//最大加速度

		public double minGTASpeed;//滚转最小角速度
		public double maxGTASpeed;//滚转最大角速度
		public double minAngle;//滚转最小角度
		public double maxAngel;//滚转最大角度
    }
}
