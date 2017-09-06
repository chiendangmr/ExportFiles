using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HDCore
{
    public struct TimeCode : IComparable
    {
        public static TimeCode tcZero = new TimeCode(0, 0, 0, 0);
        public static TimeCode tcOne = new TimeCode(0, 0, 0, 1);
        public static TimeCode tcSec = new TimeCode(0, 0, 1, 0);
        public static TimeCode tcDay = new TimeCode(24, 0, 0, 0);
        public static TimeCode tcScroll = new TimeCode(0, 0, 5, 0);

        public TimeCode(TimeCode tc)
        {
            h = tc.h;
            m = tc.m;
            s = tc.s;
            f = tc.f;
        }

        public static TimeCode TimeCodeFromFrame(long frame)
        {
            TimeCode tc = new TimeCode();
            tc.FromFrame(frame);
            return tc;
        }
        public static TimeCode TimeCodeFromString(string tcStr)
        {
            TimeCode tc = new TimeCode();
            tc.FromTimecodeString(tcStr);
            return tc;
        }
        /// <summary>
        /// Convert tu frame -> TimeCode
        /// </summary>
        /// <param name="frame"></param>
        public void FromFrame(long frame)
        {
            byte myhour, myminute, mysecond, myframe;
            myhour = (byte)(frame / 90000);
            myminute = (byte)((frame - myhour * 90000) / 1500);
            mysecond = (byte)((frame - myhour * 90000 - myminute * 1500) / 25);
            myframe = (byte)((frame - myhour * 90000 - myminute * 1500 - mysecond * 25));
            h = myhour;
            m = myminute;
            s = mysecond;
            f = myframe;
        }

        /// <summary>
        /// Convert từ TimeSpan -> TimeCode
        /// </summary>
        /// <param name="Time"></param>
        public void FromTimeSpan(TimeSpan Time)
        {
            byte myhour, myminute, mysecond, myframe;
            myhour = (byte)(Time.Hours + Time.Days * 24);
            myminute = (byte)Time.Minutes;
            mysecond = (byte)Time.Seconds;
            myframe = (byte)(Time.Milliseconds / 40);
            h = myhour;
            m = myminute;
            s = mysecond;
            f = myframe;
        }

        public void FromMiliSecond(long ms)
        {
            long myframe = (int)Math.Ceiling((double)ms / 40);
            FromFrame(myframe);
        }

        public void FromTimecodeString(String tc, int fps = 25)
        {
            long ms = 0;
            String[] times = Regex.Split(tc, "[:.]");
            if (times.Count() > 0)
            {
                ms += 3600 * int.Parse(times[0]) * 1000;
            }
            if (times.Count() > 1)
            {
                ms += 60 * int.Parse(times[1]) * 1000;
            }
            if (times.Count() > 2)
            {
                ms += int.Parse(times[2]) * 1000;
            }
            if (times.Count() > 3)
            {
                ms += int.Parse(times[3]) * ((int)(1000 / fps));
            }
            this.FromMiliSecond(ms);
        }
        public TimeCode AddMillisecond(long ms)
        {
            TimeCode tcAdd = new TimeCode();
            tcAdd.FromMiliSecond(ms);
            return this + tcAdd;
        }

        /// <summary>
        /// Convert tu time code sang frame
        /// </summary>
        /// <returns></returns>
        public long ToLong()
        {
            return (f + s * 25 + m * 1500 + h * 90000);
        }

        public long ToMillisecond()
        {
            return (f + s * 25 + m * 1500 + h * 90000) * 40;
        }

        public TimeSpan ToTimeSpan()
        {
            return new TimeSpan(0, hour, minute, second, frame * 40);
        }

        public TimeCode(byte hour, byte minute, byte second, byte frame)
        {
            h = hour;
            m = minute;
            s = second;
            f = frame;
        }

        public TimeCode Celling()
        {
            if (frame == 0)
                return this;
            else
            {
                TimeCode tc = new TimeCode();
                tc.FromFrame((s + 1) * 25 + m * 1500 + h * 90000);
                return tc;
            }
        }

        public TimeCode Round()
        {
            TimeCode tc = new TimeCode(h, m, s, 0);
            return tc;
        }

        public static bool operator ==(TimeCode tc1, TimeCode tc2)
        {
            if (tc1.hour == tc2.hour && tc1.minute == tc2.minute && tc1.second == tc2.second && tc1.frame == tc2.frame)
                return (true);
            else
                return (false);
        }

        public static bool operator !=(TimeCode tc1, TimeCode tc2)
        {
            return !(tc1 == tc2);
        }

        public static TimeCode operator -(TimeCode tc2, TimeCode tc1)
        {
            TimeCode tc = new TimeCode(0, 0, 0, 0);
            long t1, t2, t;
            t1 = tc1.frame + tc1.second * 25 + tc1.minute * 1500 + tc1.hour * 90000;
            t2 = tc2.frame + tc2.second * 25 + tc2.minute * 1500 + tc2.hour * 90000;
            if (t2 >= t1)
                t = t2 - t1;
            else
                //t = 2160000-t1 + t2;
                t = 0;

            tc.hour = (byte)(t / 90000);
            tc.minute = (byte)((t - tc.hour * 90000) / 1500);
            tc.second = (byte)((t - tc.hour * 90000 - tc.minute * 1500) / 25);
            tc.frame = (byte)((t - tc.hour * 90000 - tc.minute * 1500 - tc.second * 25));
            return tc;
        }

        public static TimeCode operator +(TimeCode tc1, TimeCode tc2)
        {
            TimeCode tc = new TimeCode(0, 0, 0, 0);
            long t1, t2, t;
            t1 = tc1.frame + tc1.second * 25 + tc1.minute * 1500 + tc1.hour * 90000;
            t2 = tc2.frame + tc2.second * 25 + tc2.minute * 1500 + tc2.hour * 90000;
            t = t1 + t2;

            tc.hour = (byte)(t / 90000);
            tc.minute = (byte)((t - (long)(tc.hour * 90000)) / 1500);
            tc.second = (byte)((t - (long)(tc.hour * 90000) - (long)(tc.minute * 1500)) / 25);
            tc.frame = (byte)((t - tc.hour * 90000 - tc.minute * 1500 - tc.second * 25));

            return (tc);
            //return new TimeCode((byte)(tc1.hour+tc2.h), 0, 0, 0);
        }
        public static bool operator >=(TimeCode tc1, TimeCode tc2)
        {
            return (tc1.ToLong() >= tc2.ToLong());
        }
        public static bool operator <=(TimeCode tc1, TimeCode tc2)
        {
            return (tc1.ToLong() <= tc2.ToLong());
        }
        public static bool operator >(TimeCode tc1, TimeCode tc2)
        {
            return (tc1.ToLong() > tc2.ToLong());
        }
        public static bool operator <(TimeCode tc1, TimeCode tc2)
        {
            return (tc1.ToLong() < tc2.ToLong());
        }

        public static explicit operator TimeCode(string str)
        {
            TimeCode tc = new TimeCode(0, 0, 0, 0);
            try
            {
                string[] arr = str.Split(new char[] { ':', '.' });
                tc.hour = Convert.ToByte(arr[0]);
                tc.minute = Convert.ToByte(arr[1]);
                tc.second = Convert.ToByte(arr[2]);
                tc.frame = Convert.ToByte(arr[3]);
                return tc;
            }
            catch
            {
                return tc;
            }
        }

        public static explicit operator TimeCode(TimeSpan Time)
        {
            TimeCode tc = new TimeCode(0, 0, 0, 0);
            tc.FromTimeSpan(Time);
            return tc;
        }

        public static explicit operator TimeCode(int frame)
        {
            TimeCode tc = new TimeCode(0, 0, 0, 0);
            tc.FromFrame(frame);
            return tc;
        }

        public static explicit operator TimeCode(long milisecond)
        {
            TimeCode tc = new TimeCode(0, 0, 0, 0);
            tc.FromMiliSecond(milisecond);
            return tc;
        }

        public static bool TryParse(string str, ref TimeCode tc)
        {
            // TimeCode tc = new TimeCode(0, 0, 0, 0);
            try
            {
                string[] arr = str.Split(new char[] { ':', '.' });
                tc.hour = Convert.ToByte(arr[0]);
                tc.minute = Convert.ToByte(arr[1]);
                tc.second = Convert.ToByte(arr[2]);
                tc.frame = Convert.ToByte(arr[3]);

                return true;
            }
            catch
            {
                return false;
            }
        }
        public void Set(TimeCode tc)
        {
            this = tc;

        }
        public byte hour
        {
            get
            {
                return (h);
            }
            set
            {
                h = value;
            }
        }
        public byte minute
        {
            get
            {
                return (m);
            }
            set
            {
                m = value;
            }
        }
        public byte second
        {
            get
            {
                return (s);
            }
            set
            {
                s = value;
            }
        }
        public byte frame
        {
            get
            {
                return (f);
            }
            set
            {
                f = value;
            }
        }
        public string ToHDString()
        {
            return string.Format("{0:00}:{1:00}:{2:00}:{3:00}", h, m, s, f);

        }
        public string ToClockString()
        {
            return string.Format("{0:00}:{1:00}:{2:00}", h, m, s);
        }
        public override string ToString()
        {

            return (String.Format("{0}:{1}:{2}:{3}", h, m, s, f));
        }
        private byte h;
        private byte m;
        private byte s;
        private byte f;

        public int CompareTo(object obj)
        {
            if (obj == null || obj.GetType() != typeof(TimeCode)) return 1;

            TimeCode otherTimeCode = (TimeCode)obj;

            return this.ToLong().CompareTo(otherTimeCode.ToLong());
        }
    }
}
