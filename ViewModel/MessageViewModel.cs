using BMW_20250523.Model;
using BMW_20250523.Model.MessageContent;
using BMW_20250523.Service.Interface;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Text;

namespace BMW_20250523.ViewModel;

public class MessageViewModel
{
    public Message Message { get; private set; }

    public bool IsReply { get; private set; }
    public bool IsAttachmentAvailable { get; private set; }

    public UserViewModel Sender { get; private set; }
    public MessageViewModel ParentViewModel { get; private set; }

    private InlineCollection _inlineCollection;
    public ObservableCollection<Inline> Inlines { get; private set; } = [];
    public ObservableCollection<string> AttachmentUris { get; private set; } = [];

    private readonly IEnumerable<MessageViewModel> _parent;
    private readonly IEnumerable<UserViewModel> _participiantUserViewModels;

    public MessageViewModel(Message message, IEnumerable<MessageViewModel> parent, IEnumerable<UserViewModel> participiantUserViewModels)
    {
        Message = message;
        _parent = parent;
        _participiantUserViewModels = participiantUserViewModels;

        ApplyMessage();

        WeakReferenceMessenger.Default.Register<ValueChangedMessage<Message>>(this, OnMessageChangedMessageReceived);
    }

    private void OnMessageChangedMessageReceived(object recipient, ValueChangedMessage<Message> message)
    {
        if(message.Value.Id != Message.Id) return;

        Message = message.Value;
        ApplyMessage();
    }

    private void ApplyMessage()
    {
        Sender = _participiantUserViewModels.FirstOrDefault(x => x.User.Id == Message.SenderId)
                    ?? throw new ArgumentException($"Sender with ID {Message.SenderId} not found in the provided list of view models.");

        if (Message.AttachmentIds.Count > 0)
        {
            IsAttachmentAvailable = true;

            var attachmentService = App.ServiceProvider.GetService<IAttachmentService>();
            var attachment = attachmentService.GetAttachmentsByIds(Message.AttachmentIds);

            foreach (var attachmentItem in attachment) AttachmentUris.Add(attachmentItem.Uri);
        }

        if (Message is Reply reply)
        {
            IsReply = true;

            ParentViewModel = _parent.FirstOrDefault(x => x.Message.Id == reply.ParentMessageId)
                ?? throw new ArgumentException($"Parent message with ID {reply.ParentMessageId} not found in the provided list of view models.");
        }

        foreach (var content in Message.Contents)
        {
            if (content is TextMessageContent text) Inlines.Add(new Run { Text = text.Text });
            else if (content is MentionMessageContent mention)
            {
                var mentionedUser = _participiantUserViewModels.FirstOrDefault(x => x.User.Id == mention.UserId)
                    ?? throw new ArgumentException($"Mentioned user with ID {mention.UserId} not found in the provided list of view models.");

                var hyperlink = new Hyperlink();
                var run = new Run
                {
                    Text = $"@{mentionedUser.FullName}",
                    TextDecorations = TextDecorations.Underline,
                    FontWeight = FontWeights.Bold,
                    Foreground = Application.Current.Resources["AccentFillColorDefaultBrush"] as SolidColorBrush,
                };
                hyperlink.Inlines.Add(run);
                Inlines.Add(hyperlink);
            }
        }

        if (Message.ModifiedAt != null)
        {
            Inlines.Add(new Run()
            {
                Text = " (수정됨)",
                Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x80, 0x80, 0x80)),
                FontSize = 12,
            });
        }

        ApplyInlines();
    }

    public void OnTextBlockLoaded(object sender, RoutedEventArgs e)
    {
        _inlineCollection = (sender as TextBlock).Inlines;
        ApplyInlines();
    }

    private void ApplyInlines()
    {
        if (_inlineCollection == null) return;

        _inlineCollection.Clear();
        foreach (var inline in Inlines) _inlineCollection.Add(inline);
    }
}
