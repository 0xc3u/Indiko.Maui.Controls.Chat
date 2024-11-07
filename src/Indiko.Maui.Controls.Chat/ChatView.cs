using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Indiko.Maui.Controls.Chat.Models;

namespace Indiko.Maui.Controls.Chat;
public class ChatView : View
{
    public event EventHandler MessagesUpdatedEvent;

    public event EventHandler LoadMoreMessagesRequested;

    public void LoadMoreMessages()
    {
        // Notify that older messages should be loaded
        LoadMoreMessagesRequested?.Invoke(this, EventArgs.Empty);
    }

    // Method to add older messages to the collection
    public void AddOlderMessages(IEnumerable<ChatMessage> olderMessages)
    {
        foreach (var message in olderMessages)
        {
            Messages.Insert(0, message);
        }
    }


    public static readonly BindableProperty MessagesProperty = BindableProperty.Create(nameof(Messages), typeof(ObservableCollection<ChatMessage>), typeof(ChatView), default(ObservableCollection<ChatMessage>),
        propertyChanged: OnMessagesChanged);

    public ObservableCollection<ChatMessage> Messages
    {
        get => (ObservableCollection<ChatMessage>)GetValue(MessagesProperty);
        set => SetValue(MessagesProperty, value);
    }


    public static readonly BindableProperty OwnMessageBackgroundColorProperty = BindableProperty.Create(nameof(OwnMessageBackgroundColor), typeof(Color), typeof(ChatView), Colors.LightBlue);
    public Color OwnMessageBackgroundColor
    {
        get => (Color)GetValue(OwnMessageBackgroundColorProperty);
        set => SetValue(OwnMessageBackgroundColorProperty, value);
    }

    public static readonly BindableProperty OtherMessageBackgroundColorProperty = BindableProperty.Create(nameof(OtherMessageBackgroundColor), typeof(Color), typeof(ChatView), Colors.LightGray);
    public Color OtherMessageBackgroundColor
    {
        get => (Color)GetValue(OtherMessageBackgroundColorProperty);
        set => SetValue(OtherMessageBackgroundColorProperty, value);
    }

    public static readonly BindableProperty OwnMessageTextColorProperty = BindableProperty.Create(nameof(OwnMessageTextColor), typeof(Color), typeof(ChatView), Colors.Black);
    public Color OwnMessageTextColor
    {
        get => (Color)GetValue(OwnMessageTextColorProperty);
        set => SetValue(OwnMessageTextColorProperty, value);
    }

    public static readonly BindableProperty OtherMessageTextColorProperty = BindableProperty.Create(nameof(OtherMessageTextColor), typeof(Color), typeof(ChatView), Colors.Black);
    public Color OtherMessageTextColor
    {
        get => (Color)GetValue(OtherMessageTextColorProperty);
        set => SetValue(OtherMessageTextColorProperty, value);
    }


    public static readonly BindableProperty MessageFontSizeProperty = BindableProperty.Create(nameof(MessageFontSize), typeof(float), typeof(ChatView), 14f);
    public float MessageFontSize
    {
        get => (float)GetValue(MessageFontSizeProperty);
        set => SetValue(MessageFontSizeProperty, value);
    }

    public static readonly BindableProperty DateTextFontSizeProperty = BindableProperty.Create(nameof(DateTextFontSize), typeof(float), typeof(ChatView), 14f);
    public float DateTextFontSize
    {
        get => (float)GetValue(DateTextFontSizeProperty);
        set => SetValue(DateTextFontSizeProperty, value);
    }

    public static readonly BindableProperty DateTextColorProperty = BindableProperty.Create(nameof(DateTextColor), typeof(Color), typeof(ChatView), Colors.LightGray);
    public Color DateTextColor
    {
        get => (Color)GetValue(DateTextColorProperty);
        set => SetValue(DateTextColorProperty, value);
    }

    public static readonly BindableProperty MessageTimeFontSizeProperty = BindableProperty.Create(nameof(MessageTimeFontSize), typeof(float), typeof(ChatView), 12f);
    public float MessageTimeFontSize
    {
        get => (float)GetValue(MessageTimeFontSizeProperty);
        set => SetValue(MessageTimeFontSizeProperty, value);
    }

    public static readonly BindableProperty MessageTimeTextColorProperty = BindableProperty.Create(nameof(MessageTimeTextColor), typeof(Color), typeof(ChatView), Colors.LightGray);
    public Color MessageTimeTextColor
    {
        get => (Color)GetValue(MessageTimeTextColorProperty);
        set => SetValue(MessageTimeTextColorProperty, value);
    }


    public static readonly BindableProperty NewMessagesSeperatorTextProperty = BindableProperty.Create(nameof(NewMessagesSeperatorText), typeof(string), typeof(ChatView), "New Messages");
    public string NewMessagesSeperatorText
    {
        get => (string)GetValue(NewMessagesSeperatorTextProperty);
        set => SetValue(NewMessagesSeperatorTextProperty, value);
    }

    public static readonly BindableProperty NewMessagesSeperatorFontSizeProperty = BindableProperty.Create(nameof(NewMessagesSeperatorFontSize), typeof(float), typeof(ChatView), 14f);
    public float NewMessagesSeperatorFontSize
    {
        get => (float)GetValue(NewMessagesSeperatorFontSizeProperty);
        set => SetValue(NewMessagesSeperatorFontSizeProperty, value);
    }


    public static readonly BindableProperty NewMessagesSeperatorTextColorProperty = BindableProperty.Create(nameof(NewMessagesSeperatorTextColor), typeof(Color), typeof(ChatView), Colors.LightGray);
    public Color NewMessagesSeperatorTextColor
    {
        get => (Color)GetValue(NewMessagesSeperatorTextColorProperty);
        set => SetValue(NewMessagesSeperatorTextColorProperty, value);
    }

    public static readonly BindableProperty AvatarSizeProperty = BindableProperty.Create(nameof(AvatarSize), typeof(float), typeof(ChatView), 36f);
    public float AvatarSize
    {
        get => (float)GetValue(AvatarSizeProperty);
        set => SetValue(AvatarSizeProperty, value);
    }


    public static readonly BindableProperty ScrollToFirstNewMessageProperty = BindableProperty.Create(nameof(ScrollToFirstNewMessage), typeof(bool), typeof(ChatView), true);
    public bool ScrollToFirstNewMessage
    {
        get => (bool)GetValue(ScrollToFirstNewMessageProperty);
        set => SetValue(ScrollToFirstNewMessageProperty, value);
    }

    private static void OnMessagesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var chatView = (ChatView)bindable;
        chatView.MessagesUpdated();
    }

    private void MessagesUpdated()
    {
        MessagesUpdatedEvent?.Invoke(this, EventArgs.Empty);
    }
}