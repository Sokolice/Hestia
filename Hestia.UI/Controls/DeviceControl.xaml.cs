using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Windows.Input;
using Hestia.Common;
using Windows.UI;
using System.Reflection;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Hestia.View.Controls
{
    public sealed partial class DeviceControl : UserControl
    {
        public int Category
        {
            get { return (int)GetValue(CategoryProperty); }
            set { SetValue(CategoryProperty, value); }
        }
        public Guid ID
        {
            get { return (Guid?)GetValue(IDProperty) ?? new Guid(); }
            set { SetValue(IDProperty, value); }
        }
        public string BlindsSlat
        {
            get { return (string)GetValue(BlindsSlatProperty); }
            set { SetValue(BlindsSlatProperty, value); }
        }
        public string BlindsMovement
        {
            get { return (string)GetValue(BlindsMovementProperty); }
            set { SetValue(BlindsMovementProperty, value); }
        }
        public string BlindsUpdDown
        {
            get { return (string)GetValue(BlindsUpdDownProperty); }
            set { SetValue(BlindsUpdDownProperty, value); }
        }
        public string LightsValue
        {
            get { return (string)GetValue(LightsValueProperty); }
            set { SetValue(LightsValueProperty, value); }
        }
        public string LightsOnOffAddress
        {
            get { return (string)GetValue(LightsOnOffAddressProperty); }
            set { SetValue(LightsOnOffAddressProperty, value); }
        }
        public string LightsDimming
        {
            get { return (string)GetValue(LightsDimmingProperty); }
            set { SetValue(LightsDimmingProperty, value); }
        }
        public ICommand ButtonOnCommand
        {
            get { return (ICommand)GetValue(ButtonOnCommandProperty); }
            set { SetValue(ButtonOnCommandProperty, value); }
        }
        public ICommand ButtonOffCommand
        {
            get { return (ICommand)GetValue(ButtonOffCommandProperty); }
            set { SetValue(ButtonOffCommandProperty, value); }
        }
        public ICommand ButtonUpCommand
        {
            get { return (ICommand)GetValue(ButtonUpCommandProperty); }
            set { SetValue(ButtonUpCommandProperty, value); }
        }
        public ICommand ButtonDownCommand
        {
            get { return (ICommand)GetValue(ButtonDownCommandProperty); }
            set { SetValue(ButtonDownCommandProperty, value); }
        }
        public ICommand DimmingPlusCommand
        {
            get { return (ICommand)GetValue(DimmingPlusCommandProperty); }
            set { SetValue(DimmingPlusCommandProperty, value); }
        }
        public ICommand DimmingMinusCommand
        {
            get { return (ICommand)GetValue(DimmingMinusCommandProperty); }
            set { SetValue(DimmingMinusCommandProperty, value); }
        }
        public ICommand SlatTiltCommand
        {
            get{ return (ICommand)GetValue(SlatTiltCommandProperty);}
            set{ SetValueDp(SlatTiltCommandProperty, value);}
        }

        public ICommand BlindHeightCommand
        {
            get { return (ICommand)GetValue(BlindHeightCommandProperty); }
            set { SetValueDp(BlindHeightCommandProperty, value); }
        }
        public string DeviceName
        {
            get
            {
                return (string)GetValue(DeviceNameText);
            }
            set
            {
                SetValueDp(DeviceNameText, value);
            }
        }
        
        public string SlatTilt
        {
            get
            {
                return (string)GetValue(SlatTiltProperty);
            }
            set
            {
                SetValueDp(SlatTiltProperty, value);
            }
        }
        public string BlindHeight
        {
            get
            {
                return (string)GetValue(BlindHeightProperty);
            }
            set
            {
                SetValueDp(BlindHeightProperty, value);
            }
        }

        public static DependencyProperty DeviceNameText = DependencyProperty.Register("DeviceName", typeof(string), typeof(DeviceControl), null);
        public static DependencyProperty ButtonOnCommandProperty = DependencyProperty.Register("ButtonOnCommand",typeof(ICommand), typeof(DeviceControl), null);
        public static DependencyProperty ButtonOffCommandProperty = DependencyProperty.Register("ButtonOffCommand", typeof(ICommand), typeof(DeviceControl), null);
        public static DependencyProperty DimmingPlusCommandProperty = DependencyProperty.Register("DimmingPlusCommand", typeof(ICommand), typeof(DeviceControl), null);
        public static DependencyProperty DimmingMinusCommandProperty = DependencyProperty.Register("DimmingMinusCommand", typeof(ICommand), typeof(DeviceControl), null);
        public static DependencyProperty ButtonUpCommandProperty = DependencyProperty.Register("ButtonUpCommand", typeof(ICommand), typeof(DeviceControl), null);
        public static DependencyProperty ButtonDownCommandProperty = DependencyProperty.Register("ButtonDownCommand", typeof(ICommand), typeof(DeviceControl), null);
        public static DependencyProperty SlatTiltProperty = DependencyProperty.Register("SlatTilt", typeof(string), typeof(DeviceControl), null);
        public static DependencyProperty SlatTiltCommandProperty = DependencyProperty.Register("SlatTiltCommand", typeof(ICommand), typeof(DeviceControl), null);
        public static DependencyProperty BlindHeightProperty = DependencyProperty.Register("BlindHeight", typeof(string), typeof(DeviceControl), null);
        public static DependencyProperty BlindHeightCommandProperty = DependencyProperty.Register("BlindHeightCommand", typeof(ICommand), typeof(DeviceControl), null);
        public static DependencyProperty LightsOnOffAddressProperty = DependencyProperty.Register("LightsOnOffAddress", typeof(string), typeof(DeviceControl), null);
        public static DependencyProperty CategoryProperty = DependencyProperty.Register("Category", typeof(int), typeof(DeviceControl), null);
        public static DependencyProperty LightsDimmingProperty = DependencyProperty.Register("LightsDimming", typeof(string), typeof(DeviceControl), null);
        public static DependencyProperty LightsValueProperty = DependencyProperty.Register("LightsValue", typeof(string), typeof(DeviceControl), null);
        public static DependencyProperty BlindsUpdDownProperty = DependencyProperty.Register("BlindsUpdDown", typeof(string), typeof(DeviceControl), null);
        public static DependencyProperty BlindsMovementProperty = DependencyProperty.Register("BlindsMovement", typeof(string), typeof(DeviceControl), null);
        public static DependencyProperty BlindsSlatProperty = DependencyProperty.Register("BlindsSlat", typeof(string), typeof(DeviceControl), null);
        public static DependencyProperty IDProperty = DependencyProperty.Register("ID", typeof(Guid), typeof(DeviceControl), null);


        public DeviceControl()
        {
            this.InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void SetValueDp(DependencyProperty property, object value, [System.Runtime.CompilerServices.CallerMemberName] String p = null)
        {
            SetValue(property, value);
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ID != null)
            {
                switch (Category)
                {
                    case (int)DeviceCategory.Lights:
                        CreateLightsControl();
                        break;
                    case (int)DeviceCategory.Blinds:
                        CreateBlindsControl();
                        break;
                }
            }
        }

        /// <summary>
        /// Vygenerování ovládacích prvků pro typ zařízení žaluzie
        /// </summary>
        private void CreateBlindsControl()
        {
            if(BlindsUpdDown != string.Empty)
            {
                Button btn = new Button();
                btn.Content = GlobalContext.ResourceLoader.GetString("txtUp");
                Binding lCommandOnBinding = new Binding();
                lCommandOnBinding.Path = new PropertyPath("ButtonUpCommand");
                btn.CommandParameter = ID;
                btn.SetBinding(Button.CommandProperty, lCommandOnBinding);

                Button btnOff = new Button();
                btnOff.Content = GlobalContext.ResourceLoader.GetString("txtDown"); ;
                Binding lCommandOffBinding = new Binding();
                lCommandOffBinding.Path = new PropertyPath("ButtonDownCommand");
                btnOff.CommandParameter = ID;
                btnOff.SetBinding(Button.CommandProperty, lCommandOffBinding);

                btn.Margin = new Thickness(1, 1, 1, 1);
                btnOff.Margin = new Thickness(1, 1, 1, 1);

                Grid lGrid = new Grid();
                lGrid.Margin = new Thickness(0, 0, 0, 5);
                lGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                lGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                lGrid.VerticalAlignment = VerticalAlignment.Center;

                btn.SetValue(Grid.ColumnProperty, 0);
                btnOff.SetValue(Grid.ColumnProperty, 1);

                lGrid.Children.Add(btn);
                lGrid.Children.Add(btnOff);
                StackPanel.Children.Add(lGrid);
            }
            if(BlindsSlat != string.Empty)
            {
                TextBlock lText = new TextBlock();
                lText.Text = GlobalContext.ResourceLoader.GetString("txtSlatTilt/Text");
                Binding lBinding = new Binding() { Converter = new StyleConverter(), ConverterParameter ="text" };
                lText.SetBinding(TextBlock.StyleProperty, lBinding);


                TextBlock lCurrentValue = new TextBlock();
                Binding lCurrentValueBinding = new Binding();
                lCurrentValueBinding.TargetNullValue = 0;
                lCurrentValueBinding.FallbackValue = 0;
                lCurrentValueBinding.Path = new PropertyPath("SlatTilt");
                lCurrentValue.SetBinding(TextBlock.TextProperty, lCurrentValueBinding);

                Slider lSlider = new Slider();
                lSlider.Orientation = Orientation.Horizontal;
                lSlider.Width = 80;
                lSlider.Maximum = 100;
                lSlider.Minimum = 0;
                //lTextBox.LostFocus += LTextBox_LostFocus;
                Binding lTextBinding = new Binding();
                //lTextBinding.Source = Device;
                lTextBinding.Path = new PropertyPath("SlatTilt");
                lTextBinding.Mode = BindingMode.TwoWay;
                lSlider.SetBinding(Slider.ValueProperty, lTextBinding);

                Button lBtn = new Button();
                lBtn.Content = "OK";
                Binding lBind = new Binding();
                lBind.Path = new PropertyPath("SlatTiltCommand");
                lBtn.CommandParameter = ID;
                lBtn.SetBinding(Button.CommandProperty, lBind);

                Grid lGrid = new Grid();
                lGrid.Margin = new Thickness(0, 0, 0, 5);
                lGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                lGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                lGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                lGrid.VerticalAlignment = VerticalAlignment.Center;

                lSlider.SetValue(Grid.ColumnProperty, 0);
                lCurrentValue.SetValue(Grid.ColumnProperty, 1);
                lBtn.SetValue(Grid.ColumnProperty, 2);

                lGrid.Children.Add(lSlider);
                lGrid.Children.Add(lCurrentValue);
                lGrid.Children.Add(lBtn);

                StackPanel.Children.Add(lText);
                StackPanel.Children.Add(lGrid);
            }

            if (BlindsMovement != string.Empty)
            {
                TextBlock lText = new TextBlock();
                lText.Text = GlobalContext.ResourceLoader.GetString("txtMovementValue/Text");
                Binding lBinding = new Binding() { Converter = new StyleConverter(), ConverterParameter = "text" };
                lText.SetBinding(TextBlock.StyleProperty, lBinding);

                TextBlock lCurrentValue = new TextBlock();
                Binding lCurrentValueBinding = new Binding();
                lCurrentValueBinding.TargetNullValue = 0;
                lCurrentValueBinding.FallbackValue = 0;
                lCurrentValueBinding.Path = new PropertyPath("BlindHeight");
                lCurrentValue.SetBinding(TextBlock.TextProperty, lCurrentValueBinding);

                Slider lSlider = new Slider();
                lSlider.Orientation = Orientation.Vertical;
                lSlider.IsDirectionReversed = true;
                lSlider.Height = 80;
                lSlider.Maximum = 100;
                lSlider.Minimum = 0;
                Binding lTextBinding = new Binding();
                lTextBinding.Path = new PropertyPath("BlindHeight");
                lTextBinding.Mode = BindingMode.TwoWay;
                lSlider.SetBinding(Slider.ValueProperty, lTextBinding);

                Button lBtn = new Button();
                lBtn.Content = "OK";
                Binding lBind = new Binding();
                lBind.Path = new PropertyPath("BlindHeightCommand");
                lBtn.CommandParameter = ID;
                lBtn.SetBinding(Button.CommandProperty, lBind);

                Grid lGrid = new Grid();
                lGrid.Margin = new Thickness(0, 0, 0, 5);
                lGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                lGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                lGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                lGrid.VerticalAlignment = VerticalAlignment.Center;

                lSlider.SetValue(Grid.ColumnProperty, 0);
                lCurrentValue.SetValue(Grid.ColumnProperty, 1);
                lBtn.SetValue(Grid.ColumnProperty, 2);

                lGrid.Children.Add(lSlider);
                lGrid.Children.Add(lCurrentValue);
                lGrid.Children.Add(lBtn);

                StackPanel.Children.Add(lText);
                StackPanel.Children.Add(lGrid);
            }
        }
                       
        /// <summary>
        /// Vygenerování ovládacích prvků pro osvětlení 
        /// </summary>
        private void CreateLightsControl()
        {
            if (LightsOnOffAddress != string.Empty)
            {
                Button btn = new Button();
                btn.Content = GlobalContext.ResourceLoader.GetString("txtOn");
                Binding lCommandOnBinding = new Binding();
                lCommandOnBinding.Path = new PropertyPath("ButtonOnCommand");
                btn.CommandParameter = ID;
                btn.SetBinding(Button.CommandProperty, lCommandOnBinding);

                Button btnOff = new Button();
                btnOff.Content = GlobalContext.ResourceLoader.GetString("infoOff");
                Binding lCommandOffBinding = new Binding();
                lCommandOffBinding.Path = new PropertyPath("ButtonOffCommand");
                btnOff.CommandParameter = ID;
                btnOff.SetBinding(Button.CommandProperty, lCommandOffBinding);

                btn.Margin = new Thickness(1, 1, 1, 1);
                btnOff.Margin = new Thickness(1, 1, 1, 1);

                Grid lGrid = new Grid();
                lGrid.Margin = new Thickness(0, 0, 0, 5);
                lGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                lGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                lGrid.VerticalAlignment = VerticalAlignment.Center;

                btn.SetValue(Grid.ColumnProperty, 0);
                btnOff.SetValue(Grid.ColumnProperty, 1);

                lGrid.Children.Add(btn);
                lGrid.Children.Add(btnOff);
                StackPanel.Children.Add(lGrid);
            }
            if(LightsDimming != string.Empty)
            {
                TextBlock lTextT = new TextBlock();
                lTextT.Text = GlobalContext.ResourceLoader.GetString("txtDimming/Text");
                Binding lBinding = new Binding() { Converter = new StyleConverter(), ConverterParameter = "text" };
                lTextT.SetBinding(TextBlock.StyleProperty, lBinding);


                Button lPlus = new Button();
                lPlus.Margin = new Thickness(1, 1, 1, 1);
                lPlus.Content = new SymbolIcon(Symbol.Add);

                Button lMinus = new Button();
                lMinus.Margin = new Thickness(1, 1, 1, 1);
                lMinus.Content = new SymbolIcon(Symbol.Remove);

                Binding lPlusBinding = new Binding();
                lPlusBinding.Path = new PropertyPath("DimmingPlusCommand");
                lPlus.CommandParameter = ID;
                lPlus.SetBinding(AppBarButton.CommandProperty, lPlusBinding);


                Binding lMinusBinding = new Binding();
                lMinusBinding.Path = new PropertyPath("DimmingMinusCommand");
                lMinus.CommandParameter = ID;
                lMinus.SetBinding(AppBarButton.CommandProperty, lMinusBinding);

                Grid lGrid = new Grid();
                lGrid.Margin = new Thickness(0, 0, 0, 5);
                lGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                lGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                lGrid.VerticalAlignment = VerticalAlignment.Center;

                lPlus.SetValue(Grid.ColumnProperty, 0);
                lMinus.SetValue(Grid.ColumnProperty, 1);

                lGrid.Children.Add(lPlus);
                lGrid.Children.Add(lMinus);
                StackPanel.Children.Add(lTextT);
                StackPanel.Children.Add(lGrid);
            }
            
        }
    }
}
