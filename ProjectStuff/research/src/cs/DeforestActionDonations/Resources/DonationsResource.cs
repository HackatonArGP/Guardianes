using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using Microsoft.ApplicationServer.Http;
using DeforestActionDonations.Repositories;
using DeforestActionDonations.Models;
using Microsoft.SqlServer.Types;
using System.ServiceModel.Channels;

namespace DeforestActionDonations.Resources
{
    [ServiceContract]
    public class DonationsResource
    {
        private readonly DonationRepository _donationRepository;

        public DonationsResource()
        {
            var connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings["DbConnectionString"].ConnectionString;
            _donationRepository = new DonationRepository(connectionstring);
        }

        //Get all adopters
        [WebGet(UriTemplate = "/all")]
        public HttpResponseMessage<List<Adopter>> GetAll(HttpRequestMessage request)
        {
            var adopters = _donationRepository.GetAllAdopters();
            return new HttpResponseMessage<List<Adopter>>(adopters) { StatusCode = HttpStatusCode.OK };
        }

        //Get all adopters
        [WebGet(UriTemplate = "/ipcheck")]
        public HttpResponseMessage<string> IpCheck(HttpRequestMessage request)
        {
            var prop = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

            var ipString = "";
            foreach(var ip in Constants.AllowedIps)
                ipString += string.Format("  {0}  -", ip);

            var message = string.Format("Your IP: {0} | Allowed IP's: {1}", prop.Address, ipString);
            return new HttpResponseMessage<string>(message) { StatusCode = HttpStatusCode.OK };
        }

        //Give piece of land to adopter and return the adopter
        [WebGet(UriTemplate = "?transaction={transaction}&username={username}&name={name}&amount={amount}&area={area}")]
        public HttpResponseMessage<Adopter> AssignLand(int transaction, string username, string name, int amount, int area, HttpRequestMessage request)
        {
            var prop = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

            bool isAllowed = false;
            foreach (var ip in Constants.AllowedIps)
            {
                if (ip.Equals(prop.Address))
                    isAllowed = true;
            }

            if(!isAllowed)
                return new HttpResponseMessage<Adopter>(null) { StatusCode = HttpStatusCode.Forbidden };

            //Create and assign land
            var adopter = new Adopter { username = username, name = name, amount = amount, area = area, transaction_nr = transaction };
            adopter = _donationRepository.AssignLand(adopter);
            if(adopter == null)
                return new HttpResponseMessage<Adopter>(null) { StatusCode = HttpStatusCode.BadRequest};

            //Add to database
            adopter = _donationRepository.AddAdopter(adopter);
            if (adopter == null)
                return new HttpResponseMessage<Adopter>(null) { StatusCode = HttpStatusCode.BadRequest };

            //Return Adopter with assigned land
            return new HttpResponseMessage<Adopter>(adopter) { StatusCode = HttpStatusCode.OK };
        }      
    }
}





