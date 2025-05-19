using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMW_20250523.Model;

public class Chat
{
    public required string Id { get; set; }

    public required List<string> ParticipantIds { get; set; }

    public required DateTime CreatedAt { get; set; }
}
