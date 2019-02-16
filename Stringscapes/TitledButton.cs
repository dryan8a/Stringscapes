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
    class TitledButton : Sprite 
    {
        readonly SpriteFont font;
        public Color initialColor;
        public Color hoveringColor;
        readonly string title;
        public bool isClicked;

        public TitledButton(string title, SpriteFont font, Vector2 pos, Color initialColor, Color hoveringColor, GraphicsDevice graphicsDevice) : base(new Texture2D(graphicsDevice, 1, 1), pos, initialColor, graphicsDevice)
        {
            Texture2D image = new Texture2D(graphicsDevice, 1, 1);
            image.SetData(new[] { Color.White });
            Image = image;
            this.font = font;
            this.initialColor = initialColor;
            this.hoveringColor = hoveringColor;
            this.title = title;
            Scale = font.MeasureString(title);
            isClicked = false;
        }

        public void Update(MouseState mouse)
        {
            if(Bounds.Contains(mouse.Position))
            {
                Color = hoveringColor;
                if(mouse.LeftButton == ButtonState.Pressed)
                {
                    isClicked = true;
                }
            }
            else
            {
                Color = initialColor;
                isClicked = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.DrawString(font, title, Position, Color.Black);
        }
    }
}