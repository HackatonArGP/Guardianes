// Copyright (c) 2011 Barend Gehrels, Amsterdam, the Netherlands

// License? Below is MIT

// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.


using System;
using System.Windows;
using System.Diagnostics;
using System.Windows.Threading;

using Microsoft.SqlServer.Types;


namespace DeforestAction
{
    public partial class MainWindow : Window
    {
        private AssignPlotOfLand assigner;
        private DispatcherTimer timer;
        private Random generator;
        private string connectionString = @"Data Source=dfrvf2t76i.database.windows.net;Initial Catalog=DeforestActionDonations;Persist Security Info=True;User ID=Beheerder;Password=KA42kaMa";

        public MainWindow()
        {
            InitializeComponent();

            assigner = new AssignPlotOfLand();
            generator = new Random();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += new EventHandler(ElapsedEventHandler);

            SetStartingPoint();
        }

        private void ShowData()
        {
            if (mapControl != null)
            {
                mapControl.ShowData(App.GetConnection(connectionString), slider1.Value);
            }
        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            ShowData();
        }

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ShowData();
        }


        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ShowData();
        }

        private void SetStartingPoint()
        {
            // Add 1m/1m (just for visualization) and set first point of it as THE starting point.
            SqlGeometry startingSquareKm = GeometryExtensions.FromWkt("POLYGON((12389976.191540662 -33080.958851181334,12389976.191540662 -33081.958851181334,12389977.191540662 -33081.958851181334,12389977.191540662 -33080.958851181334,12389976.191540662 -33080.958851181334))", 3857);
            mapControl.Add(startingSquareKm);
            assigner.SetStartingPoint(startingSquareKm.STPointN(1));
            {
                var connection = App.GetConnection(connectionString);

                foreach (var geom in connection.Select<Geom>("select top 1 gid, geom from dbo.tembak_hutan"))
                {
                    assigner.SetOuterBoundary(geom.Geometry);
                }
            }
            ShowData();
        }

        private void AssignButton_Click(object sender, RoutedEventArgs e)
        {
            // Assign 10 square meter
            mapControl.Add(assigner.Assign(10, 3857));
            ShowData();            
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            timer.Start();
        }
        private void checkBox1_Unchecked(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }

        private void ElapsedEventHandler(Object sender, EventArgs args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Assign a new plot of land and add it to the map control 
            mapControl.Add(assigner.Assign(generator.Next(1, 50), 3857));

            stopwatch.Stop();
            timerTextBox.Text = stopwatch.ElapsedMilliseconds.ToString();

            timer.Interval = TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds + 50);

            ShowData();
        }

    }
}
