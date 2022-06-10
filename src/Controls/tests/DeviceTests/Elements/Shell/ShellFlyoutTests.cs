#if !IOS
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Xunit;

#if ANDROID || IOS
using ShellHandler = Microsoft.Maui.Controls.Handlers.Compatibility.ShellRenderer;
#endif

namespace Microsoft.Maui.DeviceTests
{
	public partial class ShellTests : HandlerTestBase
	{
		[Fact]
		public async Task FlyoutHeaderContentAndFooterAllMeasureCorrectly()
		{
			await RunShellTest(shell =>
			{
				shell.FlyoutHeader = new Label() { Text = "Flyout Header" };
				shell.FlyoutFooter = new Label() { Text = "Flyout Footer" };
				shell.FlyoutContent = new VerticalStackLayout() { new Label() { Text = "Flyout Content" } };
			},
			async (shell, handler) =>
			{
				await OpenFlyout(handler);

				var flyoutFrame = GetFlyoutFrame(handler);
				var headerFrame = GetFrameRelativeToFlyout(handler, (IView)shell.FlyoutHeader);
				var contentFrame = GetFrameRelativeToFlyout(handler, (IView)shell.FlyoutContent);
				var footerFrame = GetFrameRelativeToFlyout(handler, (IView)shell.FlyoutFooter);

				// validate header position
				Assert.Equal(0, headerFrame.X);
				Assert.Equal(0, headerFrame.Y);
				Assert.Equal(headerFrame.Width, flyoutFrame.Width);

				// validate content position
				Assert.Equal(0, contentFrame.X);
				Assert.Equal(headerFrame.Height, contentFrame.Y);
				Assert.Equal(contentFrame.Width, flyoutFrame.Width);

				// validate footer position
				Assert.Equal(0, contentFrame.X);
				Assert.Equal(headerFrame.Height + contentFrame.Height, footerFrame.Y);
				Assert.Equal(footerFrame.Width, flyoutFrame.Width);

				//All three views should measure to the height of the flyout
				Assert.Equal(headerFrame.Height + contentFrame.Height + footerFrame.Height, flyoutFrame.Height);
			});
		}

		[Fact]
		public async Task FlyoutHeaderCollapsesOnScroll()
		{
			await RunShellTest(shell =>
			{
				Enumerable.Range(0, 100)
					.ForEach(i =>
					{
						shell.FlyoutHeaderBehavior = FlyoutHeaderBehavior.CollapseOnScroll;
						shell.Items.Add(new FlyoutItem() { Title = $"FlyoutItem {i}", Items = { new ContentPage() } });
					});

				var layout = new VerticalStackLayout()
				{
					new Label()
					{
						Text = "Header Content"
					}
				};

				layout.HeightRequest = 200;

				shell.FlyoutHeader = new ScrollView()
				{
					MinimumHeightRequest = 100,
					Content = layout
				};
			},
			async (shell, handler) =>
			{
				await OpenFlyout(handler);
				await Task.Delay(200);

				var initialBox = (shell.FlyoutHeader as IView).GetBoundingBox();
				Assert.Equal(200, initialBox.Height);
				await ScrollFlyoutToBottom(handler);

				var scrolledBox = (shell.FlyoutHeader as IView).GetBoundingBox();
				Assert.Equal(100, scrolledBox.Height);
			});
		}

		async Task RunShellTest(Action<Shell> action, Func<Shell, ShellHandler, Task> testAction)
		{
			SetupBuilder();
			var shell = await CreateShellAsync((shell) =>
			{
				action(shell);
				if (shell.Items.Count == 0)
					shell.CurrentItem = new FlyoutItem() { Items = { new ContentPage() } };
			});

			await CreateHandlerAndAddToWindow<ShellHandler>(shell, async (handler) =>
			{
				await OnNavigatedToAsync(shell.CurrentPage);
				await testAction(shell, handler);
			});
		}
	}
}
#endif