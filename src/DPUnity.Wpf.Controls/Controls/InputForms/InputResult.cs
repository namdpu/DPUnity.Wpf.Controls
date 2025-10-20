using DPUnity.Windows;
using DPUnity.Wpf.Controls.Controls.InputForms.Interfaces;
using System.Windows.Controls;

namespace DPUnity.Wpf.Controls.Controls.InputForms
{
    public abstract class InputResultBase<T>(MessageResult isOK, T value)
    {
        public bool IsOk => isOK == MessageResult.OK;
        public T Value => value;

    }

    public abstract class InputArrayResultBase<T>(MessageResult isOK, List<T> value)
    {
        public bool IsOk => isOK == MessageResult.OK;
        public List<T> Value => value;
    }

    public class InputTextResult(MessageResult isOK, string text) : InputResultBase<string>(isOK, text) { }

    public class InputNumericResult(MessageResult isOK, double? value) : InputResultBase<double?>(isOK, value) { }

    public class InputComboBoxResult(MessageResult isOK, IInputObject? value) : InputResultBase<IInputObject?>(isOK, value) { }

    public class InputMultiSelectResult(MessageResult isOK, List<IInputObject> values) : InputArrayResultBase<IInputObject>(isOK, values) { }

    public class InputBooleanResult(MessageResult isOK, bool value) : InputResultBase<bool>(isOK, value) { }

    public class InputReplaceResult(MessageResult isOK, string find, string replaceWith) : InputResultBase<(string Find, string ReplaceWith)>(isOK, (find, replaceWith)) { }

    public class InputDataGridReplaceResult(MessageResult isOK, List<DataGridColumn> selectedColumns, string find, string replaceWith)
        : InputResultBase<(List<DataGridColumn> SelectedColumns, string Find, string ReplaceWith)>(isOK, (selectedColumns, find, replaceWith))
    { }

    public class InputConfirmDeleteResult(MessageResult isOK, bool value) : InputResultBase<bool>(isOK, value) { }
}
