using BenchmarkDotNet.Running;
using OneBRC.Benchmarks;

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
