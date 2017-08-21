using System;
namespace HDExportMetadataAndFile.DAO
{
    public class EXPORT_JOB
    {
        public int ID { get; set; }
        public int USER_ID { get; set; }
        public long ID_CLIP { get; set; }
        public int STATUS { get; set; }
        public DateTime? LAST_UPDATE { get; set; }
    }
}