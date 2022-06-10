using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;

namespace Maui.Controls.Sample
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		void Label_HandlerChanged(object sender, EventArgs e)
		{

		}

		async void Label_Loaded(object sender, EventArgs e)
		{
			await Task.Delay(1000);

#if ANDROID
			if (limited.Handler is IPlatformViewHandler handler)
			{
				var bounds = handler.PlatformView.GetPlatformViewBounds();

			}
#endif
		}
	}
}