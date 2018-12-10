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
    class CorrectWord
    {
        public string word;
        readonly SpriteFont wordFont;
        public Rectangle Bounds;
        public Vector2 Position;
        Vector2 wordSize;
        public Color Color; 

        public CorrectWord(string word, SpriteFont wordFont)
        {
            this.word = word;
            this.wordFont = wordFont;
            wordSize = wordFont.MeasureString(word);
            Color = Color.Black;
        }

        public void UpdatePosition(Vector2 position)
        {
            Position = position;
            Bounds = new Rectangle(position.ToPoint(), wordSize.ToPoint());
        }

        public void Draw(SpriteBatch spriteBatch,GraphicsDevice graphicsDevice)
        {            
            spriteBatch.DrawString(wordFont, word, Position, Color);
        }
    }
}
