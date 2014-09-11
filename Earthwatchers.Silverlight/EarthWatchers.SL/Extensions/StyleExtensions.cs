using Mapsui.Styles;

namespace EarthWatchers.SL.Extensions
{
    public static class StyleExtensions
    {
        public static void SetOpacity(this VectorStyle vectorStyle, double opacity)
        {
            var inColor = vectorStyle.Fill.Color;
            vectorStyle.Fill.Color = new Color()
                {
                    A = (int) (255*opacity),
                    R = inColor.R,
                    G = inColor.G,
                    B = inColor.B
                };
        }
    }
}
