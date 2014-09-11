using System;
using System.Collections.Generic;
using System.Text;

namespace Earthwatchers.Models
{
    public enum ImageType
    {
        // old: Radar=0, RemoteSensing=1, Picture=2, TrueColor=3, Sar=4
        EVI = 0, Infrared = 1, TrueColor = 2, Aerial = 3, SAR = 4,
    }
}