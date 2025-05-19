using BMW_20250523.Model;
using BMW_20250523.Model.MessageContent;
using BMW_20250523.Service.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMW_20250523.Service;

public class MessageService(IServiceProvider serviceProvider) : IMessageService
{
    private readonly List<Message> _messages = [];

    // Create, Read, Update, Delete (CRUD) operations

    // Create: AddMessage, ReplyToMessage
    public Message AddMessage(string chatId, string senderId, List<IMessageContent> contents, List<string> attachmentIds = null, DateTime? createdAt = null)
    {
        if (string.IsNullOrWhiteSpace(senderId) || contents == null || contents.Count == 0)
            throw new ArgumentException("Sender ID and content IDs cannot be null or empty.");

        var newMessage = new Message
        {
            Id = Guid.NewGuid().ToString("N"),
            ChatId = chatId,
            SenderId = senderId,
            Contents = contents,
            AttachmentIds = attachmentIds,
            CreatedAt = createdAt ?? DateTime.UtcNow,
        };

        do
        {
            var existingMessage = _messages.FirstOrDefault(x => x.Id == newMessage.Id);
            if (existingMessage == null) break;

            newMessage.Id = Guid.NewGuid().ToString("N");
        } while (true);

        _messages.Add(newMessage);

        return newMessage;
    }

    public Message ReplyToMessage(string parentMessageId, string senderId, List<IMessageContent> contents, List<string> attachmentIds = null, DateTime? createdAt = null)
    {
        var parentMessage = GetMessage(parentMessageId);
        if (parentMessage == null) throw new KeyNotFoundException($"Parent message with ID {parentMessageId} not found.");

        var newMessage = new Reply
        {
            Id = Guid.NewGuid().ToString("N"),
            ParentMessageId = parentMessageId,
            ChatId = parentMessage.ChatId,
            SenderId = senderId,
            Contents = contents,
            AttachmentIds = attachmentIds,
            CreatedAt = createdAt ?? DateTime.UtcNow,
        };

        do
        {
            var existingMessage = _messages.FirstOrDefault(x => x.Id == newMessage.Id);
            if (existingMessage == null) break;
            newMessage.Id = Guid.NewGuid().ToString("N");
        } while (true);

        _messages.Add(newMessage);

        return newMessage;
    }

    // Read: GetMessage, GetAllMessages, GetMessagesByIds
    public Message GetMessage(string messageId)
    {
        return _messages.FirstOrDefault(m => m.Id == messageId) ?? throw new KeyNotFoundException($"Message with ID {messageId} not found.");
    }

    public List<Message> GetAllMessages()
    {
        return _messages;
    }

    public List<Message> GetMessagesByIds(List<string> messageIds)
    {
        return [.. _messages.Where(m => messageIds.Contains(m.Id))];
    }

    public List<Message> GetMessagesByChatId(string chatId)
    {
        var messages = _messages.Where(m => m.ChatId == chatId).OrderBy(x => x.CreatedAt).ToList();
        return messages;
    }

    // Update: ReplaceMessage
    public void ReplaceMessage(string messageId, Message newMessage)
    {
        var index = _messages.FindIndex(m => m.Id == messageId);
        if (index != -1)
        {
            newMessage.ModifiedAt = DateTime.UtcNow;
            _messages[index] = newMessage;
        }
        else throw new KeyNotFoundException($"Message with ID {messageId} not found.");
    }

    // Delete: DeleteMessage

    public void DeleteMessage(string messageId)
    {
        var index = _messages.FindIndex(m => m.Id == messageId);
        if (index != -1) _messages.RemoveAt(index);
        else throw new KeyNotFoundException($"Message with ID {messageId} not found.");
    }

    public List<Message> GenerateSampleMessages(string chatId, int numberOfMessages, List<string> participantIds)
    {
        var attachmentService = serviceProvider.GetRequiredService<IAttachmentService>();

        var lastChatTime = DateTime.Now.AddMinutes(-5);
        var today = DateTime.Now.Date;
        var gap = (lastChatTime - today);

        var tick = gap / numberOfMessages;

        var messages = new List<Message>();
        for (int i = 0; i < numberOfMessages; i++)
        {
            var createdAt = today.Add(tick * i);
            var senderId = participantIds[Random.Shared.Next(participantIds.Count)];
            var contents = new List<IMessageContent>() { new TextMessageContent { Text = GenerateRandomKoreanSentence() } };
            var attachmentIds = new List<string>();

            // Seed for random number of attachments and reply
            var numberOfAttachments = Random.Shared.Next(-20, 5);
            var replySeed = Random.Shared.Next(0, 100);
            var isReply = replySeed < 10 && messages.Count > 0;

            if (numberOfAttachments > 0)
            {
                var attachments = attachmentService.GenerateSampleAttachments(numberOfAttachments);
                attachmentIds.AddRange(attachments.Select(x => x.Id));
            }

            if (isReply)
            {
                var parentMessageId = messages[Random.Shared.Next(messages.Count)].Id;
                var message = ReplyToMessage(parentMessageId, senderId, contents, attachmentIds, createdAt);
                messages.Add(message);
            }
            else
            {
                var message = AddMessage(chatId, senderId, contents, attachmentIds, createdAt);
                messages.Add(message);
            }
        }

        return messages;
    }

    private readonly string[] s_randomSentences = 
    [
        "안녕하세요", "사랑해요", "고마워요", "잘 지내요", "행복해요",
        "아름다워요", "기분이 좋아요", "좋은 하루 되세요", "잘 자요", "보고 싶어요",
        "감사합니다", "축하해요", "사랑스러워요", "행복한 하루 되세요", "잘 부탁드립니다",
        "즐거운 시간 되세요", "행복한 순간이에요", "좋은 꿈 꾸세요", "사랑하는 마음을 전해요",
        "기분이 좋네요", "아름다운 날이에요", "행복한 기억을 만들어가요", "잘 지내고 있어요",
        "오늘 맛있는 것 먹었어요", "당신은 이것에 대해 어떻게 생각하시나요?", "좋은 소식이 있어요",
        "이것 보세요", "정말 멋져요", "이해해 주셔서 감사합니다", "당신과 함께하는 시간이 소중해요",
        "오늘 장보러 가실래요?", "이 영화 정말 재밌어요", "이 노래 좋아요",
        "머지 않아 당신을 만날 수 있을 거예요", "당신과 함께하는 시간이 소중해요",
        "풀 리퀘스트 보냈어요", "오늘 null null 해요", "이거 어때요?",
        "오빠 char 뽑았다 null 데리러 가", "오늘 귀신 꿈 꿨어요", "이거 진짜 재밌어요",
        "정말 갓겜입니다!", "저는 소박한 꿈이 있어요", "꿈보다는 해몽이죠",
        "히어로즈 오브 더 스톰 하실래요?", "저는 스타크래프트를 더 좋아합니다"
    ];

    private string GenerateRandomKoreanSentence() => s_randomSentences[Random.Shared.Next(s_randomSentences.Length)];
}
