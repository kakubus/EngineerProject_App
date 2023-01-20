//using Android.App;
//using AndroidX.ConstraintLayout.Core.Motion.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboApp
{
    enum WheelID { A, B, C, D}; // A - LP, B - PP, C - LT, D - PT
    
    struct WheelMovement // Struct for sending
    {
        public int direction;
        public int speed;
    }

    struct Wheel
    {
        readonly int id;
        public long counter;
        public int direction;
        public int velocity;
        public float rot_velocity;
    }

    internal class Robo
    {
        private static Robo instance;

        public static Robo Instance{
            get
            {
                if (instance == null) // Only one robot instance is allowed in app. (It's possible to steering only one robot in app time. Moreover, there is only one such robot in the world!)
                    instance = new Robo();
                return instance;
            }
        }

        public bool EMERGENCY_MODE;
        public Wheel Wheel;
        WheelID WheelID;
        

        private Robo()
        {
            EMERGENCY_MODE = false;
          //  Wheel[] = new Wheel()[4];
        }
        
    }
}
