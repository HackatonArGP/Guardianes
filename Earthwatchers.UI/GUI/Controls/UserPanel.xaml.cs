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
using Earthwatchers.UI.Resources;
using System.Text.RegularExpressions;

namespace Earthwatchers.UI.GUI.Controls
{
    public partial class UserPanel
    {
        public List<Language> Idiomas { get; set; }
        private readonly CollectionRequests collectionRequests;

        public UserPanel()
        {
            InitializeComponent();

            collectionRequests = new CollectionRequests(Constants.BaseApiUrl);
            collectionRequests.ItemsReceived += collectionRequests_ItemsReceived;

            this.Loaded += UserPanel_Loaded;

            this.DataContext = Current.Instance.Earthwatcher;
        }

        void UserPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (Current.Instance.Earthwatcher.IsPowerUser)
            {
                this.badgeIcon.Source = Earthwatchers.UI.Resources.ResourceHelper.GetBitmap("/Resources/Images/badgej.png");
                ToolTipService.SetToolTip(this.badgeIcon, "Jaguar");
            }

            txtName.Text = Current.Instance.Earthwatcher.FullName;
            if (this.txtName.Text.Length > 16)
            {
                txtName.Text = Current.Instance.Earthwatcher.FullName.Substring(0, 15) + "...";
                ToolTipService.SetToolTip(this.txtName, Current.Instance.Earthwatcher.FullName);
            }

            this.JaguarLock.Visibility = Current.Instance.Features.IsUnlocked(EwFeature.JaguarGame) ? Visibility.Collapsed : Visibility.Visible;
            this.CollectionsLock.Visibility = Current.Instance.Features.IsUnlocked(EwFeature.Collections) ? Visibility.Collapsed : Visibility.Visible;

            this.Idiomas = new List<Language>();
            this.Idiomas.Add(new Language { Name = "English", LocalizedName = Labels.UserPanel10 });
            this.Idiomas.Add(new Language { Name = "Spanish", LocalizedName = Labels.UserPanel9 });
            this.LanguagesCombo.ItemsSource = this.Idiomas;

            collectionRequests.GetCollectionItemsByEarthwatcher(Current.Instance.Earthwatcher.Id);

            //Concursos
            if (Current.Instance.Scores.Any(x => x.Action.StartsWith(ActionPoints.Action.ContestWon.ToString())))
            {
                this.ContestWinnerBadge.Visibility = Visibility.Visible;
            }

            //Jaguar
            int numberOfJaguarsFounded = Current.Instance.Scores.Count(x => x.Action == ActionPoints.Action.FoundTheJaguar.ToString());
            if (numberOfJaguarsFounded != 0)
            {
                this.JaguarTitle.Text = string.Format(Labels.UserPanel20, numberOfJaguarsFounded);
                this.jaguarbadge.Visibility = Visibility.Visible;
            }
            else
                this.JaguarTitle.Text = Labels.UserPanel21;

            int row = 0;
            int column = 0;
            for (int i = 0; i < numberOfJaguarsFounded; i++)
            {
                Image image = new Image();
                image.Source = Earthwatchers.UI.Resources.ResourceHelper.GetBitmap("/Resources/Images/jaguar.png");
                image.Width = 40;
                Grid.SetColumn(image, column);
                Grid.SetRow(image, row);
                this.JaguarsImagesGrid.Children.Add(image);

                column++;
                if (column >= 8)
                {
                    row++;
                    column = 0;
                }
            }
        }

        void collectionRequests_ItemsReceived(object sender, EventArgs e)
        {
            //Genero la Grid con los items de la colección
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager(typeof(Earthwatchers.UI.Resources.Labels));

            //hide loading animation
            this.loading.Visibility = Visibility.Collapsed;

            List<CollectionItem> items = sender as List<CollectionItem>;
            if (items != null)
            {
                string colName = string.Empty;
                int rows = 0;
                int column = 0;
                foreach (var item in items)
                {
                    if (item.CollectionName != colName)
                    {
                        rows++;
                        column = 0;
                        TextBlock colNameTb = new TextBlock { Text = rm.GetString(item.CollectionName), FontSize = 12, Foreground = new SolidColorBrush(Color.FromArgb(255, 25, 25, 25)), TextWrapping = TextWrapping.Wrap, VerticalAlignment = System.Windows.VerticalAlignment.Center };
                        Grid.SetRow(colNameTb, rows);
                        this.CollectionsGrid.Children.Add(colNameTb);

                        if (rows < 9)
                        {
                            Border border = new Border { Height = 1, BorderThickness = new Thickness(0), Background = new SolidColorBrush(Color.FromArgb(255, 159, 186, 14)) };
                            rows++;
                            Grid.SetRow(border, rows);
                            Grid.SetColumnSpan(border, 6);
                            this.CollectionsGrid.Children.Add(border);
                        }
                    }
                    column++;

                    //Agrego Imagen
                    Image image = new Image { Stretch = System.Windows.Media.Stretch.None };
                    ToolTipService.SetToolTip(image, rm.GetString(item.Name));
                    Grid.SetRow(image, rows < 9 ? rows - 1 : rows);
                    Grid.SetColumn(image, column);
                    image.Source = ResourceHelper.GetBitmap(string.Format("/Resources/Images/Collections/{0}", item.Icon));
                    if (!item.HasItem)
                    {
                        image.Opacity = 0.2;
                    }
                    this.CollectionsGrid.Children.Add(image);

                    colName = item.CollectionName;
                }
            }
        }

