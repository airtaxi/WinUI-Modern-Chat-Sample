using System;
using BMW_20250523.View;
using WinUIEx;

namespace BMW_20250523;

public sealed partial class MainWindow : WindowEx
{
    public MainWindow()
    {
        InitializeComponent();

        AppWindow.SetIcon("logo.ico");

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(MainTitleBar);

        this.CenterOnScreen();

        ContentFrame.Navigate(typeof(MainPage));
    }
}
