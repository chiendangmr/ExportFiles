using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HDControl
{
    public partial class VU_Meter : UserControl
    {
        private Color _BackColor;
        private int _BorderSize;
        private int _LedSpace;

        private int _Led1Count;
        private Color _Led1On;
        private Color _Led1Off;

        private int _Led2Count;
        private Color _Led2On;
        private Color _Led2Off;

        private int _Led3Count;
        private Color _Led3On;
        private Color _Led3Off;

        private Color _PeakColor;

        private int _VolumeMax;
        private int _Volume;
        private int _Peak;

        private Orientation _Orientation;

        public VU_Meter()
        {
            _BackColor = Color.Black;
            _BorderSize = 1;
            _LedSpace = 1;

            _Led1Count = 40;
            _Led1On = Color.LimeGreen;
            _Led1Off = Color.DarkGreen;

            _Led2Count = 11;
            _Led2On = Color.Yellow;
            _Led2Off = Color.Olive;

            _Led3Count = 9;
            _Led3On = Color.Red;
            _Led3Off = Color.Maroon;

            _PeakColor = Color.WhiteSmoke;

            _VolumeMax = 60;
            _Volume = 0;
            _Peak = 0;

            _Orientation = Orientation.Vertical;

            this.Paint += new System.Windows.Forms.PaintEventHandler(this.VU_Meter_Paint);
        }

        #region Properties
        [DefaultValue(typeof(Color), "Black")]
        public Color BackColor
        {
            get { return _BackColor; }
            set
            {
                _BackColor = value;
                this.Invalidate();
            }
        }

        [DefaultValue(1)]
        public int BorderSize
        {
            get { return _BorderSize; }
            set
            {
                if (value < 0)
                    _BorderSize = 0;
                else
                    _BorderSize = value;
                this.Invalidate();
            }
        }

        [DefaultValue(1)]
        public int LedSpace
        {
            get { return _LedSpace; }
            set
            {
                if (value < 0)
                    _LedSpace = 0;
                else
                    _LedSpace = value;
                this.Invalidate();
            }
        }

        [DefaultValue(10)]
        public int Led1Count
        {
            get { return _Led1Count; }
            set
            {
                if (value < 0) _Led1Count = 0;
                else
                    _Led1Count = value;
                this.Invalidate();
            }
        }

        [DefaultValue(typeof(Color), "LimeGreen")]
        public Color Led1On
        {
            get { return _Led1On; }
            set
            {
                _Led1On = value;
                this.Invalidate();
            }
        }

        [DefaultValue(typeof(Color), "DarkGreen")]
        public Color Led1Off
        {
            get { return _Led1Off; }
            set
            {
                _Led1Off = value;
                this.Invalidate();
            }
        }

        [DefaultValue(5)]
        public int Led2Count
        {
            get { return _Led2Count; }
            set
            {
                if (value < 0)
                    _Led2Count = 0;
                else
                    _Led2Count = value;
                this.Invalidate();
            }

        }

        [DefaultValue(typeof(Color), "Yellow")]
        public Color Led2On
        {
            get { return _Led2On; }
            set
            {
                _Led2On = value;
                this.Invalidate();
            }
        }

        [DefaultValue(typeof(Color), "Olive")]
        public Color Led2Off
        {
            get { return _Led2Off; }
            set
            {
                _Led2Off = value;
                this.Invalidate();
            }
        }

        [DefaultValue(5)]
        public int Led3Count
        {
            get { return _Led3Count; }
            set
            {
                if (value < 0)
                    _Led3Count = 0;
                else
                    _Led3Count = value;
                this.Invalidate();
            }
        }

        [DefaultValue(typeof(Color), "Red")]
        public Color Led3On
        {
            get { return _Led3On; }
            set
            {
                _Led3On = value;
                this.Invalidate();
            }
        }

        [DefaultValue(typeof(Color), "Maroon")]
        public Color Led3Off
        {
            get { return _Led3Off; }
            set
            {
                _Led3Off = value;
                this.Invalidate();
            }
        }

        [DefaultValue(typeof(Color), "Red")]
        public Color PeakColor
        {
            get { return _PeakColor; }
            set
            {
                _PeakColor = value;
                this.Invalidate();
            }
        }

        [DefaultValue(100)]
        public int VolumeMax
        {
            get { return _VolumeMax; }
            set
            {
                if (value < 0)
                    _VolumeMax = 0;
                else _VolumeMax = value;
                this.Invalidate();
            }
        }

        [DefaultValue(0)]
        public int Volume
        {
            get { return _Volume; }
            set
            {
                if (value < 0)
                    _Volume = 0;
                else _Volume = value;
                this.Invalidate();
            }
        }

        [DefaultValue(0)]
        public int Peak
        {
            get { return _Peak; }
            set
            {
                if (value < 0)
                    _Peak = 0;
                else _Peak = value;
                this.Invalidate();
            }
        }

        [DefaultValue(Orientation.Vertical)]
        public Orientation Orientation
        {
            get { return _Orientation; }
            set
            {
                _Orientation = value;
                this.Invalidate();
            }
        }
        #endregion

        #region Paint With Double Buffer
        private void VU_Meter_Paint(object sender, PaintEventArgs e)
        {
            // Create Image In Memory
            Bitmap drw = new Bitmap(this.Width, this.Height, e.Graphics);
            Graphics g = Graphics.FromImage(drw);
            // Draw Border
            g.Clear(_BackColor);

            if (_Orientation == System.Windows.Forms.Orientation.Vertical)
            {
                // Calc for ledsize
                int ledcount = _Led1Count + _Led2Count + _Led3Count;
                if (ledcount > 0)
                {
                    int ledwidth = this.Width - 2 * _BorderSize;
                    float ledheight = (this.Height - 2 * _BorderSize - (ledcount - 1) * _LedSpace);
                    ledheight /= ledcount;

                    if (ledwidth > 0 && ledheight > 0)
                        for (int i = 0; i < ledcount; i++)
                        {
                            int ledx = _BorderSize;
                            float ledy = _BorderSize + (ledcount - i - 1) * (ledheight + _LedSpace);
                            // Draw led
                            Brush br;
                            if (i == _Peak * ledcount / _VolumeMax - 1)
                                br = new SolidBrush(_PeakColor);
                            else if (i < _Led1Count)
                            {
                                if (i < _Volume * ledcount / _VolumeMax)
                                    br = new SolidBrush(_Led1On);
                                else
                                    br = new SolidBrush(_Led1Off);
                            }
                            else if (i < _Led1Count + _Led2Count)
                            {
                                if (i < _Volume * ledcount / _VolumeMax)
                                    br = new SolidBrush(_Led2On);
                                else
                                    br = new SolidBrush(_Led2Off);
                            }
                            else
                            {
                                if (i < _Volume * ledcount / _VolumeMax)
                                    br = new SolidBrush(_Led3On);
                                else
                                    br = new SolidBrush(_Led3Off);
                            }

                            g.FillRectangle(br, ledx, ledy, ledwidth, ledheight);
                        }
                }
            }
            else
            {
                // Calc for ledsize
                int ledcount = _Led1Count + _Led2Count + _Led3Count;
                if (ledcount > 0)
                {
                    int ledHeight = this.Height - 2 * _BorderSize;
                    float ledWidth = (this.Width - 2 * _BorderSize - (ledcount - 1) * _LedSpace);
                    ledWidth /= ledcount;

                    if (ledHeight > 0 && ledWidth > 0)
                        for (int i = 0; i < ledcount; i++)
                        {
                            int ledy = _BorderSize;
                            float ledx = _BorderSize + (i) * (ledWidth + _LedSpace);
                            // Draw led
                            Brush br;
                            if (i == _Peak * ledcount / _VolumeMax - 1)
                                br = new SolidBrush(_PeakColor);
                            else if (i < _Led1Count)
                            {
                                if (i < _Volume * ledcount / _VolumeMax)
                                    br = new SolidBrush(_Led1On);
                                else
                                    br = new SolidBrush(_Led1Off);
                            }
                            else if (i < _Led1Count + _Led2Count)
                            {
                                if (i < _Volume * ledcount / _VolumeMax)
                                    br = new SolidBrush(_Led2On);
                                else
                                    br = new SolidBrush(_Led2Off);
                            }
                            else
                            {
                                if (i < _Volume * ledcount / _VolumeMax)
                                    br = new SolidBrush(_Led3On);
                                else
                                    br = new SolidBrush(_Led3Off);
                            }

                            g.FillRectangle(br, ledx, ledy, ledWidth, ledHeight);
                        }
                }
            }

            e.Graphics.DrawImageUnscaled(drw, 0, 0);
            g.Dispose();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {

        }
        #endregion
    }
}