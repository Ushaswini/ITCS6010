using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Homework1.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string MessageBody { get; set; }
        public bool IsRead { get; set; }
        public string ReceiverId { get; set; }
        public string SenderId { get; set; }
        public string RegionId { get; set; }
        public bool IsUnLocked { get; set; }

        public string MessageTime { get; set; }

    }
}