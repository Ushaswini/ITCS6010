using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twilio;
using Twilio.AspNet.Mvc;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML;
using Twilio.Types;

namespace InClassMessaging.Controllers
{
    public class SmsController : TwilioController
    {
         readonly String AccountSid = "AC562f53f4ccf57ddf77944dc675e543d2";
         readonly String AuthToken = "d3892e5c907e4ed8829e1b2f3921c79f";
        public ActionResult SendSms()
        {
           // var accountSid = "AC562f53f4ccf57ddf77944dc675e543d2";
           // var authToken = "";

            TwilioClient.Init(AccountSid, AuthToken);
            var to = new PhoneNumber("+19805980003");
            var from = new PhoneNumber("+19808192819");
            var message = MessageResource.Create(to: to, from: from, body: "This is the test message");
 
            return Content(message.Sid);

        }

        public ActionResult ReceiveSms()
        {
            var response = new MessagingResponse();
            response.Message("The robots are coming!!");
            return TwiML(response);
        }
       
    }
}