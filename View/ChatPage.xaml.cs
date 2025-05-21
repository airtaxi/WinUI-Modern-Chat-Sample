using BMW_20250523.Model;
using BMW_20250523.Model.MessageContent;
using BMW_20250523.Model.Mvvm;
using BMW_20250523.Service.Interface;
using BMW_20250523.ViewModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.WebUI;

namespace BMW_20250523.View;

public sealed partial class ChatPage : Page
{
    private ChatViewModel _viewModel;
    public ChatPage()
    {
        InitializeComponent();
    }

    private void Send()
    {
        InputRichSuggestBox.ClearUndoRedoSuggestionHistory();
        InputRichSuggestBox.TextDocument.GetText(Microsoft.UI.Text.TextGetOptions.None, out var rawMessage);

        var tokens = InputRichSuggestBox.Tokens;
        var viewModels = tokens.Select(x => x.Item as UserViewModel).ToList();

        var contents = MessageParser.Parse(rawMessage, viewModels);
        if (contents.Count == 0) return;
        if (contents.LastOrDefault() is TextMessageContent lastText) lastText.Text = lastText.Text.TrimEnd();
        if (contents.FirstOrDefault() is TextMessageContent firstText) firstText.Text = firstText.Text.TrimStart();

        var messageService = App.ServiceProvider.GetRequiredService<IMessageService>();
        var message = messageService.AddMessage(_viewModel.Chat.Id, App.Me.Id, contents);
        WeakReferenceMessenger.Default.Send(new NewMessageCreatedMessage(message));

        InputRichSuggestBox.Clear();

        ChatScrollViewer.ChangeView(0, ChatScrollViewer.ScrollableHeight, 1);
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        var chat = e.Parameter as Chat;
        var viewModel = new ChatViewModel(chat);
        _viewModel = viewModel;
        MessageItemsRepeater.ItemsSource = viewModel.MessageViewModels;

        MessageItemsRepeater.UpdateLayout();
        ChatScrollViewer.UpdateLayout();
        ChatScrollViewer.ScrollToVerticalOffset(ChatScrollViewer.ScrollableHeight);
        // BUG: ScrollViewer doesn't scroll to the bottom completely when the page is first loaded.
        await Task.Delay(200);
        MessageItemsRepeater.UpdateLayout();
        ChatScrollViewer.UpdateLayout();
        ChatScrollViewer.ScrollToVerticalOffset(ChatScrollViewer.ScrollableHeight);
    }

    private void OnInputRichSuggestBoxSuggestionChosen(RichSuggestBox sender, SuggestionChosenEventArgs args)
    {
        var selectedItem = args.SelectedItem as UserViewModel;
        args.DisplayText = selectedItem.User.FullName;
    }

    private void OnInputRichSuggestBoxSuggestionRequested(RichSuggestBox sender, SuggestionRequestedEventArgs args)
    {
        var viewModels = _viewModel.ParticipantUserViewModels;
        var meViewModel = viewModels.FirstOrDefault(x => x.User.Id == App.Me.Id);

        var exceptions = new List<UserViewModel>();
        if (meViewModel != null) exceptions.Add(meViewModel);

        var tokens = sender.Tokens;
        foreach (var token in tokens)
        {
            if (token.Item is UserViewModel viewModel && viewModels.Contains(viewModel))
            {
                exceptions.Add(viewModel);
            }
        }

        var candidates = _viewModel.ParticipantUserViewModels.Except(exceptions);
        var filteredCandidates = candidates.Where(x => x.User.FullName.Contains(args.QueryText, StringComparison.OrdinalIgnoreCase)).ToList();
        sender.ItemsSource = filteredCandidates;
    }

    private void OnSendButtonClicked(object sender, RoutedEventArgs e) => Send();

    private void OnInputRichSuggestBoxPreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        var element = sender as UIElement;
        if (e.Key == VirtualKey.Enter)
        {
            var keyboardAccelerators = element.KeyboardAccelerators;
            if (!IsModifierDown(VirtualKey.Control))
            {
                e.Handled = true;
                Send();
            }
        }
    }

    private static bool IsModifierDown(VirtualKey virtualKey)
    {
        var state = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(virtualKey);
        var isDown = state == (CoreVirtualKeyStates.Down | CoreVirtualKeyStates.Locked);
        isDown = isDown || state == CoreVirtualKeyStates.Down;
        return isDown;
    }
}
