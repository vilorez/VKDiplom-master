﻿using System;
using MathNet.Numerics;

namespace HermiteInterpolation.MathFunctions
{
   

    /// <summary>
    ///     Defines expression and it's derivations in form of delegates.
    /// </summary>
    public class InterpolativeMathFunction
    {
        // Constant for calculating derivations.
        //private const double H = 0.00001;

        /// <summary>
        ///     Parameter z specifies expression to be approximated Hermite spline.
        ///     All required derivations are calculated automatically.
        /// </summary>
        /// <param name="z"></param>
        public InterpolativeMathFunction(MathFunction z)
        {
            //const double h = Epsilon;
            //const double h2 = h*2;
            Z = z;
            // Partial derivations 
            //const double h2 = 2*H;
            //Differentiate.FirstPa
            Dx = new MathFunction(Differentiate.FirstPartialDerivativeFunc(new Func<double[], double>(z), 0));
            //(a,b)=>Differentiate.FirstPartialDerivative2Func((x,y)=>z(x,y), 0)(a,b);
            //(x, y) => (z(x + x*h, y) - z(x - x*h, y)) / (h2*x*x);

            Dy = new MathFunction(Differentiate.FirstPartialDerivativeFunc(new Func<double[], double>(z), 1));
                //(x, y) => (z(x, y + y*h) - z(x, y - y*h)) / (y*y*h2);//

            Dxy = new MathFunction(Differentiate.FirstPartialDerivativeFunc(new Func<double[], double>(Dx), 1));
            //Differentiate.FirstPartialDerivative2Func(Dx, 1);
            //(x, y) => (Dx(x, y + y*h) - Dx(x, y - y*h)) / (y*y*h2);//

            // better derivation [f(x-2h) - 8f(x-h) + 8f(x+h) - f(x+2h)] / 12h
        }

        /// <summary>
        ///     Parameter z specifies expression to be approximated Hermite spline.
        ///     All required derivations are exactly specified.
        /// </summary>
        /// <param name="z"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dxy"></param>
        public InterpolativeMathFunction(MathFunction z, MathFunction dx,
            MathFunction dy, MathFunction dxy)
        {
            Z = z;
            Dx = dx;
            Dy = dy;
            Dxy = dxy;
        }

        public MathFunction Z { get; }
        public MathFunction Dx { get; }
        public MathFunction Dy { get; }
        public MathFunction Dxy { get; }


        public static InterpolativeMathFunction operator +(InterpolativeMathFunction f1, InterpolativeMathFunction f2)
        {
            return new InterpolativeMathFunction(vars=>f1.Z(vars)+f2.Z(vars));
        }

        public static InterpolativeMathFunction operator -(InterpolativeMathFunction f1, InterpolativeMathFunction f2)
        {
            return new InterpolativeMathFunction(vars => f1.Z(vars) - f2.Z(vars));
        }

        public static InterpolativeMathFunction operator *(InterpolativeMathFunction f1, InterpolativeMathFunction f2)
        {
            return new InterpolativeMathFunction(vars => f1.Z(vars) * f2.Z(vars));
        }

        public static InterpolativeMathFunction operator /(InterpolativeMathFunction f1, InterpolativeMathFunction f2)
        {
            return new InterpolativeMathFunction(vars => f1.Z(vars) / f2.Z(vars));
        }

        //public MathFunction Get(Derivation derivation)
        //{
        //    switch (derivation)
        //    {

        //    }
        //}

        /// <summary>
        ///     Parse aproximation Z from string.
        ///     All required derivations are calculated automatically.
        /// </summary>
        /// <param name="mathExpression">String form of aproximated Z.</param>
        /// <param name="variableX">Substring of mathExpression used as first variable.</param>
        /// <param name="variableY">Substring of mathExpression used as second variable.</param>
        /// <returns>
        ///     Instance of class if mathExpression is correct expression Z, othervise
        ///     returns null;
        /// </returns>
        public static InterpolativeMathFunction CompileFromString(string mathExpression, string variableX, string variableY)
        {
            // TODO: osetrit vynimky pre neplatne vstupy

            var function = MathFunctions.CompileFromString(mathExpression, variableX, variableY);
            return function == null ? null : new InterpolativeMathFunction(function);
        }
    }

    public static class MathExpressionExtensions
    {
        public static InterpolativeMathFunction CompileToMathFunction(this MathExpression expression)
        {
            return InterpolativeMathFunction.CompileFromString(expression.Expression,expression.Variables[0],expression.Variables[1]);
        }
    }
}