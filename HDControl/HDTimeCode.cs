using DevExpress.XtraEditors;
using HDCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDControl
{
    public class HDTimeCode : TextEdit
    {
        private bool _showFrame = true;
        private bool _showSecond = true;

        public HDTimeCode()
        {
            this.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            this.Properties.Mask.BeepOnError = true;
            this.Properties.Mask.IgnoreMaskBlank = false;
            this.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            this.ShowFrame = true;

            if (this._showSecond)
            {
                if (this._showFrame)
                    this.Text = "00:00:00.00";
                else
                    this.Text = "00:00:00";
            }
            else
                this.Text = "00:00";
        }

        public bool ShowSecond
        {
            get { return _showSecond; }
            set
            {
                _showSecond = value;

                this.Properties.Mask.EditMask = @"[0-9]{2}:[0-5][0-9]" + (_showSecond ? @":[0-5][0-9]" + (_showFrame ? @"\.([01][0-9]|2[0-4])" : "") : "");

                if (this._showSecond)
                {
                    if (this._showFrame)
                    {
                        if (this.Text.Length < 11)
                            this.Text += ".00";
                    }
                    else
                    {
                        if (this.Text.Length > 8)
                            this.Text = this.Text.Substring(0, 8);
                    }
                }
                else
                {
                    if (this.Text.Length > 5)
                        this.Text = this.Text.Substring(0, 5);
                }
            }
        }

        public bool ShowFrame
        {
            get { return _showFrame; }
            set
            {
                _showFrame = value;

                this.Properties.Mask.EditMask = @"[0-9]{2}:[0-5][0-9]:[0-5][0-9]" + (_showFrame ? @"\.([01][0-9]|2[0-4])" : "");

                if (this._showSecond)
                {
                    if (this._showFrame)
                    {
                        if (this.Text.Length < 11)
                            this.Text += ".00";
                    }
                    else
                    {
                        if (this.Text.Length > 8)
                            this.Text = this.Text.Substring(0, 8);
                    }
                }
                else
                {
                    if (this.Text.Length > 5)
                        this.Text = this.Text.Substring(0, 5);
                }
            }
        }

        public void FromMillisecond(long millisecond)
        {
            TimeCode tc = new TimeCode();
            tc.FromMiliSecond(millisecond);

            if (this._showSecond)
            {
                if (_showFrame)
                    this.Text = tc.ToHDString();
                else
                    this.Text = tc.ToHDString().Substring(0, 8);
            }
            else
                this.Text = tc.ToHDString().Substring(0, 5);
        }

        public void FromSecond(long second)
        {
            FromMillisecond(second * 1000);
        }

        public void FromMinute(long minute)
        {
            FromSecond(minute * 60);
        }

        public TimeCode TimeCode
        {
            get
            {
                string tcStr = this.Text;
                if (tcStr.Length < 8)
                    tcStr += ":00";
                if (tcStr.Length < 11)
                    tcStr += ".00";

                return (TimeCode)tcStr;
            }

            set
            {
                if (this._showSecond)
                {
                    if (_showFrame)
                        this.Text = value.ToHDString();
                    else
                        this.Text = value.ToHDString().Substring(0, 8);
                }
                else
                    this.Text = value.ToHDString().Substring(0, 5);
            }
        }
    }
}
