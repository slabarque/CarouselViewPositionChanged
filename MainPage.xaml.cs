using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CarouselViewPositionChanged;

public class MyModel
{
    public DateOnly Date { get; set; }
    public int DayOfYear { get; set; }
}

public partial class MainPage : ContentPage
{
    private static int DAYS_TO_SHOW = 60;

    private MyModel selectedDate;
    private readonly ILogger<MainPage> _logger;

    public MainPage(ILogger<MainPage> logger)
    {
        _logger = logger;
        InitializeComponent();
        Init = new Command(() =>
        {
            // In my real app, this command loads data asynchronously from a database
            _logger.LogDebug("Initializing");
            SelectedDate = null;
            Dates.Clear();
            var today = DateOnly.FromDateTime(DateTime.Today);
            var result = Enumerable.Range(-30, DAYS_TO_SHOW).Select(i => today.AddDays(i)).ToArray();
            MyModel? selected = null;
            foreach (var date in result)
            {
                Dates.Add(new MyModel { Date = date, DayOfYear = date.DayOfYear });
                if (date == today)
                {
                    selected = Dates.Last();
                }
            }
            SelectedDate = selected!;
        });
        BindingContext = this;
    }

    public MyModel SelectedDate
    {
        set { SetProperty(ref selectedDate, value); }
        get { return selectedDate; }
    }

    public ObservableCollection<MyModel> Dates { get; } = new();

    public ICommand Init { get; private set; }

    bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
        if (Object.Equals(storage, value))
            return false;

        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private async void CarouselView_PositionChanged(object sender, PositionChangedEventArgs e)
    {
        try
        {
            // Center the active indicator in the indicatorScrollView
            var indicatorWidth = indicatorScrollView.ContentSize.Width / DAYS_TO_SHOW;
            _logger.LogTrace("{indicatorWidth} = {ContentSizeWidth} / {DaysToShow}",
                indicatorWidth, indicatorScrollView.ContentSize.Width, DAYS_TO_SHOW);
            var xStart = indicatorWidth * e.CurrentPosition;
            _logger.LogTrace("xStart:{xStart}={indicatorWidth} * {CurrentPosition}",
                xStart, indicatorWidth, e.CurrentPosition);
            var x = xStart < Window.Width / 2 ? 0 : xStart - Window.Width / 2 + indicatorWidth / 2;
            _logger.LogTrace("x:{x}={xStart} < {WindowWidth}/2 ? 0 : {xStart} - {WindowWidth}/2 + {indicatorWidth}/2",
                x, xStart, Window.Width, xStart, Window.Width, indicatorWidth);
            await indicatorScrollView.ScrollToAsync(x, indicatorScrollView.ScrollY, true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while scrolling the indicatorScrollView to match the CarouselView position");
        }
    }
}

