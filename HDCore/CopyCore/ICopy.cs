using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CopyFile
{
    public interface ICopy
    {
        long GetFileSize();
        long GetByteTotal();

        void TryGetFileSourceSize();
        void TryGetFileDestSize();

        void OpenReaderBuffer(string fileName);
        void CancelReaderBuffer();
        long DoReaderBuffer(out Buffer buff);

        void OpenWriterBuffer(string fileName);
        void CancelWriterBuffer();
        long DoWriterBuffer(Buffer buff);

        void RemoveDestFile();
    }

    public class Buffer : IDisposable
    {
        public const long BufferSize = 102400;

        public byte[] buffer { get; set; }
        public long bytes_got { get; set; }

        public Buffer()
        {
            buffer = new byte[BufferSize]; // 100K
            bytes_got = 0;
        }

        public void Dispose()
        {
            CloseBuffer();
        }

        ~Buffer()
        {
            CloseBuffer();
        }

        void CloseBuffer()
        {
            if (buffer != null)
            {
                buffer = null;
            }
        }
    }
}
