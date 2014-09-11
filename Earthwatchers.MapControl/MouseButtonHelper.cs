using System;
using System.Windows;
using System.Windows.Input;

namespace Mapsui.Windows
{
    internal static class MouseButtonHelper
    {
        private const long KDoubleClickSpeed = 500;
        private const double KMaxMoveDistance = 10;

        private static long _lastClickTicks = 0;
        private static Point _lastPosition;
        private static WeakReference _lastSender;

        internal static bool IsDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(null);
            var clickTicks = DateTime.Now.Ticks;
            var elapsedTicks = clickTicks - _lastClickTicks;
            var elapsedTime = elapsedTicks / TimeSpan.TicksPerMillisecond;
            var quickClick = (elapsedTime <= KDoubleClickSpeed);
            var senderMatch = (_lastSender != null && sender.Equals(_lastSender.Target));

            if (senderMatch && quickClick && position.Distance(_lastPosition) <= KMaxMoveDistance)
            {
                // Double click!
                _lastClickTicks = 0;
                _lastSender = null;
                return true;
            }

            // Not a double click
            _lastClickTicks = clickTicks;
            _lastPosition = position;
            if (!quickClick)
                _lastSender = new WeakReference(sender);

            return false;
        }

        private static double Distance(this Point pointA, Point pointB)
        {
            var x = pointA.X - pointB.X;
            var y = pointA.Y - pointB.Y;
            return Math.Sqrt(x * x + y * y);
        }
    }
}
