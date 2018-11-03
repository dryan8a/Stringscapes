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
    public class Line : Sprite
    {
        protected Vector2 end;
        public float Length => Scale.X;

        public Line(Color color, GraphicsDevice device)
            : base(null, Vector2.Zero, color, device)
        {
        }
        
        public void CalculateLine(Vector2 start, Vector2 end)
        {
            Position = start;
            var distanceVector = end - Position;
            Rotation = (float)Math.Atan2(distanceVector.Y, distanceVector.X);
            Scale.X = distanceVector.Length();
            Scale.Y = 10;
        }        
    }
}
