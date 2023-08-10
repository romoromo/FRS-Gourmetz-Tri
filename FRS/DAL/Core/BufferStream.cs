using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DAL.Core
{
    public class BufferStream : Stream
    {
        private readonly Stream _originalStream;
        private readonly MemoryStream _buffer;

        public BufferStream(Stream originalStream)
        {
            _originalStream = originalStream ?? throw new ArgumentNullException(nameof(originalStream));
            _buffer = new MemoryStream();
            _originalStream.CopyTo(_buffer);
            _buffer.Position = 0;
        }

        public override bool CanRead => true;
        public override bool CanWrite => false;
        public override bool CanSeek => true;

        public override long Length => _buffer.Length;

        public override long Position
        {
            get => _buffer.Position;
            set => _buffer.Position = value;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _buffer.Seek(offset, origin);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _buffer.Read(buffer, offset, count);
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _buffer.Dispose();
                _originalStream.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
