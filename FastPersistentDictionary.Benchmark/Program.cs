using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using FastPersistentDictionary;
using Microsoft.Isam.Esent.Collections.Generic;
using System.Data.SqlTypes;

namespace FastPersistentDictionary.Benchmark
{
    public class BenchmarkPersistence
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Benchmarks>();
        }


    }
}
