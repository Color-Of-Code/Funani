using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Funani.FileStorage
{
    public class FileDatabaseTest
    {
        [Fact]
        public void FileDatabase_EmptyBaseDir_StartCreatesDataDirectory()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"/db/dummy", new MockFileData(string.Empty) }
            });
            var fileStorage = new FileDatabase("/db", fileSystem);
            fileStorage.StartService();
            Assert.True(fileSystem.Directory.Exists("/db/data"));
        }

        [Fact]
        public void FileDatabase_EmptyData_ImportEmptyFile()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"/db/data/dummy", new MockFileData(string.Empty) },
                { @"/home/empty", new MockFileData(string.Empty) }
            });
            var fileStorage = new FileDatabase("/db", fileSystem);
            fileStorage.StartService();
            fileStorage.StoreFile(fileSystem.FileInfo.New("/home/empty"));
            Assert.True(fileSystem.File.Exists("/db/data/da/39/da39a3ee5e6b4b0d3255bfef95601890afd80709"));
        }

        [Fact]
        public void FileDatabase_GetFileInfo()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"/db/data/dummy", new MockFileData(string.Empty) }
            });
            var fileStorage = new FileDatabase("/db", fileSystem);
            fileStorage.StartService();
            var path = fileStorage.GetFileInfo("da39a3ee5e6b4b0d3255bfef95601890afd80709");
            var expected = "/db/data/da/39/da39a3ee5e6b4b0d3255bfef95601890afd80709";
            Assert.Equal(expected, path.FullName);
        }

        [Fact]
        public void FileDatabase_FileExists_ReturnsFalse()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"/db/data/dummy", new MockFileData(string.Empty) }
            });
            var fileStorage = new FileDatabase("/db", fileSystem);
            fileStorage.StartService();
            var exists = fileStorage.FileExists("da39a3ee5e6b4b0d3255bfef95601890afd80709");
            Assert.False(exists);
        }

        [Fact]
        public void FileDatabase_FileExists_ReturnsTrue()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { "/db/data/da/39/da39a3ee5e6b4b0d3255bfef95601890afd80709", new MockFileData(string.Empty) }
            });
            var fileStorage = new FileDatabase("/db", fileSystem);
            fileStorage.StartService();
            var exists = fileStorage.FileExists("da39a3ee5e6b4b0d3255bfef95601890afd80709");
            Assert.True(exists);
        }
    }
}
