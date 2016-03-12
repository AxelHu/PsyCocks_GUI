using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PsyEx.Mapper
{
	[System.Serializable]
    public class SettingConfig4
    {
        //球颜色
        public int ballColorR;
		public int ballColorG;
		public int ballColorB;
        //背景色
		public int backgroundColorR;
		public int backgroundColorG;
		public int backgroundColorB;
        //遮挡物颜色
		public int shelterColorR;
		public int shelterColorG;
		public int shelterColorB;

		public int ballRadius;//球半径
		public int shelterRadius;//遮挡物半径
		public int ballCenterDis;//球到中心的距离

        //速度
		public int speed1;
		public int speed2;
		public int speed3;

        //起始位置
		public bool left;
		public bool right;
		public bool up;
		public bool down;

		public bool feedback;//反馈
		public int repeatNum;//重复次数
		public int timeInterval;//时间间隔
    }
}
