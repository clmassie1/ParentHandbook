using ParentHandbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using System.Collections.Generic;

namespace ParentHandbook.Controllers
{
    public class HomeController : Controller
    {

        public static string AUTH_ID { get; private set; }
        public static string NoEmailMsg { get; private set; }
        public static string UserEmail { get; private set; }
        public static string BadCode { get; private set; }


        public ActionResult button1_Click(string code)
        {


            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("YOUREMAIL");
                mail.To.Add("YOUREMAIL");
                mail.Subject = "authCode";
                mail.Body = $"This is for testing SMTP mail from GMAIL{code}";
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("YOUREMAIL", "YOUREMAIL");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }


            return RedirectToAction("Authentication");
            
        }



        public async Task<ActionResult> OnPost(string email)
        {
            var emailAddress = email;

      
            var userName = "USERNAME";
            var passwd = "PASSWORD";
            var url = "https://apitest.mnps.org:8443/sis/v1/staffcontacts";

            var client = new HttpClient();

            var authToken = Encoding.ASCII.GetBytes($"{userName}:{passwd}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(authToken));

            var result = client.GetAsync(url + $"?sysfilter=equal(email: '{emailAddress}')");
            result.Wait();

            var result2 = result.Result;
       

            if (result2.IsSuccessStatusCode)
            {
                
                var readTask2 = result2.Content.ReadAsStringAsync();
                readTask2.Wait();

                var readTask = result2.Content.ReadAsAsync<returnEmail[]>();
                readTask.Wait();

                var students = readTask.Result;

                foreach (var st in students) {
                    if (st.email != null)
                    {
                        int _min = 100000;
                        int _max = 999999;
                        Random _rdm = new Random();
                        var code = _rdm.Next(_min, _max);
                        var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        await saveCode(time, code.ToString());
                        button1_Click(code.ToString());
                        UserEmail = st.email;
                        return RedirectToAction("Authentication");
                    }
                    else
                    {
                        NoEmailMsg = "We could not find your email in our system. Please contact your student’s school to update your personal contact information. If you need ";
                        return RedirectToAction("/");
                    }
                }
            }
            NoEmailMsg = "We could not find your email in our system. Please contact your student’s school to update your personal contact information. If you need ";
            return RedirectToAction("/");
        }


        public async Task<ServerResponse> SendPropagateRequestAsync(HttpRequestMessage request, HttpClient client)
        {
            ServerResponse serverResponse = new ServerResponse();
            try
            {
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {

                    var readTask1 = response.Content.ReadAsStringAsync();
                    readTask1.Wait();


                    var result2 = readTask1.Result;

                    var objResponse1 = JsonConvert.DeserializeObject<RetrieveMultipleResponse>(result2);
                    var test = objResponse1.txsummary[0];

                    var te = test;

                    AUTH_ID = te.AUTH_ID;

                    return serverResponse;
                }
                else
                {
                    serverResponse.Response = response;
                    serverResponse.Message = await response.Content.ReadAsStringAsync();
                    return serverResponse;
                }
            }

            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest };
                serverResponse.Response = response;
                return serverResponse;
            }
        }

        public async Task<ServerResponse> Post(string resourceUri, dynamic dto)
        {


            var userName = "USERNAME";
            var passwd = "PASSWORD";

            var client = new HttpClient();

            var authToken = Encoding.ASCII.GetBytes($"{userName}:{passwd}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(authToken));

            var json = JsonConvert.SerializeObject(dto, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{resourceUri}")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
            };
            return await SendPropagateRequestAsync(request, client);
        }


        public ActionResult CodeMatch(string Code)
        {

            var code = Code;

            var userName = "USERNAME";
            var passwd = "PASSWORD";
            var url = "https://apitest.mnps.org:8443/sis/v1/handbook-auth-codes";

            var client = new HttpClient();

            var authToken = Encoding.ASCII.GetBytes($"{userName}:{passwd}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(authToken));

            var result = client.GetAsync(url + $"?sysfilter=equal(AUTH_ID: '{AUTH_ID}')");
            result.Wait();

            var result2 = result.Result;

            if (result2.IsSuccessStatusCode)
            {

                var readTask = result2.Content.ReadAsAsync<txsummary[]>();
                readTask.Wait();

                var students = readTask.Result;

                if (students[0].AUTH_CODE == code && Convert.ToDateTime(students[0].TIMESTAMP).AddMinutes(10) > DateTime.Now)
                {

                    return RedirectToAction("Search");
                }
                else
                {
                    return RedirectToAction("Authentication");
                }

            }
            return RedirectToAction("Authentication");

        }



        public async Task<ServerResponse> saveCode(string timeStamp, string code)
        {

            codes data = new codes()
            {
                AUTH_CODE = code,
                TIMESTAMP = timeStamp
            };

            return await Post($"https://apitest.mnps.org:8443/sis/v1/handbook-auth-codes", data);
        }


        public async Task<ActionResult> HandbookQuestionsPost(string MNPSHandbookReceivedSignature, string CheckOutLaptopfromLibrary)
        {

            MNPSHandbookReceivedSignature data1 = new MNPSHandbookReceivedSignature()
            {
                personID = "7",
                attributeID = "701",
                value = MNPSHandbookReceivedSignature,
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            CheckOutLaptopfromLibrary data2 = new CheckOutLaptopfromLibrary()
            {
                personID = "7",
                attributeID = "700",
                value = CheckOutLaptopfromLibrary,
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            ArrayList myAL = new ArrayList();

            myAL.Add(data1);
            myAL.Add(data2);

            await Post($"https://apitest.mnps.org:8443/sis/v1/IC:CustomStudent", myAL);

            return RedirectToAction("Search");

        }
     


        public ActionResult Index()
        {

            ViewBag.Title = "MNPS Student Parent Permission Authentication";


            ViewBag.msg = NoEmailMsg;
            return View();
        }

        public ActionResult Authentication()
        {
            ViewBag.Title = "MNPS Student Parent Permission Authentication";

            ViewBag.UserEmail = UserEmail;

            return View();
        }

        public ActionResult Search(string PersonID)
        {

            var userName = "USERNAME";
            var passwd = "PASSWORD";
            var url = "https://api.mnps.org:8443/sis/v1/guardians";

            var client = new HttpClient();

            var authToken = Encoding.ASCII.GetBytes($"{userName}:{passwd}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(authToken));

            var result = client.GetAsync(url + $"?sysfilter=equal(parentPersonID: '315966')");
            result.Wait();

            var result2 = result.Result;

            if (result2.IsSuccessStatusCode)
            {
                var readTask = result2.Content.ReadAsAsync<guardiansStudent[]>();
                readTask.Wait();

                var students = readTask.Result;

                return View(students);

            }
            return View();

        }



        public ActionResult HandbookQuestions(string studentPersonID)
        {
            var userName = "USERNAME";
            var passwd = "PASSWORD";
            var url = "https://api.mnps.org:8443/sis/v1/IC:CampusAttribute";

            ViewBag.Id = studentPersonID;

            var client = new HttpClient();

            var authToken = Encoding.ASCII.GetBytes($"{userName}:{passwd}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(authToken));

            var result = client.GetAsync(url + $"?sysfilter=equal(object:'Release Consent')&order=seq");
            result.Wait();

            var result2 = result.Result;

            if (result2.IsSuccessStatusCode)
            {
                var readTask = result2.Content.ReadAsAsync<HandbookQuestions[]>();
                readTask.Wait();

                var students = readTask.Result;

                return View(students);

            }
            return View();

        }
    }
}