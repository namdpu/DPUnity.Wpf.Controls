# DataGridReplaceInput Control

## Mô tả

`DataGridReplaceInput` là một control kết hợp giữa `MultiSelectInput` và `ReplaceInput`, cho phép người dùng:

1. **Chọn nhiều cột**: Sử dụng giao diện dual-listbox để chọn các cột từ danh sách có sẵn
2. **Nhập giá trị thay thế**: Cung cấp 2 textbox để nhập "Replace" và "Replace with"
3. **Trả về kết quả tổng hợp**: Danh sách cột được chọn cùng với các giá trị replace

## Tính năng chính

### Multi-Select với String List
- Khác với `MultiSelectInput` truyền thống sử dụng `IInputObject`, control này nhận `List<string>` để truyền danh sách tên cột
- Hỗ trợ tìm kiếm (search) trên cả 2 bên
- Hỗ trợ sắp xếp (sort) cho cả danh sách có sẵn và danh sách đã chọn
- Di chuyển item bằng double-click hoặc các nút mũi tên
- Di chuyển tất cả item cùng lúc

### Replace Functionality
- 2 textbox cho "Replace" và "Replace with"
- Validation: yêu cầu ít nhất 1 cột được chọn và trường "Replace" không được rỗng

### Kết quả trả về
```csharp
public class InputDataGridReplaceResult
{
    public bool IsOk { get; } // true nếu user nhấn OK
    public (List<string> SelectedColumns, string Find, string ReplaceWith) Value { get; }
}
```

## Cách sử dụng

### 1. Sử dụng thông qua DPInput (Static method)

```csharp
var columnNames = new List<string>
{
    "Name", "Email", "Phone", "Address", "City"
};

var result = await DPInput.ShowDataGridReplaceInput(
    title: "Replace in Selected Columns",
    columnNames: columnNames,
    findText: "old_value",      // optional
    replaceText: "new_value"    // optional
);

if (result.IsOk)
{
    var selectedColumns = result.Value.SelectedColumns;
    var findText = result.Value.Find;
    var replaceText = result.Value.ReplaceWith;
    
    // Xử lý logic replace
    foreach (var column in selectedColumns)
    {
        // Thực hiện replace trong column
        Console.WriteLine($"Replace '{findText}' with '{replaceText}' in column '{column}'");
    }
}
```

### 2. Sử dụng thông qua IDPInputService (Dependency Injection)

```csharp
public class MyService
{
    private readonly IDPInputService _inputService;
    
    public MyService(IDPInputService inputService)
    {
        _inputService = inputService;
    }
    
    public async Task ShowReplaceDialog()
    {
        var columns = new List<string> { "Column1", "Column2", "Column3" };
        
        var result = await _inputService.ShowDataGridReplaceInput(
            "Select Columns to Replace",
            columns,
            "search_text",
            "replacement_text"
        );
        
        if (result.IsOk)
        {
            ProcessReplaceOperation(result.Value);
        }
    }
}
```

## Giao diện

Control được chia thành 3 phần:

1. **Column Selection Area**: 
   - Dual-listbox với search bars và sort buttons
   - Available columns bên trái, selected columns bên phải
   - Các nút mũi tên ở giữa để di chuyển

2. **Replace Input Area**:
   - Textbox "Replace": nhập giá trị cần tìm
   - Textbox "Replace with": nhập giá trị thay thế

3. **Action Buttons**:
   - OK: chỉ enable khi có ít nhất 1 cột được chọn và "Replace" không rỗng
   - Cancel: hủy bỏ thao tác

## Keyboard Shortcuts

- **Double-click**: Di chuyển item giữa 2 listbox
- **Enter**: Submit form (tương đương OK button)
- **Escape**: Cancel form

## Window Options

- **Size**: 800x500 pixels
- **Resizable**: Có thể thay đổi kích thước
- **Minimum size**: 600x500 pixels

## Validation

- Ít nhất 1 cột phải được chọn
- Trường "Replace" không được rỗng
- Trường "Replace with" có thể để trống

## Files tạo mới

1. `DataGridReplaceInputViewModel.cs` - ViewModel chính
2. `DataGridReplaceInputPage.xaml` - XAML layout
3. `DataGridReplaceInputPage.xaml.cs` - Code-behind
4. `DataGridReplaceInputResult.cs` - Class kết quả (deprecated, sử dụng InputDataGridReplaceResult thay thế)
5. Updated `InputResult.cs` - Thêm InputDataGridReplaceResult
6. Updated `DPInput.cs` - Thêm ShowDataGridReplaceInput method
7. Updated `DPInputService.cs` - Thêm interface và implementation
8. `DataGridReplaceInputExample.cs` - File example usage
