using static ShowTrackerMaui.TMDB;

namespace ShowTrackerMaui;

public partial class MainPage : ContentPage
{
	int count = 0;
	string outputText;

	public MainPage()
	{
		InitializeComponent();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			outputText = $"Clicked {count} time";
		else
            outputText = $"Clicked {count} times";

		//RegisterSettings();

		EpisodeInfo episodeInfo = GetInfoTwo("ms marvel").Result;
		DisplayAlert("This is a test", "test", "Test");
		



		/*CounterBtn.Text = outputText;
		LabelChanger.Text = count.ToString();


		SemanticScreenReader.Announce(CounterBtn.Text);*/
	}

	private void ClickedEvent(object sender, EventArgs e)
	{
		RegisterSettings();
		DisplayAlert("Success", "Yay", "OK");
	}
}

