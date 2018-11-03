using DylanMonoGameIntro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Stringscapes
{
    class Letter : Sprite
    { 
        public char letter = ' ';
        SpriteFont font;
        Color fontColor = Color.Black;
        public bool isClicked = false;
        public bool didPass = false;
        public bool mouseHoveringOver = false;
        Line line;
        private MouseState currentState;
        private GraphicsDevice device;
        public Vector2 connectingLetterPos;
        public Letter(Texture2D texture, Vector2 position, Color color, char letter, SpriteFont font, GraphicsDevice device,Color lineColor) : base(texture, position, color, device)
        {
            this.letter = letter;
            this.font = font;
            line = new Line(lineColor, device);
            Origin = texture.Bounds.Center.ToVector2() / 2; 
            this.device = device;
        }
        public void Update(MouseState state)
        {
            currentState = state;
            if (Math.Pow((Position.X + Radius) - currentState.X, 2) + Math.Pow((Position.Y + Radius) - currentState.Y, 2) <= Math.Pow(Radius, 2) && currentState.LeftButton == ButtonState.Pressed)
            {
                Color = Color.OrangeRed;
                fontColor = Color.GhostWhite;
                isClicked = true;
                mouseHoveringOver = true;
            }
            else
            {
                mouseHoveringOver = false;
            }
            if (state.LeftButton == ButtonState.Released)
            {
                isClicked = false;
                didPass = false;
            }
            if (!isClicked && !didPass)
            {
                Color = Color.TransparentBlack;
                fontColor = Color.Black;
            }
            else
            {
                line.CalculateLine(Position + new Vector2(Radius, Radius),state.Position.ToVector2());
            }
            if(didPass)
            {
                line.CalculateLine(Position + new Vector2(Radius, Radius),connectingLetterPos);
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Image, Position, Color);
            Vector2 sizeOfText = font.MeasureString(letter.ToString());
            spriteBatch.DrawString(font, letter.ToString(), Position + new Vector2(Radius, Radius) - (sizeOfText / 2), fontColor);
        }
        public void DrawLine(SpriteBatch spriteBatch)
        {
            if (isClicked || didPass)
            {
                line.Draw(spriteBatch);
            }
        }
    }
}
