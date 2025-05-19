using BMW_20250523.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMW_20250523.Service.Interface;

public interface IChatService
{
    // Create, Read, Update, Delete (CRUD) operations
    // Create: AddChat
    public Chat AddChat(List<string> participantIds);

    // Read: GetChat, GetAllChats, GetChatsByIds
    public Chat GetChat(string chatId);
    public List<Chat> GetAllChats();
    public List<Chat> GetChatsByIds(List<string> chatIds);

    // Update: ReplaceChat
    public void ReplaceChat(string chatId, Chat newChat);

    // Delete: DeleteChat
    public void DeleteChat(string chatId);

    public List<Chat> GenerateSampleChats(int numberOfChats);
}
