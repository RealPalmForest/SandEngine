using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SandEngine;

public static class Globals
{
    public static SpriteBatch SpriteBatch { get; set; }
    public static GraphicsDevice GraphicsDevice { get; set; }
    public static ContentManager Content { get; set; }

    public static Random Random { get; private set; } = new Random();

    public static SpriteFont MainFont { get; set; }


    public static float NextFloat(float minValue, float maxValue)
    {
        double range = maxValue - minValue;
        double randomValue = Random.NextDouble() * range + minValue;
        return (float)randomValue;
    }

    public static float NextFloat(float maxValue) { return (float)Random.NextDouble() * maxValue; }
    public static float NextFloat() { return (float)Random.NextDouble(); }
}