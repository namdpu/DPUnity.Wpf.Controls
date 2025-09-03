using DPUnity.Windows;
using DPUnity.Windows.Services;
using DPUnity.Wpf.Controls.Controls.InputForms.Forms;
using DPUnity.Wpf.Controls.Controls.InputForms.Interfaces;
using System.Collections.ObjectModel;

namespace DPUnity.Wpf.Controls.Controls.InputForms
{
    public interface IDPInputService
    {
        Task<InputTextResult> ShowTextInput(string title, string defaultText);
        Task<InputNumericResult> ShowNumericInput(string title, double? defaultValue = null, bool allowDecimal = true, double? minValue = null, double? maxValue = null, bool allowEmpty = false);
        Task<InputComboBoxResult> ShowSelectInput(string title, List<IInputObject> itemSource, IInputObject? defaultSelection = null);
        Task<InputMultiSelectResult> ShowMultiSelect(string title, List<IInputObject> itemSource);
        Task<InputBooleanResult> ShowBooleanInput(string title, string trueContent = "True", string falseContent = "False", bool defaultValue = false);

    }

    public class DPInputService : IDPInputService
    {
        private readonly IWindowService _windowService;
        public DPInputService(IWindowService windowService)
        {
            _windowService = windowService;
        }

        public async Task<InputTextResult> ShowTextInput(string title, string defaultText)
        {
            var options = new WindowOptions()
            {
                Title = title,
                Width = 400,
                Height = 140,
                MinHeight = 140,
                ResizeMode = System.Windows.ResizeMode.NoResize,
            };
            var (Result, ViewModel) = await _windowService.OpenWindowDialogByLoadingAsync<TextInputPage, TextInputViewModel>
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

        public async Task<InputNumericResult> ShowNumericInput(string title, double? defaultValue = null, bool allowDecimal = true, double? minValue = null, double? maxValue = null, bool allowEmpty = false)
        {
            var options = new WindowOptions()
            {
                Title = title,
                Width = 400,
                Height = 160,
                MinHeight = 160,
                ResizeMode = System.Windows.ResizeMode.NoResize,
            };
            var (Result, ViewModel) = await _windowService.OpenWindowDialogByLoadingAsync<NumericInputPage, NumericInputViewModel>
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

        public async Task<InputComboBoxResult> ShowSelectInput(string title, List<IInputObject> itemSource, IInputObject? defaultSelection = null)
        {
            var options = new WindowOptions()
            {
                Title = title,
                Width = 400,
                Height = 120,
                MinHeight = 120,
                ResizeMode = System.Windows.ResizeMode.NoResize,
            };
            var (Result, ViewModel) = await _windowService.OpenWindowDialogByLoadingAsync<SelectInputPage, SelectInputViewModel>
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

        public async Task<InputMultiSelectResult> ShowMultiSelect(string title, List<IInputObject> itemSource)
        {
            var options = new WindowOptions()
            {
                Title = title,
                Width = 500,
                Height = 400,
                MinHeight = 400,
                ResizeMode = System.Windows.ResizeMode.CanResize,
            };
            var (Result, ViewModel) = await _windowService.OpenWindowDialogByLoadingAsync<MultiSelectInputPage, MultiSelectInputViewModel>
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

        public async Task<InputBooleanResult> ShowBooleanInput(string title, string trueContent = "True", string falseContent = "False", bool defaultValue = false)
        {
            var options = new WindowOptions()
            {
                Title = title,
                Width = 400,
                Height = 160,
                MinHeight = 160,
                ResizeMode = System.Windows.ResizeMode.NoResize,
            };
            var (Result, ViewModel) = await _windowService.OpenWindowDialogByLoadingAsync<BooleanInputPage, BooleanInputViewModel>
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
    }
}
