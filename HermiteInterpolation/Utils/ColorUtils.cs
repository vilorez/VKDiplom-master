﻿using System; using System.Threading; using System.Threading.Tasks; using HermiteInterpolation.Numerics; using Microsoft.Xna.Framework;  namespace HermiteInterpolation.Utils {     public class ColorUtils     {         public static void ToHsv(Color color, out float h, out float s, out float v, out float a)         {             float r = color.R/255f, g=color.G/255f, b=color.B/255f;             a = color.A/255f;              float min, max, delta;             min = r < g ? r : g;             min = min < b ? min : b;             max = r > g ? r : g;             max = max > b ? max : b;             v = max;             delta = max - min;             if (max > 0.0f)             {                 s = delta/max;             }             else             {                 s = 0.0f;                 h = 0;                 //return;             }             if (r >= max)                 h = (g - b)/delta;             else if (g >= max)                 h = 2f + (b - r);             else                 h = 4f + (r - g)/delta;             h *= 60f;             while (h<0f)             {                 h += 360;             }                       }          public static Color FromHsv(float h, float s, float v, float a)         {             // ######################################################################             // T. Nathan Mundhenk             // mundhenk@usc.edu             // C/C++ Macro HSV to RGB              float R, G, B;             if (v <= 0)             {                 R = G = B = 0;             }             else if (s <= 0)             {                 R = G = B = v;             }             else             {                 float hf = h/60.0f;                 var i = (int) Math.Floor(hf);                 float f = hf - i;                 float pv = v*(1 - s);                 float qv = v*(1 - s*f);                 float tv = v*(1 - s*(1 - f));                 switch (i)                 {                         // Red is the dominant color                      case 0:                         R = v;                         G = tv;                         B = pv;                         break;                          // Green is the dominant color                      case 1:                         R = qv;                         G = v;                         B = pv;                         break;                     case 2:                         R = pv;                         G = v;                         B = tv;                         break;                          // Blue is the dominant color                      case 3:                         R = pv;                         G = qv;                         B = v;                         break;                     case 4:                         R = tv;                         G = pv;                         B = v;                         break;                          // Red is the dominant color                     case 5:                         R = v;                         G = pv;                         B = qv;                         break;                          // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.                     case 6:                         R = v;                         G = tv;                         B = pv;                         break;                     case -1:                         R = v;                         G = pv;                         B = qv;                         break;                          // The color is not defined, we should throw an error.                      default:                         //LFATAL("i Value error in Pixel conversion, Value is %d", i);                         R = G = B = v; // Just pretend its black/white                         break;                 }             }             var r = (int) MathHelper.Clamp(R*255, 0, 255);             var g = (int) MathHelper.Clamp(G*255, 0, 255);             var b = (int) MathHelper.Clamp(B*255, 0, 255);             var al = (int) MathHelper.Clamp(a*255, 0, 255);              return Color.FromNonPremultiplied(r, g, b, al);         }          public delegate Color SeedColor(params object[] parameters);          public static Color Random()         {                       var random = RandomNumber.Instance;             var r = random.Next(64, 164);             var g = random.Next(64, 164);             var b = random.Next(64, 164);             return Color.FromNonPremultiplied(r, g, b, 255);         }          public static Color RandomShade(Color baseColor)         {             return RandomShade(baseColor, 0.15f, 0.3f, 0.7f);         }          public static Color RandomShade(float hue)         {             return RandomShade(hue, 0.15f, 0.3f, 0.7f);         }          public static Color RandomShade(float hue, float shadeDiffusion, float fromValue,             float toValue)         {             ////return baseColor;                       var random = RandomNumber.Instance;             var diff = (float)random.NextDouble() * shadeDiffusion;             MathHelper.Clamp(hue, 0f, 360f);             var s = random.NextDouble() < 0.5 ? 0.5f + diff : 0.5f- diff;             MathHelper.Clamp(s, fromValue, toValue);             var v = (float)random.NextDouble() * (toValue - fromValue) + fromValue;             MathHelper.Clamp(v, fromValue, toValue);              return FromHsv(hue, s, v, 1);             //new          }          public static Color RandomShade(Color baseColor, float shadeDiffusion, float fromValue,             float toValue)         {             ////return baseColor;             float h, s, v, a;             ToHsv(baseColor, out h, out s, out v, out a);             var random = RandomNumber.Instance;             var diff = (float) random.NextDouble()*shadeDiffusion;             h = random.NextDouble() < 0.5 ? h + diff : h - diff;             MathHelper.Clamp(h, 0f, 360f);             s = random.NextDouble() < 0.5 ? s + diff : s - diff;             MathHelper.Clamp(s, fromValue, toValue);             v = (float)random.NextDouble()*(toValue-fromValue) + fromValue;             MathHelper.Clamp(v, fromValue, toValue);              return FromHsv(h, s, v, a);         }     } }