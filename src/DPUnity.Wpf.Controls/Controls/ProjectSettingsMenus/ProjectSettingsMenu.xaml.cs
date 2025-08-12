using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DPUnity.Wpf.Controls.Controls.ProjectSettingsMenus
{
    /// <summary>
    /// Interaction logic for ProjectSettingsMenu.xaml
    /// </summary>
    public partial class ProjectSettingsMenu : UserControl
    {
        public ProjectSettingsMenu()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ProjectSettingsMenu));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                "SelectedItem",
                typeof(object),
                typeof(ProjectSettingsMenu),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
            );

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty AddNewCommandProperty =
            DependencyProperty.Register("AddNewCommand", typeof(ICommand), typeof(ProjectSettingsMenu));

        public ICommand AddNewCommand
        {
            get { return (ICommand)GetValue(AddNewCommandProperty); }
            set { SetValue(AddNewCommandProperty, value); }
        }

        public static readonly DependencyProperty SaveCommandProperty =
            DependencyProperty.Register("SaveCommand", typeof(ICommand), typeof(ProjectSettingsMenu));

        public ICommand SaveCommand
        {
            get { return (ICommand)GetValue(SaveCommandProperty); }
            set { SetValue(SaveCommandProperty, value); }
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.ContextMenu != null)
            {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                button.ContextMenu.IsOpen = true;
            }
        }
    }
}
