﻿using System;
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
        static string URL = "http://192.168.137.154/queue/enqueue";

        [LuisIntent("")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            Reply reply = new Reply();
            reply.key = "reply";
            reply.value = "I'm sorry, but I could didn't understand what you just said ...";

            sendToUrl(URL, JsonConvert.SerializeObject(reply));

            await context.PostAsync(reply.value);
            context.Wait(MessageReceived);
        }

        [LuisIntent("weather")]
        public async Task WeatherIntent(IDialogContext context, LuisResult result)
        {
            Reply reply = new Reply();
            reply.key = "weather";
            reply.value = "Here's the weather for the next few days,";
            reply.value += "\n\rToday : Clear skies, 23 C";
            reply.value += "\n\rTomorrow: Expected Rain, 15 C";

            sendToUrl(URL, JsonConvert.SerializeObject(reply));

            await context.PostAsync(reply.value);
            context.Wait(MessageReceived);
        }

        [LuisIntent("start")]
        public async Task StartIntent(IDialogContext context, LuisResult result)
        {

            Reply reply = new Reply();
            reply.key = "reply";
            reply.value = "Hi there, hope your'e doing fine";

            sendToUrl(URL, JsonConvert.SerializeObject(reply));

            await context.PostAsync(reply.value);
            context.Wait(MessageReceived);
        }

        [LuisIntent("end")]
        public async Task EndIntent(IDialogContext context, LuisResult result)
        {
            Reply reply = new Reply();
            reply.key = "end";
            reply.value = "Bye, I'll meet you again ...";      

            sendToUrl(URL, JsonConvert.SerializeObject(reply));

            await context.PostAsync(reply.value);
            context.Wait(MessageReceived);
        }

        [LuisIntent("agenda")]
        public async Task AgendaIntent(IDialogContext context, LuisResult result)
        {

            Reply reply = new Reply();
            reply.key = "agenda";
            reply.value = "Here are the events you requested,";
            reply.value += "\n\rBTP Meeting, 12:00 PM";
            reply.value += "\n\rParty at Zayka, 7:00 PM";
            reply.value += "\n\rCounter Strike, 10:00 PM";

            sendToUrl(URL, JsonConvert.SerializeObject(reply));

            await context.PostAsync(reply.value);
            context.Wait(MessageReceived);
        }

        [LuisIntent("news")]
        public async Task NewsIntent(IDialogContext context, LuisResult result)
        {

            Reply reply = new Reply();
            reply.key = "news";
            reply.value = "Here are the headlines you requested,";
            reply.value += "\n\rNew hardware revision from Tesla, says Elon Musk";
            reply.value += "\n\rApple is suing Qualcomm for 1 billion dollars";
            reply.value += "\n\rNougat update Samsung's devices";

            sendToUrl(URL, JsonConvert.SerializeObject(reply));

            await context.PostAsync(reply.value);
            context.Wait(MessageReceived);
        }

        private void sendToUrl(string url, string json)
        {
            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                var result = client.UploadString(url, "POST", json);
            }

        }
    }
}