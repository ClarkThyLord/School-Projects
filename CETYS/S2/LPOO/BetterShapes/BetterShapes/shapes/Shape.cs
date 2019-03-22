﻿using System;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;

namespace BetterShapes.shapes
{
    abstract class Shape
    {
        private static int _id = 0;

        public static int min_size = 10, max_size = 100;

        public int id;
        public string name = "";

        public Canvas canvas;

        private int x = 0;
        public int X
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;

                if (this.visible) draw();
            }
        }

        private int y = 0;
        public int Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;

                if (this.visible) draw();
            }
        }

        private int size = 0;
        public int Size
        {
            get
            {
                return this.size;
            }
            set
            {
                this.size = value;

                if (this.visible) draw();
            }
        }

        private double scale = 1;
        public double Scale
        {
            get
            {
                return this.scale;
            }
            set
            {
                this.scale = value;

                if (this.visible) draw();
            }
        }

        private System.Windows.Shapes.Shape _shape;
        public System.Windows.Shapes.Shape shape
        {
            get
            {
                return this._shape;
            }
            set
            {
                this._shape = value;
                this._shape.Fill = this.color;
                this._shape.Opacity = this.Opacity;

                if (this.visible) draw();
            }
        }

        public SolidColorBrush color = new SolidColorBrush();

        private double opacity = 0;
        public double Opacity
        {
            get
            {
                return this.opacity;
            }
            set
            {
                this.opacity = value > 1 ? 1 : (value < 0 ? 0 : value);

                if (this.shape != null) this.shape.Opacity = this.opacity;

                if (this.visible) draw();
            }
        }

        private bool visible = true;
        public bool Visible
        {
            get
            {
                return this.visible;
            }
            set
            {
                this.visible = value;

                if (this.visible) draw();
            }
        }

        public Shape()
        {
            this.id = _id;
            _id++;
        }

        public Shape(Canvas canvas, Random random, bool visible=true) : this()
        {
            this.canvas = canvas;

            this.visible = visible;

            randomize(random);
        }

        public Shape(Canvas canvas, int x, int y, int size, int scale=1, Color color=new Color(), double opacity = 1, bool visible=true) : this()
        {
            this.canvas = canvas;

            this.x = x;
            this.y = y;
            this.scale = scale;
            this.size = size;

            this.color.Color = color;
            this.opacity = opacity > 1 ? 1 : (opacity < 0 ? 0 : opacity);
            this.visible = visible;

            if (this.visible) this.draw();
        }

        public void randomize(Random random, int margin=0)
        {
            this.size = random.Next(min_size, max_size);
            this.x = random.Next(1 + margin, (int)this.canvas.ActualWidth - this.size - margin);
            this.y = random.Next(1 + margin, (int)this.canvas.ActualHeight - this.size - margin);

            this.color.Color = Color.FromRgb((byte)random.Next(0, 266), (byte)random.Next(0, 266), (byte)random.Next(0, 266));
            this.opacity = random.NextDouble();
            if (this.shape != null) this.shape.Opacity = this.opacity;

            if (this.visible) this.draw();
        }

        public abstract void draw();
    }
}
