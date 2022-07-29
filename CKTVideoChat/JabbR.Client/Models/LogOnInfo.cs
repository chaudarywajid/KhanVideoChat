using System.Collections.Generic;

namespace CKTChat.Client.Models
{
    public class LogOnInfo
    {
        public string UserId { get; set; }
        public IEnumerable<Room> Rooms { get; set; }

        public LogOnInfo()
        {
            Rooms = new List<Room>();
        }
    }
}
