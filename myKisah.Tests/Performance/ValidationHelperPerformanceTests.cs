using System.Diagnostics;
using Xunit;
using myKisah.Utils;

namespace myKisah.Tests.Perfromance;

public class ValidationHelperPerformanceTests
{
    private readonly ValidationHelper _validator = new ValidationHelper();

    [Fact]
    public void ValidateNotNull_1000Calls_Under5ms()
    {
        var sw = Stopwatch.StartNew();

        for (int i = 0; i < 1000; i++)
        {
            _validator.ValidateNotNull("test", "param");
        }

        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < 5,
            $"Too slow: {sw.ElapsedMilliseconds}ms (target < 5ms)");
    }
}