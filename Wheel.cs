using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
   public class Wheel
    {
        public string Name;
        public double SpeedCar;
        public bool BlockWheel;
        public double BrakingForce;
        public double RoadFrictionForce;
        public int RoadFrictionForceMin;
        public int RoadFrictionForceMax;
        public double KineticEnergy;
        public double KineticEnergyNow;
        public double Mass;
        public double Radius;
        public double Length;
        public Image image;
        public double Period;
        public double k=1;
        public bool abs = true;
        public Wheel(string Name,double Speed,double Mass, double Radius, Image Im,
            int RoadFrictionForceMin,int RoadFrictionForceMax, PictureBox pictB)
        {
            this.RoadFrictionForceMin = RoadFrictionForceMin;
            this.RoadFrictionForceMax = RoadFrictionForceMax;
            this.Name = Name;   
            this.SpeedCar = Speed;
            this.Mass = Mass;
            this.Radius = 0.0254 *Radius /2;
            pictB.Image = Im;
            pictB.Update();
            Period =  (2 * this.Radius * Math.PI)/ this.SpeedCar;

            RoadFrictionForce = Settings.r.Next(50, 100);
            BlockWheel = false;
            KineticEnergy = 10;
            KineticEnergyNow = 3;
        }

        public ResultWheel SlowDown(double Forse,double time, ref Car car)
        {
            RoadFrictionForce = car.mass*0.01*Settings.r.Next(RoadFrictionForceMin, RoadFrictionForceMax);
            // трение качения
            BrakingForce = 0.01*car.mass*Forse * k;
            if (abs)
            {
                if (BrakingForce > RoadFrictionForce)// переходимо в тертя скольжения
                {
                    RoadFrictionForce = 0.3 * RoadFrictionForce;
                    BlockWheel = true;
                    k = RoadFrictionForce / BrakingForce;
                    BrakingForce = RoadFrictionForce;
                    KineticEnergyNow = 0;
                }
                else
                {
                    if (KineticEnergyNow < KineticEnergy)
                    {
                        KineticEnergyNow++;
                    }
                    else
                    {
                        BlockWheel = false;
                        if (k < 1)
                        {
                            k = k + 0.05;
                        }
                    }
                }
            }
            else
            {
                RoadFrictionForce = 0.3 * RoadFrictionForce;
                BrakingForce = RoadFrictionForce;
                BlockWheel = true;
            }

            double BrakingWork = BrakingForce * car.speed * time;
            car.KineticEnergy -= BrakingWork;
            car.UpdateSpeed();
            SpeedCar = car.speed;
            if (BlockWheel)
            {
                return new ResultWheel(0, BrakingForce);
            }
            else
            {
                return new ResultWheel(car.speed / Radius, BrakingForce);
            }    

        }


        public async Task Draw(PictureBox pictB)
        {
            while (true)
            {
                if (!BlockWheel)
                {
                    
                    if (SpeedCar >0.2)
                    {
                        Period = (2 * Radius * Math.PI) / (4 * SpeedCar);
                    }
                    else
                    {
                        break;
                    }
               
                    await Task.Delay((int)(Period * 1000));

                    try
                    {
                        pictB.Invoke((MethodInvoker)(() =>
                        {
                            pictB.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            pictB.Invalidate();
                        }));
                    }
                    catch
                    {
                        break;
                    }
                }
                else
                {
                    await Task.Delay(100);
                }
            }
            if (Name == "FrontLeft") Settings.FLMove = false;
            if (Name == "FrontRight") Settings.FRMove = false;
            if (Name == "RearLeft") Settings.RLMove = false;
            if (Name == "RearRight") Settings.RRMove = false;
        }

    }
}
