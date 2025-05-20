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
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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

    public ObservableCollection<Inline> Inlines { get; private set; } = [];
    public ObservableCollection<string> AttachmentUris { get; private set; } = [];

    public Visibility ProfileImageVisibility { get; private set; } = Visibility.Visible;

    public string ProfileImageUrl => Sender.ProfileImageUrl;
    public HorizontalAlignment MessageHorizontalAlignment => IsMe ? HorizontalAlignment.Right : HorizontalAlignment.Left;
    public Visibility NameVisibility => IsMe ? Visibility.Collapsed : Visibility.Visible;

    private readonly IList<MessageViewModel> _parent;
    private readonly IList<UserViewModel> _participiantUserViewModels;
    public bool IsMe { get; }

    public MessageViewModel(Message message, IList<MessageViewModel> parent, IList<UserViewModel> participiantUserViewModels)
    {
        Message = message;
        _parent = parent;
        _participiantUserViewModels = participiantUserViewModels;
        IsMe = message.SenderId == App.Me.Id;

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

        if (!IsMe)
        {
            MessageViewModel previousViewModel;
            var doesParentContainThisViewModel = _parent.Any(x => x == this);
            // This view model is already in the parent list, means it was created before and now we're just updating it
            if (doesParentContainThisViewModel)
            {
                var thisIndex = _parent.IndexOf(this);
                previousViewModel = thisIndex > 0 ? _parent[thisIndex - 1] : null;
            }
            // We're not in the parent list, means this view model just got created will be added to the parent list
            else previousViewModel = _parent.LastOrDefault();

            // If previous view model's sender is the same as this view model's sender, hide the previous view model's profile image
            if (previousViewModel != null && previousViewModel.Message.SenderId == Message.SenderId) previousViewModel.ProfileImageVisibility = Visibility.Collapsed;
            else if (previousViewModel != null && !previousViewModel.IsMe) previousViewModel.ProfileImageVisibility = Visibility.Visible;
        }
        else ProfileImageVisibility = Visibility.Collapsed; // Hide profile image for sender

        Inlines.Clear();
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
    }
}
