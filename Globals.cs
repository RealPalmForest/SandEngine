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
}