using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDExportMetadataAndFile.View
{   
    public class DBCommandObj
    {
        public string SQLQuery { get; set; }
        public string SQLQueryHighresFile { get; set; }
        public string SQLQueryLowresFile { get; set; }
        public string SQLQueryHighresOriginalFile { get; set; }
        public string SQLQueryAudio { get; set; }
        public string SQLQueryAudioOriginal { get; set; }
        public string SQLQueryMetadata { get; set; }
        public string SQLQueryXmlOriginal { get; set; }
        public string SQLQuerySubtitle { get; set; }
        public string SQLUpdateStatus { get; set; }
        public string SQLQueryPreview { get; set; }
        public string SQLQueryImage { get; set; }
        public string SQLQueryMail { get; set; }
    }
    public class GlobalObj
    {
        public string NasIP { get; set; }
        public int NasPort { get; set; }
        public string NasUsername { get; set; }
        public string NasPwd { get; set; }
        public string NasPath { get; set; }
        public string SaveFolder { get; set; }
        public string Symbol { get; set; }
        public bool exMediaLowres { get; set; }
        public bool exMediaHighresOriginal { get; set; }
        public bool exMediaHighres { get; set; }
        public bool exPreview { get; set; }
        public bool exImage { get; set; }
        public bool useMaBang { get; set; }
        public bool exAudio { get; set; }
        public bool exAudioOriginal { get; set; }
        public bool exXml { get; set; }        
        public bool isHead { get; set; }
        public bool useTenCT { get; set; }
        public bool useTenCTAdd { get; set; }
        public bool useCreateDate { get; set; }
        public bool useBroadcastDate { get; set; }
        public bool useSeason { get; set; }
        public bool useEpisode { get; set; }
        public string emailList { get; set; }
    }
    public class FileObj
    {        
        public int? EPISODE_NUMBER { get; set; }        
        public string FILE_NAME { get; set; }
        public string HD_CLIP { get; set; }
        public string THUMB_FILE_NAME { get; set; }
        public string TEN_CHUONG_TRINH { get; set; }
        public string MA_BANG { get; set; }               
        public string Season { get; set; }       
        public string MASTER_CLIP_NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string NAS_IP { get; set; }
        public string USERNAME { get; set; }
        public string PASSWORD { get; set; }
        public int? PORT { get; set; }
        public string DATA1_DIRECTORY { get; set; }
        public string DATA3_DIRECTORY { get; set; }
        public string UNC_BASE_PATH_DATA1 { get; set; }
        public string UNC_BASE_PATH_DATA3 { get; set; }
        public string UNC_HOME { get; set; }
        public DateTime START_RIGHTS { get; set; }
        public DateTime END_RIGHTS { get; set; }        
        public string NAS_DATA_PATH { get; set; }
    }
    public class AudioObj
    {        
        public string FILE_NAME { get; set; }        
        public string NAS_IP { get; set; }
        public string USERNAME { get; set; }
        public string PASSWORD { get; set; }
        public int? PORT { get; set; }
        public string DATA1_DIRECTORY { get; set; }
        public string DATA3_DIRECTORY { get; set; }
        public string UNC_BASE_PATH_DATA1 { get; set; }
        public string UNC_BASE_PATH_DATA3 { get; set; }
        public string NAS_DATA_PATH { get; set; }
        public string UNC_HOME { get; set; }

    }
    public class metaObj
    {
        public int NODE_WORK_ID { get; set; }
        public DateTime CHECK_DATE { get; set; }
        public int CHECKER_USER_ID { get; set; }
        public string FILE_NAME { get; set; }
        public string MA_BANG { get; set; }
        public int AFD_TYPE { get; set; }
        public int NAS_ID { get; set; }
        public int FILE_TRANS_STATUS { get; set; }
        public string VIDEO_FORMAT { get; set; }
        public string REAL_TC_OUT { get; set; }
        public string REAL_TC_IN { get; set; }
    }
    public class configObj
    {
        public string FileType { get; set; }
        public string NasPath { get; set; }
        public string NasUsername { get; set; }
        public string NasPass { get; set; }
        public string LocalPath { get; set; }
        public string AddSymBol { get; set; }
        public string FileName { get; set; }
        public string Emails { get; set; }
    }
}
