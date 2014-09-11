using System.Collections.Generic;
using EarthWatchers.SL.Requests;
using Earthwatchers.Models;

namespace EarthWatchers.SL.GUI.Controls
{
    public partial class NewsOverview
    {
        private NewsRequests newsRequest;
        private List<News> newsItems;

        public NewsOverview()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            newsRequest = new NewsRequests(Constants.BaseApiUrl);
            newsRequest.NewsReceived += NewsRequestNewsReceived;
            newsRequest.Getnews();
        }

        void NewsRequestNewsReceived(object sender, System.EventArgs e)
        {
            newsItems = sender as List<News>;
            if (newsItems == null) return;

            foreach (var news in newsItems)
            {
                panel.Children.Add(new NewsControl(news));
            }
        }
    }
}
