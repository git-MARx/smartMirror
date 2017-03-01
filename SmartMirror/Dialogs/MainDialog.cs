using System;
using System.Linq;
using System.Net;
using SmartMirror.Dialogs;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using SmartMirror.MirrorClass;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace SmartMirror.Dialogs
{

    [LuisModel("c31d919d-5d52-41e5-ac7d-86378aa83bb1", "374123c50a8440e4a26a9b7b8bf64ff9")]
    [Serializable]
    public class MainDialog : LuisDialog<object>
    {
        //public async Task StartAsync(IDialogContext context)
        //{
        //    context.Wait(MessageReceivedAsync);
        //}
        static string URL = "http://172.16.180.41/queue/enqueue";

        [LuisIntent("")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            Reply reply = new Reply();
            reply.key = "reply";
            reply.value = "I'm sorry, but I could didn't understand what you just said ...";

            await sendToRpiPost(URL, JsonConvert.SerializeObject(reply));

            await context.PostAsync(reply.value);
            context.Wait(MessageReceived);
        }

        [LuisIntent("weather")]
        public async Task WeatherIntent(IDialogContext context, LuisResult result)
        {
            string url = "http://api.openweathermap.org/data/2.5/weather?id=1260086&APPID=20b9f22bd7f0bd90b7207884127aa967";
            var resp = await sendToUrlnoDict(url, "", "GET");
            

            JToken token = JToken.Parse(resp);
            JArray weather = (JArray)token.SelectToken("weather");
            Reply reply = new Reply();
            reply.key = "weather";
            try
            {
                reply.value = "";
                foreach (JToken x in weather)
                {
                    reply.value += "Weather : " + x["main"] + " => " + x["description"]+"\n";
                    
                }

            }
            catch (Exception ex)
            {
                reply.value = "Aww moment!";
            }
            JToken y =  token["main"];
            reply.value += "temp : " + y["temp"] + "\n" +
                           "pressure : " + y["pressure"] + "\n" +
                           "humidity : " + y["humidity"] + "\n" +
                           "temp_min : " + y["temp_min"] + "\n" +
                           "temp_max : " + y["temp_max"] + "\n";
             await sendToRpiPost(URL, JsonConvert.SerializeObject(reply));

            await context.PostAsync(reply.value);
            context.Wait(MessageReceived);
        }

        [LuisIntent("start")]
        public async Task StartIntent(IDialogContext context, LuisResult result)
        {

            Reply reply = new Reply();
            reply.key = "reply";
            reply.value = "Hi there, hope your'e doing fine";

            await sendToRpiPost(URL, JsonConvert.SerializeObject(reply));

            await context.PostAsync(reply.value);
            context.Wait(MessageReceived);
        }

        [LuisIntent("end")]
        public async Task EndIntent(IDialogContext context, LuisResult result)
        {
            Reply reply = new Reply();
            reply.key = "end";
            reply.value = "Bye, I'll meet you again ...";

            await sendToRpiPost(URL, JsonConvert.SerializeObject(reply));

            await context.PostAsync(reply.value);
            context.Wait(MessageReceived);
        }

        [LuisIntent("agenda")]
        public async Task AgendaIntent(IDialogContext context, LuisResult result)
        {
            var dict = new Dictionary<string, string>();
            //dict["Content-Type"] = "application/json";
            dict["Teamup-Token"] = "e830cda50d679f395633195c3c1e3aa869cdc44e3f1d1da05fe4d1010d46afcb";
            string url = "https://api.teamup.com/ks59606c2dae866238/events";
            var resp = await sendToUrl(url, "", dict, "GET");

            JToken token = JToken.Parse(resp);
            JArray events = (JArray)token.SelectToken("events");
            Reply reply = new Reply();
            reply.key = "agenda";
            try
            {
                reply.value = "";
                foreach (JToken eve in events)
                {
                    //var startDate = DateTime.ParseExact((String)eve["start_dt"], "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    //var endDate = DateTime.ParseExact((String)eve["end_dt"], "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    DateTime startDate = DateTime.Parse((String)eve["start_dt"]);
                    DateTime endDate = DateTime.Parse((String)eve["end_dt"]);
                    reply.value += startDate.ToString("HH:mm") + " - " + endDate.ToString("HH:mm") + " : " + eve["title"] + "\n";
                }
            } catch( Exception ex)
            {
                reply.value = "Aww moment!";
            }
            


/*
            var objects = Newtonsoft.Json.Linq.JArray.Parse(resp);  
            foreach (Newtonsoft.Json.Linq.JObject root in objects.value["events"])
            {
                foreach (KeyValuePair<String, Newtonsoft.Json.Linq.JToken> app in root)
                {

                    var start = (String)app.Value["start_dt"];
                    var end = (String)app.Value["end_dt"];
                    var title = (String)app.Value["title"];

                    reply.value = start+" "+end+" "+ title+"\n";
                    //Console.WriteLine(start);
                    //Console.WriteLine(end);
                    //Console.WriteLine(title);
                    //Console.WriteLine("\n");
                }
            }
            */






            
            //reply.value = "Here are the events you requested,";
            //reply.value += "\n\rBTP Meeting, 12:00 PM";
            //reply.value += "\n\rParty at Zayka, 7:00 PM";
            //reply.value += "\n\rCounter Strike, 10:00 PM";
            //reply.value = resp;

            await sendToRpiPost(URL, JsonConvert.SerializeObject(reply));

            await context.PostAsync(reply.value);
            context.Wait(MessageReceived);
        }

        [LuisIntent("news")]
        public async Task NewsIntent(IDialogContext context, LuisResult result)
        {

            string url = "https://newsapi.org/v1/articles?source=techcrunch&apiKey=5db720e82ef24b77830df3c7893a887b";
            var resp = await sendToUrlnoDict(url, "", "GET");



            Reply reply = new Reply();
            reply.key = "news";
            reply.value = "";
            JToken token = JToken.Parse(resp);
            JArray articles = (JArray)token.SelectToken("articles");
            try
            {
                int n = 1;
                foreach (JToken x in articles)
                {
                    reply.value += n + "   " + x["title"] + " : " + x["description"] + "\n";
                    n = n + 1;
                }
            }
            catch (Exception ex)
            {
                reply.value = "Aww moment!";
            }


            await sendToRpiPost(URL, JsonConvert.SerializeObject(reply));

            await context.PostAsync(reply.value);
            context.Wait(MessageReceived);
        }

        private async Task<string> sendToRpiPost(string url, string data)
        {
            var dict = new Dictionary<string, string>();
            dict["Content-Type"] = "application/json";
            return await sendToUrl(url, data, dict, "POST");

        }

        private async Task<string> sendToUrl(string url, string data, Dictionary<string, string> headers, string method)
        {
            try
            {
                var client = new HttpClient();

                foreach (var header in headers)
                {
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Your Oauth token");
                    client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                }

                if (method == "POST")
                {

                    var response = await client.PostAsync(url, new StringContent(data));
                    var resp = await response.Content.ReadAsStringAsync();

                    return resp;

                }
                else if (method == "GET")
                {

                    var response = await client.GetAsync(url);
                    var resp = await response.Content.ReadAsStringAsync();

                    return resp;

                }
                else
                {
                    var result = "You fucked up!!!!";
                    return result;
                }
            }
            catch (Exception ex)
            {
                return "I fucked up again !!!";
            }

        }
        private async Task<string> sendToUrlnoDict(string url, string data, string method)
        {
            try
            {
                var client = new HttpClient();

               

                if (method == "POST")
                {

                    var response = await client.PostAsync(url, new StringContent(data));
                    var resp = await response.Content.ReadAsStringAsync();

                    return resp;

                }
                else if (method == "GET")
                {

                    var response = await client.GetAsync(url);
                    var resp = await response.Content.ReadAsStringAsync();

                    return resp;

                }
                else
                {
                    var result = "You fucked up!!!!";
                    return result;
                }
            }
            catch (Exception ex)
            {
                return "I fucked up again !!!";
            }

        }
    }
}