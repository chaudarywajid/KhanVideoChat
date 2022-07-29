using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKTChat.Client.Models
{
    public class ClientNotification
    {
        public string Room { get; set; }
        public string ImageUrl { get; set; }
        public string Source { get; set; }
        public string Content { get; set; }
    }
}
