using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Earthwatchers.Models
{
    public class ApiEw
    {
        public int Id { get; set; }
        public string Api { get; set; }
        public string Mail { get; set; }
        public string UserId { get; set; }
        public string NickName { get; set; }
        public string AccessToken { get; set; }
        public string SecretToken { get; set; }
        public int EarthwatcherId { get; set; }
    }

    public class ApiPrueba
    {
        public string Mail { get; set; }
        public string AccessToken { get; set; }
    }
}
