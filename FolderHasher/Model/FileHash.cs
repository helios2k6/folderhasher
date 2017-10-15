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

using System;
using System.Linq;

namespace FolderHasher.Model
{
    /// <summary>
    /// Represents the hash code of a file
    /// </summary>
    public sealed class FileHash : IEquatable<FileHash>
    {
        #region public properties
        /// <summary>
        /// The path to the file
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// A sample of the file
        /// </summary>
        public byte[] FileSample;

        /// <summary>
        /// The size of the file
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// the SHA2-512 hash code
        /// </summary>
        public byte[] SHA2512Hash { get; set; }
        #endregion

        #region ctor
        /// <summary>
        /// Constructs a new FileHash object
        /// </summary>
        public FileHash()
        {
            FilePath = string.Empty;
            FileSample = new byte[0];
            FileSize = 0;
            SHA2512Hash = new byte[0];
        }
        #endregion

        #region public methods
        public bool Equals(FileHash other)
        {
            if (EqualsPreamble(other) == false)
            {
                return false;
            }

            return FileSize == other.FileSize &&
                string.Equals(FilePath, other.FilePath, StringComparison.Ordinal) &&
                Enumerable.SequenceEqual(FileSample, other.FileSample) &&
                Enumerable.SequenceEqual(SHA2512Hash, other.SHA2512Hash);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FileHash);
        }

        public override int GetHashCode()
        {
            return FilePath.GetHashCode() ^
                FileSize.GetHashCode() ^
                FileSample.Aggregate(0, (acc, e) => acc ^ e.GetHashCode()) ^
                SHA2512Hash.Aggregate(0, (acc, e) => acc ^ e.GetHashCode());
        }
        #endregion

        #region private methods
        private bool EqualsPreamble(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false;

            return true;
        }
        #endregion
    }
}
