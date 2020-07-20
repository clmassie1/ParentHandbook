using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace ParentHandbook.Models
{
    public class returnEmail
    {
        public string email { get; set; }
    }


    public class ServerResponse
    {
        public HttpResponseMessage Response { get; set; }
        public string Message { get; set; }
    }


    public class codes
    {
        [JsonProperty("AUTH_CODE")]
        public string AUTH_CODE { get; set; }
        public string TIMESTAMP { get; set; }
    }

    public class coderesp
    {

        public string AUTH_ID { get; set; }
        public string AUTH_CODE { get; set; }
        public string TIMESTAMP { get; set; }
    }

    public class RetrieveMultipleResponse
    {
        public List<txsummary> txsummary { get; set; }
    }

    public class txsummary
    {
        public string AUTH_ID { get; set; }
        public string AUTH_CODE { get; set; }
        public string TIMESTAMP { get; set; }
    }


    public class guardiansStudent
    {
        public string studentPersonID { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string school { get; set; }
        public string gradelevel { get; set; }
    }

    public class HandbookQuestions
    {
        public string name { get; set; }
    }


    public class HandbookQuestions2
    {
        public string name { get; set; }
    }



    public class HandbookFinal
    {
        public MNPSHandbookReceivedSignature MNPSHandbookReceivedSignature { get; set; }
        public CheckOutLaptopfromLibrary CheckOutLaptopfromLibrary { get; set; }
    }

    public class MNPSHandbookReceivedSignature
    {
        [JsonProperty("personID")]
        public string personID { get; set; }
        [JsonProperty("attributeID")]
        public string attributeID { get; set; }
        [JsonProperty("value")]
        public string value { get; set; }
        [JsonProperty("date")]
        public string date { get; set; }
    }

    public class CheckOutLaptopfromLibrary
    {
        [JsonProperty("personID")]
        public string personID { get; set; }
        [JsonProperty("attributeID")]
        public string attributeID { get; set; }
        [JsonProperty("value")]
        public string value { get; set; }
        [JsonProperty("date")]
        public string date { get; set; }
    }

}