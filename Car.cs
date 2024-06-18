using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
   public class Car
    {
        string Name;
        Image im;
        public double mass;
        public double speed;
        public double angle = 0;
        public double KineticEnergy;
        public double VectorFrontLeft, VectorFrontRight, VectorRearLeft, VectorRearRight;


        public Car(string Name,int Mass,double Speed,PictureBox pictB,Image Im)
        {
            im = Im;
            pictB.Image = im;
            angle = 0;
          
            mass = Mass;
            this.Name = Name;
            speed = Speed;
            KineticEnergy = mass * speed * speed / 2;
        }

        public void ChangeAngle()
        {     
            double diff = (0.5*VectorFrontLeft + 0.5 * VectorFrontRight) - (0.5*VectorRearRight+ 0.5 * VectorRearLeft);
            angle +=speed*(diff / mass);
            
        }
        
        public void ChangeSpeeed(double newSpeed)
        {
            speed = newSpeed;
            KineticEnergy = mass * speed * speed / 2;
        }

        public void UpdateSpeed()
        {   
            speed = Math.Sqrt(2*KineticEnergy/mass);
            if (speed < 0.2) speed = 0;
        }

        public async Task DrawChangeAngle(PictureBox pictB)
        {
            while (true)
            {
                
                try
                {
                    if (speed > 0.2)
                    {
                        ChangeAngle();
                        await RotateImageAndScale(im, (float)angle, pictB);
                        await Task.Delay(300);
                    }
                    else
                    {
                        break;
                    }
                }
                catch
                {
                    break;
                }
            }
            speed = 0;
            await Task.Delay(200);
            Settings.CarMove = false;
        }

        private async Task RotateImageAndScale(Image image, float angle, PictureBox pictB)
        {
            
            int newWidth = (int)(image.Width * Math.Cos(Math.Abs(angle) * Math.PI / 180) + 
                image.Height * Math.Sin(Math.Abs(angle) * Math.PI / 180));
            int newHeight = (int)(image.Width * Math.Sin(Math.Abs(angle) * Math.PI / 180) 
                + image.Height * Math.Cos(Math.Abs(angle) * Math.PI / 180));

            Bitmap rotatedImage = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                
                g.TranslateTransform(newWidth / 2, newHeight / 2);

                
                g.RotateTransform(angle);

                
                g.TranslateTransform(-image.Width / 2, -image.Height / 2);

                
                g.DrawImage(image, new Point(0, 0));
            }

            pictB.Invoke((MethodInvoker)(() =>
            {
                pictB.Image = rotatedImage;
                pictB.Invalidate();
            }));
        }
    }
}
