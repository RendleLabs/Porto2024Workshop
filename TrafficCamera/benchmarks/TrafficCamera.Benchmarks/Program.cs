﻿using BenchmarkDotNet.Running;
using TrafficCamera.Benchmarks;

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);