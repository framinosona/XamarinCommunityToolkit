using Xamarin.Forms;
using XamarinCommunityToolkitSample.Pages;

namespace XamarinCommunityToolkitSample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            //MainPage = new NavigationPage(new WelcomePage());

            MainPage = new ContentPage
            {
                BackgroundColor = Color.Black,
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    Children = {
                        new RangeSelectorView
                        {
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            HeightRequest = 30
                        }
                    }
                }
            };
        }
    }

    public class RangeSelectorView : TemplatedView
    {
        public RangeSelectorView()
            => ControlTemplate = new ControlTemplate(typeof(RangeSelectorLayout));

        RangeSelectorLayout Content { get; set; }

        class RangeSelectorLayout : AbsoluteLayout
        {
            Frame Track { get; } = new Frame
            {
                Padding = 0,
                HasShadow = false,
                BackgroundColor = Color.LightGray.MultiplyAlpha(.5)
            };

            Frame SelectedTrack { get; } = new Frame
            {
                Padding = 0,
                HasShadow = false,
                BackgroundColor = Color.LightSkyBlue
            };

            Frame MinThumb { get; } = new Frame
            {
                Padding = 0,
                BackgroundColor = Color.White,
            };

            Frame MaxThumb { get; } = new Frame
            {
                Padding = 0,
                BackgroundColor = Color.White,
            };

            public RangeSelectorLayout()
            {
                SetLayoutFlags(Track, AbsoluteLayoutFlags.YProportional | AbsoluteLayoutFlags.WidthProportional);
                SetLayoutFlags(SelectedTrack, AbsoluteLayoutFlags.YProportional | AbsoluteLayoutFlags.WidthProportional);
                SetLayoutFlags(MinThumb, AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.HeightProportional);
                SetLayoutFlags(MaxThumb, AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.HeightProportional);

                Children.Add(Track);
                Children.Add(SelectedTrack);
                Children.Add(MinThumb);
                Children.Add(MaxThumb);
                Update();
            }

            protected override void OnParentSet()
            {
                base.OnParentSet();
                ((RangeSelectorView)Parent).Content = this;
            }

            protected override void OnSizeAllocated(double width, double height)
            {
                base.OnSizeAllocated(width, height);
                Update();
            }

            // min & max values

            void Update()
            {
                if (Height < 0 || Width < 0)
                    return;

                var trackHeight = Height / 15;
                var trackRadius = (float)trackHeight / 2;
                var thumbRadius = (float)Height / 2;
                Track.CornerRadius = trackRadius;
                SelectedTrack.CornerRadius = trackRadius;
                MinThumb.CornerRadius = thumbRadius;
                MaxThumb.CornerRadius = thumbRadius;
                SetLayoutBounds(Track, new Rectangle(0, .5, 1, trackHeight));
                SetLayoutBounds(SelectedTrack, new Rectangle(0, .5, .5, trackHeight));
                SetLayoutBounds(MinThumb, new Rectangle(0, 0, Height, 1));
                SetLayoutBounds(MaxThumb, new Rectangle(.5, 0, Height, 1));
            }
        }
    }
}