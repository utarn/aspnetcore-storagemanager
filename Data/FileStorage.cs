using System;

namespace aspnetcore_storagemanager.Data
{
    public class FileStorage
    {
        public FileStorage()
        {

        }
        public FileStorage(string fileId, string fileName, string fileType, long size, DateTime uploadDate)
        {
            this.FileId = fileId;
            this.FileName = fileName;
            this.FileType = fileType;
            this.Size = size;
            this.UploadDate = uploadDate;

        }
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public long Size { get; set; }
        public DateTime UploadDate { get; set; }
    }
}