using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System;
using EarthWatchers.SL.Requests;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Earthwatchers.Models;

namespace EarthWatchers.SL.GUI.Controls.Comments
{
    public partial class CommentScreen
    {
        private readonly int landId;
        private readonly ObservableCollection<Earthwatchers.Models.Comment> comments;
        private readonly CommentRequests commentRequests;

        public CommentScreen(int landId, ObservableCollection<Earthwatchers.Models.Comment> comments)
        {
            if (comments == null) return;

            InitializeComponent();
            commentRequests = new CommentRequests(Constants.BaseApiUrl);
            this.landId = landId;
            this.comments = comments;
            KeyDown += CommentScreenKeyDown;
            LoadComments();
        }

        void CommentScreenKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
            if(e.Key == Key.Enter)
                StartPostComment();

        }

        private void BtnCloseClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        public void OpenCommentScreen()
        {
            Show();
        }

        private void LoadComments()
        {
            this.commentList.ItemsSource =  comments;
        }

        private void StartPostComment()
        {
            if (Current.Instance.Earthwatcher == null)
            {
                var warning = new WarningScreen("You are not logged in");
                warning.Show();
                return;
            }

            if (txtComment.Text.Trim().Equals(""))
                return;

            var comment = new Earthwatchers.Models.Comment
            {
                FullName = Current.Instance.Earthwatcher.FullName,
                Published = DateTime.Now,
                UserName = Current.Instance.Username,
                EarthwatcherId = Current.Instance.Earthwatcher.Id,
                LandId = landId,
                UserComment = txtComment.Text,               
            };

            commentRequests.Post(comment, Current.Instance.Username,Current.Instance.Password);

            txtComment.Text = "";

            comment.Published = DateTime.Now;
            comments.Insert(0, comment);
        }

        private void BtnPostClick(object sender, RoutedEventArgs e)
        {
            StartPostComment();
        }
        
        private void BtnRemoveMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image button = sender as Image;
            if (button != null && button.Tag != null && button.Tag is Comment)
            {
                Comment comment = button.Tag as Comment;
                commentRequests.Delete(comment.Id, Current.Instance.Username, Current.Instance.Password);
                comments.Remove(comment);
            }
        }
    }
}
