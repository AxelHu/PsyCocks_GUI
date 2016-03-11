using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PsyEx.Mapper
{
	[System.Serializable]
    public class SettingConfig3
    {
		public bool mainTest;//主任务

        public string moveTrail;//运动轨迹
        public int direction;//运动方向
        public int speedMode;//速度模式0匀速1匀加速
        public double speed;//起始速度
        public double minSpeed;//最小速度
        public double maxSpeed;//最大速度
        public double minASpeed;//最小加速度
        public double maxASpeed;//最大加速度

        public int ctrlDirection;//控制方向0正向1反向
        public string backgroundColor;//背景颜色
        public bool feedback;//反馈

        public int secTestMode;//次任务模式0简单反应1选择反应
        public bool leftUp;
        public bool leftDown;
        public bool rightUp;
        public bool rightDown;

        public int plane;//飞机比率
        public int copter;//直升机比率
        public int viewTime;//呈现时间
        public int waitTime;//等待反应时间
        public int minTimeSpace;
        public int maxTimeSpace;
        public int testNum;//测试次数
    }
}
