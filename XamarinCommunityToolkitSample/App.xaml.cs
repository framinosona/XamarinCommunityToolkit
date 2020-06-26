using System.Collections.Generic;
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

        sealed class RangeSelectorLayout : AbsoluteLayout
        {
            readonly Dictionary<View, double> thumbPositionMap = new Dictionary<View, double>();

            double MinValue { get; set; } = 0;

            double MaxValue { get; set; } = .5;

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
                AddGesture(MinThumb);
                AddGesture(MaxThumb);

                AddChild(Track);
                AddChild(SelectedTrack);
                AddChild(MinThumb);
                AddChild(MaxThumb);

                UpdateLayot();
            }

            public void UpdateLayot()
            {
                var width = Width;
                var height = Height;
                if (width < 0 || height < 0)
                    return;

                var trackHeight = height / 10;
                var trackRadius = (float)trackHeight / 2;
                var thumbSize = height;
                var thumbRadius = (float)thumbSize / 2;

                Track.CornerRadius = trackRadius;
                SelectedTrack.CornerRadius = trackRadius;
                MinThumb.CornerRadius = thumbRadius;
                MaxThumb.CornerRadius = thumbRadius;

                var minValue = MinValue * width;
                var maxValue = MaxValue * width;

                SetLayoutBounds(Track, new Rectangle(0, .5, width, trackHeight));
                SetLayoutBounds(SelectedTrack, new Rectangle(minValue, .5, maxValue - minValue, trackHeight));
                SetLayoutBounds(MinThumb, new Rectangle(minValue, .5, thumbSize, thumbSize));
                SetLayoutBounds(MaxThumb, new Rectangle(maxValue, .5, thumbSize, thumbSize));
            }

            protected override void OnParentSet()
            {
                base.OnParentSet();
                ((RangeSelectorView)Parent).Content = this;
            }

            protected override void OnSizeAllocated(double width, double height)
            {
                base.OnSizeAllocated(width, height);
                UpdateLayot();
            }

            void AddChild(View view)
            {
                SetLayoutFlags(view, AbsoluteLayoutFlags.YProportional);
                Children.Add(view);
            }

            void AddGesture(View view)
            {
                var gesture = new PanGestureRecognizer();
                gesture.PanUpdated += OnPanUpdated;
                view.GestureRecognizers.Add(gesture);
            }

            void OnPanUpdated(object sender, PanUpdatedEventArgs e)
            {
                var view = (View)sender;
                switch (e.StatusType)
                {
                    case GestureStatus.Started:
                        OnPanStarted(view);
                        break;
                    case GestureStatus.Running:
                        OnPanRunning(view, e.TotalX);
                        break;
                    case GestureStatus.Completed:
                        OnPanCompleted(view);
                        break;
                    case GestureStatus.Canceled:
                        OnPanCanceled(view);
                        break;
                }
            }

            void OnPanStarted(View view)
                => thumbPositionMap[view] = GetLayoutBounds(view).X;

            void OnPanRunning(View view, double value)
            {
                SetValue(view, value + thumbPositionMap[view]);
                UpdateLayot();
            }

            void OnPanCompleted(View view)
            {
            }

            void OnPanCanceled(View view)
                => SetValue(view, thumbPositionMap[view]);

            void SetValue(View view, double value)
            {
                value /= Width;
                if (view == MinThumb)
                {
                    MinValue = value;
                    return;
                }
                MaxValue = value;
            }
        }
    }
}