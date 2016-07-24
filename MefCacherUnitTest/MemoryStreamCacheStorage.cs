using OhNoPub.MefCacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;

namespace OhNoPub.MefCacherUnitTest
{
    class MemoryStreamCacheStorage
        : Component
        , ICacheStorage
    {
        MemoryStream Stream { get; } = new MemoryStream();
        string Version { get; set; }

        public Stream GetReadStream(string expectedVersion, out string version)
        {
            version = Version;
            return new MemoryStream(
                Stream.GetBuffer(),
                0,
                (int)Stream.Length,
                false);
        }

        public Stream GetWriteStream(string version)
        {
            Version = version;
            Stream.SetLength(0);
            // Give write access without letting the caller dispose our stream.
            return new DelegatingStream(Stream);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                Stream.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        private class DelegatingStream : Stream
        {
            MemoryStream Stream { get; }

            public override bool CanRead => Stream.CanRead;

            public override bool CanSeek => Stream.CanSeek;

            public override bool CanWrite => Stream.CanWrite;

            public override long Length => Stream.Length;

            public override long Position
            {
                get
                {
                    return Stream.Position;
                }

                set
                {
                    Stream.Position = Position;
                }
            }

            public DelegatingStream(
                MemoryStream stream)
            {
                Stream = stream;
            }

            public override void Flush() => Stream.Flush();

            public override long Seek(long offset, SeekOrigin origin) => Stream.Seek(offset, origin);

            public override void SetLength(long value) => Stream.SetLength(value);

            public override int Read(byte[] buffer, int offset, int count) => Stream.Read(buffer, offset, count);

            public override void Write(byte[] buffer, int offset, int count) => Stream.Write(buffer, offset, count);
        }
    }
}
