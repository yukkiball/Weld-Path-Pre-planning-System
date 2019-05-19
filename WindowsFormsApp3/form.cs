using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Collections;
using System.Drawing.Drawing2D;


namespace WindowsFormsApp3
{
    public partial class form : Form
    {
        //机器人连接成员
        public string HostName;
        private Random rnd = new Random();
        private const string cnstApp = "weld";
        private const string cnstSection = "setting";

        private FRRJIf.Core mobjCore;
        private FRRJIf.DataTable mobjDataTable;
        private FRRJIf.DataTable mobjDataTable2;
        private FRRJIf.DataCurPos mobjCurPos;
        private FRRJIf.DataCurPos mobjCurPosUF;
        private FRRJIf.DataCurPos mobjCurPos2;
        private FRRJIf.DataTask mobjTask;
        private FRRJIf.DataTask mobjTaskIgnoreMacro;
        private FRRJIf.DataTask mobjTaskIgnoreKarel;
        private FRRJIf.DataTask mobjTaskIgnoreMacroKarel;
        private FRRJIf.DataPosReg mobjPosReg;
        private FRRJIf.DataPosReg mobjPosReg2;
        private FRRJIf.DataPosRegXyzwpr mobjPosRegXyzwpr;
        private FRRJIf.DataSysVar mobjSysVarInt;
        private FRRJIf.DataSysVar mobjSysVarInt2;
        private FRRJIf.DataSysVar mobjSysVarReal;
        private FRRJIf.DataSysVar mobjSysVarReal2;
        private FRRJIf.DataSysVar mobjSysVarString;
        private FRRJIf.DataSysVarPos mobjSysVarPos;
        private FRRJIf.DataSysVar[] mobjSysVarIntArray;
        private FRRJIf.DataNumReg mobjNumReg;
        private FRRJIf.DataNumReg mobjNumReg2;
        private FRRJIf.DataNumReg mobjNumReg3;
        private FRRJIf.DataAlarm mobjAlarm;
        private FRRJIf.DataAlarm mobjAlarmCurrent;
        private FRRJIf.DataSysVar mobjVarString;
        private FRRJIf.DataString mobjStrReg;
        private FRRJIf.DataString mobjStrRegComment;

        //软件实现静态成员
        public static double height = 0;
        public static double angle = 0;
        public static double gap = 0;
        public static double edge = 0;
        public static int piles = 0;    //总层数
        public static int sum_pass = 0;     //总道数
        public static int mode = 1;
        private static Layer[] mylayer = new Layer[100];
        private static double PI = 3.14159265;
        private static SolidBrush[] mybrush = new SolidBrush[10000];
        private static Random ran = new Random();
        private static int i = 0;   //输入时记录层数
        private static Pen pen = new Pen(Color.Red, 3);
        private static Pen pen2 = new Pen(Color.Blue, 3);




