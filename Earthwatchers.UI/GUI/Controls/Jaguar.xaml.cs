using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Earthwatchers.UI.Requests;

namespace Earthwatchers.UI.GUI.Controls
{
    public partial class Jaguar
    {
        public int posId { get; set; }

        public Jaguar()
        {
            //InitializeComponent();

        }

        private void JaguarClicked(object sender, MouseButtonEventArgs e)
        {
            var posId = Current.Instance.JaguarPositon.Id;
            //Current.Instance.JaguarPositon = null; //game over.
            JaguarRequests requests = new JaguarRequests(Constants.BaseApiUrl);
            requests.UpdateWinner(Current.Instance.Username, posId);  //Updatear el que lo encontro
        }


    }
}

