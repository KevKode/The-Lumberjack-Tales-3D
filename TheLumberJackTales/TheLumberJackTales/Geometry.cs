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
    class Geometry
    {
        public Model geo;
        public Vector3 geoPos, lastGeoPos;
        public float geoRotY;
        public BoundingSphere boundSphere1, boundSphere2;

        public int jumpTime, deathTime;

        public Geometry(Model model, Vector3 position, float rotation, float radius1, float radius2)
        {
            geo = model;
            geoPos = position;
            geoRotY = rotation;
            boundSphere1 = new BoundingSphere(position, radius1);
            boundSphere2 = new BoundingSphere(position, radius2);
            deathTime = 50;
        }

        public void UpdateCharacter(KeyboardState keyState, KeyboardState lastKeyState, float moveFactor, float rotFactor, float boundFactor, List<Geometry> trees)
        {
            lastGeoPos = geoPos;

            //START ROTATION
            //LEFT
            if (keyState.IsKeyDown(Keys.A))
                geoRotY += rotFactor;

            //RIGHT
            if (keyState.IsKeyDown(Keys.D))
                geoRotY -= rotFactor;
            //END ROTATION  

            if (keyState.IsKeyDown(Keys.S))
                moveFactor = .28f;
            else if ((keyState.IsKeyDown(Keys.Q) && keyState.IsKeyDown(Keys.W)) || (keyState.IsKeyDown(Keys.E) && keyState.IsKeyDown(Keys.W)))
                moveFactor = .42f;
            else
                moveFactor = .5f;

            //START MOVEMENT
            //FORWARD
            if (keyState.IsKeyDown(Keys.W))
            {
                geoPos.X += (float)Math.Sin(geoRotY) * moveFactor;
                geoPos.Z += (float)Math.Cos(geoRotY) * moveFactor;
            }

            //BACKWARD
            if (keyState.IsKeyDown(Keys.S))
            {
                geoPos.X -= (float)Math.Sin(geoRotY) * moveFactor;
                geoPos.Z -= (float)Math.Cos(geoRotY) * moveFactor;
            }

            //STRAFE LEFT
            if (keyState.IsKeyDown(Keys.Q))
            {
                geoPos.X += (float)Math.Sin(geoRotY + MathHelper.PiOver2) * moveFactor;
                geoPos.Z += (float)Math.Cos(geoRotY + MathHelper.PiOver2) * moveFactor;
            }

            //STRAFE RIGHT
            if (keyState.IsKeyDown(Keys.E))
            {
                geoPos.X -= (float)Math.Sin(geoRotY + MathHelper.PiOver2) * moveFactor;
                geoPos.Z -= (float)Math.Cos(geoRotY + MathHelper.PiOver2) * moveFactor;
            }

            //START JUMP
            if (keyState.IsKeyDown(Keys.Space) && !lastKeyState.IsKeyDown(Keys.Space) && geoPos.Y == 0f)
                jumpTime = 30;

            if (jumpTime > 25)
                geoPos.Y += .5f;
            else if (jumpTime > 20)
                geoPos.Y += .25f;
            else if (jumpTime > 15)
                geoPos.Y += .125f;
            else if (jumpTime > 10)
                geoPos.Y -= .125f;
            else if (jumpTime > 5)
                geoPos.Y -= .25f;
            else if (jumpTime > 0)
                geoPos.Y -= .5f;
            else
                geoPos.Y = 0f;

            jumpTime--;
            //END JUMP 

            if (geoPos.X >= -13.25f && geoPos.X <= 13.25f && geoPos.Z >= -35.75f && geoPos.Z <= 25.75f)
            {
                if (geoPos.X > 0 && lastGeoPos.X >= 13.25f)
                    geoPos.X = 13.25f;
                if (geoPos.X < 0 && lastGeoPos.X <= -13.25f)
                    geoPos.X = -13.25f;
                if (geoPos.Z > 0 && lastGeoPos.Z >= 25.75f)
                    geoPos.Z = 25.75f;
                if (geoPos.Z < 0 && lastGeoPos.Z <= -35.75f)
                    geoPos.Z = -35.75f;
            }

            if (geoPos.X > boundFactor)
                geoPos.X = boundFactor;
            if (geoPos.X < -boundFactor)
                geoPos.X = -boundFactor;   
            if (geoPos.Z > boundFactor)
                geoPos.Z = boundFactor;
            if (geoPos.Z < -boundFactor)
                geoPos.Z = -boundFactor;

            boundSphere1.Center = geoPos;
            boundSphere2.Center = geoPos;

            foreach (Geometry tree in trees)
            {
                if (tree.boundSphere1.Intersects(boundSphere1))
                {
                    geoPos.X = tree.boundSphere1.Center.X - (float)Math.Cos(Math.Atan2(tree.boundSphere1.Center.Z - geoPos.Z, tree.boundSphere1.Center.X - geoPos.X)) * (tree.boundSphere1.Radius + boundSphere1.Radius);
                    geoPos.Z = tree.boundSphere1.Center.Z - (float)Math.Sin(Math.Atan2(tree.boundSphere1.Center.Z - geoPos.Z, tree.boundSphere1.Center.X - geoPos.X)) * (tree.boundSphere1.Radius + boundSphere1.Radius);
                    boundSphere1.Center = geoPos;
                    boundSphere2.Center = geoPos;
                }
            }
            //END MOVEMENT

            //Console.Out.WriteLine(geoPos.X + ", " + geoPos.Z);
        }

        public List<int> UpdateSquirrel(Geometry target, List<int> nums, List<Geometry> trees, List<Geometry> squirrels, float boundFactor, Model blood, BoundingSphere axe)
        {
            if (geo != blood)
            {
                if (geoPos.Y > 0)
                {
                    geoPos.Y -= .75f;
                }
                else
                {
                    geoRotY = (float)Math.Atan2(target.geoPos.X - geoPos.X, target.geoPos.Z - geoPos.Z);

                    lastGeoPos = geoPos;

                    geoPos.X += (float)Math.Sin(geoRotY) * .25f;
                    geoPos.Z += (float)Math.Cos(geoRotY) * .25f;

                    if (geoPos.X > boundFactor)
                        geoPos.X = boundFactor;
                    if (geoPos.X < -boundFactor)
                        geoPos.X = -boundFactor;
                    if (geoPos.Z > boundFactor)
                        geoPos.Z = boundFactor;
                    if (geoPos.Z < -boundFactor)
                        geoPos.Z = -boundFactor;

                    boundSphere1.Center = geoPos;
                    boundSphere2.Center = geoPos;
                    boundSphere2.Radius = 3f;

                    foreach (Geometry tree in trees)
                    {
                        if (tree.boundSphere1.Intersects(boundSphere1))
                        {
                            geoPos.X = tree.boundSphere1.Center.X - (float)Math.Cos(Math.Atan2(tree.boundSphere1.Center.Z - geoPos.Z, tree.boundSphere1.Center.X - geoPos.X)) * (tree.boundSphere1.Radius + boundSphere1.Radius);
                            geoPos.Z = tree.boundSphere1.Center.Z - (float)Math.Sin(Math.Atan2(tree.boundSphere1.Center.Z - geoPos.Z, tree.boundSphere1.Center.X - geoPos.X)) * (tree.boundSphere1.Radius + boundSphere1.Radius);
                            boundSphere1.Center = geoPos;
                        }
                    }

                    if (target.boundSphere2.Contains(boundSphere1) == ContainmentType.Contains)
                    {
                        foreach (Geometry s in squirrels)
                        {
                            if (target.boundSphere2.Contains(s.boundSphere1) == ContainmentType.Contains && s.boundSphere1.Intersects(boundSphere1) && s.geo != blood && s != this)
                            {
                                geoPos.X = s.boundSphere1.Center.X - (float)Math.Cos(Math.Atan2(s.boundSphere1.Center.Z - geoPos.Z, s.boundSphere1.Center.X - geoPos.X)) * (s.boundSphere1.Radius + boundSphere1.Radius);
                                geoPos.Z = s.boundSphere1.Center.Z - (float)Math.Sin(Math.Atan2(s.boundSphere1.Center.Z - geoPos.Z, s.boundSphere1.Center.X - geoPos.X)) * (s.boundSphere1.Radius + boundSphere1.Radius);
                                boundSphere1.Center = geoPos;
                            }
                        }

                        if (boundSphere1.Intersects(axe) && Mouse.GetState().LeftButton == ButtonState.Pressed)
                        {
                            //nums[0]--;
                            geo = blood;
                            nums[1]++;
                            //geoPos = lastGeoPos;
                            //boundSphere1.Center = geoPos;
                        }

                        if (boundSphere1.Intersects(target.boundSphere1))
                        {
                            nums[0]--;
                            //geo = blood;
                            //geoPos = lastGeoPos;
                            //boundSphere1.Center = geoPos;
                        }
                    }

                    if (geoPos.X >= -13.25f && geoPos.X <= 13.25f && geoPos.Z >= -35.75f && geoPos.Z <= 25.75f)
                    {
                        if (geoPos.X > 0 && lastGeoPos.X >= 13.25f)
                            geoPos.X = 13.25f;
                        if (geoPos.X < 0 && lastGeoPos.X <= -13.25f)
                            geoPos.X = -13.25f;
                        if (geoPos.Z > 0 && lastGeoPos.Z >= 25.75f)
                            geoPos.Z = 25.75f;
                        if (geoPos.Z < 0 && lastGeoPos.Z <= -35.75f)
                            geoPos.Z = -35.75f;
                        boundSphere1.Center = geoPos;
                    }

                    //num[0]--;
                }
            }

            else
                deathTime--;

            return nums;
        }
    }
}
