using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SandEngine
{
    public static class InputManager
    {
        public static KeyboardState Keyboard { get; private set; }
        public static KeyboardState KeyboardOld { get; private set; }
        public static MouseState Mouse { get; private set; }
        public static MouseState MouseOld { get; private set; }

        public static Vector2 Direction => direction;

        public static int ScrollWheelValue { get; private set; }
        public static int OldScrollWheelValue { get; private set; }

        private static Vector2 direction;


        public static void Update()
        {
            KeyboardOld = Keyboard;
            Keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();

            MouseOld = Mouse;
            Mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();

            OldScrollWheelValue = ScrollWheelValue;
            ScrollWheelValue = Mouse.ScrollWheelValue;

            direction = Vector2.Zero;

            if (Keyboard.IsKeyDown(Keys.W) || Keyboard.IsKeyDown(Keys.Up)) direction.Y--;
            if (Keyboard.IsKeyDown(Keys.S) || Keyboard.IsKeyDown(Keys.Down)) direction.Y++;
            if (Keyboard.IsKeyDown(Keys.A) || Keyboard.IsKeyDown(Keys.Left)) direction.X--;
            if (Keyboard.IsKeyDown(Keys.D) || Keyboard.IsKeyDown(Keys.Right)) direction.X++;

            if (direction != Vector2.Zero)
            {
                direction.Normalize();
            }
        }

        public static bool KeyDown(Keys key)
        {
            return Keyboard.IsKeyDown(key) && KeyboardOld.IsKeyUp(key);
        }

        public static bool KeyUp(Keys key)
        {
            return Keyboard.IsKeyUp(key) && KeyboardOld.IsKeyDown(key);
        }

        public static bool MouseDown(int button)
        {
            if (button == 0)
            {
                return Mouse.LeftButton == ButtonState.Pressed && MouseOld.LeftButton == ButtonState.Released;
            }
            else if (button == 1)
            {
                return Mouse.RightButton == ButtonState.Pressed && MouseOld.RightButton == ButtonState.Released;
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}
