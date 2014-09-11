using System;
using System.Net;
using System.Collections.Generic;
using Earthwatchers.UI.Layers;
using Mapsui.Windows;
using Earthwatchers.Models;

namespace Earthwatchers.UI
{
    public class Current
    {
        private Current() { }
        private static Current _instance;                 
        public MapControl MapControl { get; set; }
        public LayerHelper LayerHelper { get; set; }
        public Boolean IsAuthenticated { get; set; } //Is the earthwatcher authenticated
        public Earthwatcher Earthwatcher { get; set; } //Earthwatcher taken from provided username
        //public Land EarthwatcherLand { get; set; } //Land of logged in earthwatcher
        public List<Land> LandInView { get; set; } //Land show in the current view
        public List<Land> Lands { get; set; }
        //public MainPage Main { get; set; } //TODO: remove after demo, used to access legend view
        public string Username { get; set; }
        public string Password { get; set; }
        public List<Score> Scores { get; set; }
        public bool TutorialStarted { get; set; }
        public int TutorialCurrentStep { get; set; }
        public string LastImageDate { get; set; }
        public JaguarGame JaguarGame { get; set; }
        public bool JaguarGameStarted { get; set; }
        public Features Features { get; set; }
        public CookieContainer CookieContainer { get; set; }

        public List<Score> AddScore { get; set; }

        public static Current Instance
        {
            get
            {
                return _instance ?? (_instance = new Current());
            }
        }
    }
}
