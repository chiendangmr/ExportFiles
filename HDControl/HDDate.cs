using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDControl
{
    public class HDDate : DateEdit
    {
        public HDDate()
        {
            this.Properties.DisplayFormat.FormatType = this.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.Properties.DisplayFormat.FormatString = this.Properties.EditFormat.FormatString = this.Properties.EditMask = "dd-MM-yyyy";
        }
    }
}
