using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArticoliWebService.Dtos
{
    public class infoMsg
    {
        public DateTime Data {get; set;}
        public string Message {get; set;}
        public infoMsg(DateTime data, string Message)
        {
            this.Message = Message;
            this.Data = Data;
        }
    }
}