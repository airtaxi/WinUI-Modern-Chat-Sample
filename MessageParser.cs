using BMW_20250523.Model.MessageContent;
using BMW_20250523.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BMW_20250523;

public static partial class MessageParser
{
    public static List<IMessageContent> Parse(string rawMessage, List<UserViewModel> users)
    {
        var results = new List<IMessageContent>();

        // Regex to find all HYPERLINK "@name" entries with zero-width spaces
        var regex = HyperlinkRegex();

        var matches = regex.Matches(rawMessage);
        int currentIndex = 0;
        int userIndex = 0;

        foreach (Match match in matches)
        {
            // Append preceding plain text if any
            if (match.Index > currentIndex)
            {
                var text = rawMessage[currentIndex..match.Index];
                if (!string.IsNullOrWhiteSpace(text))
                {
                    results.Add(new TextMessageContent { Text = text });
                }
            }

            // Map mention to User based on order
            if (userIndex < users.Count)
            {
                var userId = users[userIndex].User.Id;
                results.Add(new MentionMessageContent { UserId = userId });
                userIndex++;
            }

            currentIndex = match.Index + match.Length;
        }

        // Append remaining text if any
        if (currentIndex < rawMessage.Length)
        {
            var remaining = rawMessage[currentIndex..];
            if (!string.IsNullOrWhiteSpace(remaining))
            {
                results.Add(new TextMessageContent { Text = remaining });
            }
        }

        return results;
    }

    [GeneratedRegex(@"HYPERLINK ""[^""]+""\u200b@[^​]+\u200b")]
    private static partial Regex HyperlinkRegex();
}
