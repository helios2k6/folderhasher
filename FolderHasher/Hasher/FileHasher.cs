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
using System;
using System.IO;
using System.Security.Cryptography;

namespace FolderHasher.Hasher
{
    /// <summary>
    /// The utility class that hashes files
    /// </summary>
    public static class FileHasher
    {
        private static readonly int StandardSkipAmount = 4096;
        private static readonly int StandardFileSampleSize = 12288;

        #region public methods
        /// <summary>
        /// Hashes a file specified at the <paramref name="filePath"/>
        /// </summary>
        /// <param name="filePath">The file path to hash</param>
        /// <returns>A newly constructed file hash</returns>
        public static FileHash HashFile(string filePath)
        {
            if (File.Exists(filePath) == false)
            {
                throw new ArgumentException(string.Format("File path {0} does not exist", filePath));
            }

            return new FileHash
            {
                FileSample = GetFileSample(filePath),
                FileSize = (ulong)new FileInfo(filePath).Length,
                FilePath = filePath,
                SHA2512Hash = GetSHA512Hash(filePath),
            };
        }
        #endregion

        #region private methods
        private static byte[] GetSHA512Hash(string filePath)
        {
            using (var shaHasher = new SHA512Cng())
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return shaHasher.ComputeHash(fileStream);
            }
        }

        /// <summary>
        /// This gets a sample of the file by simply reading the first 12 kibibytes of a file after skipping
        /// 4 kibibytes -- that's just to prevent reading common header information about the file.
        /// 
        /// If the file is less than or equal to 16 kibibytes, then we simply just read as much as we can without
        /// skipping any of the file.
        /// </summary>
        /// <param name="filePath">The file path to return a sample of</param>
        /// <returns>A byte buffer with a sample of the file</returns>
        private static byte[] GetFileSample(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[StandardFileSampleSize];
                if (fileInfo.Length <= StandardFileSampleSize + StandardSkipAmount)
                {
                    // We know for sure that the length is less than 4 kibibytes, so we're OK with truncating
                    // this read
                    int numReadBytes = fileStream.Read(buffer, 0, (int)fileInfo.Length);

                    // We don't want to send back the entire array since we read less than 12 kibibytes
                    return Truncate(buffer, numReadBytes);
                }
                else
                {
                    // Skip the first 4 kibibytes and then read as much as you can into the 12 kibibyte buffer
                    fileStream.Seek(StandardSkipAmount, SeekOrigin.Begin);

                    int numReadBytes = fileStream.Read(buffer, 0, StandardFileSampleSize);

                    // This probably is a NOOP but we do it just in case of some sort of odd issue with reading occurs
                    return Truncate(buffer, numReadBytes);
                }
            }
        }

        private static byte[] Truncate(byte[] srcBuffer, int truncateToSize)
        {
            if (srcBuffer.Length == truncateToSize || srcBuffer.Length < truncateToSize)
            {
                return srcBuffer;
            }

            var dstBuffer = new byte[truncateToSize];
            Buffer.BlockCopy(srcBuffer, 0, dstBuffer, 0, truncateToSize);
            return dstBuffer;
        }
        #endregion
    }
}
