using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace OhNoPub.MefCacher
{
    public class FileCacheStorage
        : ICacheStorage
    {
        byte[] Magic => new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true).GetBytes(GetType().FullName);

        string Filename { get; }
        public FileCacheStorage(
            string filename)
        {
            Filename = filename;
        }

        public Stream GetReadStream(string expectedVersion, out string version)
        {
            Stream stream;
            try
            {
                stream = File.OpenRead(Filename);
            }
            catch (FileNotFoundException)
            {
                version = null;
                return null;
            }
            try
            {
                // Read the magic.
                var magic = Magic;
                var fileMagic = new byte[magic.Length];
                if (magic.Length != stream.Read(fileMagic, 0, fileMagic.Length)
                    || !fileMagic.SequenceEqual(magic))
                {
                    version = null;
                    return null;
                }

                // Read ushort to learn length of stored version.
                var versionLengthBytes = BitConverter.GetBytes((short)0);
                if (versionLengthBytes.Length != stream.Read(versionLengthBytes, 0, versionLengthBytes.Length))
                {
                    version = null;
                    return null;
                }
                var versionLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(versionLengthBytes, 0));

                // Read version.
                var versionBytes = new byte[versionLength];
                if (versionBytes.Length != stream.Read(versionBytes, 0, versionBytes.Length))
                {
                    version = null;
                    return null;
                }
                version = Encoding.UTF8.GetString(versionBytes);

                var callerStream = stream;
                stream = null;
                return callerStream;
            }
            finally
            {
                stream?.Dispose();
            }
        }

        public Stream GetWriteStream(string version)
        {
            var stream = File.Open(Filename, FileMode.Create);
            try
            {
                // Write magic.
                var magic = Magic;
                stream.Write(magic, 0, magic.Length);

                // Write length of encoded version.
                var versionBytes = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true).GetBytes(version);
                var versionLengthBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)versionBytes.Length));
                stream.Write(versionLengthBytes, 0, versionLengthBytes.Length);

                // Write version.
                stream.Write(versionBytes, 0, versionBytes.Length);

                var callerStream = stream;
                stream = null;
                return callerStream;
            }
            finally
            {
                stream?.Dispose();
            }
        }
    }
}
