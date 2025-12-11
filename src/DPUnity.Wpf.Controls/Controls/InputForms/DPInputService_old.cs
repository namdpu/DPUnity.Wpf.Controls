//using DPUnity.Windows;
//using DPUnity.Wpf.Common.Controls;
//using DPUnity.Wpf.Common.Models;
//using DPUnity.Wpf.Common.Windows;
//using DPUnity.Wpf.Controls.Controls.InputForms.Forms;
//using System.Collections.ObjectModel;
//using System.Windows;
//using System.Windows.Controls;

//namespace DPUnity.Wpf.Controls.Controls.InputForms
//{
//    public class DPInputService : IInputService
//    {
//        private readonly IWindowManager _windowService;
//        private readonly IDialogService _dialogService;

//        public DPInputService(IWindowManager windowService, IDialogService dialogService)
//        {
//            _windowService = windowService;
//            _dialogService = dialogService;
//        }

//        public async Task<InputTextResult> ShowTextInput(string title, string defaultText)
//        {
//            var options = new WindowOptions()
//            {
//                Title = title,
//                Width = 400,
//                Height = 140,
//                MinHeight = 140,
//                ResizeMode = System.Windows.ResizeMode.CanResize,
//            };
//            var (Result, ViewModel) = await _windowService.OpenWindowDialogByLoadingAsync<TextInputPage, TextInputViewModel>
//               (options, false, (vm) =>
//               {
//                   if (vm is TextInputViewModel viewModel)
//                   {
//                       viewModel.Text = defaultText;
//                   }
//               });
//            if (Result == MessageResult.OK && ViewModel is not null)
//            {
//                return new InputTextResult(Result, ViewModel.Text);
//            }
//            return new InputTextResult(Result, string.Empty);
//        }

//        public async Task<InputNumericResult> ShowNumericInput(string title, double? defaultValue = null, bool allowDecimal = true, double? minValue = null, double? maxValue = null, bool allowEmpty = false)
//        {
//            var options = new WindowOptions()
//            {
//                Title = title,
//                Width = 400,
//                Height = 160,
//                MinHeight = 160,
//                ResizeMode = System.Windows.ResizeMode.CanResize,
//            };
//            var (Result, ViewModel) = await _windowService.OpenWindowDialogByLoadingAsync<NumericInputPage, NumericInputViewModel>
//               (options, false, (vm) =>
//               {
//                   if (vm is NumericInputViewModel viewModel)
//                   {
//                       viewModel.AllowDecimal = allowDecimal;
//                       viewModel.MinValue = minValue;
//                       viewModel.MaxValue = maxValue;
//                       if (defaultValue.HasValue)
//                       {
//                           if (!allowDecimal && defaultValue.Value != Math.Truncate(defaultValue.Value))
//                           {
//                               defaultValue = Math.Truncate(defaultValue.Value);
//                           }
//                           viewModel.Text = defaultValue.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
//                           viewModel.NumericValue = defaultValue.Value;
//                           viewModel.AllowEmpty = allowEmpty;
//                       }
//                   }
//               });

//            if (Result == MessageResult.OK && ViewModel is not null)
//            {
//                return new InputNumericResult(Result, ViewModel.NumericValue);
//            }
//            return new InputNumericResult(Result, null);
//        }

//        public async Task<InputComboBoxResult> ShowSelectInput(string title, List<IInputObject> itemSource, IInputObject? defaultSelection = null)
//        {
//            var options = new WindowOptions()
//            {
//                Title = title,
//                Width = 400,
//                Height = 130,
//                MinHeight = 130,
//                ResizeMode = System.Windows.ResizeMode.CanResize,
//            };
//            var (Result, ViewModel) = await _windowService.OpenWindowDialogByLoadingAsync<SelectInputPage, SelectInputViewModel>
//               (options, false, (vm) =>
//               {
//                   if (vm is SelectInputViewModel viewModel)
//                   {
//                       viewModel.ItemsSource = itemSource;
//                       viewModel.SelectedItem = defaultSelection;
//                   }
//               });
//            if (Result == MessageResult.OK && ViewModel is not null)
//            {
//                return new InputComboBoxResult(Result, ViewModel.SelectedItem);
//            }
//            return new InputComboBoxResult(Result, null);
//        }

