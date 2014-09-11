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
using DonationsViewer.Models;

namespace DonationsViewer.GUI.Controls
{
    public partial class Overview : UserControl
    {
        public Overview()
        {
            InitializeComponent();
        }

        public void UpdateOverview(List<Adopter> adopters)
        {
            double totalAreaAdopted = 0;
            double totalAreas = 0;
            double totalDonated = 0;

            foreach (var adopter in adopters)
            {
                //calculate amounts to show in overview
                totalAreaAdopted += adopter.area;
                totalAreas++;
                totalDonated += adopter.amount;
            }

            txtAdoptedArea.Text = totalAreaAdopted + " m2";
            txtAdoptions.Text = totalAreas.ToString();
            txtDonated.Text = string.Format("${0},-", totalDonated);
        }
    }
}
