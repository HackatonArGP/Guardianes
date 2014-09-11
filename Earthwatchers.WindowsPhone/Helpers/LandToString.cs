using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Earthwatchers.WindowsPhone.Helpers
{
    public partial class Land
    {
        public override string ToString()
        {
            string items = "EarthwatcherGuid={0}|GeohexKey={1}|Id={2}|LandThreat={3}|LandType={4}";
            return string.Format(items, EarthwatcherGuid, GeohexKey, Id, LandThreat, LandType);
        }

        public void FromString(string landAsString)
        {
            string[] parts = landAsString.Split('|');
            if (parts.Length == 5)
            {
                this.EarthwatcherGuid = parts[0].Replace("EarthwatcherGuid=","");
                this.GeohexKey = parts[1].Replace("GeohexKey=","");
                this.Id = Convert.ToInt32(parts[2].Replace("Id=",""));
                this.LandThreat = Convert.ToInt32(parts[3].Replace("LandThreat=",""));
                this.LandType = Convert.ToInt32(parts[4].Replace("LandType=",""));
            }
        }
    }
}
