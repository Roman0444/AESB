using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class ResultWheel
    {
        public double Speed;
        public double Forse;
        public ResultWheel(double speed, double forse)
        {
            Speed = speed;
            Forse = forse;
            if (Speed < 0.1) Speed = 0;
        }

    }
}
