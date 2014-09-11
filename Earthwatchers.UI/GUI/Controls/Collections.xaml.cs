using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Earthwatchers.Models;
using System.Collections.Generic;
using Earthwatchers.UI.Requests;
using System;
using Earthwatchers.UI.Layers;
using System.Linq;
using System.Windows.Threading;

namespace Earthwatchers.UI.GUI.Controls
{
    public partial class Collections
    {
        private readonly CollectionRequests collectionRequests;
        private readonly CollectionItem _item;
        private DispatcherTimer myTimer;
        private System.Resources.ResourceManager rm;

        public delegate void CollectionCompleteEventHandler(object sender, CollectionCompleteEventArgs e);
        public event CollectionCompleteEventHandler CollectionCompleted;

        public Collections()
        {
            InitializeComponent();
        }
        public Collections(CollectionItem item)
        {
            InitializeComponent();
            _item = item;
            collectionRequests = new CollectionRequests(Constants.BaseApiUrl);
            collectionRequests.ItemsReceived += collectionRequests_ItemsReceived;
            this.FoundedStoryBoard.Completed += FoundedStoryBoard_Completed;
            this.HighlightFoundedStoryBoard.Completed += HighlightFoundedStoryBoard_Completed;

            rm = new System.Resources.ResourceManager(typeof(Earthwatchers.UI.Resources.Labels));
            this.PrizeText.Text = rm.GetString(_item.Name).ToUpper();
            this.CollectionNameText.Text = rm.GetString(_item.CollectionName);
            this.Image3.Source = Earthwatchers.UI.Resources.ResourceHelper.GetBitmap(string.Format("/Resources/Images/Collections/{0}", _item.Icon));
            ToolTipService.SetToolTip(this.Image3Border, rm.GetString(_item.Name));
        }

        void HighlightFoundedStoryBoard_Completed(object sender, EventArgs e)
        {
            //TODO: animación y puntaje por si completó la colección
            if (hasCompletedCollection)
            {
                this.CompleteCollectionGrid.Visibility = System.Windows.Visibility.Visible;
                this.CloseButton.Visibility = System.Windows.Visibility.Visible;
                this.CollectionCompleteStoryBoard.Begin();

                CollectionCompleted(this, new CollectionCompleteEventArgs { CollectionId = _item.CollectionId, Points = ActionPoints.Points(ActionPoints.Action.CollectionComplete) });
            }
        }

        void FoundedStoryBoard_Completed(object sender, EventArgs e)
        {
            //suspenseSound.Stop();
            foundSound.Play();
            this.Polygon3.Fill = new SolidColorBrush(Color.FromArgb(255, 237, 237, 237));
            this.HighlightFoundedStoryBoard.Begin();
        }

        private List<CollectionItem> _items;
        void collectionRequests_ItemsReceived(object sender, EventArgs e)
        {
            _items = sender as List<CollectionItem>;
            ShowFoundedItem();

        }

        public void loadCollections()
        {
            this.Loaded += Collections_Loaded;
        }

        bool hasCompletedCollection = false;
        private void ShowFoundedItem()
        {
            if (secondsSinceStart > 5 && _items != null) 
            {
                myTimer.Stop();

                int count = 0;

                if (_items.Where(x => x.CollectionId == _item.CollectionId && x.HasItem).Count() == _items.Where(x => x.CollectionId == _item.CollectionId).Count())
                {
                    hasCompletedCollection = true;
                }

                foreach (var item in _items.Where(x => x.CollectionId == _item.CollectionId && x.Id != _item.Id))
                {
                    count++;
                    if (count == 3)
                    {
                        count++;
                    }
                    Image icon = this.FindName("Image" + count) as Image;
                    Border border = this.FindName(string.Format("Image{0}Border", count)) as Border;
                    if (icon != null && border != null)
                    {
                        icon.Source = Earthwatchers.UI.Resources.ResourceHelper.GetBitmap(string.Format("/Resources/Images/Collections/{0}", item.Icon));
                        ToolTipService.SetToolTip(border, rm.GetString(item.Name));

                        if (!item.HasItem)
                        {
                            icon.Opacity = 0.3;
                        }
                    }
                }

                //this.StartStoryBoard.Stop();
                this.FoundedStoryBoard.Begin();
            }
        }

        void Collections_Loaded(object sender, RoutedEventArgs e)
        {
            //suspenseSound.Play();
            this.BackgroundImage.ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri(string.Format("/Images/{0}", _item.BackgroundImage), UriKind.RelativeOrAbsolute));

            myTimer = new DispatcherTimer();
            myTimer.Interval = TimeSpan.FromMilliseconds(1000); // 1 second
            myTimer.Tick += myTimer_Tick;
            myTimer.Start();

            this.StartStoryBoard.Begin();

            collectionRequests.GetCollectionItemsByEarthwatcher(Current.Instance.Earthwatcher.Id);
        }

        int secondsSinceStart = 0;
        void myTimer_Tick(object sender, EventArgs e)
        {
            secondsSinceStart++;
            if (secondsSinceStart > 5)
            {
                ShowFoundedItem();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void sound_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void BackgroundImage_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            CollectionCompleted += collections_CollectionCompleted;
            ContinueButton.Visibility = Visibility.Collapsed;
            CollectionsTutorial.Visibility = Visibility.Collapsed;

            this.BackgroundImage.ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri(string.Format("/Images/{0}", _item.BackgroundImage), UriKind.RelativeOrAbsolute));

            myTimer = new DispatcherTimer();
            myTimer.Interval = TimeSpan.FromMilliseconds(1000); // 1 second
            myTimer.Tick += myTimer_Tick;
            myTimer.Start();

            this.StartStoryBoard.Begin();

            collectionRequests.GetCollectionItemsByEarthwatcher(Current.Instance.Earthwatcher.Id);

        }

        private void collections_CollectionCompleted(object sender, CollectionCompleteEventArgs e)
        {
            CollectionCompleted(this, e);
        }
    }
}

