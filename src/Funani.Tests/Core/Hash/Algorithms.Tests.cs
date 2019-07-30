using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Funani.Core.Hash
{
    public class AlgorithmsTest
    {
        [Fact]
        public void HashComputation_OnEmptyFiles_ShouldBeCorrect()
        {
            const string path = @"/temp/empty.txt";
            // Arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { path, new MockFileData("") }
            });
            var a = new Algorithms(fileSystem);

            Assert.Equal("da39a3ee5e6b4b0d3255bfef95601890afd80709", a.ComputeSha1(path));
            Assert.Equal("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855", a.ComputeSha256(path));
        }
    }
}
