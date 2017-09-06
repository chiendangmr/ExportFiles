namespace HDControl
{
    using DevExpress.LookAndFeel;
    using DevExpress.Utils;
    using DevExpress.Utils.Drawing;
    using DevExpress.XtraEditors;
    using DevExpress.XtraEditors.Controls;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows.Forms;

    public static class HDMessageBox
    {
        const string DefaultCaption = "";
        const IWin32Window DefaultOwner = null;
        const MessageBoxButtons DefaultButtons = MessageBoxButtons.OK;
        const MessageBoxIcon DefaultIcon = MessageBoxIcon.None;
        const MessageBoxDefaultButton DefaultDefButton = MessageBoxDefaultButton.Button1;
        public static DialogResult Show(string text) { return Show(DefaultOwner, text, DefaultCaption, DefaultButtons, DefaultIcon, DefaultDefButton); }
        public static DialogResult Show(IWin32Window owner, string text) { return Show(owner, text, DefaultCaption, DefaultButtons, DefaultIcon, DefaultDefButton); }
        public static DialogResult Show(string text, string caption) { return Show(DefaultOwner, text, caption, DefaultButtons, DefaultIcon, DefaultDefButton); }
        public static DialogResult Show(IWin32Window owner, string text, string caption) { return Show(owner, text, caption, DefaultButtons, DefaultIcon, DefaultDefButton); }
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons) { return Show(DefaultOwner, text, caption, buttons, DefaultIcon, DefaultDefButton); }
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons) { return Show(owner, text, caption, buttons, DefaultIcon, DefaultDefButton); }
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) { return Show(DefaultOwner, text, caption, buttons, icon, DefaultDefButton); }
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) { return Show(owner, text, caption, buttons, icon, DefaultDefButton); }
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton) { return Show(DefaultOwner, text, caption, buttons, icon, defaultButton); }
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            return Show(owner, text, caption, MessageBoxButtonsToDialogResults(buttons), MessageBoxIconToIcon(icon), MessageBoxDefaultButtonToInt(defaultButton), icon);
        }
        public static DialogResult Show(UserLookAndFeel lookAndFeel, string text) { return Show(lookAndFeel, DefaultOwner, text, DefaultCaption, DefaultButtons, DefaultIcon, DefaultDefButton); }
        public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, string text) { return Show(lookAndFeel, owner, text, DefaultCaption, DefaultButtons, DefaultIcon, DefaultDefButton); }
        public static DialogResult Show(UserLookAndFeel lookAndFeel, string text, string caption) { return Show(lookAndFeel, DefaultOwner, text, caption, DefaultButtons, DefaultIcon, DefaultDefButton); }
        public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, string text, string caption) { return Show(lookAndFeel, owner, text, caption, DefaultButtons, DefaultIcon, DefaultDefButton); }
        public static DialogResult Show(UserLookAndFeel lookAndFeel, string text, string caption, MessageBoxButtons buttons) { return Show(lookAndFeel, DefaultOwner, text, caption, buttons, DefaultIcon, DefaultDefButton); }
        public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, string text, string caption, MessageBoxButtons buttons) { return Show(lookAndFeel, owner, text, caption, buttons, DefaultIcon, DefaultDefButton); }
        public static DialogResult Show(UserLookAndFeel lookAndFeel, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) { return Show(lookAndFeel, DefaultOwner, text, caption, buttons, icon, DefaultDefButton); }
        public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) { return Show(lookAndFeel, owner, text, caption, buttons, icon, DefaultDefButton); }
        public static DialogResult Show(UserLookAndFeel lookAndFeel, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton) { return Show(lookAndFeel, DefaultOwner, text, caption, buttons, icon, defaultButton); }
        public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            return Show(lookAndFeel, owner, text, caption, MessageBoxButtonsToDialogResults(buttons), MessageBoxIconToIcon(icon), MessageBoxDefaultButtonToInt(defaultButton), icon);
        }
        static DialogResult[] MessageBoxButtonsToDialogResults(MessageBoxButtons buttons)
        {
            if (!Enum.IsDefined(typeof(MessageBoxButtons), buttons))
            {
                throw new InvalidEnumArgumentException("buttons", (int)buttons, typeof(DialogResult));
            }
            switch (buttons)
            {
                case MessageBoxButtons.OK:
                    return new DialogResult[] { DialogResult.OK };
                case MessageBoxButtons.OKCancel:
                    return new DialogResult[] { DialogResult.OK, DialogResult.Cancel };
                case MessageBoxButtons.AbortRetryIgnore:
                    return new DialogResult[] { DialogResult.Abort, DialogResult.Retry, DialogResult.Ignore };
                case MessageBoxButtons.RetryCancel:
                    return new DialogResult[] { DialogResult.Retry, DialogResult.Cancel };
                case MessageBoxButtons.YesNo:
                    return new DialogResult[] { DialogResult.Yes, DialogResult.No };
                case MessageBoxButtons.YesNoCancel:
                    return new DialogResult[] { DialogResult.Yes, DialogResult.No, DialogResult.Cancel };
                default:
                    throw new ArgumentException("buttons");
            }
        }
        static Icon MessageBoxIconToIcon(MessageBoxIcon icon)
        {
            if (!Enum.IsDefined(typeof(MessageBoxIcon), icon))
            {
                throw new InvalidEnumArgumentException("icon", (int)icon, typeof(DialogResult));
            }
            switch (icon)
            {
                case MessageBoxIcon.None:
                    return null;
                case MessageBoxIcon.Error:
                    return SystemIcons.Error;
                case MessageBoxIcon.Exclamation:
                    return SystemIcons.Exclamation;
                case MessageBoxIcon.Information:
                    return SystemIcons.Information;
                case MessageBoxIcon.Question:
                    return SystemIcons.Question;
                default:
                    throw new ArgumentException("icon");
            }
        }
        static int MessageBoxDefaultButtonToInt(MessageBoxDefaultButton defButton)
        {
            if (!Enum.IsDefined(typeof(MessageBoxDefaultButton), defButton))
            {
                throw new InvalidEnumArgumentException("defaultButton", (int)defButton, typeof(DialogResult));
            }
            switch (defButton)
            {
                case MessageBoxDefaultButton.Button1:
                    return 0;
                case MessageBoxDefaultButton.Button2:
                    return 1;
                case MessageBoxDefaultButton.Button3:
                    return 2;
                default:
                    throw new ArgumentException("defaultButton");
            }
        }
        public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, string text, string caption, DialogResult[] buttons, Icon icon, int defaultButton, MessageBoxIcon messageBeepSound)
        {
            MessageBeep((int)messageBeepSound);
            HDMessageBoxForm form = new HDMessageBoxForm();
            return form.ShowMessageBoxDialog(new HDMessageBoxArgs(lookAndFeel, owner, text, caption, buttons, icon, defaultButton));
        }
        public static DialogResult Show(IWin32Window owner, string text, string caption, DialogResult[] buttons, Icon icon, int defaultButton, MessageBoxIcon messageBeepSound)
        {
            return Show(null, owner, text, caption, buttons, icon, defaultButton, messageBeepSound);
        }
        #region API
        [DllImport("user32.dll")]
        static extern bool MessageBeep(int uType);
        #endregion
        static bool _AllowCustomLookAndFeel = false;
        public static bool AllowCustomLookAndFeel
        {
            get { return _AllowCustomLookAndFeel; }
            set { _AllowCustomLookAndFeel = value; }
        }
    }
    public class HDMessageBoxArgs
    {
        IWin32Window owner;
        string text, caption;
        DialogResult[] buttons;
        Icon icon;
        int defaultButtonIndex;
        UserLookAndFeel lookAndFeel;
        public HDMessageBoxArgs() : this(null, string.Empty, string.Empty, new DialogResult[0], null, 0) { }
        public HDMessageBoxArgs(IWin32Window owner, string text, string caption, DialogResult[] buttons, Icon icon, int defaultButtonIndex) :
            this(null, owner, text, caption, buttons, icon, defaultButtonIndex) { }
        public HDMessageBoxArgs(UserLookAndFeel lookAndFeel, IWin32Window owner, string text, string caption, DialogResult[] buttons, Icon icon, int defaultButtonIndex)
        {
            this.lookAndFeel = lookAndFeel;
            this.owner = owner;
            this.text = text;
            this.caption = caption;
            this.buttons = buttons;
            this.icon = icon;
            this.defaultButtonIndex = defaultButtonIndex;
        }
        public UserLookAndFeel LookAndFeel
        {
            get { return lookAndFeel; }
            set { lookAndFeel = value; }
        }
        public UserLookAndFeel GetLookAndFeel()
        {
            if (lookAndFeel != null) return lookAndFeel;
            XtraForm form = Owner as XtraForm;
            if (form != null) return form.LookAndFeel;
            return null;
        }
        public Icon Icon { get { return icon; } set { icon = value; } }
        public string Caption { get { return caption; } set { caption = value; } }
        public string Text { get { return text; } set { text = value; } }
        public int DefaultButtonIndex { get { return defaultButtonIndex; } set { defaultButtonIndex = value; } }
        public IWin32Window Owner { get { return owner; } set { owner = value; } }
        public DialogResult[] Buttons { get { return buttons; } set { buttons = value; } }
    }
    public class HDMessageBoxForm : HDForm
    {
        const int Spacing = 15;
        HDMessageBoxArgs message;
        SimpleButton[] buttons;
        Rectangle iconRectangle;
        Rectangle messageRectangle;
        public HDMessageBoxForm()
        {
            this.KeyPreview = true;
            this.message = new HDMessageBoxArgs();
        }
        protected internal HDMessageBoxArgs Message { get { return message; } set { message = value; } }
        global::DevExpress.Accessibility.BaseAccessible dxAccessible;
        protected internal virtual global::DevExpress.Accessibility.BaseAccessible DXAccessible
        {
            get
            {
                if (dxAccessible == null) dxAccessible = new global::DevExpress.Accessibility.BaseAccessible();
                return dxAccessible;
            }
        }
        protected override bool AllowSkinForEmptyText
        {
            get { return true; }
        }
        protected virtual string GetButtonText(DialogResult target)
        {
            switch (target)
            {
                case DialogResult.Abort:
                    return LanguageSetting.Lang == Language.VietNamese ? "Abort" : "Abort";
                case DialogResult.Cancel:
                    return LanguageSetting.Lang == Language.VietNamese ? "Cancel" : "Cancel";
                case DialogResult.Ignore:
                    return LanguageSetting.Lang == Language.VietNamese ? "Ignore" : "Ignore";
                case DialogResult.No:
                    return LanguageSetting.Lang == Language.VietNamese ? "No" : "No";
                case DialogResult.OK:
                    return LanguageSetting.Lang == Language.VietNamese ? "Ok" : "OK";
                case DialogResult.Retry:
                    return LanguageSetting.Lang == Language.VietNamese ? "Retry" : "Retry";
                case DialogResult.Yes:
                    return LanguageSetting.Lang == Language.VietNamese ? "Yes" : "Yes";
                default:
                    return '&' + target.ToString();
            }
        }
        public DialogResult ShowMessageBoxDialog(HDMessageBoxArgs message)
        {
            this.message = message;
            return ShowMessageBoxDialog();
        }
        void CalcIconBounds()
        {
            if (Message.Icon != null)
                this.iconRectangle = new Rectangle(Spacing, Spacing, Message.Icon.Width, Message.Icon.Height);
            else
                this.iconRectangle = new Rectangle(Spacing, Spacing, 0, 0);
        }
        Size GetBordersSizes()
        {
            if (IsCustomPainter)
            {
                var margins = FormPainter.Margins;
                return new Size(margins.Left + margins.Right, margins.Top + margins.Bottom);
            }
            else
            {
                return new Size(2 * SystemInformation.FixedFrameBorderSize.Width, 2 * SystemInformation.FixedFrameBorderSize.Height + SystemInformation.CaptionHeight);
            }
        }
        void CalcMessageBounds()
        {
            int messageTop, messageLeft, messageWidth, messageHeight;
            messageTop = Spacing;
            messageLeft = (Message.Icon == null) ? Spacing : (this.iconRectangle.Left + this.iconRectangle.Width + Spacing);
            int maxFormWidth = this.MaximumSize.Width;
            if (maxFormWidth <= 0)
                maxFormWidth = SystemInformation.WorkingArea.Width;
            int maxTextWidth = maxFormWidth - GetBordersSizes().Width - Spacing - messageLeft;
            if (maxTextWidth < 10)
                maxTextWidth = 10;
            GraphicsInfo ginfo = new GraphicsInfo();
            ginfo.AddGraphics(null);
            SizeF textSize = GetPaintAppearance().CalcTextSize(ginfo.Graphics, Message.Text, maxTextWidth);
            ginfo.ReleaseGraphics();
            messageWidth = (int)Math.Ceiling(textSize.Width);
            int maxFormHeight = this.MaximumSize.Height;
            if (maxFormHeight <= 0)
                maxFormHeight = SystemInformation.WorkingArea.Height;
            int maxTextHeight = maxFormHeight - Spacing - buttons[0].Height - Spacing - messageTop - GetBordersSizes().Height;
            if (maxTextHeight < 10)
                maxTextHeight = 10;
            messageHeight = (int)Math.Ceiling(textSize.Height);
            if (messageHeight > maxTextHeight)
                messageHeight = maxTextHeight;
            this.messageRectangle = new Rectangle(messageLeft, messageTop, messageWidth, messageHeight);
        }
        void CreateButtons()
        {
            if (Message.Buttons == null || Message.Buttons.Length <= 0)
                throw new ArgumentException("At least one button must be specified", "buttons");
            buttons = new SimpleButton[Message.Buttons.Length];
            int maxButtonHeight = 0;
            for (int i = 0; i < buttons.Length; ++i)
            {
                int currentButtonIndex = (Message.DefaultButtonIndex + i) % buttons.Length;
                SimpleButton currentButton = new SimpleButton();
                currentButton.LookAndFeel.Assign(LookAndFeel);
                buttons[currentButtonIndex] = currentButton;
                currentButton.DialogResult = Message.Buttons[currentButtonIndex];
                if (currentButton.DialogResult == DialogResult.None)
                    throw new ArgumentException("The 'DialogResult.None' button cannot be specified", "buttons");
                if (currentButton.DialogResult == DialogResult.Cancel)
                    this.CancelButton = currentButton;
                currentButton.Text = GetButtonText(currentButton.DialogResult);
                Size best = currentButton.CalcBestSize();
                if (best.Width > currentButton.Width)
                    currentButton.Width = best.Width;
                if (best.Height > maxButtonHeight && best.Height > currentButton.Height)
                    maxButtonHeight = best.Height;
                this.Controls.Add(currentButton);
            }
            if (maxButtonHeight > 0)
            {
                foreach (SimpleButton currentButton in buttons)
                {
                    currentButton.Height = maxButtonHeight;
                }
            }
            if (buttons.Length == 1)
                this.CancelButton = buttons[0];
            //if (this.CancelButton == null)
            //    this.ControlBox = false;
        }
        Font GetCaptionFont()
        {
            if (IsCustomPainter)
            {
                return (Font)new AppearanceObject(FormPainter.GetDefaultAppearance()).Font.Clone();
            }
            else
            {
                return ControlUtils.GetCaptionFont();
            }
        }
        int GetCloseButtonWidth()
        {
            if (IsCustomPainter)
            {
                return new global::DevExpress.Skins.XtraForm.FormCaptionButtonSkinPainter(FormPainter).CalcObjectMinBounds(new global::DevExpress.Skins.XtraForm.FormCaptionButton(global::DevExpress.Skins.XtraForm.FormCaptionButtonKind.Close)).Width;
            }
            else
            {
                return SystemInformation.CaptionButtonSize.Width;
            }
        }
        void CalcFinalSizes()
        {
            int buttonsTotalWidth = 0;
            foreach (SimpleButton button in buttons)
            {
                if (buttonsTotalWidth != 0)
                    buttonsTotalWidth += Spacing;
                buttonsTotalWidth += button.Width;
            }
            int buttonsTop = this.messageRectangle.Bottom + Spacing;
            if (Message.Icon != null && (this.iconRectangle.Bottom + Spacing > buttonsTop))
                buttonsTop = this.iconRectangle.Bottom + Spacing;
            int wantedFormWidth = this.MinimumSize.Width;
            if (wantedFormWidth == 0)
                wantedFormWidth = SystemInformation.MinimumWindowSize.Width;
            if (wantedFormWidth < this.messageRectangle.Right + Spacing)
                wantedFormWidth = this.messageRectangle.Right + Spacing;
            if (wantedFormWidth < buttonsTotalWidth + Spacing + Spacing)
                wantedFormWidth = buttonsTotalWidth + Spacing + Spacing;
            GraphicsInfo ginfo = new GraphicsInfo();
            ginfo.AddGraphics(null);
            try
            {
                using (StringFormat fmt = TextOptions.DefaultStringFormat.Clone() as StringFormat)
                {
                    fmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                    using (Font captionFont = GetCaptionFont())
                    {
                        int captionTextWidth = (int)Math.Ceiling(ginfo.Cache.CalcTextSize(this.Text, captionFont, fmt, 0).Width);
                        int captionTextWidthWithMargins = captionTextWidth + GetBordersSizes().Width;
                        int captionTextWidthWithButtonsAndSpacing = captionTextWidthWithMargins;
                        if (this.ControlBox)
                            captionTextWidthWithButtonsAndSpacing += GetCloseButtonWidth();
                        captionTextWidthWithButtonsAndSpacing += 2;
                        int maxCaptionForcedWidth = SystemInformation.WorkingArea.Width / 3 * 2;
                        int captionWidth = Math.Min(maxCaptionForcedWidth, captionTextWidthWithButtonsAndSpacing);
                        if (wantedFormWidth < captionWidth)
                            wantedFormWidth = captionWidth;
                    }
                }
            }
            finally
            {
                ginfo.ReleaseGraphics();
            }
            this.Width = wantedFormWidth + GetBordersSizes().Width;
            this.Height = buttonsTop + buttons[0].Height + Spacing + GetBordersSizes().Height;
            int nextButtonPos = (wantedFormWidth - buttonsTotalWidth) / 2;
            for (int i = 0; i < buttons.Length; ++i)
            {
                buttons[i].Location = new Point(nextButtonPos, buttonsTop);
                nextButtonPos += buttons[i].Width + Spacing;
            }
            if (Message.Icon == null)
            {
                this.messageRectangle.Offset((wantedFormWidth - (this.messageRectangle.Right + Spacing)) / 2, 0);
            }
            if (Message.Icon != null && this.messageRectangle.Height < this.iconRectangle.Height)
            {
                this.messageRectangle.Offset(0, (this.iconRectangle.Height - this.messageRectangle.Height) / 2);
            }
        }
        public Rectangle MessageRectangle { get { return messageRectangle; } }
        public Rectangle IconRectangle { get { return iconRectangle; } }
        protected virtual bool AllowCustomLookAndFeel { get { return HDMessageBox.AllowCustomLookAndFeel; } }
        DialogResult ShowMessageBoxDialog()
        {
            if (Message.GetLookAndFeel() != null)
                LookAndFeel.Assign(Message.GetLookAndFeel());
            if (!AllowCustomLookAndFeel)
            {
                if (LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin)
                {
                    ActiveLookAndFeelStyle active = UserLookAndFeel.Default.ActiveStyle;
                    if (active == ActiveLookAndFeelStyle.Office2003)
                    {
                        LookAndFeel.SetStyle(LookAndFeelStyle.Office2003, true, false, "");
                    }
                    else
                        LookAndFeel.SetDefaultStyle();
                }
            }
            this.Text = Message.Caption;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MinimizeBox = false;
            MaximizeBox = false;
            IWin32Window owner = Message.Owner;
            if (owner == null)
            {
                Form activeForm = Form.ActiveForm;
                if (activeForm != null && !activeForm.InvokeRequired)
                {
                    owner = activeForm;
                }
            }
            if (owner != null)
            {
                Control ownerControl = owner as Control;
                if (ownerControl != null)
                {
                    if (!ownerControl.Visible)
                    {
                        owner = null;
                    }
                    else
                    {
                        Form ownerForm = ownerControl.FindForm();
                        if (ownerForm != null)
                        {
                            if ((!ownerForm.Visible)
                                || ownerForm.WindowState == FormWindowState.Minimized
                                || ownerForm.Right <= 0
                                || ownerForm.Bottom <= 0)
                            {
                                owner = null;
                            }
                        }
                    }
                }
            }
            if (owner == null)
            {
                ShowInTaskbar = true;
                StartPosition = FormStartPosition.CenterScreen;
            }
            else
            {
                ShowInTaskbar = false;
                StartPosition = FormStartPosition.CenterParent;
            }
            CreateButtons();
            CalcIconBounds();
            CalcMessageBounds();
            CalcFinalSizes();
            Form frm = owner as Form;
            if (frm != null && frm.TopMost) this.TopMost = true;
            return DoShowDialog(owner);
            return this.ShowDialog(owner);
        }
        protected virtual DialogResult DoShowDialog(IWin32Window owner)
        {
            DialogResult result = ShowDialog(owner);
            this.Dispose();
            if (Array.IndexOf(Message.Buttons, result) != -1)
                return result;
            else
                return Message.Buttons[0];
        }
        AppearanceObject GetPaintAppearance()
        {
            AppearanceObject paintAppearance = new AppearanceObject(Appearance, DefaultAppearance);
            paintAppearance.TextOptions.WordWrap = WordWrap.Wrap;
            paintAppearance.TextOptions.VAlignment = VertAlignment.Top;
            paintAppearance.TextOptions.Trimming = Trimming.EllipsisCharacter;
            return paintAppearance;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (GraphicsCache gcache = new GraphicsCache(e))
            {
                if (Message.Icon != null)
                    gcache.Graphics.DrawIcon(Message.Icon, this.iconRectangle);
                GetPaintAppearance().DrawString(gcache, Message.Text, this.messageRectangle);
            }
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            if (this.CancelButton == null && this.DialogResult == DialogResult.Cancel)
                e.Cancel = true;
            base.OnClosing(e);
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == '\x03')
            {
                e.Handled = true;
                Clipboard.SetDataObject(message.Caption + Environment.NewLine + Environment.NewLine + message.Text, true);
            }
        }
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (this.Visible && !this.ContainsFocus)
                this.Activate();
        }
    }
}
