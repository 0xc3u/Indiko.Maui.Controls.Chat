using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Handlers;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;
public class ChatViewHandler : ViewHandler<ChatView, UIView>
{
    public ChatViewHandler(IPropertyMapper mapper, CommandMapper? commandMapper = null) : base(mapper, commandMapper)
    {
    }

    protected override UIView CreatePlatformView()
    {
        var view = new UIView
        {
            BackgroundColor = UIColor.White // Or customize as needed
        };

        // Add your custom chat bubbles or other UI elements
        return view;
    }

    protected override void ConnectHandler(UIView platformView)
    {
        base.ConnectHandler(platformView);
        // Bind your Messages property and update UI as needed
    }

    protected override void DisconnectHandler(UIView platformView)
    {
        base.DisconnectHandler(platformView);
        // Clean up any resources or listeners
    }
}
