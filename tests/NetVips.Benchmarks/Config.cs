namespace NetVips.Benchmarks
{
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Jobs;
    using BenchmarkDotNet.Toolchains.CsProj;

    public class Config : ManualConfig
    {
        public Config()
        {
            // Disable this policy because our benchmarks refer
            // to a non-optimized SkiaSharp that we do not own.
            Options |= ConfigOptions.DisableOptimizationsValidator;

            Add(Job.Default
#if NETCOREAPP2_1
                    .With(CsProjCoreToolchain.NetCoreApp21)
                    .WithId(".Net Core 2.1 CLI")
#elif NETCOREAPP3_0
                    .With(CsProjCoreToolchain.NetCoreApp30)
                    .WithId(".Net Core 3.0 CLI")
#endif
            );
        }
    }
}