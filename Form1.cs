using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private float rotationAngle;
        public Form1()
        {
            InitializeComponent();
        }

        List<Wheel> wheels = new List<Wheel>();


        private List<Task> drawTasks = new List<Task>();
        public Car car;
        MemoryStream ms1;
        MemoryStream ms2;
        double BrakingForce=0;
        ResultWheel res;
        double mytime = 0;
        double timeToBreak = 0;


        void StartSet()
        {
            byte[] imageBytes = File.ReadAllBytes("Колесо.png");
            ms1 = new MemoryStream(imageBytes);

            imageBytes = File.ReadAllBytes("2.jpg");
            ms2 = new MemoryStream(imageBytes);

            wheels.Clear();
           

            car = new Car("BMW", (int)numericUpDown7.Value, (double)numericUpDown1.Value, pictureBox1, Image.FromStream(ms2));

            wheels.Add(new Wheel("FrontLeft",(int)numericUpDown1.Value ,15, (int)numericUpDown6.Value, Image.FromStream(ms1),
                (int)numericUpDown2.Value, (int)numericUpDown3.Value, pictureBox2));
            wheels.Add(new Wheel("FrontRight", (int)numericUpDown1.Value, 15, (int)numericUpDown6.Value, Image.FromStream(ms1),
                (int)numericUpDown2.Value, (int)numericUpDown3.Value, pictureBox3));
            wheels.Add(new Wheel("RearLeft", (int)numericUpDown1.Value, 15, (int)numericUpDown6.Value, Image.FromStream(ms1),
                (int)numericUpDown4.Value, (int)numericUpDown5.Value, pictureBox4));
            wheels.Add(new Wheel("RearRight", (int)numericUpDown1.Value, 15, (int)numericUpDown6.Value, Image.FromStream(ms1),
                (int)numericUpDown4.Value, (int)numericUpDown5.Value, pictureBox5));

            Settings.FLMove = true;
            Settings.FRMove = true;
            Settings.RLMove = true;
            Settings.RRMove = true;
            Settings.CarMove = true;

            chart1.Series.Clear();
            chart2.Series.Clear();



            chart1.Series.Add("FrontLeft");
            chart1.Series.Add("FrontRight");
            chart1.Series.Add("RearLeft");
            chart1.Series.Add("RearRight");
           
            chart1.Series[0].BorderWidth = 3;
            chart1.Series[1].BorderWidth = 3;
            chart1.Series[2].BorderWidth = 3;
            chart1.Series[3].BorderWidth = 3;

            chart1.Series[0].ChartType = SeriesChartType.Spline;
            chart1.Series[1].ChartType = SeriesChartType.Spline;
            chart1.Series[2].ChartType = SeriesChartType.Spline;
            chart1.Series[3].ChartType = SeriesChartType.Spline;

            chart1.ChartAreas[0].AxisX.Title = "час (с)";
            chart1.ChartAreas[0].AxisY.Title = "кутова швидкість (рад/с)";


            chart2.Series.Add("FLeft");
            chart2.Series.Add("FRight");
            chart2.Series.Add("RLeft");
            chart2.Series.Add("RRight");

            chart2.Series[0].BorderWidth = 3;
            chart2.Series[1].BorderWidth = 3;
            chart2.Series[2].BorderWidth = 3;
            chart2.Series[3].BorderWidth = 3;

            chart2.Series[0].ChartType = SeriesChartType.Spline;
            chart2.Series[1].ChartType = SeriesChartType.Spline;
            chart2.Series[2].ChartType = SeriesChartType.Spline;
            chart2.Series[3].ChartType = SeriesChartType.Spline;

            chart2.ChartAreas[0].AxisX.Title = "час (с)";
            chart2.ChartAreas[0].AxisY.Title = "гальмівне зусилля (Н)";

            mytime = 0;
            timeToBreak = Settings.r.Next(1, 5);

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            StartSet();
            button1.Enabled = false;
            drawTasks.Clear();
            button1.Enabled = false;
            drawTasks.Add(wheels[0].Draw(pictureBox2));
            drawTasks.Add(wheels[1].Draw(pictureBox3));
            drawTasks.Add(wheels[2].Draw(pictureBox4));
            drawTasks.Add(wheels[3].Draw(pictureBox5));
            drawTasks.Add(car.DrawChangeAngle(pictureBox1));
            if (radioButton2.Checked)
            {
                label14.Visible = true;
                label14.Text = "Бачимо перешкоду через " + timeToBreak.ToString("##.##") + " c";
                timer2.Start();
            }
            await Task.WhenAll(drawTasks);
            drawTasks.Clear();
           
            button1.Enabled = true;

        }

       
        private void button2_Click(object sender, EventArgs e)
        {
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StartSet();
        }

        void ConfirmChanges()
        {
            foreach (Wheel w in wheels)
            {
                w.SpeedCar = (double)numericUpDown1.Value; 
            }
            car.ChangeSpeeed((double)numericUpDown1.Value);
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            ConfirmChanges();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Thread.Sleep(1000);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            int i = 0;
            foreach(Wheel w in wheels)
            {
                w.abs = checkBox1.Checked; 
                res = w.SlowDown(BrakingForce,0.1, ref car);

                if (i==0)
                {
                    label3.Text = res.Speed.ToString("#0.0#")+" r/с";
                    label10.Text = res.Forse.ToString("#0.0#")+ " Н";
                    chart1.Series[0].Points.AddXY(mytime,res.Speed);
                    chart2.Series[0].Points.AddXY(mytime, res.Forse);
                    car.VectorFrontLeft = res.Forse;
                }
                if (i == 1)
                {
                    label4.Text = res.Speed.ToString("#0.0#") + " r/с";
                    label11.Text = res.Forse.ToString("#0.0#") + " Н";
                    chart1.Series[1].Points.AddXY(mytime, res.Speed);
                    chart2.Series[1].Points.AddXY(mytime, res.Forse);
                    car.VectorFrontRight = res.Forse;
                }
                if (i == 2)
                {
                    label5.Text = res.Speed.ToString("#0.0#") + " r/с";
                    label12.Text = res.Forse.ToString("#0.0#") + " Н";
                    chart1.Series[2].Points.AddXY(mytime, res.Speed);
                    chart2.Series[2].Points.AddXY(mytime, res.Forse);
                    car.VectorRearLeft = res.Forse;
                }
                if (i == 3)
                {
                    label6.Text = res.Speed.ToString("#0.0#") + " r/с";
                    label13.Text = res.Forse.ToString("#0.0#") + " Н";
                    chart1.Series[3].Points.AddXY(mytime, res.Speed);
                    chart2.Series[3].Points.AddXY(mytime, res.Forse);
                    car.VectorRearRight = res.Forse;
                }
                i++;
            }
            label2.Text = car.speed.ToString("#0.0#") + " m/s"; ;
            label2.Update();
            label9.Text = car.angle.ToString("#0.0#") + "град";
            label9.Update();

            chart1.Update();
            mytime+= 0.1;

            if (Settings.FLMove == false &&
            Settings.FRMove == false &&
            Settings.RLMove == false &&
            Settings.RRMove == false &&
            Settings.CarMove == false) timer1.Stop();

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            BrakingForce = trackBar1.Value*10;
            timer1.Start();
            if (BrakingForce == 0) timer1.Stop();
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label25_Click(object sender, EventArgs e)
        {

        }

        private void label23_Click(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
          
            timeToBreak = timeToBreak - 0.1;
            label14.Text = "До перешкоди "+ timeToBreak.ToString("##.##") + " с";


            if (timeToBreak < 0.1)
            {
                trackBar1.Value = trackBar1.Maximum;
                timer2.Stop();
                BrakingForce = trackBar1.Value * 10;
                timer1.Start();
            }
            
        }
    }
}
