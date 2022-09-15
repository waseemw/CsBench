using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace CsBench;

[ApiController]
[Route("")]
public class MainController : ControllerBase
{
    private static int _i;
    private readonly Stopwatch _sw = new();


    [RequestFormLimits(MultipartBodyLengthLimit = 1_000_000_000)]
    [Consumes("multipart/form-data")]
    [HttpPost("accept-file")]
    public async Task AcceptFile()
    {
        var body = Request.Body;
        var reader = new MultipartReader("file", body);
        while (await reader.ReadNextSectionAsync() is { } section)
        {
            var n = section.Body.Length;
            Console.WriteLine(n);
            var bytes = new byte[1024];
            while (n > 0) n -= await section.Body.ReadAsync(bytes);
        }

        Console.WriteLine("GOT IT");
    }


    [HttpPost("stream")]
    [Consumes("application/octet-stream")]
    public async Task<string> TestStream(Stream stream)
    {
        var n = stream.Length;
        var buffer = new byte[1024 * 1024];
        while (n > 0) n -= await stream.ReadAsync(buffer);

        return "done";
    }

    // [RequestFormLimits(MultipartBodyLengthLimit = 1_000_000_000)]
    // [RequestSizeLimit(1_000_000_000)]
    // [Consumes("application/zip")]
    // [DisableRequestSizeLimit]
    [HttpPost("upload-file")]
    public async Task<string> UploadFileTest()
    {
        var n = Request.ContentLength;
        var bytes = new byte[1024 * 1024];
        while (n > 0)
        {
            var read = await Request.Body.ReadAsync(bytes);
            n -= read;
        }

        return "done";
    }

    [HttpGet("test")]
    public string Test()
    {
        _i = 0;
        _sw.Start();
        for (var i = 0; i < 1000000; i++) TestAsync();

        return "WORKED";
    }

    private async void TestAsync()
    {
        List<string> stList = new(10);
        _ = stList;
        await Task.Delay(3000);
        PrintI();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private void PrintI()
    {
        Console.WriteLine(++_i);
        if (_i == 1000000)
        {
            _sw.Stop();
            Console.WriteLine(_sw.Elapsed.TotalMilliseconds);
        }
    }

    [HttpGet("cpu")]
    public int TestCpu()
    {
        _i++;
        var ans = 15;
        for (var j = 6; j < 160000000; j++) ans += ans * (j * (5 - j) / j) * 2 * j - j / (5 + j) * j * 14;

        return ans;
    }
}