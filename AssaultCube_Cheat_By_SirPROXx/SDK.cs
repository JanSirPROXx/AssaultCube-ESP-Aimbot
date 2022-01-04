using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssaultCube_Cheat_By_SirPROXx
{


    public class SDK
    {

       




    }
    public class Enemy
    {
        public int WindowsHigh { get; set; }
        public int WindowsWidth { get; set; }

        public int Health { get; set; }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public float Distance { get; set; }
        public float ViewDistance { get; set; }
        public Point top, bottom;
        public int Team { get; set; }// Get checked if the player is in team 1 or 2
        
        public Vector3 xyz { get; set; }

        //For ESP
        public Rectangle rect()
        {
            return new Rectangle
            {
                /*Location = new Point(bottom.X - (bottom.Y - top.Y) / 4, top.Y),
                Size = new Size((bottom.Y - top.Y) / 2, (bottom.Y - top.Y) )*/
                Location = new Point(bottom.X - 30, bottom.Y),
                Size = new Size((bottom.Y - top.Y) / 10, (bottom.Y - top.Y)/7)

            };
        }
        
        
    }
    public class Vector3
    {
        public float x, y, z;
    }
    public class Vector2
    {
        public float x, y;

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class Local
    {
        public int Health { get; set; }

        public float ViewX { get; set; }
        public float ViewY { get; set; }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public int Team { get; set; }// Get checked if the player is in team 1 or 2
    }

    public class ViewMatrix
    {
        public float m11, m12, m13, m14; //00, 01, 02, 03
        public float m21, m22, m23, m24; //04, 05, 06, 07
        public float m31, m32, m33, m34; //08, 09, 10, 11
        public float m41, m42, m43, m44; //12, 13, 14, 15

        

    }
}
