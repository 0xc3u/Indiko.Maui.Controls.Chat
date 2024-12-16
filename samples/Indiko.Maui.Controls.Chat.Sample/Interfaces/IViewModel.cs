namespace Indiko.Maui.Controls.Chat.Sample.Interfaces;
interface IViewModel
{
	bool IsBusy { get; set; }
    Task OnAppearing(object param);
}
