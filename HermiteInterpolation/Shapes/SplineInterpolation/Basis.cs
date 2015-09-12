﻿using HermiteInterpolation.SplineKnots;
using MathNet.Numerics.LinearAlgebra;

namespace HermiteInterpolation.Shapes.SplineInterpolation
{
    internal abstract class Basis
    {
        protected Basis(Knot[][] knots, Derivation derivation)
        {
            Knots = knots;
            Derivation = derivation;
        }

        internal Knot[][] Knots { get; private set; }
        internal Derivation Derivation { get; }
        //internal delegate Vector<double> BasisVector(double t, double t0, double t1);

        internal Vector<double> Vector(double t, double t0, double t1)
        {
            switch (Derivation)
            {
                case Derivation.First:
                    return FirstDerivationVector(t, t0, t1);

                case Derivation.Second:
                    return SecondDerivationVector(t, t0, t1);

                default:
                    return FunctionVector(t, t0, t1);
            }
        }

        private static void RoundIfInVicinity(ref float value, float target, float vicinitySize)
        {
            if (value <= target + vicinitySize && target - vicinitySize <= value) value = target;
        }

        protected abstract Vector<double> FunctionVector(double t, double t0, double t1);
        protected abstract Vector<double> FirstDerivationVector(double t, double t0, double t1);
        protected abstract Vector<double> SecondDerivationVector(double t, double t0, double t1);
        internal abstract Matrix<double> Matrix(int uIdx, int vIdx);
    }
}