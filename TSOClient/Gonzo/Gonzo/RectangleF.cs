using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gonzo
{
    public class Rectanglef
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;

        public Rectanglef(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public float Top
        {
            get { return Y; }
        }
        public float Bottom
        {
            get { return Y + Height; }
        }
        public float Left
        {
            get { return X; }
        }
        public float Right
        {
            get { return X + Width; }
        }

        public Vector2 Position
        {
            get { return new Vector2(X, Y); }
        }
    }
}
