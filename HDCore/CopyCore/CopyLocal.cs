using System;
using System.IO;
using System.Threading;

namespace CopyFile
{
    public class CopyLocal : ICopy
    {
        private string fileSourceName;
        private string fileDestName;

        private long fileSize;
        private long bytes_total;
        private FileStream file;

        public long FileSize
        {
            get { return fileSize; }
        }

        public long Bytes_Total
        {
            get { return bytes_total; }
        }

        public string FileSourceName
        {
            get { return fileSourceName; }
        }

        public string FileDestName
        {
            get { return fileDestName; }
        }

        public void OpenReader(string fileName)
        {
            try
            {
                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            }
            catch (Exception ex)
            {
                file = null;
                throw new Exception(ex.Message);
            }

            fileSize = file.Length;
            this.fileSourceName = fileName;
        }

        public long DoReader(out Buffer buffer)
        {
            buffer = new Buffer();
            try
            {
                buffer.bytes_got = file.Read(buffer.buffer, 0, buffer.buffer.Length);
                bytes_total += buffer.bytes_got;
                if (buffer.bytes_got <= 0)
                {
                    file.Close();
                    file = null;
                }
            }
            catch
            {
                try
                {
                    file.Close();
                    file = null;
                }
                catch { }
            }

            return buffer.bytes_got;
        }

        public void OpenWriter(string fileName)
        {
            try
            {
                file = new FileStream(fileName, FileMode.Create);
            }
            catch (Exception ex)
            {
                file = null;
                throw new Exception(ex.Message);
            }
            this.fileDestName = fileName;
        }

        public long DoWriter(Buffer buffer)
        {
            try
            {
                if (buffer.bytes_got <= 0)
                {
                    file.Flush();
                    file.Close();
                    file = null;
                    return buffer.bytes_got;
                }

                file.Write(buffer.buffer, 0, (int)buffer.bytes_got);
                bytes_total += buffer.bytes_got;
            }
            catch(Exception ex)
            {
                try
                {
                    file.Close();
                    file = null;
                }
                catch { }
                throw ex;
            }
            return buffer.bytes_got;
        }

        public void CancelReader()
        {
            try
            {
                file.Close();
                file = null;
            }
            catch { }
        }

        public void CancelWriter()
        {
            try
            {
                file.Close();
                file = null;
            }
            catch { }
        }

        // Interface
        public long GetFileSize()
        {
            return FileSize;
        }

        public long GetByteTotal()
        {
            return Bytes_Total;
        }

        public void OpenReaderBuffer(string fileName)
        {
            OpenReader(fileName);
        }

        public void CancelReaderBuffer()
        {
            CancelReader();
        }

        public long DoReaderBuffer(out Buffer buff)
        {
            return DoReader(out buff);
        }

        public void OpenWriterBuffer(string fileName)
        {
            OpenWriter(fileName);
        }

        public void CancelWriterBuffer()
        {
            CancelWriter();
        }

        public long DoWriterBuffer(Buffer buff)
        {
            return DoWriter(buff);
        }

        public void TryGetFileSourceSize()
        {
            try
            {
                fileSize = new FileInfo(fileSourceName).Length;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void TryGetFileDestSize()
        {
            try
            {
                fileSize = new FileInfo(fileDestName).Length;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void RemoveDestFile()
        {
            File.Delete(fileDestName);
        }
    }
}