//        public async Task<InputMultiSelectResult> ShowMultiSelect(string title, List<IInputObject> itemSource)
//        {
//            var options = new WindowOptions()
//            {
//                Title = title,
//                Width = 500,
//                Height = 400,
//                MinHeight = 400,
//                ResizeMode = System.Windows.ResizeMode.CanResize,
//            };
//            var (Result, ViewModel) = await _windowService.OpenWindowDialogByLoadingAsync<MultiSelectInputPage, MultiSelectInputViewModel>
//               (options, false, (vm) =>
//               {
//                   if (vm is MultiSelectInputViewModel viewModel)
//                   {
//                       viewModel.ItemsSource = new ObservableCollection<IInputObject>(itemSource);
//                   }
//               });
//            if (Result == MessageResult.OK && ViewModel is not null)
//            {
//                return new InputMultiSelectResult(Result, [.. ViewModel.SelectedItems]);
//            }
//            return new InputMultiSelectResult(Result, []);
//        }

//        public async Task<InputBooleanResult> ShowBooleanInput(string title, string trueContent = "Đúng", string falseContent = "Sai", bool defaultValue = false)
//        {
//            var options = new WindowOptions()
//            {
//                Title = title,
//                Width = 400,
//                Height = 160,
//                MinHeight = 160,
//                ResizeMode = System.Windows.ResizeMode.CanResize,
//            };
//            var (Result, ViewModel) = await _windowService.OpenWindowDialogByLoadingAsync<BooleanInputPage, BooleanInputViewModel>
//               (options, false, (vm) =>
//               {
//                   if (vm is BooleanInputViewModel viewModel)
//                   {
//                       viewModel.TrueContent = trueContent;
//                       viewModel.FalseContent = falseContent;
//                       viewModel.Value = defaultValue;
//                   }
//               });
//            if (Result == MessageResult.OK && ViewModel is not null)
//            {
//                return new InputBooleanResult(Result, ViewModel.Value);
//            }
//            return new InputBooleanResult(Result, false);
//        }

//        public async Task<InputReplaceResult> ShowReplaceInput(string title, string findText = "", string replaceText = "")
//        {
//            var options = new WindowOptions()
//            {
//                Title = title,
//                Width = 400,
//                Height = 200,
//                MinHeight = 200,
//                ResizeMode = System.Windows.ResizeMode.CanResize,
//            };
//            var (Result, ViewModel) = await _windowService.OpenWindowDialogByLoadingAsync<ReplaceInputPage, ReplaceInputViewModel>
//               (options, false, (vm) =>
//               {
//                   if (vm is ReplaceInputViewModel viewModel)
//                   {
//                       viewModel.Replace = findText;
//                       viewModel.ReplaceWith = replaceText;
//                   }
//               });
//            if (Result == MessageResult.OK && ViewModel is not null)
//            {
//                return new InputReplaceResult(Result, ViewModel.Replace, ViewModel.ReplaceWith);
//            }
//            return new InputReplaceResult(Result, string.Empty, string.Empty);
//        }

//        public async Task<InputDataGridReplaceResult> ShowDataGridReplaceInput(string title, List<DataGridColumn> columns, string findText = "", string replaceText = "")
//        {
//            var options = new WindowOptions()
//            {
//                Title = title,
//                Width = 400,
//                Height = 500,
//                MinWidth = 400,
//                MinHeight = 500,
//                ResizeMode = System.Windows.ResizeMode.CanResize,
//            };
//            var (Result, ViewModel) = await _windowService.OpenWindowDialogByLoadingAsync<DataGridReplaceInputPage, DataGridReplaceInputViewModel>
//               (options, false, (vm) =>
//               {
//                   if (vm is DataGridReplaceInputViewModel viewModel)
//                   {
//                       viewModel.InitializeColumns(columns);
//                       viewModel.Replace = findText;
//                       viewModel.ReplaceWith = replaceText;
//                   }
//               });
//            if (Result == MessageResult.OK && ViewModel is not null)
//            {
//                return new InputDataGridReplaceResult(Result, [.. ViewModel.SelectedColumns], ViewModel.Replace, ViewModel.ReplaceWith);
//            }
//            return new InputDataGridReplaceResult(Result, [], string.Empty, string.Empty);
//        }

