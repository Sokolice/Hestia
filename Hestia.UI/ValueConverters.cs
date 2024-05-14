using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Hestia.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Resources;

namespace Hestia.View
{
    public class CategoryToVisibilityLightsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((int)value == (int)DeviceCategory.Lights) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class CategoryToVisibilityBlindsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((int)value == (int)DeviceCategory.Blinds) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Na základě velikosti písma aplikace a zadaného typu text vrátí styl pro zobrazení ve View
    /// </summary>
    public class StyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch(parameter.ToString())
            {
                case "title":                    
                        if (GlobalContext.FontSize == FontSize.Small)
                            return App.Current.Resources["SmallTitleTextBlock"];
                        else if (GlobalContext.FontSize == FontSize.Large)
                            return App.Current.Resources["LargeTitleTextBlock"];
                        else return App.Current.Resources["MediumTitleTextBlock"];
                case "subtitle":
                    if (GlobalContext.FontSize == FontSize.Small)
                        return App.Current.Resources["SmallSubtitleTextBlock"];
                    else if (GlobalContext.FontSize == FontSize.Large)
                        return App.Current.Resources["LargeSubtitleTextBlock"];
                    else return App.Current.Resources["MediumSubtitleTextBlock"];
                case "subsubtitle":
                    if (GlobalContext.FontSize == FontSize.Small)
                        return App.Current.Resources["SmallSubSubtitleTextBlock"];
                    else if (GlobalContext.FontSize == FontSize.Large)
                        return App.Current.Resources["LargeSubSubtitleTextBlock"];
                    else return App.Current.Resources["MediumSubSubtitleTextBlock"];
                case "heading":
                    if (GlobalContext.FontSize == FontSize.Small)
                        return App.Current.Resources["SmallHeadingTextBlock"];
                    else if (GlobalContext.FontSize == FontSize.Large)
                        return App.Current.Resources["LargeHeadingTextBlock"];
                    else return App.Current.Resources["MediumHeadingTextBlock"];
                case "headingbold":
                    if (GlobalContext.FontSize == FontSize.Small)
                        return App.Current.Resources["SmallBoldHeadingTextBlock"];
                    else if (GlobalContext.FontSize == FontSize.Large)
                        return App.Current.Resources["LargeBoldHeadingTextBlock"];
                    else return App.Current.Resources["MediumBoldHeadingTextBlock"];
                case "text":
                    if (GlobalContext.FontSize == FontSize.Small)
                        return App.Current.Resources["SmallTextBlock"];
                    else if (GlobalContext.FontSize == FontSize.Large)
                        return App.Current.Resources["LargeTextBlock"];
                    else return App.Current.Resources["MediumTextBlock"];
                case "textbox":
                    if (GlobalContext.FontSize == FontSize.Small)
                        return App.Current.Resources["SmallTextBox"];
                    else if (GlobalContext.FontSize == FontSize.Large)
                        return App.Current.Resources["LargeTextBox"];
                    else return App.Current.Resources["MediumTextBox"];
                case "combo":
                    if (GlobalContext.FontSize == FontSize.Small)
                        return App.Current.Resources["SmallCombo"];
                    else if (GlobalContext.FontSize == FontSize.Large)
                        return App.Current.Resources["LargeCombo"];
                    else return App.Current.Resources["MediumCombo"];
                default:
                    return App.Current.Resources["BaseTextBlockStyle"];

            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
