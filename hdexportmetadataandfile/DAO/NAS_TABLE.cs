namespace HDExportMetadataAndFile.DAO
{
    using System;
    
    public class NAS_TABLE
    {
        public int NAS_ID { get; set; }
        public string NAS_IP { get; set; }
        public long? NAS_SIZE { get; set; }
        public long? NAS_REMAIN { get; set; }
        public string NAS_DISCRIPTION { get; set; }
        public string USERNAME { get; set; }
        public string PASSWORD { get; set; }
        public int? PORT { get; set; }
        public int? IS_ALIVE { get; set; }
        public string DATA1_DIRECTORY { get; set; }
        public string DATA2_DIRECTORY { get; set; }
        public string DATA3_DIRECTORY { get; set; }
        public string DATA4_DIRECTORY { get; set; }
        public string UNC_BASE_PATH_DATA1 { get; set; }
        public string UNC_BASE_PATH_DATA2 { get; set; }
        public string UNC_BASE_PATH_DATA3 { get; set; }
        public string UNC_BASE_PATH_DATA4 { get; set; }
        public string CONFIG_IP_ADDRESS { get; set; }
        public int? CONFIG_PORT { get; set; }
        public string CONFIG_USER_NAME { get; set; }
        public string CONFIG_PASSWORD { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public int? NUMBER_OF_DISK { get; set; }
        public DateTime? ASSEMBLE_DATE { get; set; }
        public int? CURRENT_CONNECTED_SESSION { get; set; }
        public int? DEVICE_TYPE { get; set; }
    }
}