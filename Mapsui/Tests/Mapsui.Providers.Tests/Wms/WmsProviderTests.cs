﻿using NUnit.Framework;
using Mapsui.Providers.Wms;
using System.Linq;
using System.Xml;

namespace Mapsui.Providers.Tests.Wms
{
    [TestFixture]
    class WmsProviderTests
    {
        [Test]
        public void GetLegendRequestUrls_WhenInitialized_ShouldReturnListOfUrls()
        {
            // arrange
            var capabilties = new XmlDocument();
            capabilties.XmlResolver = null;
            capabilties.Load(".\\Resources\\capabilities_1_3_0.xml");
            var provider = new WmsProvider(capabilties);
            provider.SpatialReferenceSystem = "EPSG:900913";
            provider.AddLayer("Maasluis complex - top");
            provider.AddLayer("Kreftenheye z2 - top");
            provider.SetImageFormat(provider.OutputFormats[0]);
            provider.ContinueOnError = true;

            // act
            var legendUrls = provider.GetLegendRequestUrls();

            // assert
            Assert.True(legendUrls.Count() == 2);
        }

        [Test]
        public void GetLegend_WhenCalled_ShouldReturnLegend()
        {
            // arrange
            var capabilties = new XmlDocument();
            capabilties.XmlResolver = null;
            capabilties.Load(".\\Resources\\capabilities_1_3_0.xml");
            var provider = new WmsProvider(capabilties);
            provider.SpatialReferenceSystem = "EPSG:900913";
            provider.AddLayer("Maasluis complex - top");
            provider.AddLayer("Kreftenheye z2 - top");
            provider.SetImageFormat(provider.OutputFormats[0]);
            provider.ContinueOnError = true;

            // act
            var legendImages = provider.GetLegends();
            

            // assert
            Assert.True(legendImages.Count() == 2);
        }
    }
}
