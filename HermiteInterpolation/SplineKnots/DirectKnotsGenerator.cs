﻿using System;
using HermiteInterpolation.Numerics;
using HermiteInterpolation.Numerics.MathFunctions;
using HermiteInterpolation.Shapes.SplineInterpolation;

namespace HermiteInterpolation.SplineKnots
{
    public class DirectKnotsGenerator : KnotsGenerator
    {
        public DirectKnotsGenerator(InterpolativeMathFunction function)
            : base(function)
        {
        }

        public DirectKnotsGenerator(MathExpression expression)
            : base(expression)
        {
        }

        public override KnotMatrix GenerateKnots(SurfaceDimension uDimension,
            SurfaceDimension vDimension)
        {
            var values = new KnotMatrix(uDimension.KnotCount,
                vDimension.KnotCount);
            var uSize = Math.Abs(uDimension.Max - uDimension.Min)
                        /(uDimension.KnotCount - 1);
            var vSize = Math.Abs(vDimension.Max - vDimension.Min)
                        /(vDimension.KnotCount - 1);
            var u = uDimension.Min;

            for (var i = 0; i < uDimension.KnotCount; i++, u += uSize)
            {
                var v = vDimension.Min;
                for (var j = 0; j < vDimension.KnotCount; j++, v += vSize)
                {
                    var z = Function.Z.SafeCall(u, v); //Z(u, v);
                    var dx = Function.Dx.SafeCall(u, v); //Dx(u, v);
                    var dy = Function.Dy.SafeCall(u, v); //Dy(u, v);
                    var dxy = Function.Dxy.SafeCall(u, v); //Dxy(u, v);
                    values[i, j] = new Knot(u, v, z, dx, dy, dxy);
                }
            }
            return values;
        }
    }
}