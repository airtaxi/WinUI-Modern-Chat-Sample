using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMW_20250523.Model.Mvvm;

public class NewMessageCreatedMessage(Message message) : ValueChangedMessage<Message>(message);
