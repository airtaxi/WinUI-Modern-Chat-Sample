using BMW_20250523.Model;
using BMW_20250523.Service.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMW_20250523.Service;

public class ChatService(IServiceProvider serviceProvider) : IChatService
{
    private readonly List<Chat> _chats = [];

    // Create, Read, Update, Delete (CRUD) operations

    // Create: AddChat
    public Chat AddChat(List<string> participantIds)
    {
        if (participantIds == null || participantIds.Count == 0)
            throw new ArgumentException("Participant IDs cannot be null or empty.");

        var newChat = new Chat
        {
            Id = Guid.NewGuid().ToString("N"),
            ParticipantIds = participantIds,
            CreatedAt = DateTime.UtcNow,
        };

        do
        {
            var existingChat = _chats.FirstOrDefault(x => x.Id == newChat.Id);
            if (existingChat == null) break;

            newChat.Id = Guid.NewGuid().ToString("N");
        } while (true);

        _chats.Add(newChat);

        return newChat;
    }

    // Read: GetChat, GetAllChats, GetChatsByIds
    public Chat GetChat(string chatId) => _chats.FirstOrDefault(x => x.Id == chatId)
        ?? throw new KeyNotFoundException($"Chat with ID {chatId} not found.");

    public List<Chat> GetAllChats()
    {
        // Return chats sorted by the latest message date
        var messagesService = serviceProvider.GetRequiredService<IMessageService>();
        var latestMessages = messagesService.GetAllMessages()
            .GroupBy(x => x.ChatId)
            .Select(g => g.OrderByDescending(x => x.CreatedAt).FirstOrDefault())
            .ToList();

        return _chats.OrderByDescending(chat => latestMessages.FirstOrDefault(m => m.ChatId == chat.Id)?.CreatedAt ?? chat.CreatedAt).ToList();
    }

    public List<Chat> GetChatsByIds(List<string> chatIds)
    {
        if (chatIds == null || chatIds.Count == 0) return [.. _chats];
        return [.. _chats.Where(x => chatIds.Contains(x.Id))];
    }

    // Update: ReplaceChat
    public void ReplaceChat(string chatId, Chat newChat)
    {
        var index = _chats.FindIndex(x => x.Id == chatId);
        if (index != -1) _chats[index] = newChat;
        else throw new KeyNotFoundException($"Chat with ID {chatId} not found.");
    }

    // Delete: DeleteChat
    public void DeleteChat(string chatId)
    {
        var index = _chats.FindIndex(x => x.Id == chatId);
        if (index != -1) _chats.RemoveAt(index);
        else throw new KeyNotFoundException($"Chat with ID {chatId} not found.");
    }

    public List<Chat> GenerateSampleChats(int numberOfChats)
    {
        var userService = serviceProvider.GetRequiredService<IUserService>();
        var messagesService = serviceProvider.GetRequiredService<IMessageService>();

        var chats = new List<Chat>();

        for (int i = 0; i < numberOfChats; i++)
        {
            var participantCount = Random.Shared.Next(2, 10);
            var participants = userService.GetRandomUsers(participantCount);
            var participantIds = participants.Select(x => x.Id).ToList();

            var chat = AddChat(participantIds);
            chats.Add(chat);

            var numberOfMessages = Random.Shared.Next(10, 100);
            messagesService.GenerateSampleMessages(chat.Id, numberOfMessages, participantIds);
        }

        return chats;
    }
}

