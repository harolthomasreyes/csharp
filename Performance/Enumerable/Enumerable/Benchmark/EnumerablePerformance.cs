
using BenchmarkDotNet.Attributes;
using Records;

namespace Enumerable.Benchmark
{
    [MemoryDiagnoser(false)]
    public class EnumerablePerformance
    {
        [Benchmark]
        public async Task ReadFileNotAsyncForeach()
        {
            var register = ReadFile_NotAsync_Foreach();
        }
        [Benchmark]
        public async Task ReadFileNotAsyncSelect()
        {
            var register = ReadFile_NotAsync_Select();
        }
        [Benchmark]
        public async Task ReadFileAsyncSelect()
        {
            var register = await ReadFile_Async_Select();
        }
       

        private IEnumerable<FileRegister> ReadFile_NotAsync_Select()
        {
            var lines = File.ReadAllLines("./Registers.csv");

            return lines.Select(x =>
            {
                var line = x.Split(',');
                return new FileRegister(line[0], int.Parse(line[1]));
            });
        }
        private async Task<IEnumerable<FileRegister>> ReadFile_Async_Select()
        {
            var lines = File.ReadAllLines("./Registers.csv");

            return lines.Select(x =>
            {
                var line = x.Split(',');
                return new FileRegister(line[0], int.Parse(line[1]));
            });
        }
        private IEnumerable<FileRegister> ReadFile_NotAsync_Foreach()
        {
            var lines = File.ReadAllLines("./Registers.csv");

            foreach(var x in lines)
            {
                var line = x.Split(',');
                yield return new FileRegister(line[0], int.Parse(line[1]));
            }
        }
      
    }
}
