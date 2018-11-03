using DylanMonoGameIntro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stringscapes
{
    class BaseCircle : Sprite
    {
        public Vector2[] innerPoints;

        public Texture2D point;
        public BaseCircle(Texture2D image, Vector2 position, Color color, GraphicsDevice device,int distanceFromInnerCircle,int numberOfPoints) : base(image,position, color, device)
        {
            var points = new List<Vector2>();
            float arcLength = (float)(Math.PI * 2f / (double)numberOfPoints);
            innerPoints = new Vector2[numberOfPoints + 1];
            for (int i = 0; i < numberOfPoints + 1; i++)
            {
                float x = (float)Math.Cos(arcLength * i) * (Radius - distanceFromInnerCircle);
                float y = (float)Math.Sin(arcLength * i) * (Radius - distanceFromInnerCircle);
                x += Position.X+Radius;
                y += Position.Y+Radius;

                innerPoints[i] = new Vector2((int)x, (int)y);
            }
            point = new Texture2D(device, 2, 2);
            point.SetData(new[] { Color.White,Color.White,Color.White,Color.White });
        }
    }
}
