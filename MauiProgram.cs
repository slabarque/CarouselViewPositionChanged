using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace CarouselViewPositionChanged;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
        builder.Logging.AddDebug().SetMinimumLevel(LogLevel.Trace);
        builder.Logging.AddConsole().SetMinimumLevel(LogLevel.Trace);
#endif

        return builder.Build();
	}
}
