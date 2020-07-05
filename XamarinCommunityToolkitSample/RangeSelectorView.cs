using System.Collections.Generic;
using Xamarin.Forms;
using static System.Math;

namespace XamarinCommunityToolkitSample
{
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

            ContentView MinThumb { get; } = new ContentView
            {
                Content = new Frame
                {
                    Padding = 0,
                    HasShadow = false,
                    BackgroundColor = Color.White,
                }
            };

            ContentView MaxThumb { get; } = new ContentView
            {
                Content = new Frame
                {
                    Padding = 0,
                    HasShadow = false,
                    BackgroundColor = Color.White,
                }
            };

            public RangeSelectorLayout()
            {
                AddGesture(MinThumb);
                AddGesture(MaxThumb);

                AddChild(Track);
                AddChild(SelectedTrack);
                AddChild(MinThumb);
                AddChild(MaxThumb);

                Track.SizeChanged += OnViewSizeChanged;
                MinThumb.SizeChanged += OnViewSizeChanged;
                MaxThumb.SizeChanged += OnViewSizeChanged;
            }

            void OnViewSizeChanged(object sender, System.EventArgs e)
                => UpdateLayot();

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
                ((Frame)MinThumb.Content).CornerRadius = thumbRadius;
                ((Frame)MaxThumb.Content).CornerRadius = thumbRadius;

                var trackSize = width - MinThumb.Width - MaxThumb.Width;
                var minValue = MinValue * trackSize;
                var maxValue = MaxValue * trackSize + MinThumb.Width;

                MinThumb.TranslationX = minValue;
                MaxThumb.TranslationX = maxValue;

                SetLayoutBounds(Track, new Rectangle(0, .5, width, trackHeight));
                SetLayoutBounds(SelectedTrack, new Rectangle(minValue, .5, maxValue - minValue + MaxThumb.Width, trackHeight));
                SetLayoutBounds(MinThumb, new Rectangle(0, .5, thumbSize, thumbSize));
                SetLayoutBounds(MaxThumb, new Rectangle(0, .5, thumbSize, thumbSize));
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
                Children.Add(view);
                SetLayoutFlags(view, AbsoluteLayoutFlags.YProportional);
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
                => thumbPositionMap[view] = view.TranslationX;

            void OnPanRunning(View view, double value)
                => SetValue(view, value + thumbPositionMap[view]);

            void OnPanCompleted(View view)
                => thumbPositionMap[view] = view.TranslationX;

            void OnPanCanceled(View view)
                => SetValue(view, thumbPositionMap[view]);

            void SetValue(View view, double position)
            {
                var trackSize = Width - MinThumb.Width - MaxThumb.Width;
                if (view == MinThumb)
                {
                    MinValue = Min(Max(0, position / trackSize), MaxValue);
                }
                else
                {
                    MaxValue = Min(Max(MinValue, (position - MinThumb.Width) / trackSize), 1);
                }

                UpdateLayot();
            }
        }
    }
}
