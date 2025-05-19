using BMW_20250523.Model;
using BMW_20250523.Service;
using BMW_20250523.Service.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BMW_20250523;

public partial class App : Application
{
    private Window _window;
    private static readonly ServiceCollection s_services = new();
    private static IServiceProvider s_serviceProvider;

    public static IServiceProvider ServiceProvider => s_serviceProvider;
    public static User Me { get; set; }

    public App()
    {
        InitializeComponent();

        s_services.AddSingleton<IAttachmentService, AttachmentService>();
        s_services.AddSingleton<IChatService, ChatService>();
        s_services.AddSingleton<IMessageService, MessageService>();
        s_services.AddSingleton<IUserService, UserService>();
        s_serviceProvider = s_services.BuildServiceProvider();

        var userService = s_serviceProvider.GetRequiredService<IUserService>();
        userService.GenerateSampleUsers(30);

        var chatService = s_serviceProvider.GetRequiredService<IChatService>();
        chatService.GenerateSampleChats(50);
    }


    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _window.Activate();
    }
}
