
using BenchmarkDotNet.Attributes;
using Records;

namespace Enumerable.Benchmark
{
    [MemoryDiagnoser(false)]
    public class EnumerablePerformance
    {
        private string[] registers { get; set; }
      

        [GlobalSetup]
        public void Setup()
        {
            registers = File.ReadAllLines("./Registers.csv");
        }

        [Benchmark]
        public async Task ReadFileAsyncForeach()
        {
            var register = await ReadFile_Async_Foreach();
        }
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
            
            return registers.Select(x =>
            {
                var line = x.Split(',');
                return new FileRegister(line[0], int.Parse(line[1]));
            });
        }
        private async Task<IEnumerable<FileRegister>> ReadFile_Async_Select()
        {
            
            return registers.Select(x =>
            {
                var line = x.Split(',');
                return new FileRegister(line[0], int.Parse(line[1]));
            });
        }
        private IEnumerable<FileRegister> ReadFile_NotAsync_Foreach()
        {
           
            foreach(var x in registers)
            {
                var line = x.Split(',');
                yield return new FileRegister(line[0], int.Parse(line[1]));
            }
        }
        private async Task<IEnumerable<FileRegister>> ReadFile_Async_Foreach()
        {
            var ret = new List<FileRegister>();
            foreach (var x in registers)
            {
                var line = x.Split(',');
                ret.Add( new FileRegister(line[0], int.Parse(line[1])));
            }
            return ret;
        }
    }
}