//        public async Task< IProcessViewModel >ShowProcess(string title, bool hideParent = true)
//        {
//            var windowOptions = new WindowOptions()
//            {
//                ResizeMode = ResizeMode.NoResize,
//                MinWidth = 125,
//                MinHeight = 145,
//                Width = 450,
//                Height = 145,
//                Title = title,
//            };
//            var viewModel = await _windowService.Op<Forms.ProcessPage>(() =>
//            {
//                return await _dialogService.ShowAsk($"Bạn có muốn dừng tiến trình đang chạy không?") == true;
//            }, windowOptions, hideParent);
//            return viewModel;
//        }
//        public IProcessViewModel ShowProcess2(string title, bool hideParent = true)
//        {
//            var windowOptions = new WindowOptions()
//            {
//                ResizeMode = ResizeMode.NoResize,
//                MinWidth = 125,
//                MinHeight = 175,
//                Width = 450,
//                Height = 175,
//                Title = title,
//            };
//            var viewModel = _windowService.OpenProcess<Forms.ProcessPage>( () =>
//            {
//                return _dialogService.ShowAsk($"Bạn có muốn dừng tiến trình đang chạy không?").GetAwaiter().GetResult() == true;
//            }, windowOptions, hideParent);
//            if (viewModel is ProcessViewModel vm)
//            {
//                vm.Progress2Visibility = Visibility.Visible;
//            }
//            return viewModel;
//        }

//        public IProcessViewModel ShowProcess3(string title, bool hideParent = true)
//        {
//            var windowOptions = new WindowOptions()
//            {
//                ResizeMode = ResizeMode.NoResize,
//                MinWidth = 125,
//                MinHeight = 175,
//                Width = 450,
//                Height = 170,
//                Title = title,
//            };
//            var viewModel = _windowService.OpenProcess<Forms.ProcessPage>(() =>
//            {
//                return _dialogService.ShowAsk($"Bạn có muốn dừng tiến trình đang chạy không?").GetAwaiter().GetResult() == true;
//            }, windowOptions, hideParent);
//            if (viewModel is ProcessViewModel vm)
//            {
//                vm.WindowService.CurrentWindow.Window.Height = 175;
//                vm.Progress2Visibility = Visibility.Visible;
//                vm.Progress3Visibility = Visibility.Visible;
//            }
//            return viewModel;
//        }

//        public async Task<InputConfirmDeleteResult> ShowInputConfirmDelete(string title, string message = "Are you sure you want to delete this item?", string confirmString = "DELETE")
//        {
//            double width = 400;
//            double height = CalculateStringHeight(message, width);

//            var options = new WindowOptions()
//            {
//                ResizeMode = ResizeMode.NoResize,
//                MinWidth = 450,
//                MinHeight = 180,
//                Width = 450,
//                Height = 180,
//                Title = title,
//            };
//            var (Result, ViewModel) = await _windowService.OpenWindowDialogByLoadingAsync<BooleanInputPage, BooleanInputViewModel>
//               (options, false, (vm) =>
//               {
//                   if (vm is ConfirmDeleteViewModel viewModel)
//                   {
//                       viewModel.Message = message;
//                       viewModel.ConfirmString = confirmString;
//                   }
//               });
//            if (Result == MessageResult.OK && ViewModel is not null)
//            {
//                return new(Result, ViewModel.Value);
//            }
//            return new(Result, false);
//        }

//        private const double LINE_HEIGHT = 14 * 1.7;
//        private const int CHARACTER_PER_LINE = 65;
//        private static double CalculateStringHeight(string message, double width)
//        {
//            string[] stringLine = message.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
//            int newLineCount = stringLine.Length;
//            if (newLineCount == 0) newLineCount = 1;
//            int newLinesFromWrap = 0;
//            foreach (var line in stringLine)
//            {
//                if (line.Length > CHARACTER_PER_LINE)
//                {
//                    newLinesFromWrap += 1;
//                }
//            }
//            newLineCount += newLinesFromWrap;

//            double desiredHeight = newLineCount * LINE_HEIGHT;
//            return desiredHeight + 110;
//        }
//    }
//}