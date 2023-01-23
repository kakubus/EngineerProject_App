//using Android.App;
//using AndroidX.ConstraintLayout.Core.Motion.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;



namespace Robot1
{
    static class ID
    {
        public const int A = 0;
        public const int B = 1;
        public const int C = 2;
        public const int D = 3;
    }
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
        Wheel(int id, long counter, int direction, int velocity, float rot_velocity)
        {
            this.id = id;
            this.counter = counter;
            this.direction = direction;
            this.velocity = velocity;
            this.rot_velocity = rot_velocity;
        }
        
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
        public Wheel[] Wheel = new Robot1.Wheel[4];
        WheelID WheelID ;
        

        private Robo()
        {
            EMERGENCY_MODE = false;
      //      Wheel[ID.A] = new Wheel(0, 0, 1, 0, 0.0);


          //  Wheel[] = new Wheel()[4];
        }

        public async Task parseInput(string inputMessage)
        {
            string[] parsedMsg = inputMessage.Split(", ");
        }
        
    }
}
