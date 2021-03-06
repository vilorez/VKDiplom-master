﻿using Microsoft.Xna.Framework;

namespace VKDiplom.Utilities
{
    public class ColorWheel
    {
        private static readonly Color[] Colors =
        {
            //Red
            Color.FromNonPremultiplied(165, 73, 78, 255),
            //Cyan
            Color.FromNonPremultiplied(73, 147, 165, 255),

            //Green
            Color.FromNonPremultiplied(73, 165, 101, 255),
            //Purple
            Color.FromNonPremultiplied(119, 73, 165, 255),

            //Yellow
            Color.FromNonPremultiplied(165, 145, 73, 255),
            //Blue
            Color.FromNonPremultiplied(73, 80, 165, 255)
        };

        private int _idx;
        public Color this[int i] => Colors[i%Colors.Length];

        public Color Next
        {
            get
            {
                var color = this[_idx];
                if (_idx < int.MaxValue)
                    ++_idx;
                else
                    _idx = 0;
                return color;
            }
        }

        public void Reset()
        {
            _idx = 0;
        }
    }
}