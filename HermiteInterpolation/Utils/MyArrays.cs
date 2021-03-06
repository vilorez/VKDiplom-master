﻿using System;
using System.Text;

namespace HermiteInterpolation.Utils
{
    public static class MyArrays
    {
        public static T[][] JaggedArray<T>(int n, int m)
        {
            var array = new T[n][];
            for (var i = 0; i < n; i++)
            {
                array[i] = new T[m];
            }
            return array;
        }

        public static T[] InitalizedArray<T>(int n, T value)
        {
            var array = new T[n];
            for (var i = 0; i < n; i++)
            {
                array[i] = value;
            }
            return array;
        }

        public static string[] AsStrings<T>(T[][] array)
        {
            var strings = new string[array.Length];
                //JaggedArray<string>(array.Length, array[0].Length);

            for (var i = 0; i < array.Length; i++)
            {
                var sb = new StringBuilder("[\t");
                for (var j = 0; j < array[0].Length; j++)
                {
                    sb.Append($"{i} {j}| ").Append(array[i][j]).Append("\t");
                }
                strings[i] = sb.ToString();
            }
            return strings;
        }

        public static void WriteArray<T>(T[][] array)
        {
            var strings = AsStrings(array);
            for (var i = 0; i < strings.Length; i++)
            {
                //System.Diagnostics.Debug.WriteLine(strings[i]);
                Console.WriteLine(strings[i]);
            }
        }

        public static T[] ArraysOperation<T>(T[] leftOp, T[] rightOp,
            Func<T, T, T> operation, int fromIdx = 0)
        {
            var result = new T[leftOp.Length];

            for (var k = 0; k < fromIdx; k++)
            {
                result[k] = leftOp[k];
            }

            int i = fromIdx, j = 0;
            for (; i < leftOp.Length && j < rightOp.Length; i++,j++)
            {
                result[i] = operation(leftOp[i], rightOp[j]);
            }
            for (; i < leftOp.Length; i++)
            {
                result[i] = leftOp[i];
            }
            return result;
        }

        public static T[][] ArraysOperation<T>(T[][] leftOp, T[][] rightOp,
            Func<T, T, T> operation, int fromIdx = 0, int fromJdx = 0)
        {
            var result = new T[leftOp.Length][];

            for (var k = 0; k < fromIdx; k++)
            {
                result[k] = leftOp[k];
            }

            int i = fromIdx, j = fromJdx;
            for (; i < leftOp.Length && j < rightOp.Length; i++, j++)
            {
                result[i] = ArraysOperation(leftOp[i], rightOp[j], operation,
                    fromJdx);
            }
            for (; i < leftOp.Length; i++)
            {
                result[i] = leftOp[i];
            }
            return result;
        }
    }
}