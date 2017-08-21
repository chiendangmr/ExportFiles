using System;
using HDCore;
using System.Linq;
using System.Text;
using System.Xml;

namespace HDExportMetadataAndFile.View
{
    public class XMLChildObject
    {
        public long fileID { get; set; }
        public string FileName { get; set; }
        public string MaBang { get; set; }
        public TimeCode Duration { get; set; }
        public string Resolution { get; set; }
        public int AFD { get; set; }
        public string AudioChannels { get; set; }
        public int CensorMan { get; set; }
        public DateTime CensorTime { get; set; }
        public int TransferStatus { get; set; }
        public int FileOnNas { get; set; }        
    }   
}
