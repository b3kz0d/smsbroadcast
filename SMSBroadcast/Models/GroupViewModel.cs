using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMSBroadcast.Models
{
    public class GroupViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ContactViewModel> Contacts { get; set; } = new List<ContactViewModel>();
    }
}