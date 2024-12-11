using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Indiko.Maui.Controls.Chat.Sample.Messages;
internal class HideKeyboardMessage : ValueChangedMessage<DateTime>
{
    public HideKeyboardMessage() : base(DateTime.UtcNow)
    {
    }
}
