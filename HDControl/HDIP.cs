using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDControl
{
    public class HDIP : TextEdit
    {
        public HDIP()
        {
            this.Properties.NullText = this.Properties.NullValuePrompt = "Địa chỉ IP";
            this.Properties.NullValuePromptShowForEmptyValue = true;

            this.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            this.Properties.Mask.BeepOnError = true;
            this.Properties.Mask.EditMask = @"(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)";
        }
    }
}
