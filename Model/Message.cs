using BMW_20250523.Model.MessageContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMW_20250523.Model;

public class Message
{
    public required string Id { get; set; }

    public required string ChatId { get; set; }

    public required string SenderId { get; set; }

    public required List<IMessageContent> Contents { get; set; }

    public required List<string> AttachmentIds { get; set; }

    public required DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
