namespace Earthwatchers.UI.Layers
{
    public class EducationalLayer
    {
        public string Name { get; private set; }
        public string Url { get; private set; }
        public string WikiUrl { get; private set; }

        public EducationalLayer(string name, string url, string wikiUrl)
        {
            Name = name;
            Url = url;
            WikiUrl = wikiUrl;
        }
    }
}
