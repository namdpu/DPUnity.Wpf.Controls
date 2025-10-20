using DPUnity.Windows;
using DPUnity.Wpf.Controls.Controls.DialogService;
using DPUnity.Wpf.Controls.Controls.InputForms.Forms;
using DPUnity.Wpf.Controls.Controls.InputForms.Interfaces;
using DPUnity.Wpf.Controls.Helpers;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace DPUnity.Wpf.Controls.Controls.InputForms
{
    public static class DPInput
    {
        public static ProcessViewModel ShowProcess(string title, nint owner = 0)
        {
            var windowOptions = new WindowOptions()
            {
                ResizeMode = ResizeMode.NoResize,
                MinWidth = 125,
                MinHeight = 145,
                Width = 450,
                Height = 145,
                Title = title,
                WindowOwner = owner,
                windowAction = (wd) => { if (owner == 0) { WindowHelper.SetWindowOwner(wd.Window); } }
            };

            var viewModel = WindowManager.OpenWindowProcess<Forms.ProcessPage>(() =>
            {
                return DPDialog.Ask($"Bạn có muốn dừng tiến trình đang chạy không?") == true;
            }, windowOptions);

            return viewModel;
        }

        public static ProcessViewModel ShowProcess2(string title, nint owner = 0)
        {
            var windowOptions = new WindowOptions()
            {
                ResizeMode = ResizeMode.NoResize,
                MinWidth = 125,
                MinHeight = 175,
                Width = 450,
                Height = 175,
                Title = title,
                WindowOwner = owner,
                windowAction = (wd) => { if (owner == 0) { WindowHelper.SetWindowOwner(wd.Window); } }
            };
            var viewModel = WindowManager.OpenWindowProcess<Forms.ProcessPage>(() =>
            {
                return DPDialog.Ask($"Bạn có muốn dừng tiến trình đang chạy không?") == true;
            }, windowOptions);
            if (viewModel is ProcessViewModel vm)
            {
                vm.Progress2Visibility = Visibility.Visible;
            }
            return viewModel;
        }

        public static ProcessViewModel ShowProcess3(string title, nint owner = 0)
        {
            var windowOptions = new WindowOptions()
            {
                ResizeMode = ResizeMode.NoResize,
                MinWidth = 125,
                MinHeight = 175,
                Width = 450,
                Height = 170,
                Title = title,
                WindowOwner = owner,
                windowAction = (wd) => { if (owner == 0) { WindowHelper.SetWindowOwner(wd.Window); } }
            };
            var viewModel = WindowManager.OpenWindowProcess<Forms.ProcessPage>(() =>
            {
                return DPDialog.Ask($"Bạn có muốn dừng tiến trình đang chạy không?") == true;
            }, windowOptions);
            if (viewModel is ProcessViewModel vm)
            {
                vm.Progress2Visibility = Visibility.Visible;
                vm.Progress3Visibility = Visibility.Visible;
            }
            return viewModel;
        }

        public static async Task<InputBooleanResult> ShowBooleanInput(string title, string trueContent = "Đúng", string falseContent = "Sai", bool defaultValue = false, nint owner = 0)
        {
            var options = new WindowOptions()
            {
                Title = title,
                Width = 300,
                Height = 160,
                MinHeight = 160,
                ResizeMode = ResizeMode.CanResize,
                WindowOwner = owner,
                windowAction = (wd) => { if (owner == 0) { WindowHelper.SetWindowOwner(wd.Window); } }
            };
            var (Result, ViewModel) = await WindowManager.ShowDialogAsync<BooleanInputPage, BooleanInputViewModel>
               (options, false, (vm) =>
               {
                   if (vm is BooleanInputViewModel viewModel)
                   {
                       viewModel.TrueContent = trueContent;
                       viewModel.FalseContent = falseContent;
                       viewModel.Value = defaultValue;
                   }
               });
            if (Result == MessageResult.OK && ViewModel is not null)
            {
                return new InputBooleanResult(Result, ViewModel.Value);
            }
            return new InputBooleanResult(Result, false);
        }

        public static async Task<InputTextResult> ShowTextInput(string title, string defaultText = "", nint owner = 0)
        {
            var options = new WindowOptions()
            {
                Title = title,
                Width = 400,
                Height = 140,
                MinHeight = 140,
                ResizeMode = ResizeMode.CanResize,
                WindowOwner = owner,
                windowAction = (wd) => { if (owner == 0) { WindowHelper.SetWindowOwner(wd.Window); } }
            };
            var (Result, ViewModel) = await WindowManager.ShowDialogAsync<TextInputPage, TextInputViewModel>
               (options, false, (vm) =>
               {
                   if (vm is TextInputViewModel viewModel)
                   {
                       viewModel.Text = defaultText;
                   }
               });
            if (Result == MessageResult.OK && ViewModel is not null)
            {
                return new InputTextResult(Result, ViewModel.Text);
            }
            return new InputTextResult(Result, string.Empty);
        }

        public static async Task<InputNumericResult> ShowNumericInput(string title, double? defaultValue = null, bool allowDecimal = true, double? minValue = null, double? maxValue = null, bool allowEmpty = false, nint owner = 0)
        {
            var options = new WindowOptions()
            {
                Title = title,
                Width = 400,
                Height = 160,
                MinHeight = 160,
                ResizeMode = ResizeMode.CanResize,
                WindowOwner = owner,
                windowAction = (wd) => { if (owner == 0) { WindowHelper.SetWindowOwner(wd.Window); } }
            };
            var (Result, ViewModel) = await WindowManager.ShowDialogAsync<NumericInputPage, NumericInputViewModel>
               (options, false, (vm) =>
               {
                   if (vm is NumericInputViewModel viewModel)
                   {
                       viewModel.AllowDecimal = allowDecimal;
                       viewModel.MinValue = minValue;
                       viewModel.MaxValue = maxValue;
                       if (defaultValue.HasValue)
                       {
                           if (!allowDecimal && defaultValue.Value != Math.Truncate(defaultValue.Value))
                           {
                               defaultValue = Math.Truncate(defaultValue.Value);
                           }
                           viewModel.Text = defaultValue.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                           viewModel.NumericValue = defaultValue.Value;
                           viewModel.AllowEmpty = allowEmpty;
                       }
                   }
               });

            if (Result == MessageResult.OK && ViewModel is not null)
            {
                return new InputNumericResult(Result, ViewModel.NumericValue);
            }
            return new InputNumericResult(Result, null);
        }

        public static async Task<InputComboBoxResult> ShowSelectInput(string title, List<IInputObject> itemSource, IInputObject? defaultSelection = null, nint owner = 0)
        {
            var options = new WindowOptions()
            {
                Title = title,
                Width = 400,
                Height = 120,
                MinHeight = 120,
                ResizeMode = ResizeMode.CanResize,
                WindowOwner = owner,
                windowAction = (wd) => { if (owner == 0) { WindowHelper.SetWindowOwner(wd.Window); } }
            };
            var (Result, ViewModel) = await WindowManager.ShowDialogAsync<SelectInputPage, SelectInputViewModel>
               (options, false, (vm) =>
               {
                   if (vm is SelectInputViewModel viewModel)
                   {
                       viewModel.ItemsSource = itemSource;
                       viewModel.SelectedItem = defaultSelection;
                   }
               });
            if (Result == MessageResult.OK && ViewModel is not null)
            {
                return new InputComboBoxResult(Result, ViewModel.SelectedItem);
            }
            return new InputComboBoxResult(Result, null);
        }

        public static async Task<InputMultiSelectResult> ShowMultiSelectInput(string title, List<IInputObject> itemSource, nint owner = 0)
        {
            var options = new WindowOptions()
            {
                Title = title,
                Width = 500,
                Height = 400,
                MinHeight = 400,
                ResizeMode = ResizeMode.CanResize,
                WindowOwner = owner,
                windowAction = (wd) => { if (owner == 0) { WindowHelper.SetWindowOwner(wd.Window); } }
            };
            var (Result, ViewModel) = await WindowManager.ShowDialogAsync<MultiSelectInputPage, MultiSelectInputViewModel>
               (options, false, (vm) =>
               {
                   if (vm is MultiSelectInputViewModel viewModel)
                   {
                       viewModel.ItemsSource = new ObservableCollection<IInputObject>(itemSource);
                   }
               });
            if (Result == MessageResult.OK && ViewModel is not null)
            {
                return new InputMultiSelectResult(Result, [.. ViewModel.SelectedItems]);
            }
            return new InputMultiSelectResult(Result, []);
        }

        public static async Task<InputReplaceResult> ShowReplaceInput(string title, string findText = "", string replaceText = "", nint owner = 0)
        {
            var options = new WindowOptions()
            {
                Title = title,
                Width = 400,
                Height = 200,
                MinHeight = 200,
                ResizeMode = ResizeMode.CanResize,
                WindowOwner = owner,
                windowAction = (wd) => { if (owner == 0) { WindowHelper.SetWindowOwner(wd.Window); } }
            };
            var (Result, ViewModel) = await WindowManager.ShowDialogAsync<ReplaceInputPage, ReplaceInputViewModel>
               (options, false, (vm) =>
               {
                   if (vm is ReplaceInputViewModel viewModel)
                   {
                       viewModel.Replace = findText;
                       viewModel.ReplaceWith = replaceText;
                   }
               });
            if (Result == MessageResult.OK && ViewModel is not null)
            {
                return new InputReplaceResult(Result, ViewModel.Replace, ViewModel.ReplaceWith);
            }
            return new InputReplaceResult(Result, string.Empty, string.Empty);
        }

        public static async Task<InputDataGridReplaceResult> ShowDataGridReplaceInput(string title, List<DataGridColumn> columns, string findText = "", string replaceText = "", nint owner = 0)
        {
            var options = new WindowOptions()
            {
                Title = title,
                Width = 400,
                Height = 500,
                MinWidth = 400,
                MinHeight = 500,
                ResizeMode = ResizeMode.CanResize,
                WindowOwner = owner,
                windowAction = (wd) => { if (owner == 0) { WindowHelper.SetWindowOwner(wd.Window); } }
            };
            var (Result, ViewModel) = await WindowManager.ShowDialogAsync<DataGridReplaceInputPage, DataGridReplaceInputViewModel>
               (options, false, (vm) =>
               {
                   if (vm is DataGridReplaceInputViewModel viewModel)
                   {
                       viewModel.InitializeColumns(columns);
                       viewModel.Replace = findText;
                       viewModel.ReplaceWith = replaceText;
                   }
               });
            if (Result == MessageResult.OK && ViewModel is not null)
            {
                return new InputDataGridReplaceResult(Result, [.. ViewModel.SelectedColumns], ViewModel.Replace, ViewModel.ReplaceWith);
            }
            return new InputDataGridReplaceResult(Result, [], string.Empty, string.Empty);
        }

        public static async Task<InputConfirmDeleteResult> ShowConfirmDelete(string title, string confirmString = "DELETE", string message = "Are you sure you want to delete this item?", nint owner = 0)
        {
            double width = 400;
            double height = CalculateStringHeight(message, width);

            var options = new WindowOptions()
            {
                Title = title,
                Width = width,
                Height = height,
                MinHeight = height,
                ResizeMode = ResizeMode.NoResize,
                WindowOwner = owner,
                windowAction = (wd) => { if (owner == 0) { WindowHelper.SetWindowOwner(wd.Window); } }
            };
            var (Result, ViewModel) = await WindowManager.ShowDialogAsync<ConfirmDeletePage, ConfirmDeleteViewModel>
               (options, false, (vm) =>
               {
                   if (vm is ConfirmDeleteViewModel viewModel)
                   {
                       viewModel.ConfirmString = confirmString;
                       viewModel.Message = message;
                   }
               });
            if (Result == MessageResult.OK && ViewModel is not null)
            {
                return new InputConfirmDeleteResult(Result, ViewModel.ConfirmString == ViewModel.InputString);
            }
            return new InputConfirmDeleteResult(Result, false);
        }

        private const double LINE_HEIGHT = 14 * 1.7;
        private const int CHARACTER_PER_LINE = 65;

        private static double CalculateStringHeight(string message, double width)
        {
            string[] stringLine = message.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            int newLineCount = stringLine.Length;
            if (newLineCount == 0) newLineCount = 1;
            int newLinesFromWrap = 0;
            foreach (var line in stringLine)
            {
                if (line.Length > CHARACTER_PER_LINE)
                {
                    newLinesFromWrap += 1;
                }
            }
            newLineCount += newLinesFromWrap;

            double desiredHeight = newLineCount * LINE_HEIGHT;
            return desiredHeight + 110;
        }
    }
}
