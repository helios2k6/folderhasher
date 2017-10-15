/* 
 * Copyright (c) 2015 Andrew Johnson
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
 * Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN
 * AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
 * WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using FolderHasher.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class TestFileHashDatabaseSerialization
    {
        [TestMethod]
        public void SerializeFileHash()
        {
            byte[] fileSample = { 0, 1, 0 };
            byte[] sha2512Hash = { 3, 2, 1, };
            var fileHash = new FileHash
            {
                FilePath = @"test_path.mkv",
                FileSample = fileSample,
                FileSize = 1024,
                SHA2512Hash = sha2512Hash,
            };

            var database = new FileHashDatabase
            {
                FileHashes = new FileHash[] { fileHash },
            };

            byte[] serializedDatabase = database.Serialize();
            var deserializedDatabase = FileHashDatabase.Deserialize(serializedDatabase);

            Assert.AreEqual(database, deserializedDatabase);
        }
    }
}
