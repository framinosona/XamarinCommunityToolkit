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
}