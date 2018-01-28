using System;
using System.Collections.Generic;

using Xunit;

using Composite;
using Composite.Cs.Interfaces;

namespace Composite.Cs.Tests
{
    public class Tests
    {
        private class Simple
        {
            public int Number { get; set; }
        }

        [Fact]
        public void Test1()
        {
            var scn = new Func<Simple, Simple[]>[] {

                x => x.Number == 1
                        ? new[] { new Simple { Number = 3, }, new Simple { Number = 4, }, }
                        : new[] { x, },

                x => x.Number == 3
                        ? new[] { new Simple { Number = 5, }, new Simple { Number = 6, }, }
                        : new[] { x, },
                };

            var obj = C.Composite(new[] {
                new Simple { Number = 1, },
                new Simple { Number = 2, },
            });

            var result = C.Ana(scn, obj);

            Assert.Equal(true, true);
        }
    }
}