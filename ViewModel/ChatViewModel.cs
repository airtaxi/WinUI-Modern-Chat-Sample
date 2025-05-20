using BMW_20250523.Model;
using BMW_20250523.Model.MessageContent;
using BMW_20250523.Model.Mvvm;
using BMW_20250523.Service.Interface;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace BMW_20250523.ViewModel;

public partial class ChatViewModel : ObservableObject
{
    public Chat Chat { get; set; }

    public ObservableCollection<UserViewModel> ParticipantUserViewModels { get; } = [];
    public ObservableCollection<MessageViewModel> MessageViewModels { get; } = [];

    [ObservableProperty]
    public partial ObservableCollection<Inline> Inlines { get; private set; }

    public ChatViewModel(Chat chat)
    {
        Chat = chat ?? throw new ArgumentNullException(nameof(chat));

        var userService = App.ServiceProvider.GetRequiredService<IUserService>();
        var messageService = App.ServiceProvider.GetRequiredService<IMessageService>();

        var participants = userService.GetUsersByIds(chat.ParticipantIds);
        foreach(var participant in participants)
        {
            var userViewModel = new UserViewModel(participant);
            ParticipantUserViewModels.Add(userViewModel);
        }

        var messages = messageService.GetMessagesByChatId(chat.Id);

        foreach(var message in messages)
        {
            var messageViewModel = new MessageViewModel(message, MessageViewModels, ParticipantUserViewModels);
            MessageViewModels.Add(messageViewModel);
        }

        var lastMessage = messages.LastOrDefault();
        if (lastMessage != null) Inlines = GenerateLastMessageInlines(lastMessage);
        else Inlines = [new Run { Text = "새로운 채팅", FontWeight = FontWeights.Bold }]; ;

        WeakReferenceMessenger.Default.Register<NewMessageCreatedMessage>(this, OnNewMessageCreatedMessageReceived);
    }

    private void OnNewMessageCreatedMessageReceived(object recipient, NewMessageCreatedMessage message)
    {
        if (message.Value.ChatId != Chat.Id) return;

        var messageViewModel = new MessageViewModel(message.Value, MessageViewModels, ParticipantUserViewModels);
        MessageViewModels.Add(messageViewModel);

        Inlines = GenerateLastMessageInlines(message.Value);
    }

    public ObservableCollection<Inline> GenerateLastMessageInlines(Message lastMessage)
    {
        var inlines = new ObservableCollection<Inline>();

        var participiants = ParticipantUserViewModels.Select(x => x.User);
        var sender = participiants.FirstOrDefault(x => x.Id == lastMessage.SenderId)
            ?? throw new ArgumentException($"Sender with ID {lastMessage.SenderId} not found in chat participants.");

        inlines.Add(new Run
        {
            Text = $"{sender.FullName}: ",
            FontWeight = FontWeights.Bold,
        });

        foreach (var content in lastMessage.Contents)
        {
            if (content is TextMessageContent text) inlines.Add(new Run { Text = text.Text });
            else if (content is MentionMessageContent mention)
            {
                var mentionedUser = participiants.FirstOrDefault(x => x.Id == mention.UserId)
                    ?? throw new ArgumentException($"Mentioned user with ID {mention.UserId} not found in chat participants.");

                inlines.Add(new Run
                {
                    Text = $"@{mentionedUser.FullName}",
                    FontWeight = FontWeights.Bold,
                });
            }
        }

        return inlines;
    }
}
