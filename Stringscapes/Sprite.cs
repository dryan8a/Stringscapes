using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DylanMonoGameIntro
{
    public class Sprite
    {
        public enum BoundingBoxTypes
        {
            None,
            Rectangle,
            Circle
        }

        public Texture2D Image;
        public Vector2 Position;
        public Color Color;
        public Vector2 Scale = Vector2.One;
        public Vector2 Origin = Vector2.Zero;
        public BoundingBoxTypes DrawBounds = BoundingBoxTypes.None;
        public float Rotation = 0f;
        public static Texture2D pixel;        

        public virtual Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)(Image.Width * Scale.X), (int)(Image.Height * Scale.Y));
            }          
        }
//        public Vector2 Midpoint
//        {
//           get { return new Vector2(Image.Width * Scale.X / 2 + Position.X, Image.Height * Scale.Y / 2 + Position.Y); }
//        }
        public float Radius
        {
            get { return Image.Width * Scale.X / 2; }
        }
        public static void CreatePixel(GraphicsDevice graphicsDevice)
        {
            if(pixel == null)
            {
                pixel = new Texture2D(graphicsDevice, 1, 1);
                pixel.SetData(new [] { Color.White });
            }
        }

        public Sprite(Texture2D Image, Vector2 Position, Color Color,GraphicsDevice device)
        {
            if (Image == null)
            {
                CreatePixel(device);
                Image = pixel;
            }
            this.Image = Image;
            this.Position = Position;
            this.Color = Color;     
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Image, Position, null, Color, Rotation, Origin, Scale, SpriteEffects.None, 0);

            switch(DrawBounds)
            {
                case BoundingBoxTypes.Rectangle:
                    spriteBatch.Draw(pixel, Position - Origin * Scale, null, Color.Red * 0.5f, 0f, Vector2.Zero, new Vector2(Image.Width, Image.Height) * Scale, SpriteEffects.None, 0f);            
                    break;

                case BoundingBoxTypes.Circle:
                    var points = new List<Vector2>();
                    int numberOfPoints = 1080;
                        float arcLength = (float)(Math.PI * 2f / (double)numberOfPoints);
                        Vector2[] boundPoints = new Vector2[numberOfPoints+1];
                    for (int t = 0; t < 3; t++)
                    {
                        for (int i = 0; i < numberOfPoints + 1; i++)
                        {
                            float x = (float)Math.Cos(arcLength * i) * (Radius-t);
                            float y = (float)Math.Sin(arcLength * i) * (Radius-t);

                            x += Position.X;
                            y += Position.Y;

                            boundPoints[i] = new Vector2((int)x, (int)y);
                        }
                    }   
                    for (int i = 0; i < numberOfPoints+1; i++)
                    {
                        spriteBatch.Draw(pixel, boundPoints[i], null, Color.Red * 0.5f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }
                        boundPoints[numberOfPoints] = boundPoints[0];
                    
                    break;
            }
        }
    }
}
