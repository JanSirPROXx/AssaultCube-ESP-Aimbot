using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Memory;
using ezOverLay;

namespace AssaultCube_Cheat_By_SirPROXx
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //For the ESP:
        int MyTeam;
        Graphics g;
        Pen TeamPen = new Pen(Color.Blue, 3);
        Pen EnemyPen = new Pen(Color.Red, 3);
        //Forms
        ez ez = new ez();





        //Für Memorylibary
        Mem m = new Mem();
        #region offsets



        //Base
        string PlayerBase = "ac_client.exe+0x109B74";
        String EntityList = "ac_client.exe+0x110D90";
        //Offsets
        string Health = ",0xF8";
        string X = ",0x4";
        string Y = ",0x8";
        string Z = ",0xC";
        string XView = ",0x40";
        string YView = ",0x44";
        string Team1 = ",0x204";
        string Team2 = ",0x32C";
        string Secondary_Ammo = ",0x114";
        string Primary_Ammo = ",0x128";
        //Viewmatrix for ESP
        //string ViewMatrix = "ac_client.exe+0x101AAC";
        string ViewMatrix = "0x00501AE8";
        #endregion

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(Keys vKey);




        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            int PID = m.GetProcIdFromName("ac_client");
            if (PID > 0)
                m.OpenProcess(PID);

            
            ez.SetInvi(this);
            ez.StartLoop(10, "AssaultCube", this);

            //ez.SetInvi(this);
            ez.DoStuff("AssaultCube", this);
            Aimbot_bw.RunWorkerAsync();
            Thread WH = new Thread(ESP) { IsBackground = true };
            WH.Start();
            m.WriteMemory(PlayerBase + Primary_Ammo, "int", "999");
            m.WriteMemory(PlayerBase + Health, "int", "999");
            
        }

        Local GetLocal()
        {
            var alocal = new Local
            {
                X = m.ReadFloat(PlayerBase + X),
                Y = m.ReadFloat(PlayerBase + Y),
                Z = m.ReadFloat(PlayerBase + Z),
                ViewX = m.ReadFloat(PlayerBase + XView),
                ViewY = m.ReadFloat(PlayerBase + YView),
                Health = m.ReadInt(PlayerBase + Health),

            };

            if (m.ReadInt(PlayerBase + Team1) == 1)
                alocal.Team = 1;
            else
                alocal.Team = 2;

            return alocal;
        }
        float GetDistance(Local local, Enemy enemy)
        {
            float dist = (float)Math.Sqrt(Math.Pow(enemy.X - local.X, 2) + Math.Pow(enemy.Y - local.Y, 2) + Math.Pow(enemy.Z - local.Z, 2));
            return dist;
        }

        List<Enemy> GetEnemys(Local local)
        {
            var aenemys = new List<Enemy>();

            for (int i = 0; i < 20; i++) //Go 20 time through
            {
                var entitylist_i = EntityList + ",0x" + (i * 0x4).ToString("X");
                var axyz = new Vector3()
                {
                    x = m.ReadFloat(entitylist_i + X),
                    y = m.ReadFloat(entitylist_i + Y),
                    z = m.ReadFloat(entitylist_i + Z)
                };

                var aenemy = new Enemy()
                {
                    X = m.ReadFloat(entitylist_i + X),
                    Y = m.ReadFloat(entitylist_i + Y),
                    Z = m.ReadFloat(entitylist_i + Z),
                    Health = m.ReadInt(entitylist_i + Health),

                    WindowsHigh = Height,
                    WindowsWidth = Width
                    


                };
                aenemy.bottom = WorldToScreen(IntoTheMatrix(), axyz, Width, Height, false);
                aenemy.top = WorldToScreen(IntoTheMatrix(), axyz, Width, Height, true);
                /*
                Vector2 vec2;
                WorldtoScreen(IntoTheMatrix(), axyz, Width, Height,false, out vec2);
                aenemy.bottom.X = (int)vec2.x;
                aenemy.bottom.Y = (int)vec2.y;
                Vector2 vec2_2;
                WorldtoScreen(IntoTheMatrix(), axyz, Width, Height, true, out vec2_2);
                aenemy.top.X = (int)vec2_2.x;
                aenemy.top.Y = (int)vec2_2.y;
                */

                aenemy.xyz = axyz;
                if (m.ReadInt(entitylist_i + Team2) == 0)
                    aenemy.Team = 1;
                if (m.ReadInt(entitylist_i + Team2) == 1)
                    aenemy.Team = 2;
                aenemy.Distance = GetDistance(local, aenemy);


                

                if (aenemy.Health > 0)
                {
                    if(aenemy.Health < 101)
                       aenemys.Add(aenemy);
                       
                }


            }
            
            return aenemys;
            
        }
        
        List<Enemy> GetNearestEnemys()
        {
            var LocalPlayer = GetLocal();
            var EntityPlayer = GetEnemys(LocalPlayer);
            EntityPlayer = EntityPlayer.OrderBy(o => o.Distance).ToList();

            

            return EntityPlayer;
        }
        List<Enemy> GetFadenkreuzEnemys()
        {
            var LocalPlayer = GetLocal();
            var EntityPlayer = GetEnemys(LocalPlayer);
            //EntityPlayer = EntityPlayer.OrderBy(o => o.Distance).ToList();

            for (int i = 0; i < EntityPlayer.Count; i++)
            {
                Enemy Enemy = EntityPlayer[i];
                Local Player = LocalPlayer;

                float deltaX = Enemy.X - Player.X;
                float deltaY = Enemy.Y - Player.Y;
                float deltaZ = Enemy.Z - Player.Z;

                float viewX = (float)(Math.Atan2(deltaY, deltaX) * 180 / Math.PI) + 90;
                double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                float viewY = (float)(Math.Atan2(deltaZ, distance) * 180 / Math.PI);

                float MyViewX = LocalPlayer.ViewX;
                float MyViewY = LocalPlayer.ViewY;

                float dif1 = ((float)((float)(Math.Sqrt(viewX * viewX) - Math.Sqrt(MyViewX * MyViewX)) + (Math.Sqrt(viewY*viewY) - Math.Sqrt(MyViewY*MyViewY))));
                float difi2 = (float)Math.Sqrt(dif1 * dif1);
                EntityPlayer[i].ViewDistance = difi2;

                

            }
            EntityPlayer = EntityPlayer.OrderBy(o => o.ViewDistance).ToList();
            return EntityPlayer;
        }

        private void Aim(Local Player, Enemy Enemy) //Aimbot Methode (Aim's on the nearest Enemy)
        {

            float deltaX = Enemy.X - Player.X;
            float deltaY = Enemy.Y - Player.Y;
            float deltaZ = Enemy.Z - Player.Z;

            float viewX = (float)(Math.Atan2(deltaY, deltaX) * 180 / Math.PI) + 90;
            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            float viewY = (float)(Math.Atan2(deltaZ, distance) * 180 / Math.PI);

            m.WriteMemory(PlayerBase + XView, "float", viewX.ToString());
            m.WriteMemory(PlayerBase + YView, "float", viewY.ToString());

        }
        

        private void Aimbot_bw_DoWork(object sender, DoWorkEventArgs e) //A Thread
        {
            while (true)
            {
                if (GetAsyncKeyState(Keys.X) < 0)
                {
                    var LocalPlayer = GetLocal();
                    var EntityPlayer = GetEnemys(LocalPlayer);
                    EntityPlayer = EntityPlayer.OrderBy(o => o.Distance).ToList();

                    if (EntityPlayer.Count != 0)
                    {
                        Aim(LocalPlayer, EntityPlayer[0]);
                    }

                    
                }
                if(GetAsyncKeyState(Keys.C) < 0)
                {
                    var LocalPlayer = GetLocal();
                    var EntityPlayer = GetFadenkreuzEnemys();

                    if (EntityPlayer.Count != 0)
                    {
                        Aim(LocalPlayer, EntityPlayer[0]);
                    }

                }
            }
        }
        


        
        void ESP()//This is the funktion for the ESP
        {
            while (true)
            {
                //Getting the Listes
                List<Enemy> elist = new List<Enemy>();
                Local local = GetLocal();
                elist = GetEnemys(local);




                panel1.Refresh();
                Thread.Sleep(10);
            }
            
        }


        
        ViewMatrix IntoTheMatrix()
        {
            byte[] buffer = new byte[16 * 4];

            var bytes = m.ReadBytes(ViewMatrix, (long)buffer.Length); //Versteh ich noch nicht


            var mat = new ViewMatrix();
            mat.m11 = BitConverter.ToSingle(bytes, (0 * 4));
            mat.m12 = BitConverter.ToSingle(bytes, (1 * 4));
            mat.m13 = BitConverter.ToSingle(bytes, (2 * 4));
            mat.m14 = BitConverter.ToSingle(bytes, (3 * 4));

            mat.m21 = BitConverter.ToSingle(bytes, (4 * 4));
            mat.m22 = BitConverter.ToSingle(bytes, (5 * 4));
            mat.m23 = BitConverter.ToSingle(bytes, (6 * 4));
            mat.m24 = BitConverter.ToSingle(bytes, (7 * 4));

            mat.m31 = BitConverter.ToSingle(bytes, (8 * 4));
            mat.m32 = BitConverter.ToSingle(bytes, (9 * 4));
            mat.m33 = BitConverter.ToSingle(bytes, (10 * 4));
            mat.m34 = BitConverter.ToSingle(bytes, (11 * 4));

            mat.m41 = BitConverter.ToSingle(bytes, (12 * 4));
            mat.m42 = BitConverter.ToSingle(bytes, (13 * 4));
            mat.m43 = BitConverter.ToSingle(bytes, (14 * 4));
            mat.m44 = BitConverter.ToSingle(bytes, (15 * 4));
            

            return mat;

        }
        public bool WorldtoScreen(ViewMatrix mat,Vector3 worldPos, int width, int height,bool head, out Vector2 screenPos)
        {
            if (head)
                worldPos.z += 58;

            //multiply vector against matrix
            float screenX = (mat.m11 * worldPos.x) + (mat.m21 * worldPos.y) + (mat.m31 * worldPos.z) + mat.m41;
            float screenY = (mat.m12 * worldPos.x) + (mat.m22 * worldPos.y) + (mat.m32 * worldPos.z) + mat.m42;
            float screenW = (mat.m14 * worldPos.x) + (mat.m24 * worldPos.y) + (mat.m34 * worldPos.z) + mat.m44;

            //camera position (eye level/middle of screen)
            float camX = width / 2f;
            float camY = height / 2f;

            //convert to homogeneous position
            float x = camX + (camX * screenX / screenW);
            float y = camY - (camY * screenY / screenW);
            screenPos = new Vector2(x, y);

            //check if object is behind camera / off screen (not visible)
            //w = z where z is relative to the camera 
            return (screenW > 0.001f);
        }

        Point WorldToScreen(ViewMatrix mtx, Vector3 vec, int withe, int height, bool head) //ATM i dont fully understand this worldtoscreen Methode, so i paste it out of the internet
        {
            ////
            //Windows heith = 563
            //Windows width = 1174
            ///


            if (head)
                vec.z += 58;

            var twoD = new Point();
            
            float screenW = (mtx.m14 * vec.x) + (mtx.m24 * vec.y) + (mtx.m34 * vec.z) + mtx.m44;
            //float screenW = (mtx.m41 * vec.x) + (mtx.m42 * vec.y) + (mtx.m43 * vec.z) + mtx.m44;
            float screenX = (mtx.m11 * vec.x) + (mtx.m21 * vec.y) + (mtx.m31 * vec.z) + mtx.m41;
            float screenY = (mtx.m12 * vec.x) + (mtx.m22 * vec.y) + (mtx.m32 * vec.z) + mtx.m42;

            float camX = withe / 2f;
            float camY = height / 2f;

            float X = camX + (camX * screenX / screenW);
            float Y = camY - (camY * screenY / screenW);

            twoD.X = (int)X;
            twoD.Y = (int)Y;
            /*
            if (screenW > 0.001f)
            {

                return twoD;


            }
            else
            {
                return new Point(-99, -99);
            }
            */
            return twoD;





        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Local local = GetLocal();
            List<Enemy> enemys = GetEnemys(local);
            g = e.Graphics;

           for(int i = 0; i < enemys.Count; i++)
            {
                try
                {
                    //g.DrawRectangle(TeamPen, enemys[i].rect());
                    /*
                    g.DrawRectangle(TeamPen,Width /2 - 10,Height /2 + 10, 20, 20);

                    if (enemys[i].bottom.X > 0 && enemys[i].bottom.Y < Height && enemys[i].bottom.X < Width && enemys[i].bottom.Y > 0)
                    {
                        g.DrawRectangle(TeamPen, enemys[i].rect());
                        g.DrawRectangle(TeamPen, enemys[i].bottom.X, enemys[i].bottom.Y, 20, 20);
                    }
                    */
                   


                    if (enemys[i].Team == local.Team)
                    {
                        if(enemys[i].bottom.X > 0 && enemys[i].bottom.Y < Height && enemys[i].bottom.X < Width && enemys[i].bottom.Y > 0)
                        {
                            g.DrawRectangle(TeamPen, enemys[i].rect());
                        }
                    }
                    else
                    {
                        if (enemys[i].bottom.X > 0 && enemys[i].bottom.Y < Height && enemys[i].bottom.X < Width && enemys[i].bottom.Y > 0)
                        {
                            g.DrawRectangle(EnemyPen, enemys[i].rect());
                            
                        }
                    }
                    


                }
                catch
                {
                    MessageBox.Show("Eror");
                }
            }

        }
    }
}
