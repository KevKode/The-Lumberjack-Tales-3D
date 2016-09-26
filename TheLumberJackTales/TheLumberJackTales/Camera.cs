using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TheLumberJackTales
{
    class Camera
    {
        public Matrix camera;

        public Vector3 camPos, lookPos;

        public Camera(Vector3 posititon, Vector3 lookPosition)
        {
            camPos = posititon;
            lookPos = lookPosition;
            
            camera = Matrix.CreateLookAt(camPos, lookPos, Vector3.Up);
        }

        public void Update(KeyboardState keyState, Geometry g)
        {
            lookPos = g.geoPos + new Vector3(0f, 7f, 0f);
            camPos.Z = (float)Math.Cos(g.geoRotY + Math.PI) * 25 + g.geoPos.Z;
            camPos.X = (float)Math.Sin(g.geoRotY + Math.PI) * 25 + g.geoPos.X;

            camera = Matrix.CreateLookAt(camPos, lookPos, Vector3.Up);
        }
    }
}
