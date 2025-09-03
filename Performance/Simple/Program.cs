using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace MyBenchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }

    [SimpleJob(RuntimeMoniker.Net70)]
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net481)]
    [SimpleJob(RuntimeMoniker.Net462)]
    public class Simple
    {

        [Benchmark]
        public string GetString() => "Name";

        [Benchmark]
        public string GetStringNameOf() => nameof(Person.Name);
    }

    public class Person
    {
        public string    Name { get; set; }
    }
}
