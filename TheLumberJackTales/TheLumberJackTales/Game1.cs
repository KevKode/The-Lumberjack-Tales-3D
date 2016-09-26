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

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        List<Geometry> geometry, squirrels, trees;
        Model tree1, tree2, tree3, shrub1, shrub2, squirrel, blood;
        float aspectRatio;

        Camera cam;

        KeyboardState keyState;
        KeyboardState lastKeyState;

        Random rand;

        float moveFactor;
        float rotFactor;
        float boundFactor;

        SpriteFont scorehealth;

        List <int> nums;

        BoundingSphere axe;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1300;
            graphics.PreferredBackBufferHeight = 750;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            geometry = new List<Geometry>();
            squirrels = new List<Geometry>();
            trees = new List<Geometry>();

            cam = new Camera(new Vector3(0f, 13f, 10f), new Vector3(0f, 7f, 35f));

            boundFactor = 379f;
            rotFactor = .025f;

            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;

            rand = new Random();

            nums = new List<int>();
            nums.Add(500);
            nums.Add(0);

            axe = new BoundingSphere(new Vector3(0f, 0f, 35f + (2.36f * 2.533f)), .75f);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            /*
             allObjects = $*
            for OBJ in allObjects do(
	            if OBJ.name [1] == "T" do (print (OBJ.pos.x as string+",0,"+OBJ.pos.y as string+","+OBJ.rotation.z_rotation as string))
            )
            */

            spriteBatch = new SpriteBatch(GraphicsDevice);

            scorehealth = Content.Load<SpriteFont>("scorehealth");

            geometry.Add(new Geometry(Content.Load<Model>("world"), Vector3.Zero, 0f, 0f, 0f));
            geometry.Add(new Geometry(Content.Load<Model>("house"), Vector3.Zero, 0f, 0f, 0f));
            geometry.Add(new Geometry(Content.Load<Model>("worker"), new Vector3(0f, 0f, 35f), 0f, .8f, 50f));

            tree1 = Content.Load<Model>("tree1");
            tree2 = Content.Load<Model>("tree2");
            tree3 = Content.Load<Model>("tree3");
            Console.Out.WriteLine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            string[] lines = System.IO.File.ReadAllLines(@"C:\Users\Kevin\Documents\Visual Studio 2015\Projects\TheLumberJackTales\TheLumberJackTales\TheLumberJackTalesContent\trees.txt");
            foreach (string line in lines)
            {
                string[] floats = line.Split(',');
                int r = rand.Next(1, 4);
                if (r == 3)
                    trees.Add(new Geometry(tree3, new Vector3(float.Parse(floats[0]) * 2.533f, float.Parse(floats[1]), float.Parse(floats[2]) * 2.533f), float.Parse(floats[3]), 8f, 0f));
                else if (r == 2)
                    trees.Add(new Geometry(tree2, new Vector3(float.Parse(floats[0]) * 2.533f, float.Parse(floats[1]), float.Parse(floats[2]) * 2.533f), float.Parse(floats[3]), 8f, 0f));
                else
                    trees.Add(new Geometry(tree1, new Vector3(float.Parse(floats[0]) * 2.533f, float.Parse(floats[1]), float.Parse(floats[2]) * 2.533f), float.Parse(floats[3]), 8f, 0f));
            }

            shrub1 = Content.Load<Model>("shrub1");
            lines = System.IO.File.ReadAllLines(@"C:\Users\Kevin\Documents\Visual Studio 2015\Projects\TheLumberJackTales\TheLumberJackTales\TheLumberJackTalesContent\shrub1.txt");
            foreach (string line in lines)
            {
                string[] floats = line.Split(',');
                geometry.Add(new Geometry(shrub1, new Vector3(float.Parse(floats[0]) * 2.533f, float.Parse(floats[1]), float.Parse(floats[2]) * 2.533f), float.Parse(floats[3]), 0f, 0f));
            }

            shrub2 = Content.Load<Model>("shrub2");
            lines = System.IO.File.ReadAllLines(@"C:\Users\Kevin\Documents\Visual Studio 2015\Projects\TheLumberJackTales\TheLumberJackTales\TheLumberJackTalesContent\shrub2.txt");
            foreach (string line in lines)
            {
                string[] floats = line.Split(',');
                geometry.Add(new Geometry(shrub2, new Vector3(float.Parse(floats[0]) * 2.533f, float.Parse(floats[1]), float.Parse(floats[2]) * 2.533f), float.Parse(floats[3]), 0f, 0f));
            }

            squirrel = Content.Load<Model>("squirrel");
            for (int y = 0; y < 100; y++)
            {
                float x = rand.Next(-350, 350);
                float z = rand.Next(-350, 350);
                squirrels.Add(new Geometry(squirrel, new Vector3(x, 0f, z), 0f, 1f, 0f));
            }

            blood = Content.Load<Model>("blood");

        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (nums[0] > 0)
            {
                keyState = Keyboard.GetState();

                geometry[2].UpdateCharacter(keyState, lastKeyState, moveFactor, rotFactor, boundFactor, trees);

                axe.Center.X = geometry[2].geoPos.X + (2.36f * 2.533f) * (float)Math.Sin(geometry[2].geoRotY);
                axe.Center.Z = geometry[2].geoPos.Z + (2.36f * 2.533f) * (float)Math.Cos(geometry[2].geoRotY);

                foreach (Geometry s in squirrels)
                    nums = s.UpdateSquirrel(geometry[2], nums, trees, squirrels, boundFactor, blood, axe);

                squirrels.RemoveAll(x => x.deathTime <= 0);

                cam.Update(keyState, geometry[2]);

                if (gameTime.TotalGameTime.Ticks % 15 == 0)
                {
                    int r = rand.Next(0, trees.Count());
                    float x = trees[r].boundSphere1.Center.X + trees[r].boundSphere1.Radius / 1.2f;
                    float z = trees[r].boundSphere1.Center.Z + trees[r].boundSphere1.Radius / 1.2f;
                    squirrels.Add(new Geometry(squirrel, new Vector3(x, 65f, z), (float)Math.Atan2(trees[r].boundSphere1.Center.X - x, trees[r].boundSphere1.Center.Z - z), 1f, 0f));
                }

                lastKeyState = keyState;
            }

            if (nums[0] < 0)
                nums[0] = 0;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            foreach (Geometry g in geometry)
            {
                // Copy any parent transforms.
                Matrix[] transforms = new Matrix[g.geo.Bones.Count];
                g.geo.CopyAbsoluteBoneTransformsTo(transforms);

                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in g.geo.Meshes)
                {
                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(g.geoRotY) * Matrix.CreateTranslation(g.geoPos);
                        effect.View = cam.camera;
                        effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10000.0f);
                    }

                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }
            }

            foreach (Geometry g in squirrels)
            {
                // Copy any parent transforms.
                Matrix[] transforms = new Matrix[g.geo.Bones.Count];
                g.geo.CopyAbsoluteBoneTransformsTo(transforms);

                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in g.geo.Meshes)
                {
                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(g.geoRotY) * Matrix.CreateTranslation(g.geoPos);
                        effect.View = cam.camera;
                        effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10000.0f);
                    }

                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }
            }

            foreach (Geometry g in trees)
            {
                // Copy any parent transforms.
                Matrix[] transforms = new Matrix[g.geo.Bones.Count];
                g.geo.CopyAbsoluteBoneTransformsTo(transforms);

                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in g.geo.Meshes)
                {
                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(g.geoRotY) * Matrix.CreateTranslation(g.geoPos);
                        effect.View = cam.camera;
                        effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10000.0f);
                    }

                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }
            }

            spriteBatch.Begin();
            spriteBatch.DrawString(scorehealth, "Killed: "+nums[1], new Vector2(0f, 20f), Color.White);
            spriteBatch.DrawString(scorehealth, "Health: "+nums[0], new Vector2(0f, 0f), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}