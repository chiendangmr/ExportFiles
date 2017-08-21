namespace HDExportMetadataAndFile.DAO
{
    using System;
    
    public class SYSTEM_USERS
    {
        public int USER_ID { get; set; }
        public string USER_NAME { get; set; }
        public string PASSWORD { get; set; }
        public bool? PASSWORD_EXPIRE_STATUS { get; set; }
        public string FULL_NAME { get; set; }
        public int? PRIORITY { get; set; }
        public int STATUS { get; set; }
        public string PHONE { get; set; }
        public string MOBILE { get; set; }
        public string FAX { get; set; }
        public string EMAIL { get; set; }
        public string ADDRESS { get; set; }
        public DateTime? LAST_CHANGE_PASSWORD { get; set; }
        public DateTime? LAST_BLOCK_DATE { get; set; }
        public int? LOGIN_FAILURE_COUNT { get; set; }
        public int? EMPLOYEE_ID { get; set; }
        public int LANGUAGE_ID { get; set; }
    }
}