using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Drawing;

namespace WindowsFormsApp3
{
    class Layer
    {

        
        private int current_pass;   //当前层数中第几道
        private int layer;  //当前层数
        private int pass;   //当前层的道数
        private int pass_f; //当前层结束后的总道数
        private int pass_l; //当前层开始时的道数
        private int mode;   //等高规划或等面积规划

        public List<Pass> mypass = new List<Pass>();    //储存当前层每一道的属性
        //public void get_info();   
        public int get_current_pass() { return current_pass; }
        public int get_layer() { return layer; }
        public int get_pass() { return pass; }
        public int get_pass_f() { return pass_f; }
        public int get_pass_l() { return pass_l; }
        public int get_mode() { return mode; }
        public void set_pass(int p) { pass = p; }
        public void set_pass_f(int pf) { pass_f = pf; }
        public void set_pass_l(int pl) { pass_l = pl; }
        public void set_layer(int l) { layer = l; }
        public void set_mode(int m) { mode = m; }
        public void set_current_pass(int cp) { current_pass = cp; }



    }

    class Pass
    {
        private double current; //焊接电流
        private double speed;   //焊接速度
        private double frequency;   //摆频
        private double range;   //摆幅
        private double time;    //停留时间
        private double X;   //TCP左右
        private double Z;   //TCP上下
        private double P;   //焊枪倾角
        private double extension;   //干伸长

        public double get_current() { return current; }
        public double get_frequency() { return frequency; }
        public double get_speed() { return speed; }
        public double get_range() { return range; }
        public double get_extension() { return extension; }
        public double get_time() { return time; }
        public double get_X() { return X; }
        public double get_Z() { return Z; }
        public double get_P() { return P; }
        public PointF[] pots;

        //构造函数
        public Pass(double c, double s, double f, double r, double t, double x, double z, double p, double e)
        {
            current = c;
            speed = s;
            frequency = f;
            range = r;
            time = t;
            X = x;
            Z = z;
            P = p;
            extension = e;
        }

    }
}
