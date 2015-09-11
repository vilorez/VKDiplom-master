﻿using System;
using System.Collections.Generic;
using HermiteInterpolation.MathFunctions;
using HermiteInterpolation.Primitives;
using HermiteInterpolation.Shapes.SplineInterpolation;
using HermiteInterpolation.SplineKnots;
using Microsoft.Xna.Framework;

namespace HermiteInterpolation.Shapes
{
    public sealed class MathFunctionSurface : CompositeSurface
    {

        public MathFunctionSurface(SurfaceDimension uDimension, SurfaceDimension vDimension,
           string functionExpression, string variableX, string variableY)
            : this(uDimension,vDimension, MathFunctions.MathFunctions.FromString(functionExpression,variableX,variableY))
        {
            if (Name == null)
                Name = functionExpression;
        }


        public MathFunctionSurface(SurfaceDimension uDimension, SurfaceDimension vDimension,
            MathFunction function)

        {
            var uCount_min_1 = uDimension.KnotCount-1;//surface.UKnotsCount-1;
            var vCount_min_1 = vDimension.KnotCount-1;//surface.VKnotsCount-1;

            var segments = new List<ISurface>(uCount_min_1 * vCount_min_1);

            for (int i = 0; i < uCount_min_1; i++)
            {
                for (int j = 0; j < vCount_min_1; j++)
                {
                    var segment = CreateSegment(i, j, uDimension, vDimension, function);
                    segments.Add(segment);
                }
            }
            Segments = segments;
            //return segments;
        }

        private ISurface CreateSegment(int uIdx, int vIdx, SurfaceDimension uDimension, SurfaceDimension vDimension, MathFunction function)
        {
            //var afv = Knots;
            
            var meshDensity = MeshDensity;
            var uSize = Math.Abs(uDimension.Max - uDimension.Min) /(uDimension.KnotCount-1);
            var vSize = Math.Abs(vDimension.Max - vDimension.Min) /(vDimension.KnotCount-1);

            var u0 = uDimension.Min + uSize*uIdx;//afv[uIdx][vIdx].X;
            var u1 = uDimension.Min + uSize * (uIdx+1);
            var v0 = vDimension.Min + vSize * vIdx;
            var v1 = vDimension.Min + vSize * (vIdx+1);

            var uKnotsDistance = Math.Abs(u1 - u0);
            var xCount = Math.Ceiling(uKnotsDistance / meshDensity);
            //var xMeshDensity = (float)(uKnotsDistance / xCount);
            var yKnotDistance = Math.Abs(v1 - v0);
            var yCount = Math.Ceiling(yKnotDistance / meshDensity);
            //var yMeshDensity = (float)(yKnotDistance / yCount);
            var verticesCount = (int)((++xCount) * (++yCount));
            var segmentMeshVertices = new VertexPositionNormalColor[verticesCount];
            var k = 0;
            var x = (float)u0;
            for (var i = 0; i < xCount; i++, x += meshDensity)
            {
                var y = (float)v0;
                for (var j = 0; j < yCount; j++, y += meshDensity)
                {
                    var z = (float)function(x, y);
                    segmentMeshVertices[k++] = new VertexPositionNormalColor(new Vector3(x, y, z), DefaultNormal,
                        DefaultColor);
                }
            }
            return new SimpleSurface(segmentMeshVertices, (int)xCount, (int)yCount);
        }

        //private IEnumerable<ISurface> CreateMesh(MathFunction mathFunction)
        //{

        //}  

        //protected InterpolativeMathFunction InterpolativeFunction { get; }

        public float MeshDensity { get; set; } = 0.1f;

        //public MathFunctionSurface(SurfaceDimension uDimension, SurfaceDimension vDimension, string functionExpression, string variableX, string variableY, Derivation derivation = Derivation.Zero) 
        //    : this(uDimension, vDimension, InterpolativeMathFunction.FromString(functionExpression, variableX, variableY), derivation)
        //{
        //   // _mathFunction = InterpolativeMathFunction.FromString(functionExpression, variableX, variableY);
        //}

        //public MathFunctionSurface(SurfaceDimension uDimension, SurfaceDimension vDimension, InterpolativeMathFunction function, Derivation derivation = Derivation.Zero) 
        //    : base(uDimension, vDimension, function, derivation)
        //{

        //}

        //protected override ISurface CreateSegment(int uIdx, int vIdx)
        //{
        //   var afv = Knots;
        //    var u0 = afv[uIdx][vIdx].X;
        //    var u1 = afv[uIdx+1][vIdx].X;
        //    var v0 = afv[uIdx][vIdx].Y;
        //    var v1 = afv[uIdx][vIdx+1].Y;
        //    var meshDensity = MeshDensity;
        //    var uKnotsDistance = Math.Abs(u1-u0);          
        //    var xCount = Math.Ceiling(uKnotsDistance/meshDensity);
        //    //var xMeshDensity = (float)(uKnotsDistance / xCount);
        //    var yKnotDistance =  Math.Abs(v1 - v0);
        //    var yCount = Math.Ceiling(yKnotDistance / meshDensity);
        //    //var yMeshDensity = (float)(yKnotDistance / yCount);
        //    var verticesCount = (int)((++xCount) * (++yCount));
        //    var segmentMeshVertices = new VertexPositionNormalColor[verticesCount];      
        //    var k = 0;
        //    var x = (float) u0 ;
        //    for (var i = 0; i < xCount; i++,x += meshDensity)
        //    {               
        //        var y = (float) v0;
        //        for (var j = 0; j < yCount; j++,y += meshDensity)
        //        {
        //            var z = (float)InterpolativeFunction.Z(x,y);
        //            segmentMeshVertices[k++] = new VertexPositionNormalColor(new Vector3(x, y, z), DefaultNormal,
        //                DefaultColor);     
        //        }              
        //    }
        //    return new SimpleSurface(segmentMeshVertices, (int)xCount, (int)yCount);         
        //}

    }
}
