﻿

using HermiteInterpolation.Functions;

namespace HermiteInterpolation.SplineKnots
{
    internal static class KnotsGeneratorFactory
    {
        internal static IKnotsGenerator DefaultImplementation(InterpolatedFunction function)
        {
            return new DeBoorKnotsGenerator(function);
            //return new DirectKnotsGenerator(function);
        }
    }
}
