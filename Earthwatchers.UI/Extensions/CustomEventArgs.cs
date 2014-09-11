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

namespace Earthwatchers.UI
{
    public class SharedEventArgs : EventArgs
    {
        public string Action { get; set; }
        public int Points { get; set; }
    }

    public class CollectionCompleteEventArgs : EventArgs
    {
        public int CollectionId { get; set; }
        public int Points { get; set; }
    }

    public class ChangingOpacityEventArgs : EventArgs
    {
        public string Title { get; set; }
        public bool IsCloudy { get; set; }
        public bool IsInitial { get; set; }
    }
}
