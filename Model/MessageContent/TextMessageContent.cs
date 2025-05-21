using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMW_20250523.Model.MessageContent;

public class TextMessageContent : IMessageContent
{
    public string Text { get; set; }
}