    public form()
        {
            InitializeComponent();
            //设置文本框高度
            textBox1.AutoSize = false;
            textBox2.AutoSize = false;
            textBox3.AutoSize = false;
            textBox4.AutoSize = false;
            textBox5.AutoSize = false;
            textBox6.AutoSize = false;
            textBox7.AutoSize = false;
            textBox8.AutoSize = false;
            textBox9.AutoSize = false;
            textBox10.AutoSize = false;
            textBox11.AutoSize = false;
            textBox12.AutoSize = false;
            textBox13.AutoSize = false;
            textBox1.Height = 20;
            textBox2.Height = 20;
            textBox3.Height = 20;
            textBox4.Height = 20;
            textBox5.Height = 20;
            textBox6.Height = 20;
            textBox7.Height = 20;
            textBox8.Height = 20;
            textBox9.Height = 20;
            textBox10.Height = 20;
            textBox11.Height = 20;
            textBox12.Height = 20;
            textBox13.Height = 20;
            for (int m = 0; m < 100; m++)
            {
                mylayer[m] = new Layer();
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            //连接数据库
            string stpath = Application.StartupPath;
            string connectionstring = "Data Source= " + Application.StartupPath + "\\test.db";
            SQLiteConnection connection = new SQLiteConnection(connectionstring);
            connection.Open();
            //选择所需参数
            string sql = "SELECT layer, pass, X, Z, P, extension, current, speed, frequency, range, time FROM welding WHERE height = {0} AND angle = {1} AND gap = {2} AND edge = {3} AND mode = {4}";
            string s = string.Format(sql, height, angle, gap, edge, mode);
            //Console.WriteLine(s);
            SQLiteDataAdapter da = new SQLiteDataAdapter(s, connection);
            DataSet ds = new DataSet();
            da.Fill(ds);

            //数据表格测试（从数据库中提取表格并存入变量d1）
            DataTable d1 = ds.Tables[0];
            //Console.WriteLine(d1.Rows[0]["layer"]);

            //设置控件内容与标题
            dataGridView1.DataSource = ds.Tables[0];
            dataGridView1.Columns[0].HeaderCell.Value = "层数";
            dataGridView1.Columns[1].HeaderCell.Value = "道数";
            dataGridView1.Columns[2].HeaderCell.Value = "TCP左右";
            dataGridView1.Columns[3].HeaderCell.Value = "TCP上下";
            dataGridView1.Columns[4].HeaderCell.Value = "焊枪倾角";
            dataGridView1.Columns[5].HeaderCell.Value = "干伸长";
            dataGridView1.Columns[6].HeaderCell.Value = "焊接电流";
            dataGridView1.Columns[7].HeaderCell.Value = "焊接电压";
            dataGridView1.Columns[8].HeaderCell.Value = "摆频";
            dataGridView1.Columns[9].HeaderCell.Value = "摆幅";
            dataGridView1.Columns[10].HeaderCell.Value = "停留时间";
            //设置列宽
            for (int i = 0; i < 11; i++)
            {
                dataGridView1.Columns[i].Width = 89;
            }

        }



        //连接机器人
        private void button8_Click(object sender, EventArgs e)
        {
            //连接
            if (mobjCore == null)
            {
                //connect
                msubInit();
            }
            else
            {
                //disconnect
                mobjCore.Disconnect();
                msubDisconnected2();
            }
        }


        //初始化，生成Datatable
        private void msubInit()
        {
            bool blnRes = false;
            string strHost = null;
            int lngTmp = 0;

            try
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

                mobjCore = new FRRJIf.Core();

                // You need to set data table before connecting.
                mobjDataTable = mobjCore.get_DataTable();

                {
                    mobjAlarm = mobjDataTable.AddAlarm(FRRJIf.FRIF_DATA_TYPE.ALARM_LIST, 5, 0);
                    mobjAlarmCurrent = mobjDataTable.AddAlarm(FRRJIf.FRIF_DATA_TYPE.ALARM_CURRENT, 1, 0);
                    mobjCurPos = mobjDataTable.AddCurPos(FRRJIf.FRIF_DATA_TYPE.CURPOS, 1);
                    mobjCurPosUF = mobjDataTable.AddCurPosUF(FRRJIf.FRIF_DATA_TYPE.CURPOS, 1, 15);
                    mobjCurPos2 = mobjDataTable.AddCurPos(FRRJIf.FRIF_DATA_TYPE.CURPOS, 2);
                    mobjTask = mobjDataTable.AddTask(FRRJIf.FRIF_DATA_TYPE.TASK, 1);
                    mobjTaskIgnoreMacro = mobjDataTable.AddTask(FRRJIf.FRIF_DATA_TYPE.TASK_IGNORE_MACRO, 1);
                    mobjTaskIgnoreKarel = mobjDataTable.AddTask(FRRJIf.FRIF_DATA_TYPE.TASK_IGNORE_KAREL, 1);
                    mobjTaskIgnoreMacroKarel = mobjDataTable.AddTask(FRRJIf.FRIF_DATA_TYPE.TASK_IGNORE_MACRO_KAREL, 1);
                    mobjPosReg = mobjDataTable.AddPosReg(FRRJIf.FRIF_DATA_TYPE.POSREG, 1, 1, 10);
                    mobjPosReg2 = mobjDataTable.AddPosReg(FRRJIf.FRIF_DATA_TYPE.POSREG, 2, 1, 4);
                    mobjSysVarInt = mobjDataTable.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_INT, "$FAST_CLOCK");
                    mobjSysVarInt2 = mobjDataTable.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_INT, "$TIMER[10].$TIMER_VAL");
                    mobjSysVarReal = mobjDataTable.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_REAL, "$MOR_GRP[1].$CURRENT_ANG[1]");
                    mobjSysVarReal2 = mobjDataTable.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_REAL, "$DUTY_TEMP");
                    mobjSysVarString = mobjDataTable.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_STRING, "$TIMER[10].$COMMENT");
                    mobjSysVarPos = mobjDataTable.AddSysVarPos(FRRJIf.FRIF_DATA_TYPE.SYSVAR_POS, "$MNUTOOL[1,1]");
                    mobjVarString = mobjDataTable.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_STRING, "$[HTTPKCL]CMDS[1]");
                    mobjNumReg = mobjDataTable.AddNumReg(FRRJIf.FRIF_DATA_TYPE.NUMREG_INT, 6, 10);
                    mobjNumReg2 = mobjDataTable.AddNumReg(FRRJIf.FRIF_DATA_TYPE.NUMREG_REAL, 1, 5);
                    mobjPosRegXyzwpr = mobjDataTable.AddPosRegXyzwpr(FRRJIf.FRIF_DATA_TYPE.POSREG_XYZWPR, 1, 1, 10);
                    mobjStrReg = mobjDataTable.AddString(FRRJIf.FRIF_DATA_TYPE.STRREG, 1, 3);
                    mobjStrRegComment = mobjDataTable.AddString(FRRJIf.FRIF_DATA_TYPE.STRREG_COMMENT, 1, 3);
                    Debug.Assert(mobjStrRegComment != null);
                }

                // 2nd data table.
                // You must not set the first data table.
                mobjDataTable2 = mobjCore.get_DataTable2();
                mobjNumReg3 = mobjDataTable2.AddNumReg(FRRJIf.FRIF_DATA_TYPE.NUMREG_INT, 1, 5);
                mobjSysVarIntArray = new FRRJIf.DataSysVar[10];
                mobjSysVarIntArray[0] = mobjDataTable2.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_INT, "$TIMER[1].$TIMER_VAL");
                mobjSysVarIntArray[1] = mobjDataTable2.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_INT, "$TIMER[2].$TIMER_VAL");
                mobjSysVarIntArray[2] = mobjDataTable2.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_INT, "$TIMER[3].$TIMER_VAL");
                mobjSysVarIntArray[3] = mobjDataTable2.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_INT, "$TIMER[4].$TIMER_VAL");
                mobjSysVarIntArray[4] = mobjDataTable2.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_INT, "$TIMER[5].$TIMER_VAL");
                mobjSysVarIntArray[5] = mobjDataTable2.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_INT, "$TIMER[6].$TIMER_VAL");
                mobjSysVarIntArray[6] = mobjDataTable2.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_INT, "$TIMER[7].$TIMER_VAL");
                mobjSysVarIntArray[7] = mobjDataTable2.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_INT, "$TIMER[8].$TIMER_VAL");
                mobjSysVarIntArray[8] = mobjDataTable2.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_INT, "$TIMER[9].$TIMER_VAL");
                mobjSysVarIntArray[9] = mobjDataTable2.AddSysVar(FRRJIf.FRIF_DATA_TYPE.SYSVAR_INT, "$TIMER[10].$TIMER_VAL");

                //get host name
                if (string.IsNullOrEmpty(HostName))
                {
                    strHost = Interaction.GetSetting(cnstApp, cnstSection, "HostName", "");
                    strHost = Interaction.InputBox("Please input robot host name", "Welding", strHost, 0, 0);
                    if (string.IsNullOrEmpty(strHost))
                    {
                        System.Environment.Exit(0);
                    }
                    Interaction.SaveSetting(cnstApp, cnstSection, "HostName", strHost);
                    HostName = strHost;
                }
                else
                {
                    strHost = HostName;
                }

                //get time out value
                lngTmp = Convert.ToInt32(Interaction.GetSetting(cnstApp, cnstSection, "TimeOut", "-1"));

                //connect
                if (lngTmp > 0)
                    mobjCore.set_TimeOutValue(lngTmp);
                blnRes = mobjCore.Connect(strHost);
                if (blnRes == false)
                {
                    msubDisconnected();
                }
                else
                {
                    msubConnected();
                }

                System.Windows.Forms.Cursor.Current = Cursors.Default;
                return;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message);
                System.Environment.Exit(0);
            }


        }

        //连接成功
        private void msubConnected()
        {

            //txtResult.Text = "Connect OK to " + HostName;
            //lblConnect.Text = txtResult.Text;
            MessageBox.Show("Connected");
            //this.Text = HostName + " - FRRJIf Test";

            //msubSetTestControls(true);
            //cmdConnect.Text = "Disconnect";

            //timLoop.Enabled = true;

        }

        //连接失败
        private void msubDisconnected()
        {

            //disabled continous
            //timLoop.Enabled = false;

            MessageBox.Show("Connect error");

            //txtResult.Text = "Connect Failed to " + HostName;
            //lblConnect.Text = txtResult.Text;
            //this.Text = " Weld Test";

            //msubClearVars();

            //msubSetTestControls(false);
            //cmdConnect.Text = "Connect";

        }

        private void msubDisconnected2()
        {

            //disabled continous
            //timLoop.Enabled = false;

            //txtResult.Text = "Disconnect to " + HostName;
            // & " (" & mobjCore.ProtectStatus & ")"
            //lblConnect.Text = txtResult.Text;
            MessageBox.Show("Connect error");
            //this.Text = "Weld Test";

            //msubClearVars();

            //msubSetTestControls(false);
            //cmdConnect.Text = "Connect";

        }


        //写入寄存器
        private void button9_Click(object sender, EventArgs e)
        {
            //写入数值变量寄存器
            int[] intValues = new int[101];
            float[] sngValues = new float[101];

            
            {
                //整型变量
                //    for (ii = 0; ii <= mobjNumReg.EndIndex - mobjNumReg.StartIndex; ii++)
                //    {
                //        intValues[ii] = (ii + 1) * intRand;
                //    }
                //    if (mobjNumReg.SetValues(mobjNumReg.StartIndex, intValues, mobjNumReg.EndIndex - mobjNumReg.StartIndex + 1) == false)
                //    {
                //        MessageBox.Show("SetNumReg Int Error");
                //    }

                //}
                //{
                //浮点型变量
                //    for (ii = 0; ii <= mobjNumReg2.EndIndex - mobjNumReg2.StartIndex; ii++)
                //    {
                //        sngValues[ii] = (float)((ii + 1) * intRand * 1.1);
                //    }
                //    if (mobjNumReg2.SetValues(mobjNumReg2.StartIndex, sngValues, mobjNumReg2.EndIndex - mobjNumReg2.StartIndex + 1) == false)
                //    {
                //        MessageBox.Show("SetNumReg Real Error");
                //    }


                List<float> l_fre = new List<float>();  //摆频
                List<float> l_ran = new List<float>();  //摆幅
                for (int m = 0; m < piles; m++)
                {
                    for (int n = 0; n < mylayer[m].get_pass(); n++)
                    {
                        l_fre.Add(Convert.ToSingle(mylayer[m].mypass[n].get_frequency()));
                        l_ran.Add(Convert.ToSingle(mylayer[m].mypass[n].get_range()));
                    }
                }

                for (int v = 0; v < sum_pass * 2; v = v + 2)
                {
                    sngValues[v] = l_fre[v / 2];
                    sngValues[v + 1] = l_ran[v / 2];
                }

                Console.WriteLine(sngValues[14]);


                if (mobjNumReg2.SetValues(0, sngValues, sum_pass * 2) == false)
                {
                    MessageBox.Show("SetNumReg Float Error");
                }
                else
                {
                    MessageBox.Show("SetNumReg Float Success");
                }


            }

            //写入位置变量寄存器
            //int intRand = 0;
            int ii = 0;
            
            List<Array> sng = new List<Array>();
            for(int jj = 0; jj <sum_pass; jj++)
            {
                sng.Add(new float[9]);
            }

            for (int mm = 0; mm < piles; mm++)
            {
                for (int nn = 0; nn < mylayer[mm].get_pass(); nn++)
                {
                    sng[mm].SetValue(Convert.ToSingle(mylayer[mm].mypass[nn].get_X()), 0);  //TCP左右
                    sng[mm].SetValue(Convert.ToSingle(mylayer[mm].mypass[nn].get_Z()), 2);  //TCP上下
                    sng[mm].SetValue(Convert.ToSingle(mylayer[mm].mypass[nn].get_P()), 4);  //倾角
                }
            }

            //Array sngArray = new float[9];
            Array intConfig = new short[7];

            {
                for (ii = mobjPosReg.StartIndex; ii <= mobjPosReg.EndIndex; ii++)
                {
                    intConfig.SetValue((short)ii, 4);
                    intConfig.SetValue((short)ii, 5);
                    intConfig.SetValue((short)ii, 6);
                    Array sngArray = sng[ii];
                    mobjPosReg.SetValueXyzwpr(ii, ref sngArray, ref intConfig, -1, -1);
                }
            }


        }





        //自动排道（从数据库中提取参数）
        private void button7_Click(object sender, EventArgs e)
        {
            height = Convert.ToDouble(textBox1.Text);
            angle = Convert.ToDouble(textBox2.Text);
            gap = Convert.ToDouble(textBox3.Text);
            edge = Convert.ToDouble(textBox4.Text);
            if (radioButton1.Checked)
                mode = 1;
            else
                mode = 2;
            //连接数据库
            string stpath = Application.StartupPath;
            string connectionstring = "Data Source= " + Application.StartupPath + "\\test.db";
            SQLiteConnection connection = new SQLiteConnection(connectionstring);
            connection.Open();
            //选择所需参数
            string sql = "SELECT layer, pass, X, Z, P, extension, current, speed, frequency, range, time FROM welding WHERE height = {0} AND angle = {1} AND gap = {2} AND edge = {3} AND mode = {4}";
            string s = string.Format(sql, height, angle, gap, edge, mode);
            Console.WriteLine(s);
            SQLiteDataAdapter da = new SQLiteDataAdapter(s, connection);
            DataSet ds = new DataSet();
            da.Fill(ds);

            //数据表格测试（从数据库中提取表格并存入变量d1）
            DataTable d1 = ds.Tables[0];
            piles = Convert.ToInt32(d1.Rows[d1.Rows.Count - 1]["layer"]);
            sum_pass = d1.Rows.Count;

            //for (int m = 0; m < piles; m++)
            //{
            //    mylayer[m] = new Layer();
            //}

            for (int i = 0; i < d1.Rows.Count; i++)
            {
                
                int j = Convert.ToInt32(d1.Rows[i]["layer"]) - 1;
                double d = Convert.ToDouble(d1.Rows[i]["speed"]);
                Pass pass = new Pass(Convert.ToDouble(d1.Rows[i]["current"]), d, Convert.ToDouble(d1.Rows[i]["frequency"]), Convert.ToDouble(d1.Rows[i]["range"]), Convert.ToDouble(d1.Rows[i]["time"]), Convert.ToDouble(d1.Rows[i]["X"]), Convert.ToDouble(d1.Rows[i]["Z"]), Convert.ToDouble(d1.Rows[i]["P"]), Convert.ToDouble(d1.Rows[i]["extension"]));                
                mylayer[j].mypass.Add(pass);
                mylayer[j].set_layer(j + 1);
                
            }
            
            //Console.WriteLine(mylayer[1].mypass[1].get_current()); 
            mylayer[piles - 1].set_pass_l(sum_pass);
            mylayer[0].set_pass_f(0);
            for (int k = 0; k < piles; ++k)
                mylayer[k].set_pass(mylayer[k].mypass.Count);
            for (int j = piles - 1; j > 0; --j)
            {
                mylayer[j - 1].set_pass_l(mylayer[j].get_pass_l() - mylayer[j].get_pass());
                mylayer[j].set_pass_f(mylayer[j - 1].get_pass_l());
            }
            i = piles - 1;
            label28.Text = Convert.ToString(i + 1);
            label29.Text = Convert.ToString(sum_pass);

        }


        /*画图函数*/

        //清屏
        private void button5_Click(object sender, EventArgs e)
        {

            Graphics g = pictureBox1.CreateGraphics();
            g.Clear(Color.White);
        }

        //显示图像
        private void button4_Click(object sender, EventArgs e)
        {
            //生产随机填充色
            for (int i = 0; i < 10000; i++)
            {
                mybrush[i] = new SolidBrush(Color.FromArgb(ran.Next(255), ran.Next(255), ran.Next(200)));
            }
            Graphics gh = this.pictureBox1.CreateGraphics();
            
            //画钢板和填充第一层
            PointF[] P = paint_steel_l();
            PointF[] P2 = paint_steel_r();
            paint_first();
            
            gh.FillPolygon(new SolidBrush(Color.Gray), P);
            gh.FillPolygon(new SolidBrush(Color.Gray), P2);
            gh.FillPolygon(mybrush[rnd.Next(9999)], mylayer[0].mypass[0].pots);
            gh.DrawPolygon(pen2, P);
            gh.DrawPolygon(pen2, P2);
            gh.DrawPolygon(pen, mylayer[0].mypass[0].pots);
            for (int j = 1; j < piles; j++)
            {
                if (mylayer[j].get_pass() % 2 == 0)
                {
                    paint_even(mylayer[j].get_layer(), mylayer[j].get_pass());
                    for (int k = 0; k < mylayer[j].get_pass(); ++k)
                    {

                        gh.FillPolygon(mybrush[rnd.Next(9999)], mylayer[j].mypass[k].pots);
                        gh.DrawPolygon(pen, mylayer[j].mypass[k].pots);
                    }
                }
                else
                {
                    paint_odd(mylayer[j].get_layer(), mylayer[j].get_pass());
                    for (int k = 0; k < mylayer[j].get_pass(); ++k)
                    {

                        gh.FillPolygon(mybrush[rnd.Next(9999)], mylayer[j].mypass[k].pots);
                        gh.DrawPolygon(pen, mylayer[j].mypass[k].pots);
                    }
                }

            }

        }

            //左侧钢板
            private PointF[] paint_steel_l()
        {
            
            
            double height_1 = height - edge;
            double height_0 = 120;
            double height_2 = 120 - 10;
            double length_1 = 250;
            double gap_1 = 10;
            double angle_1 = 90 - angle / 2;
            PointF p1 = new PointF(80, 50);
            PointF p2 = new PointF(Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - length_1) + 80, 50);
            PointF p3 = new PointF(Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - length_1) + 80, 50 +Convert.ToSingle(height_0));
            PointF p4 = new PointF(Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180)) + 80, 50 + Convert.ToSingle(height_0));
            PointF p5 = new PointF(Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180)) + 80, 50 + Convert.ToSingle(height_2));
            PointF p6 = new PointF(80, 50);
            PointF[] P = new PointF[6] {p1, p2, p3, p4, p5, p6};
            return P;
        }

        //右侧钢板
        private PointF[] paint_steel_r()
        {


            double height_1 = height - edge;
            double height_0 = 120;
            double height_2 = 120 - 10;
            double length_1 = 250;
            double gap_1 = 10;
            double angle_1 = 90 - angle / 2;
            PointF p1 = new PointF(80 + Convert.ToSingle(2 * height_2 / Math.Tan(angle_1 * PI / 180) + gap_1), 50);
            PointF p2 = new PointF(Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) + gap_1 + length_1) + 80, 50);
            PointF p3 = new PointF(Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) + gap_1 + length_1) + 80, 50 + Convert.ToSingle(height_0));
            PointF p4 = new PointF(Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) + gap_1) + 80, 50 + Convert.ToSingle(height_0));
            PointF p5 = new PointF(Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) + gap_1) + 80, 50 + Convert.ToSingle(height_2));
            PointF p6 = new PointF(80 + Convert.ToSingle(2 * height_2 / Math.Tan(angle_1 * PI / 180) + gap_1), 50);
            PointF[] P = new PointF[6] { p1, p2, p3, p4, p5, p6 };
            return P;
        }

        //填充第一层
        private void paint_first()
        {
            double height_1 = height - edge;
            double height_0 = 120;
            double height_2 = 120 - 10;
            double gap_1 = 10;
            double angle_1 = 90 - angle / 2;
            PointF p1 = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180)), 50 + Convert.ToSingle(height_2));
            PointF p2 = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180)), 50 + Convert.ToSingle(height_0));
            PointF p3 = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) + gap_1), 50 + Convert.ToSingle(height_0));
            PointF p4 = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) + gap_1), 50 + Convert.ToSingle(height_2));
            PointF p5 = new PointF(80 + Convert.ToSingle(2 * height_2 / Math.Tan(angle_1 * PI / 180) + gap_1 - (piles - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))), 50 + Convert.ToSingle(height_2 - 1 * height_2 / piles));
            PointF p6 = new PointF(80 + Convert.ToSingle((piles - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))), 50 + Convert.ToSingle(height_2 - 1 * height_2 / piles));
            PointF p7 = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180)), 50 + Convert.ToSingle(height_2));
            mylayer[0].mypass[0].pots = new PointF[7] { p1, p2, p3, p4, p5, p6, p7 };
        }

        //填充奇数层
        private void paint_odd(int l, int p)
        {
            double height_1 = height - edge;
            double height_2 = 120 - 10;
            double gap_1 = 10;
            double angle_1 = 90 - angle / 2;
            //左侧填充
            for (int j = 0; j < (p - 1) / 2; ++j)
            {
                PointF p1 = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + (j + 1) * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p)), 50 + Convert.ToSingle(height_2 - (l - 1) * height_2 / piles));
                PointF p2 = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + (j + 1) * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p) - height_2 / (piles * Math.Tan(angle_1 * PI / 180))), 50 + Convert.ToSingle(height_2 - l * height_2 / piles));
                PointF p3 = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + (j) * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p) - height_2 / (piles * Math.Tan(angle_1 * PI / 180))), 50 + Convert.ToSingle(height_2 - l * height_2 / piles));
                PointF p4 = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + (j) * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p)), 50 + Convert.ToSingle(height_2 - (l - 1) * height_2 / piles));
                PointF p5 = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + (j + 1) * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p)), 50 + Convert.ToSingle(height_2 - (l - 1) * height_2 / piles));
                mylayer[l - 1].mypass[j].pots = new PointF[5] { p1, p2, p3, p4, p5 };
            }

            //右侧填充
            for (int j = (p + 1) / 2; j < p; ++j)
            {
                PointF p1 = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + j * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p)), 50 + Convert.ToSingle(height_2 - (l - 1) * height_2 / piles));
                PointF p2 = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + j * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p) + height_2 / (piles * Math.Tan(angle_1 * PI / 180))), 50 + Convert.ToSingle(height_2 - l * height_2 / piles));
                PointF p3 = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + (j + 1) * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p) + height_2 / (piles * Math.Tan(angle_1 * PI / 180))), 50 + Convert.ToSingle(height_2 - l * height_2 / piles));
                PointF p4 = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + (j + 1) * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p)), 50 + Convert.ToSingle(height_2 - (l - 1) * height_2 / piles));
                PointF p5 = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + j * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p)), 50 + Convert.ToSingle(height_2 - (l - 1) * height_2 / piles));
                mylayer[l - 1].mypass[j].pots = new PointF[5] { p1, p2, p3, p4, p5 };
            }

            //中间填充
            PointF pa = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + ((p - 3) / 2 + 1) * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p)), 50 + Convert.ToSingle(height_2 - (l - 1) * height_2 / piles));
            PointF pb = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + ((p - 3) / 2 + 1) * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p) - height_2 / (piles * Math.Tan(angle_1 * PI / 180))), 50 + Convert.ToSingle(height_2 - l * height_2 / piles));
            PointF pc = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + ((p + 1) / 2) * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p) + height_2 / (piles * Math.Tan(angle_1 * PI / 180))), 50 + Convert.ToSingle(height_2 - l * height_2 / piles));
            PointF pd = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + ((p + 1) / 2) * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p)), 50 + Convert.ToSingle(height_2 - (l - 1) * height_2 / piles));
            PointF pe = pa;
            mylayer[l - 1].mypass[(p - 1) / 2].pots = new PointF[5] { pa, pb, pc, pd, pe};

        }

        //填充偶数层
        private void paint_even(int l, int p)
        {
            double height_1 = height - edge;
            double height_2 = 120 - 10;
            double gap_1 = 10;
            double angle_1 = 90 - angle / 2;
            //左侧填充
            for (int j = 0; j < p / 2; ++j)
            {
                PointF p1 = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + (j + 1) * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p)), 50 + Convert.ToSingle(height_2 - (l - 1) * height_2 / piles));
                PointF p2 = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + (j + 1) * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p) - height_2 / (piles * Math.Tan(angle_1 * PI / 180))), 50 + Convert.ToSingle(height_2 - l * height_2 / piles));
                PointF p3 = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + (j) * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p) - height_2 / (piles * Math.Tan(angle_1 * PI / 180))), 50 + Convert.ToSingle(height_2 - l * height_2 / piles));
                PointF p4 = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + (j) * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p)), 50 + Convert.ToSingle(height_2 - (l - 1) * height_2 / piles));
                PointF p5 = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + (j + 1) * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p)), 50 + Convert.ToSingle(height_2 - (l - 1) * height_2 / piles));
                mylayer[l - 1].mypass[j].pots = new PointF[5] { p1, p2, p3, p4, p5 };
            }

            //右侧填充
            for (int j = (p + 2) / 2; j < p; ++j)
            {
                PointF p1 = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + j * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p)), 50 + Convert.ToSingle(height_2 - (l - 1) * height_2 / piles));
                PointF p2 = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + j * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p) + height_2 / (piles * Math.Tan(angle_1 * PI / 180))), 50 + Convert.ToSingle(height_2 - l * height_2 / piles));
                PointF p3 = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + (j + 1) * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p) + height_2 / (piles * Math.Tan(angle_1 * PI / 180))), 50 + Convert.ToSingle(height_2 - l * height_2 / piles));
                PointF p4 = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + (j + 1) * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p)), 50 + Convert.ToSingle(height_2 - (l - 1) * height_2 / piles));
                PointF p5 = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + j * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p)), 50 + Convert.ToSingle(height_2 - (l - 1) * height_2 / piles));
                mylayer[l - 1].mypass[j].pots = new PointF[5] { p1, p2, p3, p4, p5 };
            }

            //中间填充
            PointF pa = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + ((p - 2) / 2 + 1) * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p)), 50 + Convert.ToSingle(height_2 - (l - 1) * height_2 / piles));
            PointF pb = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + ((p - 2) / 2 + 1) * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p) - height_2 / (piles * Math.Tan(angle_1 * PI / 180))), 50 + Convert.ToSingle(height_2 - l * height_2 / piles));
            PointF pc = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + ((p + 2) / 2) * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p) + height_2 / (piles * Math.Tan(angle_1 * PI / 180))), 50 + Convert.ToSingle(height_2 - l * height_2 / piles));
            PointF pd = new PointF(80 + Convert.ToSingle(height_2 / Math.Tan(angle_1 * PI / 180) - (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180)) + ((p + 2) / 2) * ((gap_1 + 2 * (l - 1) * height_2 / (piles * Math.Tan(angle_1 * PI / 180))) / p)), 50 + Convert.ToSingle(height_2 - (l - 1) * height_2 / piles));
            PointF pe = pa;
            mylayer[l - 1].mypass[ p / 2].pots = new PointF[5] { pa, pb, pc, pd, pe };

        }

        //提交
        private void button3_Click(object sender, EventArgs e)
        {
            piles = i + 1;
            mylayer[i].set_pass_l(sum_pass);
            mylayer[0].set_pass_f(0);
            for (int j = i; j > 0; --j)
            {
                mylayer[j - 1].set_pass_l(mylayer[j].get_pass_l() - mylayer[j].get_pass());
                mylayer[j].set_pass_f(mylayer[j - 1].get_pass_l());


            }
        }

        //数据输入
        private void button1_Click(object sender, EventArgs e)
        {

            //从界面获取数据
            height = Convert.ToDouble(textBox1.Text);
            angle = Convert.ToDouble(textBox2.Text);
            gap = Convert.ToDouble(textBox3.Text);
            edge = Convert.ToDouble(textBox4.Text);
            if (radioButton1.Checked)
                mode = 1;
            else
                mode = 2;

            i = Convert.ToInt32(numericUpDown1.Text) - 1;
            Pass newpass = new Pass(Convert.ToDouble(textBox8.Text), Convert.ToDouble(textBox7.Text), Convert.ToDouble(textBox5.Text), Convert.ToDouble(textBox6.Text), Convert.ToDouble(textBox13.Text), Convert.ToDouble(textBox12.Text), Convert.ToDouble(textBox11.Text), Convert.ToDouble(textBox10.Text), Convert.ToDouble(textBox9.Text));
            mylayer[i].mypass.Add(newpass);
            mylayer[i].set_layer(Convert.ToInt32(numericUpDown1.Text));
            mylayer[i].set_current_pass(Convert.ToInt32(numericUpDown2.Text));
            mylayer[i].set_pass(mylayer[i].mypass.Count);
            ++sum_pass;
            label28.Text = numericUpDown1.Text;
            label29.Text = Convert.ToString(sum_pass);


            Console.WriteLine(mylayer[i].mypass.Count);
            //连接数据库
            string stpath = Application.StartupPath;
            string connectionstring = "Data Source= " + Application.StartupPath + "\\test.db";
            SQLiteConnection connection = new SQLiteConnection(connectionstring);
            connection.Open();
            string insertstr = "INSERT into welding (layer, pass, X, Z, P, extension, current, speed, frequency, range, time, height, angle, gap, edge, mode) values ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15})";
            string insertsql = string.Format(insertstr, mylayer[i].get_layer(), mylayer[i].get_current_pass(), mylayer[i].mypass[mylayer[i].get_current_pass() - 1].get_X(), mylayer[i].mypass[mylayer[i].get_current_pass() - 1].get_Z(), mylayer[i].mypass[mylayer[i].get_current_pass() - 1].get_P(), mylayer[i].mypass[mylayer[i].get_current_pass() - 1].get_extension(), mylayer[i].mypass[mylayer[i].get_current_pass() - 1].get_current(), mylayer[i].mypass[mylayer[i].get_current_pass() - 1].get_speed(), mylayer[i].mypass[mylayer[i].get_current_pass() - 1].get_frequency(), mylayer[i].mypass[mylayer[i].get_current_pass() - 1].get_range(), mylayer[i].mypass[mylayer[i].get_current_pass() - 1].get_time(), height, angle, gap, edge, mode);
            SQLiteCommand command = new SQLiteCommand(insertsql, connection);
            command.ExecuteNonQuery();  //执行命令


        }


        //数据删除
        private void button2_Click(object sender, EventArgs e)
        {
            //连接数据库
            string stpath = Application.StartupPath;
            string connectionstring = "Data Source= " + Application.StartupPath + "\\test.db";
            SQLiteConnection connection = new SQLiteConnection(connectionstring);
            connection.Open();
            if (i >= 0)
            {
                string deletestr = "DELETE from welding WHERE layer = {0} AND pass = {1} AND height = {2} AND angle = {3} AND gap = {4} AND edge = {5} AND mode = {6}";
                string deletesql = string.Format(deletestr, mylayer[i].get_layer(), mylayer[i].get_current_pass(), height, angle, gap, edge, mode);
                SQLiteCommand command = new SQLiteCommand(deletesql, connection);
                command.ExecuteNonQuery();  //执行命令
                --sum_pass;
                if (i > 0)
                    mylayer[i].mypass.RemoveAt(mylayer[i].mypass.Count - 1);
                if (i == 0 && mylayer[i].mypass.Count == 1)
                    mylayer[i].mypass.RemoveAt(0);
                mylayer[i].set_pass(mylayer[i].mypass.Count);
                mylayer[i].set_current_pass(mylayer[i].mypass.Count);
                if (mylayer[i].mypass.Count == 0 && i != 0)
                    --i;

                label28.Text = Convert.ToString(i + 1);
                if (i == 0 && mylayer[i].mypass.Count == 0)
                {
                    sum_pass = 0;
                    label28.Text = Convert.ToString(0);
                }
                label29.Text = Convert.ToString(sum_pass);

            }

        }


        
    }

    


}