        private string step = "ChangePassword";
        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            if (border != null && border.Tag != null)
            {
                step = border.Tag.ToString();
                switch (step)
                {
                    case "ChangePassword":
                        SelectedTabArrow.Y = 20;
                        this.ChangePasswordGrid.Visibility = System.Windows.Visibility.Visible;
                        this.ChangeLanguageGrid.Visibility = System.Windows.Visibility.Collapsed;
                        this.CollectionsGrid.Visibility = System.Windows.Visibility.Collapsed;
                        this.JaguarGrid.Visibility = System.Windows.Visibility.Collapsed;
                        this.OkButton.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case "ChangeRegional":
                        SelectedTabArrow.Y = 80;
                        this.ChangePasswordGrid.Visibility = System.Windows.Visibility.Collapsed;
                        this.ChangeLanguageGrid.Visibility = System.Windows.Visibility.Visible;
                        this.CollectionsGrid.Visibility = System.Windows.Visibility.Collapsed;
                        this.JaguarGrid.Visibility = System.Windows.Visibility.Collapsed;
                        this.OkButton.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case "Collections":
                        if (Current.Instance.Features.IsUnlocked(EwFeature.Collections))
                        {
                            SelectedTabArrow.Y = 205;
                            this.ChangePasswordGrid.Visibility = System.Windows.Visibility.Collapsed;
                            this.ChangeLanguageGrid.Visibility = System.Windows.Visibility.Collapsed;
                            this.CollectionsGrid.Visibility = System.Windows.Visibility.Visible;
                            this.JaguarGrid.Visibility = System.Windows.Visibility.Collapsed;
                            this.OkButton.Visibility = System.Windows.Visibility.Collapsed;
                            break;
                        }
                        else
                            break;
                    case "Jaguar":
                        if (Current.Instance.Features.IsUnlocked(EwFeature.JaguarGame))
                        {
                            SelectedTabArrow.Y = 245;
                            this.ChangePasswordGrid.Visibility = System.Windows.Visibility.Collapsed;
                            this.ChangeLanguageGrid.Visibility = System.Windows.Visibility.Collapsed;
                            this.CollectionsGrid.Visibility = System.Windows.Visibility.Collapsed;
                            this.JaguarGrid.Visibility = System.Windows.Visibility.Visible;
                            this.OkButton.Visibility = System.Windows.Visibility.Collapsed;
                            break;
                        }
                        else
                            break;

                }
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.OkButton.IsEnabled = false;
            this.ErrorMessage.Foreground = new SolidColorBrush(Colors.Red);
            this.ErrorMessage.Text = string.Empty;

            if (step == "ChangePassword")
            {
                //Valido que los datos ingresados estén ok
                if (!this.CurrentPassword.Password.Equals(Current.Instance.Password))
                {
                    this.ErrorMessage.Text += Labels.UserPanel22;
                }

                if (this.CurrentPassword.Password.Equals(this.NewPassword.Password))
                {
                    if (this.ErrorMessage.Text != string.Empty)
                    {
                        this.ErrorMessage.Text += "\n";
                    }
                    this.ErrorMessage.Text += Labels.UserPanel23;
                }

                if (!Regex.IsMatch(this.NewPassword.Password, @"^.*(?=.{6,})(?=.*[a-zA-Z])(?=.*\d).*$"))
                {
                    if (this.ErrorMessage.Text != string.Empty)
                    {
                        this.ErrorMessage.Text += "\n";
                    }
                    this.ErrorMessage.Text += Labels.UserPanel24;
                }

                if (!this.NewPassword.Password.Equals(this.NewPassword2.Password))
                {
                    if (this.ErrorMessage.Text != string.Empty)
                    {
                        this.ErrorMessage.Text += "\n";
                    }
                    this.ErrorMessage.Text += Labels.UserPanel25;
                }

                if (string.IsNullOrEmpty(this.ErrorMessage.Text))
                {
                    this.loadinAnim.Visibility = System.Windows.Visibility.Visible;
                    var req = new EarthwatcherRequests(Constants.BaseApiUrl);
                    req.PasswordChanged += req_PasswordChanged;
                    Earthwatcher ew = new Earthwatcher { Name = Current.Instance.Earthwatcher.Name, Password = this.NewPassword.Password };
                    req.ChangePassword(ew);
                }
                else
                {
                    this.OkButton.IsEnabled = true;
                }
            }

            if (step == "ChangeRegional")
            {
                var username = Current.Instance.Username;
                var name = Current.Instance.Earthwatcher.Name;

                var ew = this.DataContext as Earthwatcher;
                // PARA CAMBIAR EL MAIL
                Current.Instance.Earthwatcher.MailChanged = true;
                if (Current.Instance.Username.Equals(ew.Name)) //Si no cambio el mail
                {
                    ew.MailChanged = false;
                    Current.Instance.Earthwatcher.MailChanged = false;
                }
                else if (!Regex.IsMatch(this.MailTextBox.Text, "^([a-zA-Z0-9_\\.\\-])+\\@(([a-zA-Z0-9\\-])+\\.)+([a-zA-Z0-9]{2,4})+$")) //Si no es un formato de mail valido
                {
                    this.ErrorMessage.Text = "Ingrese un mail valido";
                    Current.Instance.Earthwatcher.Name = Current.Instance.Username;
                }

                if (ew == null)
                {
                    this.ErrorMessage.Text = Labels.UserPanel26;
                }

                if (!Regex.IsMatch(this.NickNameTextBox.Text, "^[0-9a-zA-Z ]{0,20}$"))
                {
                    if (this.ErrorMessage.Text != string.Empty)
                    {
                        this.ErrorMessage.Text += "\n";
                    }
                    this.ErrorMessage.Text += Labels.UserPanel27;
                }

                if (string.IsNullOrEmpty(this.ErrorMessage.Text))
                {
                    this.loadinAnim.Visibility = System.Windows.Visibility.Visible;
                    var req = new EarthwatcherRequests(Constants.BaseApiUrl);
                    req.EarthwatcherUpdated += req_EarthwatcherUpdated;
                    req.Update(ew);
                }
                else
                {
                    this.OkButton.IsEnabled = true;
                }
            }
        }


