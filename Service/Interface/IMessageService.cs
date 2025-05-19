using BMW_20250523.Model;
using BMW_20250523.Model.MessageContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMW_20250523.Service.Interface;

public interface IMessageService
{
    // Create, Read, Update, Delete (CRUD) operations

    // Create: AddMessage, ReplyToMessage
    public Message AddMessage(string chatId, string senderId, List<IMessageContent> contents, List<string> attachmentIds = null, DateTime? createdAt = null);
    public Message ReplyToMessage(string parentMessageId, string senderId, List<IMessageContent> contents, List<string> attachmentIds = null, DateTime? createdAt = null);

    // Read: GetMessage, GetAllMessages, GetMessagesByIds
    public Message GetMessage(string messageId);
    public List<Message> GetAllMessages();
    public List<Message> GetMessagesByIds(List<string> messageIds);
    public List<Message> GetMessagesByChatId(string chatId);

    // Update: ReplaceMessage
    public void ReplaceMessage(string messageId, Message newMessage);

    // Delete: DeleteMessage
    public void DeleteMessage(string messageId);

    public List<Message> GenerateSampleMessages(string chatId, int numberOfMessages, List<string> participantIds);
}
