using System;

using Xunit;

using Composite;

namespace Composite.Cs.Tests {

    public class Tests {

        private class Simple {
            public string Name { get; set; }
        }

        [Fact]
        public void Test1 () {
            const string Alice = "Alice";
            const string Bob = "Bob";

            var scn = new Func<Simple, Simple[]>[] {
                    (x) => {
                        if (x.Name.Equals (Alice, StringComparison.InvariantCulture)) {
                            return new [] { new Simple { Name = "First", }, new Simple { Name = "Second", }, };
                        }
                        return new Simple[0];
                    },
                    (x) => {
                        if (x.Name.Equals (Bob, StringComparison.InvariantCulture)) {
                            return new [] { new Simple { Name = "Third", }, new Simple { Name = "Fourth", }, };
                        }
                        return new Simple[0];
                    },
                };

            var obj = C.Composite (new [] {
                new Simple { Name = Alice, },
                new Simple { Name = Bob, },
            });

            var result = C.Ana (scn, obj);
        }
    }
}