        void req_EarthwatcherUpdated(object sender, EventArgs e)
        {
            this.OkButton.IsEnabled = true;
            this.loadinAnim.Visibility = System.Windows.Visibility.Collapsed;
            if (sender is System.Net.HttpStatusCode)
            {
                if (((System.Net.HttpStatusCode)sender) == System.Net.HttpStatusCode.OK)
                {
                    this.ErrorMessage.Foreground = new SolidColorBrush(Color.FromArgb(255, 105, 125, 0));
                    this.ErrorMessage.Text = Labels.UserPanel28;

                    if (Current.Instance.Earthwatcher.MailChanged == true)  
                    {
                        App.Logout();
                    }

                    #region Chequeo para ver si cambió el idioma (comentado)
                    /*
                    string newCulture = string.Empty;
                    if (Current.Instance.Earthwatcher.Language == "Spanish" && System.Threading.Thread.CurrentThread.CurrentCulture.Name != "es-AR")
                    {
                        newCulture = "es-AR";
                    }

                    if (Current.Instance.Earthwatcher.Language == "English" && System.Threading.Thread.CurrentThread.CurrentCulture.Name != "en-US")
                    {
                        newCulture = "en-US";
                    }

                    if (newCulture != string.Empty)
                    {
                        System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo(newCulture);
                        System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(newCulture);
                        ((CustomLabels)App.Current.Resources["CustomLabels"]).LocalizedLabels = new Labels();
                    }
                     * */
                    #endregion
                }
                else if (((System.Net.HttpStatusCode)sender) == System.Net.HttpStatusCode.MultipleChoices) //Si ya existia el mail
                {
                    this.ErrorMessage.Text = Labels.UserPanel33;
                }
                else
                {
                    this.ErrorMessage.Text = Labels.UserPanel26;
                }
            }
        }

        void req_PasswordChanged(object sender, EventArgs e)
        {
            this.OkButton.IsEnabled = true;
            this.loadinAnim.Visibility = System.Windows.Visibility.Collapsed;

            if (sender != null)
            {
                this.ErrorMessage.Foreground = new SolidColorBrush(Color.FromArgb(255, 105, 125, 0));
                this.ErrorMessage.Text = Labels.UserPanel30;
                Current.Instance.Password = this.NewPassword.Password;
                this.CurrentPassword.Password = string.Empty;
                this.NewPassword.Password = string.Empty;
                this.NewPassword2.Password = string.Empty;
            }
            else
            {
                this.ErrorMessage.Text = Labels.UserPanel31;
            }
        }

        private void BtnCloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class Language
    {
        public string Name { get; set; }
        public string LocalizedName { get; set; }
    }
}

