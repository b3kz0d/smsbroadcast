using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMSBroadcast.Models
{
    public class ContactViewModel
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string Phone { get; set; }
    }
}