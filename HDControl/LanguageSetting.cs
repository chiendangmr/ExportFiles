using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDControl
{
    public enum Language
    {
        VietNamese = 0,
        English = 1
    }

    public static class LanguageSetting
    {
        public static Language Lang = Language.VietNamese;
    }
}
