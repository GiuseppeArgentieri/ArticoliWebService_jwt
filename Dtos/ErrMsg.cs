using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArticoliWebService.Dtos
{
    public class ErrMsg
    {
        public ErrMsg(string message, int errcode)
        {
            this.message = message;
            this.errcode = errcode;
        }
        public string message { get; set; }
        public int errcode { get; set; }
    }
}