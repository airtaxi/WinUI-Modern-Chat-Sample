using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace BMW_20250523;

public static class TextBlockHelper
{
    public static readonly DependencyProperty BindableInlinesProperty =
        DependencyProperty.RegisterAttached(
            "BindableInlines",
            typeof(IEnumerable<Inline>),
            typeof(TextBlockHelper),
            new PropertyMetadata(null, OnBindableInlinesChanged));

    public static void SetBindableInlines(UIElement element, IEnumerable<Inline> value) =>
        element.SetValue(BindableInlinesProperty, value);

    public static IEnumerable<Inline> GetBindableInlines(UIElement element) =>
        (IEnumerable<Inline>)element.GetValue(BindableInlinesProperty);

    private static void OnBindableInlinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextBlock textBlock)
        {
            textBlock.Inlines.Clear();
            var inlines = e.NewValue as IEnumerable<Inline>;
            if (inlines == null) return;

            foreach (var inline in inlines)
            {
                var clonedInline = CloneInline(inline);
                textBlock.Inlines.Add(clonedInline);
            }
        }
    }

    private static Inline CloneInline(Inline inline)
    {
        switch (inline)
        {
            case Run run:
                return new Run
                {
                    Text = run.Text,
                    FontWeight = run.FontWeight,
                    FontStyle = run.FontStyle,
                    Foreground = run.Foreground
                };
            case Bold bold:
                var newBold = new Bold();
                foreach (var child in bold.Inlines)
                {
                    newBold.Inlines.Add(CloneInline(child));
                }
                return newBold;
            case Italic italic:
                var newItalic = new Italic();
                foreach (var child in italic.Inlines)
                {
                    newItalic.Inlines.Add(CloneInline(child));
                }
                return newItalic;
            case Underline underline:
                var newUnderline = new Underline();
                foreach (var child in underline.Inlines)
                {
                    newUnderline.Inlines.Add(CloneInline(child));
                }
                return newUnderline;
            case Hyperlink hyperlink:
                var newHyperlink = new Hyperlink
                {
                    NavigateUri = hyperlink.NavigateUri,
                    Foreground = hyperlink.Foreground,
                    FontWeight = hyperlink.FontWeight,
                    FontStyle = hyperlink.FontStyle
                };
                foreach (var child in hyperlink.Inlines)
                {
                    newHyperlink.Inlines.Add(CloneInline(child));
                }
                return newHyperlink;
            default:
                throw new NotSupportedException($"Inline type {inline.GetType()} is not supported.");
        }
    }
}
