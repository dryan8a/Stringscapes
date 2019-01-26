using DylanMonoGameIntro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stringscapes
{
    class Textbox : Sprite
    {
        bool isClicked;
        public string currentWord;
        int cursorIndex;

        public Textbox(Vector2 Position, Vector2 startSize, Color color, SpriteFont font, GraphicsDevice graphicsDevice) : base(new Texture2D(graphicsDevice, 1, 1), Position, color, graphicsDevice)
        {
            Texture2D image = new Texture2D(graphicsDevice, (int)startSize.X, (int)startSize.Y);
            image.SetData(new[] { Color.White });

            isClicked = false;
            currentWord = " ";
            cursorIndex = currentWord.Length - 1;
        }

        public void Update(MouseState mouse, KeyboardState keyboard)
        {
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                if (Bounds.Contains(mouse.Position))
                {
                    isClicked = true;
                }
                else
                { 
                    isClicked = false;
                }
            }

            if(isClicked)
            {
                if (keyboard.IsKeyDown(Keys.D0))
                {
                    currentWord.Insert(cursorIndex, "0");
                    cursorIndex++;
                }
                else if (keyboard.IsKeyDown(Keys.D1))
                {
                    currentWord.Insert(cursorIndex, "1");
                    cursorIndex++;
                }
                else if (keyboard.IsKeyDown(Keys.D2))
                {
                    currentWord.Insert(cursorIndex, "2");
                    cursorIndex++;
                }
                else if (keyboard.IsKeyDown(Keys.D3))
                {
                    currentWord.Insert(cursorIndex, "3");
                    cursorIndex++;
                }            
                else if (keyboard.IsKeyDown(Keys.D4))
                {
                    currentWord.Insert(cursorIndex, "4");
                    cursorIndex++;
                }
                else if (keyboard.IsKeyDown(Keys.D5))
                {
                    currentWord.Insert(cursorIndex, "5");
                    cursorIndex++;
                }
                else if (keyboard.IsKeyDown(Keys.D6))
                {
                    currentWord.Insert(cursorIndex, "6");
                    cursorIndex++;
                }
                else if (keyboard.IsKeyDown(Keys.D7))
                {
                    currentWord.Insert(cursorIndex, "7");
                    cursorIndex++;
                }
                else if (keyboard.IsKeyDown(Keys.D8))
                {
                    currentWord.Insert(cursorIndex, "8");
                    cursorIndex++;
                }
                else if (keyboard.IsKeyDown(Keys.D9))
                {
                    currentWord.Insert(cursorIndex, "9");
                    cursorIndex++;
                }
                else if (keyboard.IsKeyDown(Keys.Back))
                {
                    currentWord.Remove(cursorIndex, 1);
                }
                else if (keyboard.IsKeyDown(Keys.Left) && cursorIndex < currentWord.Length-1)
                {
                    cursorIndex++;
                }
                else if (keyboard.IsKeyDown(Keys.Right) && cursorIndex > 0)
                {
                    cursorIndex--;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

        }                           
    }
}
