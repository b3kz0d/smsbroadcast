using SMSBroadcast.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMSBroadcast.Models
{
    public class BroadcastViewModel
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        [Required(ErrorMessage = "*")]
        public string Message { get; set; }
        [Required(ErrorMessage = "*")]
        public DateTime SendDate { get; set; } = DateTime.Today;
        [Required(ErrorMessage = "*")]
        public TimeSpan SendTime { get; set; }
        [Required(ErrorMessage = "*")]
        public Group Group { get; set; } = new Group();

        [DataType(DataType.Upload)]
     
        public HttpPostedFileBase FileUpload { get; set; }

        public List<BroadcastViewModel> Messages { get; set; } = new List<BroadcastViewModel>();
    }
}