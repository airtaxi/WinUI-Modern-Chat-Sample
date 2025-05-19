using BMW_20250523.Model.MessageContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMW_20250523.Model;

public class Reply : Message
{
    public required string ParentMessageId { get; set; }
}