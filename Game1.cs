using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SandEngine;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private GameMap world;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        Globals.GraphicsDevice = GraphicsDevice;
        Globals.SpriteBatch = _spriteBatch;
        Globals.Content = Content;
        Globals.MainFont = Content.Load<SpriteFont>("MainFont");

        world = new GameMap(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        //Debug.WriteLine(GraphicsDevice.Viewport.Bounds);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if(IsActive)
        {
            InputManager.Update();
        }

        world.UpdateMap();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        world.Draw();

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
