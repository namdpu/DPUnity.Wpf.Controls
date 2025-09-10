using DPUnity.Wpf.Controls.Controls.InputForms;

namespace DPUnity.Wpf.Controls.Examples
{
    /// <summary>
    /// Example usage of DataGridReplaceInput control
    /// </summary>
    public class DataGridReplaceInputExample
    {
        public async Task ShowExample()
        {
            // Danh sách tên cột để người dùng chọn
            var columnNames = new List<string>
            {
                "Name",
                "Email", 
                "Phone",
                "Address",
                "City",
                "Country",
                "PostalCode",
                "Company",
                "Department",
                "Position"
            };

            // Hiển thị dialog DataGridReplaceInput
            var result = await DPInput.ShowDataGridReplaceInput(
                title: "Replace in Selected Columns",
                columnNames: columnNames,
                findText: "old_value",
                replaceText: "new_value"
            );

            // Xử lý kết quả
            if (result.IsOk)
            {
                var selectedColumns = result.Value.SelectedColumns;
                var findText = result.Value.Find;
                var replaceText = result.Value.ReplaceWith;

                // Thực hiện logic replace trên các cột được chọn
                Console.WriteLine($"Selected columns: {string.Join(", ", selectedColumns)}");
                Console.WriteLine($"Replace '{findText}' with '{replaceText}'");
                
                // Ví dụ thực hiện replace logic
                foreach (var column in selectedColumns)
                {
                    Console.WriteLine($"Processing column: {column}");
                    // TODO: Implement actual replace logic here
                }
            }
            else
            {
                Console.WriteLine("User cancelled the operation");
            }
        }

        /// <summary>
        /// Example using DPInputService (for dependency injection scenarios)
        /// </summary>
        public async Task ShowExampleWithService(IDPInputService inputService)
        {
            var columnNames = new List<string>
            {
                "Column1", "Column2", "Column3", "Column4", "Column5"
            };

            var result = await inputService.ShowDataGridReplaceInput(
                "Select Columns and Replace Values",
                columnNames,
                "search_text",
                "replacement_text"
            );

            if (result.IsOk)
            {
                // Process the results
                ProcessReplaceOperation(result.Value.SelectedColumns, result.Value.Find, result.Value.ReplaceWith);
            }
        }

        private void ProcessReplaceOperation(List<string> columns, string find, string replaceWith)
        {
            // Implementation of your replace logic
            foreach (var column in columns)
            {
                // Replace logic for each column
                Console.WriteLine($"Replacing '{find}' with '{replaceWith}' in column '{column}'");
            }
        }
    }
}
