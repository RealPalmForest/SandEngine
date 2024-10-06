using System.Data;
using System.Linq;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SandEngine.Particles;
using SandEngine.AbstractParticles;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SandEngine;

public class GameMap
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public Texture2D ParticleTexture { get; private set; }

    public Particle[,] map;

    public GameMap(int width, int height)
    {
        ParticleTexture = new Texture2D(Globals.GraphicsDevice, 1, 1);
        ParticleTexture.SetData(new Color[] { Color.White });

        Width = width;
        Height = height;

        map = new Particle[Width, Height];
    }

    public void Draw()
    {
        List<Color> render = new List<Color>();

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (map[x, y] != null)
                {
                    render.Add(map[x, y].Color);

                    //Globals.SpriteBatch.Draw(ParticleTexture, new Rectangle(x, y, 1, 1), map[x, y].Color);
                    //map[x, y].Draw();
                }
                else render.Add(default);
            }
        }

        Texture2D texture = new Texture2D(Globals.GraphicsDevice, Width, Height);
        texture.SetData(render.ToArray());
        Globals.SpriteBatch.Draw(texture, Vector2.Zero, Color.White);
    }

    public void UpdateMap()
    {
        int mouseX = InputManager.Mouse.X;
        int mouseY = InputManager.Mouse.Y;
        // if(IsInBounds(mouseX, mouseY))
        // {
        //     Debug.Write(InputManager.Mouse.Position + " --- " + map[mouseX, mouseY]);
        //     if(map[mouseX, mouseY] == null)
        //         Debug.WriteLine("");
        //     else Debug.WriteLine("");
        // }           

        if (InputManager.Mouse.LeftButton == ButtonState.Pressed)
        {
            Fill(InputManager.Mouse.X - 2, InputManager.Mouse.Y - 2, InputManager.Mouse.X + 2, InputManager.Mouse.Y + 2, new Sand(this));
            //SetParticleAt(InputManager.Mouse.X, InputManager.Mouse.Y, new Sand(this));
        }
        else if (InputManager.Mouse.RightButton == ButtonState.Pressed)
        {
            Fill(InputManager.Mouse.X - 2, InputManager.Mouse.Y - 2, InputManager.Mouse.X + 2, InputManager.Mouse.Y + 2, new Water(this));
            //SetParticleAt(InputManager.Mouse.X, InputManager.Mouse.Y, new Water(this));
        }
        else if (InputManager.Mouse.MiddleButton == ButtonState.Pressed)
        {
            Fill(InputManager.Mouse.X - 2, InputManager.Mouse.Y - 2, InputManager.Mouse.X + 2, InputManager.Mouse.Y + 2, new Wood(this));
            //SetParticleAt(InputManager.Mouse.X, InputManager.Mouse.Y, new Water(this));
        }

        if (InputManager.KeyUp(Keys.C))
        {
            int particleCount = 0;
            int staticCount = 0;

            for (int y = Height - 1; y > 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (map[x, y] != null)
                    {
                        particleCount++;

                        if (map[x, y].StaticState)
                            staticCount++;
                    }
                }
            }

            Debug.WriteLine("PARTICLE COUNT: " + particleCount + "  |   STATIC: " + staticCount);
        }

        for (int y = Height - 1; y > 0; y--)
        {
            for (int x = 0; x < Width; x++)
            {
                if (map[x, y] == null)
                    continue;

                map[x, y].HasBeenUpdated = false;
            }
        }

        for (int y = Height - 1; y > 0; y--)
        {
            for (int x = 0; x < Width; x++)
            {
                if (map[x, y] == null)
                    continue;

                if (map[x, y].HasBeenUpdated || map[x, y].StaticState)
                    continue;

                map[x, y].HasBeenUpdated = true;
                map[x, y].UpdateStaticState();
                map[x, y].Update();
            }
        }
    }

    public Particle GetParticleAt(int x, int y)
    {
        if (!IsInBounds(x, y)) return null;
        return (map[x, y] is MovingParticle) ? (MovingParticle)map[x, y] : map[x, y];
    }

    public void SetParticleAt(int x, int y, Particle particle)
    {
        if (x < 0 || y < 0 || x > Width - 1 || y > Height - 1)
            return;

        if (particle != null)
        {
            particle.X = x;
            particle.Y = y;
        }

        map[x, y] = particle;
    }

    private void Fill(int startX, int startY, int endX, int endY, Particle particle)
    {
        for (int y = startY; y <= endY; y++)
        {
            for (int x = startX; x <= endX; x++)
            {
                SetParticleAt(x, y, particle.Clone());
            }
        }
    }

    public bool IsInBounds(int x, int y)
    {
        return !(x < 0 || y < 0 || x > Width - 1 || y > Height - 1);
    }
}