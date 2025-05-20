using BMW_20250523.Model;
using BMW_20250523.Service.Interface;
using BMW_20250523.ViewModel;
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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.WebUI;

namespace BMW_20250523.View;

public sealed partial class MainPage : Page
{
    private readonly ObservableCollection<ChatViewModel> _chatViewModels = [];

    public MainPage()
    {
        InitializeComponent();

        var chatService = App.ServiceProvider.GetRequiredService<IChatService>();
        var chats = chatService.GetAllChats();

        var chatViewModels = chats.Select(chat => new ChatViewModel(chat)).ToList();
        foreach (var chatViewModel in chatViewModels) _chatViewModels.Add(chatViewModel);

        ChatListView.ItemsSource = _chatViewModels;
    }

    private void OnGridSplitterPointerPressed(object sender, PointerRoutedEventArgs e) => (sender as UIElement).CapturePointer(e.Pointer);
    private void OnGridSplitterPointerReleased(object sender, PointerRoutedEventArgs e) => (sender as UIElement).ReleasePointerCapture(e.Pointer);

    private void OnChatListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var listView = sender as ListView;
        if (listView == null) return;

        var viewModel = listView.SelectedItem as ChatViewModel;
        ContentFrame.Navigate(typeof(ChatPage), viewModel.Chat);
    }
}
