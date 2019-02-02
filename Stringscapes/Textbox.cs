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
        SpriteFont font;

        public void reset()
        {
            currentWord = "";
            Scale = font.MeasureString(" ");
            cursorIndex = 0;
        }

        public Textbox(Vector2 Position, Color color, SpriteFont font, GraphicsDevice graphicsDevice) : base(new Texture2D(graphicsDevice, 1, 1), Position, color, graphicsDevice)
        {            
            Texture2D image = new Texture2D(graphicsDevice, 1, 1);
            image.SetData(new[] { Color.White });
            Image = image;
            this.font = font;
            isClicked = false;
            currentWord = "";
            cursorIndex = 0;
            Scale = font.MeasureString(" ");
        }

        public void Update(MouseState mouse, KeyboardState keyboard, KeyboardState previousKeyboard)
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
                if (keyboard.IsKeyDown(Keys.D0) && previousKeyboard.IsKeyUp(Keys.D0))
                {
                    currentWord = currentWord.Insert(cursorIndex, "0");
                    cursorIndex++;
                }
                else if (keyboard.IsKeyDown(Keys.D1) && previousKeyboard.IsKeyUp(Keys.D1))
                {
                    currentWord = currentWord.Insert(cursorIndex, "1");
                    cursorIndex++;
                }
                else if (keyboard.IsKeyDown(Keys.D2) && previousKeyboard.IsKeyUp(Keys.D2))
                {
                    currentWord = currentWord.Insert(cursorIndex, "2");
                    cursorIndex++;
                }
                else if (keyboard.IsKeyDown(Keys.D3) && previousKeyboard.IsKeyUp(Keys.D3))
                {
                    currentWord = currentWord.Insert(cursorIndex, "3");
                    cursorIndex++;
                }            
                else if (keyboard.IsKeyDown(Keys.D4) && previousKeyboard.IsKeyUp(Keys.D4))
                {
                    currentWord = currentWord.Insert(cursorIndex, "4");
                    cursorIndex++;
                }
                else if (keyboard.IsKeyDown(Keys.D5) && previousKeyboard.IsKeyUp(Keys.D5))
                {
                    currentWord = currentWord.Insert(cursorIndex, "5");
                    cursorIndex++;
                }
                else if (keyboard.IsKeyDown(Keys.D6) && previousKeyboard.IsKeyUp(Keys.D6))
                {
                    currentWord = currentWord.Insert(cursorIndex, "6");
                    cursorIndex++;
                }
                else if (keyboard.IsKeyDown(Keys.D7) && previousKeyboard.IsKeyUp(Keys.D7))
                {
                    currentWord = currentWord.Insert(cursorIndex, "7");
                    cursorIndex++;
                }
                else if (keyboard.IsKeyDown(Keys.D8) && previousKeyboard.IsKeyUp(Keys.D8))
                {
                    currentWord = currentWord.Insert(cursorIndex, "8");
                    cursorIndex++;
                }
                else if (keyboard.IsKeyDown(Keys.D9) && previousKeyboard.IsKeyUp(Keys.D9))
                {
                    currentWord = currentWord.Insert(cursorIndex, "9");
                    cursorIndex++;
                }
                else if (keyboard.IsKeyDown(Keys.Back) && previousKeyboard.IsKeyUp(Keys.Back) && cursorIndex > 0)
                {
                    currentWord = currentWord.Remove(cursorIndex - 1, 1);
                    if (cursorIndex != 0)
                    {
                        cursorIndex--;
                    }
                }
                else if (keyboard.IsKeyDown(Keys.Left) && previousKeyboard.IsKeyUp(Keys.Left) && cursorIndex > 0)
                {
                    cursorIndex--;
                }
                else if (keyboard.IsKeyDown(Keys.Right) && previousKeyboard.IsKeyUp(Keys.Right) && cursorIndex < currentWord.Length)
                {
                    cursorIndex++;
                }
                if(currentWord == "")
                {
                    reset();
                }
                else
                {
                    Scale = font.MeasureString(currentWord);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.DrawString(font, currentWord, Position, Color.Black);
        }                           
    }
}
