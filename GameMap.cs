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
using System;
using System.Reflection;

namespace SandEngine;

public class GameMap
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public Texture2D ParticleTexture { get; private set; }

    public Particle[,] map;

    private Type[] spawnableParticles;
    private int selectedParticleIndex = 0;

    public GameMap(int width, int height)
    {
        ParticleTexture = new Texture2D(Globals.GraphicsDevice, 1, 1);
        ParticleTexture.SetData(new Color[] { Color.White });

        Width = width;
        Height = height;

        map = new Particle[Width, Height];

        spawnableParticles = new Type[] {
            new Sand(this).GetType(),
            new Water(this).GetType(),
            new Wood(this).GetType(),
            new Smoke(this).GetType(),
            new Dirt(this).GetType(),
            new Cement(this).GetType(),
            new Concrete(this).GetType()
        };
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


        DrawUI();
    }

    public void UpdateMap()
    {
        if (Math.Sign(InputManager.MouseOld.ScrollWheelValue - InputManager.Mouse.ScrollWheelValue) != 0)
        {
            selectedParticleIndex += Math.Sign(InputManager.MouseOld.ScrollWheelValue - InputManager.Mouse.ScrollWheelValue);
            selectedParticleIndex = Math.Clamp(selectedParticleIndex, 0, spawnableParticles.Length - 1);
            Debug.WriteLine("Selected particle: " + spawnableParticles[selectedParticleIndex].Name.ToUpper());
        }

        Point mousePos = InputManager.Mouse.Position;
        Point oldMousePos = InputManager.MouseOld.Position;

        if (InputManager.Mouse.LeftButton == ButtonState.Pressed)
        {
            foreach ((int, int) pos in GetLine(oldMousePos.X, oldMousePos.Y, mousePos.X, mousePos.Y))
            {
                bool stable = spawnableParticles[selectedParticleIndex].IsSubclassOf(typeof(StableParticle));

                //SetParticleAt(pos.Item1, pos.Item2, CreateParticle(spawnableParticles[selectedParticleIndex]));
                Fill(pos.Item1 - 4, pos.Item2 - 4, pos.Item1 + 4, pos.Item2 + 4, CreateParticle(spawnableParticles[selectedParticleIndex]), stable ? 1f : 0.03f);
            }
        }
        else if (InputManager.Mouse.RightButton == ButtonState.Pressed)
        {
            foreach ((int, int) pos in GetLine(oldMousePos.X, oldMousePos.Y, mousePos.X, mousePos.Y))
            {
                //SetParticleAt(pos.Item1, pos.Item2, spawnableParticles[selectedParticleIndex].Clone());
                Fill(pos.Item1 - 4, pos.Item2 - 4, pos.Item1 + 4, pos.Item2 + 4, null);
            }
        }

        // if (GetParticleAt(mousePos.X, mousePos.Y) != null) Debug.WriteLine(GetParticleAt(mousePos.X, mousePos.Y).Lifetime);

        for (int y = Height - 1; y >= 0; y--)
        {
            for (int x = 0; x < Width; x++)
            {
                if (map[x, y] == null)
                    continue;

                map[x, y].HasBeenUpdated = false;
                map[x, y].Lifetime++;
            }
        }

        for (int y = Height - 1; y >= 0; y--)
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

        if (InputManager.KeyUp(Keys.C))
            CountParticles();
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

    public Particle[] GetParticlesAround(int x, int y)
    {
        Particle[] around = new Particle[8];

        if (IsInBounds(x, y - 1)) around[0] = map[x, y - 1]; // Up
        if (IsInBounds(x + 1, y - 1)) around[1] = map[x + 1, y - 1]; // Up Right
        if (IsInBounds(x + 1, y)) around[2] = map[x + 1, y]; // Right
        if (IsInBounds(x + 1, y + 1)) around[3] = map[x + 1, y + 1]; // Down Right
        if (IsInBounds(x, y + 1)) around[4] = map[x, y + 1]; // Down
        if (IsInBounds(x - 1, y + 1)) around[5] = map[x - 1, y + 1]; // Down Left
        if (IsInBounds(x - 1, y)) around[6] = map[x - 1, y]; // Left
        if (IsInBounds(x - 1, y - 1)) around[7] = map[x - 1, y - 1]; // Up Left

        return around;
    }

    private void Fill(int startX, int startY, int endX, int endY, Particle particle, float chance = 1.0f)
    {
        for (int y = startY; y <= endY; y++)
        {
            for (int x = startX; x <= endX; x++)
            {
                if (Globals.Random.NextDouble() < chance)
                {
                    if (GetParticleAt(x, y) != null)
                        GetParticleAt(x, y).DisableSurroundingStaticState();

                    SetParticleAt(x, y, particle == null ? null : CreateParticle(particle.GetType()));
                }
            }
        }
    }

    public bool IsInBounds(int x, int y)
    {
        return !(x < 0 || y < 0 || x > Width - 1 || y > Height - 1);
    }


    /// <summary>
    /// Returns a list of points representing a line from (startX, startY) to (endX, endY) using Bresenham's Line Algorithm
    /// </summary>
    /// <param name="startX">The starting x coordinate</param>
    /// <param name="startY">The starting y coordinate</param>
    /// <param name="endX">The ending x coordinate</param>
    /// <param name="endY">The ending y coordinate</param>
    /// <returns>A list of points forming a line from the start to the end position.</returns>
    public static List<(int, int)> GetLine(int startX, int startY, int endX, int endY)
    {
        List<(int, int)> line = new List<(int, int)>();

        int changeX = Math.Abs(endX - startX);
        int changeY = Math.Abs(endY - startY);
        int dirX = startX < endX ? 1 : -1;
        int dirY = startY < endY ? 1 : -1;
        int err = changeX - changeY;

        while (true)
        {
            // Include the current point
            line.Add((startX, startY));

            // Check if the end point has been reached
            if (startX == endX && startY == endY)
                break;

            // Increase the X and Y in the appropriate directions
            int e2 = 2 * err;
            if (e2 > -changeY)
            {
                err -= changeY;
                startX += dirX;
            }

            if (e2 < changeX)
            {
                err += changeX;
                startY += dirY;
            }
        }

        return line;
    }

    public Particle CreateParticle(Type type)
    {
        return Activator.CreateInstance(type, this) as Particle;
    }

    public int CountParticles()
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

        Debug.WriteLine("PARTICLES: " + particleCount.ToString("#,##0") + "   |   STATIC: " + staticCount.ToString("#,##0"));
        return particleCount;
    }

    private void DrawUI()
    {
        Globals.SpriteBatch.DrawString(Globals.MainFont, spawnableParticles[selectedParticleIndex].Name, new Vector2(15, 15), Color.White);
    }
}