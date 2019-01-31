﻿using Sdl.Community.ApplyTMTemplate.UI;
using Sdl.Community.ApplyTMTemplate.Utilities;
using Sdl.Community.ApplyTMTemplate.ViewModels;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.ApplyTMTemplate
{
	[Action("ApplyTMTemplateAction", Icon = "ATTA", Name = "Apply TM Template", Description = "Applies settings from a TM template to a TM")]
	[ActionLayout(typeof(ApplyTMTemplateRibbonGroup), 10, DisplayType.Large)]
	public class ApplyTMTemplateAction : AbstractAction
	{
		protected override void Execute()
		{
			var mainWindowViewModel = new MainWindowViewModel(new TemplateLoader(), new TMLoader());

			var mainWindow = new MainWindow
			{
				DataContext = mainWindowViewModel
			};

			mainWindow.Show();
		}
	}
}