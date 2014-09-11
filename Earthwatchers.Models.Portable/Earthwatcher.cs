using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Earthwatchers.Models
{
    public class Earthwatcher : INotifyPropertyChanged
    {
        //private string baseEarthwatcherUri = "earthwatchers/{0}";
        //private string baseLandUri = "land/{0}";

        public int Id { get; set; }
        public Guid Guid { get; set; }
        [Required]
        public string Name { get; set; }
        public Role Role { get; set; }
        //public int? LandId { set; get; }
        public string Basecamp { get; set; }
        public string Password { get; set; }
        public string Language { get; set; }
        public string Region { get; set; }
        public bool IsPowerUser { get; set; }
        public bool NotifyMe { get; set; }
        public bool MailChanged { get; set; }
        public int ApiEwId { get; set; }
        public string UserId { get; set; }
        public string AccessToken { get; set; }

        private string _nickName;
        public string NickName
        {
            get { return _nickName; }
            set
            {
                if (_nickName != value)
                {
                    _nickName = value;
                    OnPropertyChanged("NickName");
                    OnPropertyChanged("FullName");
                }
            }
        }

        public string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.NickName))
                {
                    return this.NickName;
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.Name))
                    {
                        return this.Name.Split('@')[0];
                    }
                }
                return null;
            }
        }
        public string Country { get; set; }

        public List<Land> Lands { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public string[] GetRoles()
        {
            var roles = new List<string> { Role.Earthwatcher.ToString() };
            switch (this.Role)
            {
                case Role.Moderator:
                    roles.Add(Role.Moderator.ToString());
                    break;
                case Role.Admin:
                    roles.Add(Role.Admin.ToString());
                    roles.Add(Role.Moderator.ToString());
                    break;
            }
            return roles.ToArray();
        }
    }
}
