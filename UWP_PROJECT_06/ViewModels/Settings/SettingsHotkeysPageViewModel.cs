using Microsoft.Toolkit.Uwp.Helpers;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.History;
using UWP_PROJECT_06.Services;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Sensors;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace UWP_PROJECT_06.ViewModels.Settings
{
    public class SettingsHotkeysPageViewModel : ViewModelBase
    {
        private string text; public string Text { get => text; set => SetProperty(ref text, value); }
        private string messageText; public string MessageText { get => messageText; set => SetProperty(ref messageText, value); }
        private TextBox CurrentTextBox;

        private bool isPressed; public bool IsPressed { get => isPressed; set => SetProperty(ref isPressed, value); }  
        private bool isControlSelected; public bool IsControlSelected { get => isControlSelected; set => SetProperty(ref isControlSelected, value); }
        private bool isMenuSelected; public bool IsMenuSelected { get => isMenuSelected; set => SetProperty(ref isMenuSelected, value); }
        private bool isShiftSelected; public bool IsShiftSelected { get => isShiftSelected; set => SetProperty(ref isShiftSelected, value); }  
        

        public ObservableRangeCollection<Hotkey> Hotkeys { get; set; }

        public AsyncCommand ReloadCommand { get; set; }
        
        public AsyncCommand ControlSelectedCommand { get; set; }
        public AsyncCommand MenuSelectedCommand { get; set; }
        public AsyncCommand ShiftSelectedCommand { get; set; }

        public AsyncCommand<object> ClearCommand { get; set; }
        public AsyncCommand<object> FocusCommand { get; set; }
        public AsyncCommand<object> PressCommand { get; set; }


        public SettingsHotkeysPageViewModel()
        {
            Text = String.Empty;
            MessageText = "Press hotkey";
            CurrentTextBox = null;

            IsPressed = false;
            
            IsControlSelected= false;
            IsMenuSelected= true;
            IsShiftSelected= false;

            Hotkeys = new ObservableRangeCollection<Hotkey>();

            Load();

            ReloadCommand = new AsyncCommand(Load);
            
            ControlSelectedCommand = new AsyncCommand(ControlSelected);
            MenuSelectedCommand = new AsyncCommand(MenuSelected);
            ShiftSelectedCommand = new AsyncCommand(ShiftSelected);
            
            ClearCommand = new AsyncCommand<object>(Clear);
            PressCommand = new AsyncCommand<object>(Press);
            FocusCommand = new AsyncCommand<object>(Focus);
        }

        private async Task Load()
        {
            IEnumerable<Hotkey> hotkeys = await SettingsService.ReadHotkeys();

            Hotkeys.Clear();

            foreach (Hotkey hotkey in hotkeys)
                Hotkeys.Add(hotkey);

        }

        private async Task Press(object arg)
        {
            var e = arg as KeyRoutedEventArgs;

            if (e == null)
                return;

            if (CurrentTextBox == null)
                return;

            if (Window.Current.CoreWindow.GetKeyState(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down)
                || Window.Current.CoreWindow.GetKeyState(VirtualKey.Menu).HasFlag(CoreVirtualKeyStates.Down)
                || Window.Current.CoreWindow.GetKeyState(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down))
            {
                return;
            }

            string key = e.Key.ToString();

            if (e.Key == Windows.System.VirtualKey.Menu ||
                e.Key == Windows.System.VirtualKey.LeftMenu ||
                e.Key == Windows.System.VirtualKey.RightMenu ||
                e.Key == Windows.System.VirtualKey.NavigationMenu ||

                e.Key == Windows.System.VirtualKey.Shift ||
                e.Key == Windows.System.VirtualKey.LeftShift ||
                e.Key == Windows.System.VirtualKey.RightShift ||

                e.Key == Windows.System.VirtualKey.LeftWindows ||
                e.Key == Windows.System.VirtualKey.RightWindows ||

                e.Key == Windows.System.VirtualKey.CapitalLock ||
                e.Key == Windows.System.VirtualKey.Escape ||
                e.Key == Windows.System.VirtualKey.Enter ||
                e.Key == Windows.System.VirtualKey.Back ||
                e.Key == Windows.System.VirtualKey.Delete ||

                e.Key == Windows.System.VirtualKey.Control ||
                e.Key == Windows.System.VirtualKey.LeftControl ||
                e.Key == Windows.System.VirtualKey.RightControl ||

                key == "173" ||
                key == "174" ||
                key == "175" ||
                key == "186" ||
                key == "187" ||
                key == "188" ||
                key == "189" ||
                key == "190" ||
                key == "191" ||
                key == "192" ||
                key == "219" ||
                key == "220" ||
                key == "221" ||
                key == "222"
                )
            {
                return;
            }

            for (int i = 0; i < 10; i++)
                key = key == String.Format("Number{0}", i) ? i.ToString() : key;

            TextBox currentTextBox = e.OriginalSource as TextBox;
            if (currentTextBox == null) return;

            Grid childGrid = currentTextBox.Parent as Grid;
            if (childGrid == null) return;

            Grid parent = childGrid.Parent as Grid;
            if (parent == null) return;

            Text = String.Empty;
            CurrentTextBox.Text = String.Empty;

            foreach (object child in parent.Children)
            {
                TextBlock tip = child as TextBlock;
                if (tip == null) continue;

                tip.Text = String.Format("{0}", tip.Text.Split(" (")[0]);
                tip.Text += String.Format(" ({0}{1}{2}{3})",
                    IsControlSelected ? "Ctrl+" : "",
                    IsMenuSelected ? "Alt+" : "",
                    IsShiftSelected ? "Shift+" : "",
                    key);

                IsPressed = false;

                Hotkey hotkey = tip.DataContext as Hotkey;
                List<string> listOfModifiers = new List<string>();

                if (IsControlSelected) listOfModifiers.Add("Control");
                if (IsMenuSelected) listOfModifiers.Add("Menu");
                if (IsShiftSelected) listOfModifiers.Add("Shift");

                string modifiers = "";

                foreach (string modifier in listOfModifiers)
                    modifiers += modifier != listOfModifiers.Last()
                        ? modifier + ","
                        : modifier;

                await SettingsService.UpdateHotkey(hotkey.Name, modifiers, key);

                return;
            }

            CurrentTextBox.IsEnabled = false;
            CurrentTextBox = null;
        }
        private async Task Focus(object arg)
        {
            if (IsPressed)
            {
                IsPressed = false;

                return;
            }
            
            Text = String.Empty;
            TextBox textBox = arg as TextBox; 

            if (textBox == null) return;

            IsPressed = true;

            CurrentTextBox = textBox;
            CurrentTextBox.IsEnabled = true;
            CurrentTextBox.Text = String.Empty;
            CurrentTextBox.Focus(FocusState.Programmatic);
        }


        private async Task Clear(object arg)
        {
            TextBlock textBlock = arg as TextBlock;
            
            if (textBlock == null)
                return;

            textBlock.Text = String.Format("{0} (None)", textBlock.Text.Split(" (")[0]);

            Hotkey hotkey = textBlock.DataContext as Hotkey;

            if (hotkey == null) return;

            await SettingsService.UpdateHotkey(hotkey.Name);
            
            IsPressed = false;
            IsControlSelected = false;
            IsMenuSelected = true;
            IsShiftSelected = false;
        }


        private async Task ControlSelected()
        {
            IsControlSelected = IsControlSelected ? false : true;

            if (!(IsControlSelected || IsMenuSelected || IsShiftSelected))
                IsMenuSelected = true;

            if (CurrentTextBox != null)
            {
                CurrentTextBox.Text = String.Empty;
                CurrentTextBox.Focus(FocusState.Programmatic);
            }
        }

        private async Task ShiftSelected()
        {
            IsShiftSelected = IsShiftSelected ? false : true;

            if (!(IsControlSelected || IsMenuSelected || IsShiftSelected))
                IsMenuSelected = true;

            if (CurrentTextBox != null)
            {
                CurrentTextBox.Text = String.Empty;
                CurrentTextBox.Focus(FocusState.Programmatic);
            }
        }

        private async Task MenuSelected()
        {
            IsMenuSelected = IsMenuSelected ? false : true;

            if (!(IsControlSelected || IsMenuSelected || IsShiftSelected))
                IsControlSelected = true;

            if (CurrentTextBox != null)
            {
                CurrentTextBox.Text = String.Empty;
                CurrentTextBox.Focus(FocusState.Programmatic);
            }
        }

    }
}
