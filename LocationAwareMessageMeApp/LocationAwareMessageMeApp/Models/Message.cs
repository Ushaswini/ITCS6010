using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LocationAwareMessageMeApp.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string MessageBody { get; set; }
        public bool IsRead { get; set; }
        public string ReceiverId { get; set; }
        public string SenderId { get; set; }

        public bool IsUnLocked { get; set; }
        public string RegionId { get; set; }

        public string MessageTime { get; set; }

        public string RegionName { get; set; }
        public string SenderUserName { get; set; }
        public string SenderFullName { get; set; }
        public string ReceiverUserName { get; set; }
        public string ReceiverFullName { get; set; }

        public Dictionary<string, string> ToMap()
        {

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            parameters.Add("MessageBody", this.MessageBody);
            parameters.Add("IsRead", this.IsRead+"");
            parameters.Add("ReceiverId", this.ReceiverId);
            parameters.Add("SenderId", this.SenderId);
            parameters.Add("MessageTime", this.MessageTime);
            parameters.Add("IsLocked", this.IsUnLocked + "");
            parameters.Add("RegionId", this.RegionId);
            


            return parameters;
        }

    }
}