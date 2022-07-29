using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKTChat.Client.Models
{
    public class ClientMessage
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string Room { get; set; }
    }
}
