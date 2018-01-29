using System;
using System.Collections.Generic;
using System.Linq;

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
                        return new[]{x};
                    },
                    (x) => {
                        if (x.Name.Equals ("First", StringComparison.InvariantCulture)) {
                            return new [] { new Simple { Name = "Third", }, new Simple { Name = "Fourth", }, };
                        }
                        return new[]{x};
                    },
                };

            var obj = C.Composite (new [] {
                new Simple { Name = Alice, },
                new Simple { Name = Bob, },
            });

            var result = C.Ana (scn, obj);

            Assert.True(result.IsComposite);

            var compositeResult = (DataTypes.Composite<Simple>.Composite)result;
            var resultArray = compositeResult.Item.ToArray();

            Assert.True(resultArray.Length == 2);
            Assert.True(resultArray[0].IsComposite);
            Assert.True(resultArray[1].IsValue);
        }
    }
}