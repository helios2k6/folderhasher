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

using FlatBuffers;
using FolderHasher.Model.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FolderHasher.Model
{
    /// <summary>
    /// Represents an database of file hashes
    /// </summary>
    public sealed class FileHashDatabase : IEquatable<FileHashDatabase>
    {
        #region public properties
        /// <summary>
        /// The file hashes
        /// </summary>
        public FileHash[] FileHashes { get; set; }
        #endregion

        #region ctor
        /// <summary>
        /// Constructs a new FileHashDatabase
        /// </summary>
        public FileHashDatabase()
        {
            FileHashes = new FileHash[0];
        }
        #endregion

        #region public methods
        public bool Equals(FileHashDatabase other)
        {
            if (EqualsPreamble(other) == false)
            {
                return false;
            }

            return Enumerable.SequenceEqual(FileHashes, other.FileHashes);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FileHashDatabase);
        }

        public override int GetHashCode()
        {
            return FileHashes.Aggregate(0, (acc, e) => acc ^ e.GetHashCode());
        }

        public byte[] Serialize()
        {
            var builder = new FlatBufferBuilder(4);
            Offset<FileHashSerialized>[] fileHashOffsetArray = CreateFileHashOffsetArray(builder);

            VectorOffset fileHashVectorOffset = FileHashDatabaseSerialized.CreateFileHashesVector(builder, fileHashOffsetArray);
            Offset<FileHashDatabaseSerialized> databaseOffset = FileHashDatabaseSerialized.CreateFileHashDatabaseSerialized(builder, fileHashVectorOffset);
            FileHashDatabaseSerialized.FinishFileHashDatabaseSerializedBuffer(builder, databaseOffset);

            return builder.SizedByteArray();
        }

        public static FileHashDatabase Deserialize(byte[] buffer)
        {
            FileHashDatabaseSerialized serializedFileHashDatabase = FileHashDatabaseSerialized.GetRootAsFileHashDatabaseSerialized(new ByteBuffer(buffer));
            IEnumerable<FileHash> fileHashes = from i in Enumerable.Range(0, serializedFileHashDatabase.FileHashesLength)
                                               select FileHash.Convert(serializedFileHashDatabase.FileHashes(i));
            return new FileHashDatabase
            {
                FileHashes = fileHashes.ToArray(),
            };
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

        private Offset<FileHashSerialized>[] CreateFileHashOffsetArray(FlatBufferBuilder builder)
        {
            int fileHashCounter = 0;
            var fileHashOffsetArray = new Offset<FileHashSerialized>[FileHashes.Length];
            foreach (FileHash fileHash in FileHashes)
            {
                fileHashOffsetArray[fileHashCounter] = fileHash.SerializeWithBuilder(builder);
            }

            return fileHashOffsetArray;
        }
        #endregion
    }
}
