using API.Utils.Jwt.Options;
using CommandLine;

namespace API.Utils.Jwt;

class Program
{
    static int Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        return Parser.Default.ParseArguments<GeneratorOptions, ReadOptions, WriteOptions>(args)
            .MapResult(
                (GeneratorOptions options) => Generate(options),
                (ReadOptions options) => Read(options),
                (WriteOptions options) => Write(options),
                errors => 1
            );
    }

    private static int Generate(GeneratorOptions options)
    {
        return 0;
    }

    private static int Read(ReadOptions options)
    {
        return 0;
    }

    private static int Write(WriteOptions options)
    {
        return 0;
    }
}
