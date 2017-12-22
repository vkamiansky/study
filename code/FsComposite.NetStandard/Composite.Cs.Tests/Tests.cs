using System;
using System.Collections.Generic;

using Xunit;

using Composite;

namespace Composite.Cs.Tests
{
    public class Tests
    {
        [Fact]
        public void Test()
        {
            Func<bool> func = () => true;
            // var fsFunc = func.ToFSharpFunc();
            
            var scn = new List<Func<int, int[]>>() {
                y => new[] { 1, 2},
            };
            
            Func<string, string[]> step1 = x => new[] { "test" };

            var result = C.Ana(new[] { step1 }, C.Composite(new[]{ "test1" }));

            Assert.Equal(true, true);
        }
    }
}
