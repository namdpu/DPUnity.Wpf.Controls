using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DPUnity.Wpf.Controls.Controls.ColorPickers
{
    public partial class ColorPickerButton : UserControl
    {
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPickerButton),
                new PropertyMetadata(Colors.Black, OnSelectedColorChanged));

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public static readonly DependencyProperty SelectedColorIndexProperty =
            DependencyProperty.Register("SelectedColorIndex", typeof(int?), typeof(ColorPickerButton),
                new PropertyMetadata(0, OnSelectedColorIndexChanged));

        public int? SelectedColorIndex
        {
            get { return (int?)GetValue(SelectedColorIndexProperty); }
            set { SetValue(SelectedColorIndexProperty, value); }
        }

        public event EventHandler<Color>? ColorChanged;

        private bool _isInternalUpdate = false;
        private Rectangle? colorRectangle;

        public ColorPickerButton()
        {
            InitializeComponent();

            colorRectangle = FindName("ColorRectangle") as Rectangle;

            Loaded += ColorPickerButton_Loaded;
        }

        private void ColorPickerButton_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateBinding(); // Force update binding for initial color display
        }

        private void UpdateBinding()
        {
            // Force update the binding expression for the color rectangle
            if (colorRectangle != null)
            {
                var bindingExpression = BindingOperations.GetBindingExpression(colorRectangle, Shape.FillProperty);
                bindingExpression?.UpdateTarget();
            }
        }

        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var colorPickerButton = d as ColorPickerButton;
            if (colorPickerButton?._isInternalUpdate == true) return;

            // No need to sync with ColorPicker since it's now in a separate window
        }

        private static void OnSelectedColorIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var colorPickerButton = d as ColorPickerButton;
            if (colorPickerButton?._isInternalUpdate == true) return;

            colorPickerButton?.UpdateColorFromIndex();
        }

        private void UpdateColorFromIndex()
        {
            if (SelectedColorIndex.HasValue)
            {
                // Get color from ACItoRGBLookup based on index
                var color = GetColorByIndex(SelectedColorIndex.Value);
                if (color.HasValue)
                {
                    _isInternalUpdate = true;
                    SelectedColor = color.Value;
                    _isInternalUpdate = false;
                }
            }
        }

        private Color? GetColorByIndex(int index)
        {
            // Use the same lookup as ColorPicker
            var lookup = ACItoRGBLookup();
            var match = lookup.FirstOrDefault(x => x.Item1 == index);
            return match?.Item2;
        }

        private static List<Tuple<int, Color>> ACItoRGBLookup()
        {
            // Full lookup table matching ColorPicker
            List<Tuple<int, Color>> ret = new List<Tuple<int, Color>>();
            ret.Add(Tuple.Create(0, new Color() { A = 255, R = 0, G = 0, B = 0 }));
            ret.Add(Tuple.Create(1, new Color() { A = 255, R = 255, G = 0, B = 0 }));
            ret.Add(Tuple.Create(2, new Color() { A = 255, R = 255, G = 255, B = 0 }));
            ret.Add(Tuple.Create(3, new Color() { A = 255, R = 0, G = 255, B = 0 }));
            ret.Add(Tuple.Create(4, new Color() { A = 255, R = 0, G = 255, B = 255 }));
            ret.Add(Tuple.Create(5, new Color() { A = 255, R = 0, G = 0, B = 255 }));
            ret.Add(Tuple.Create(6, new Color() { A = 255, R = 255, G = 0, B = 255 }));
            ret.Add(Tuple.Create(7, new Color() { A = 255, R = 255, G = 255, B = 255 }));
            ret.Add(Tuple.Create(8, new Color() { A = 255, R = 128, G = 128, B = 128 }));
            ret.Add(Tuple.Create(9, new Color() { A = 255, R = 192, G = 192, B = 192 }));
            ret.Add(Tuple.Create(10, new Color() { A = 255, R = 255, G = 0, B = 0 }));
            ret.Add(Tuple.Create(11, new Color() { A = 255, R = 255, G = 127, B = 127 }));
            ret.Add(Tuple.Create(12, new Color() { A = 255, R = 165, G = 0, B = 0 }));
            ret.Add(Tuple.Create(13, new Color() { A = 255, R = 165, G = 82, B = 82 }));
            ret.Add(Tuple.Create(14, new Color() { A = 255, R = 127, G = 0, B = 0 }));
            ret.Add(Tuple.Create(15, new Color() { A = 255, R = 127, G = 63, B = 63 }));
            ret.Add(Tuple.Create(16, new Color() { A = 255, R = 76, G = 0, B = 0 }));
            ret.Add(Tuple.Create(17, new Color() { A = 255, R = 76, G = 38, B = 38 }));
            ret.Add(Tuple.Create(18, new Color() { A = 255, R = 38, G = 0, B = 0 }));
            ret.Add(Tuple.Create(19, new Color() { A = 255, R = 38, G = 19, B = 19 }));
            ret.Add(Tuple.Create(20, new Color() { A = 255, R = 255, G = 63, B = 0 }));
            ret.Add(Tuple.Create(21, new Color() { A = 255, R = 255, G = 159, B = 127 }));
            ret.Add(Tuple.Create(22, new Color() { A = 255, R = 165, G = 41, B = 0 }));
            ret.Add(Tuple.Create(23, new Color() { A = 255, R = 165, G = 103, B = 82 }));
            ret.Add(Tuple.Create(24, new Color() { A = 255, R = 127, G = 31, B = 0 }));
            ret.Add(Tuple.Create(25, new Color() { A = 255, R = 127, G = 79, B = 63 }));
            ret.Add(Tuple.Create(26, new Color() { A = 255, R = 76, G = 19, B = 0 }));
            ret.Add(Tuple.Create(27, new Color() { A = 255, R = 76, G = 47, B = 38 }));
            ret.Add(Tuple.Create(28, new Color() { A = 255, R = 38, G = 9, B = 0 }));
            ret.Add(Tuple.Create(29, new Color() { A = 255, R = 38, G = 23, B = 19 }));
            ret.Add(Tuple.Create(30, new Color() { A = 255, R = 255, G = 127, B = 0 }));
            ret.Add(Tuple.Create(31, new Color() { A = 255, R = 255, G = 191, B = 127 }));
            ret.Add(Tuple.Create(32, new Color() { A = 255, R = 165, G = 82, B = 0 }));
            ret.Add(Tuple.Create(33, new Color() { A = 255, R = 165, G = 124, B = 82 }));
            ret.Add(Tuple.Create(34, new Color() { A = 255, R = 127, G = 63, B = 0 }));
            ret.Add(Tuple.Create(35, new Color() { A = 255, R = 127, G = 95, B = 63 }));
            ret.Add(Tuple.Create(36, new Color() { A = 255, R = 76, G = 38, B = 0 }));
            ret.Add(Tuple.Create(37, new Color() { A = 255, R = 76, G = 57, B = 38 }));
            ret.Add(Tuple.Create(38, new Color() { A = 255, R = 38, G = 19, B = 0 }));
            ret.Add(Tuple.Create(39, new Color() { A = 255, R = 38, G = 28, B = 19 }));
            ret.Add(Tuple.Create(40, new Color() { A = 255, R = 255, G = 191, B = 0 }));
            ret.Add(Tuple.Create(41, new Color() { A = 255, R = 255, G = 223, B = 127 }));
            ret.Add(Tuple.Create(42, new Color() { A = 255, R = 165, G = 124, B = 0 }));
            ret.Add(Tuple.Create(43, new Color() { A = 255, R = 165, G = 145, B = 82 }));
            ret.Add(Tuple.Create(44, new Color() { A = 255, R = 127, G = 95, B = 0 }));
            ret.Add(Tuple.Create(45, new Color() { A = 255, R = 127, G = 111, B = 63 }));
            ret.Add(Tuple.Create(46, new Color() { A = 255, R = 76, G = 57, B = 0 }));
            ret.Add(Tuple.Create(47, new Color() { A = 255, R = 76, G = 66, B = 38 }));
            ret.Add(Tuple.Create(48, new Color() { A = 255, R = 38, G = 28, B = 0 }));
            ret.Add(Tuple.Create(49, new Color() { A = 255, R = 38, G = 33, B = 19 }));
            ret.Add(Tuple.Create(50, new Color() { A = 255, R = 255, G = 255, B = 0 }));
            ret.Add(Tuple.Create(51, new Color() { A = 255, R = 255, G = 255, B = 127 }));
            ret.Add(Tuple.Create(52, new Color() { A = 255, R = 165, G = 165, B = 0 }));
            ret.Add(Tuple.Create(53, new Color() { A = 255, R = 165, G = 165, B = 82 }));
            ret.Add(Tuple.Create(54, new Color() { A = 255, R = 127, G = 127, B = 0 }));
            ret.Add(Tuple.Create(55, new Color() { A = 255, R = 127, G = 127, B = 63 }));
            ret.Add(Tuple.Create(56, new Color() { A = 255, R = 76, G = 76, B = 0 }));
            ret.Add(Tuple.Create(57, new Color() { A = 255, R = 76, G = 76, B = 38 }));
            ret.Add(Tuple.Create(58, new Color() { A = 255, R = 38, G = 38, B = 0 }));
            ret.Add(Tuple.Create(59, new Color() { A = 255, R = 38, G = 38, B = 19 }));
            ret.Add(Tuple.Create(60, new Color() { A = 255, R = 191, G = 255, B = 0 }));
            ret.Add(Tuple.Create(61, new Color() { A = 255, R = 223, G = 255, B = 127 }));
            ret.Add(Tuple.Create(62, new Color() { A = 255, R = 124, G = 165, B = 0 }));
            ret.Add(Tuple.Create(63, new Color() { A = 255, R = 145, G = 165, B = 82 }));
            ret.Add(Tuple.Create(64, new Color() { A = 255, R = 95, G = 127, B = 0 }));
            ret.Add(Tuple.Create(65, new Color() { A = 255, R = 111, G = 127, B = 63 }));
            ret.Add(Tuple.Create(66, new Color() { A = 255, R = 57, G = 76, B = 0 }));
            ret.Add(Tuple.Create(67, new Color() { A = 255, R = 66, G = 76, B = 38 }));
            ret.Add(Tuple.Create(68, new Color() { A = 255, R = 28, G = 38, B = 0 }));
            ret.Add(Tuple.Create(69, new Color() { A = 255, R = 33, G = 38, B = 19 }));
            ret.Add(Tuple.Create(70, new Color() { A = 255, R = 127, G = 255, B = 0 }));
            ret.Add(Tuple.Create(71, new Color() { A = 255, R = 191, G = 255, B = 127 }));
            ret.Add(Tuple.Create(72, new Color() { A = 255, R = 82, G = 165, B = 0 }));
            ret.Add(Tuple.Create(73, new Color() { A = 255, R = 124, G = 165, B = 82 }));
            ret.Add(Tuple.Create(74, new Color() { A = 255, R = 63, G = 127, B = 0 }));
            ret.Add(Tuple.Create(75, new Color() { A = 255, R = 95, G = 127, B = 63 }));
            ret.Add(Tuple.Create(76, new Color() { A = 255, R = 38, G = 76, B = 0 }));
            ret.Add(Tuple.Create(77, new Color() { A = 255, R = 57, G = 76, B = 38 }));
            ret.Add(Tuple.Create(78, new Color() { A = 255, R = 19, G = 38, B = 0 }));
            ret.Add(Tuple.Create(79, new Color() { A = 255, R = 28, G = 38, B = 19 }));
            ret.Add(Tuple.Create(80, new Color() { A = 255, R = 63, G = 255, B = 0 }));
            ret.Add(Tuple.Create(81, new Color() { A = 255, R = 159, G = 255, B = 127 }));
            ret.Add(Tuple.Create(82, new Color() { A = 255, R = 41, G = 165, B = 0 }));
            ret.Add(Tuple.Create(83, new Color() { A = 255, R = 103, G = 165, B = 82 }));
            ret.Add(Tuple.Create(84, new Color() { A = 255, R = 31, G = 127, B = 0 }));
            ret.Add(Tuple.Create(85, new Color() { A = 255, R = 79, G = 127, B = 63 }));
            ret.Add(Tuple.Create(86, new Color() { A = 255, R = 19, G = 76, B = 0 }));
            ret.Add(Tuple.Create(87, new Color() { A = 255, R = 47, G = 76, B = 38 }));
            ret.Add(Tuple.Create(88, new Color() { A = 255, R = 9, G = 38, B = 0 }));
            ret.Add(Tuple.Create(89, new Color() { A = 255, R = 23, G = 38, B = 19 }));
            ret.Add(Tuple.Create(90, new Color() { A = 255, R = 0, G = 255, B = 0 }));
            ret.Add(Tuple.Create(91, new Color() { A = 255, R = 127, G = 255, B = 127 }));
            ret.Add(Tuple.Create(92, new Color() { A = 255, R = 0, G = 165, B = 0 }));
            ret.Add(Tuple.Create(93, new Color() { A = 255, R = 82, G = 165, B = 82 }));
            ret.Add(Tuple.Create(94, new Color() { A = 255, R = 0, G = 127, B = 0 }));
            ret.Add(Tuple.Create(95, new Color() { A = 255, R = 63, G = 127, B = 63 }));
            ret.Add(Tuple.Create(96, new Color() { A = 255, R = 0, G = 76, B = 0 }));
            ret.Add(Tuple.Create(97, new Color() { A = 255, R = 38, G = 76, B = 38 }));
            ret.Add(Tuple.Create(98, new Color() { A = 255, R = 0, G = 38, B = 0 }));
            ret.Add(Tuple.Create(99, new Color() { A = 255, R = 19, G = 38, B = 19 }));
            ret.Add(Tuple.Create(100, new Color() { A = 255, R = 0, G = 255, B = 63 }));
            ret.Add(Tuple.Create(101, new Color() { A = 255, R = 127, G = 255, B = 159 }));
            ret.Add(Tuple.Create(102, new Color() { A = 255, R = 0, G = 165, B = 41 }));
            ret.Add(Tuple.Create(103, new Color() { A = 255, R = 82, G = 165, B = 103 }));
            ret.Add(Tuple.Create(104, new Color() { A = 255, R = 0, G = 127, B = 31 }));
            ret.Add(Tuple.Create(105, new Color() { A = 255, R = 63, G = 127, B = 79 }));
            ret.Add(Tuple.Create(106, new Color() { A = 255, R = 0, G = 76, B = 19 }));
            ret.Add(Tuple.Create(107, new Color() { A = 255, R = 38, G = 76, B = 47 }));
            ret.Add(Tuple.Create(108, new Color() { A = 255, R = 0, G = 38, B = 9 }));
            ret.Add(Tuple.Create(109, new Color() { A = 255, R = 19, G = 38, B = 23 }));
            ret.Add(Tuple.Create(110, new Color() { A = 255, R = 0, G = 255, B = 127 }));
            ret.Add(Tuple.Create(111, new Color() { A = 255, R = 127, G = 255, B = 191 }));
            ret.Add(Tuple.Create(112, new Color() { A = 255, R = 0, G = 165, B = 82 }));
            ret.Add(Tuple.Create(113, new Color() { A = 255, R = 82, G = 165, B = 124 }));
            ret.Add(Tuple.Create(114, new Color() { A = 255, R = 0, G = 127, B = 63 }));
            ret.Add(Tuple.Create(115, new Color() { A = 255, R = 63, G = 127, B = 95 }));
            ret.Add(Tuple.Create(116, new Color() { A = 255, R = 0, G = 76, B = 38 }));
            ret.Add(Tuple.Create(117, new Color() { A = 255, R = 38, G = 76, B = 57 }));
            ret.Add(Tuple.Create(118, new Color() { A = 255, R = 0, G = 38, B = 19 }));
            ret.Add(Tuple.Create(119, new Color() { A = 255, R = 19, G = 38, B = 28 }));
            ret.Add(Tuple.Create(120, new Color() { A = 255, R = 0, G = 255, B = 191 }));
            ret.Add(Tuple.Create(121, new Color() { A = 255, R = 127, G = 255, B = 223 }));
            ret.Add(Tuple.Create(122, new Color() { A = 255, R = 0, G = 165, B = 124 }));
            ret.Add(Tuple.Create(123, new Color() { A = 255, R = 82, G = 165, B = 145 }));
            ret.Add(Tuple.Create(124, new Color() { A = 255, R = 0, G = 127, B = 95 }));
            ret.Add(Tuple.Create(125, new Color() { A = 255, R = 63, G = 127, B = 111 }));
            ret.Add(Tuple.Create(126, new Color() { A = 255, R = 0, G = 76, B = 57 }));
            ret.Add(Tuple.Create(127, new Color() { A = 255, R = 38, G = 76, B = 66 }));
            ret.Add(Tuple.Create(128, new Color() { A = 255, R = 0, G = 38, B = 28 }));
            ret.Add(Tuple.Create(129, new Color() { A = 255, R = 19, G = 38, B = 33 }));
            ret.Add(Tuple.Create(130, new Color() { A = 255, R = 0, G = 255, B = 255 }));
            ret.Add(Tuple.Create(131, new Color() { A = 255, R = 127, G = 255, B = 255 }));
            ret.Add(Tuple.Create(132, new Color() { A = 255, R = 0, G = 165, B = 165 }));
            ret.Add(Tuple.Create(133, new Color() { A = 255, R = 82, G = 165, B = 165 }));
            ret.Add(Tuple.Create(134, new Color() { A = 255, R = 0, G = 127, B = 127 }));
            ret.Add(Tuple.Create(135, new Color() { A = 255, R = 63, G = 127, B = 127 }));
            ret.Add(Tuple.Create(136, new Color() { A = 255, R = 0, G = 76, B = 76 }));
            ret.Add(Tuple.Create(137, new Color() { A = 255, R = 38, G = 76, B = 76 }));
            ret.Add(Tuple.Create(138, new Color() { A = 255, R = 0, G = 38, B = 38 }));
            ret.Add(Tuple.Create(139, new Color() { A = 255, R = 19, G = 38, B = 38 }));
            ret.Add(Tuple.Create(140, new Color() { A = 255, R = 0, G = 191, B = 255 }));
            ret.Add(Tuple.Create(141, new Color() { A = 255, R = 127, G = 223, B = 255 }));
            ret.Add(Tuple.Create(142, new Color() { A = 255, R = 0, G = 124, B = 165 }));
            ret.Add(Tuple.Create(143, new Color() { A = 255, R = 82, G = 145, B = 165 }));
            ret.Add(Tuple.Create(144, new Color() { A = 255, R = 0, G = 95, B = 127 }));
            ret.Add(Tuple.Create(145, new Color() { A = 255, R = 63, G = 111, B = 127 }));
            ret.Add(Tuple.Create(146, new Color() { A = 255, R = 0, G = 57, B = 76 }));
            ret.Add(Tuple.Create(147, new Color() { A = 255, R = 38, G = 66, B = 76 }));
            ret.Add(Tuple.Create(148, new Color() { A = 255, R = 0, G = 28, B = 38 }));
            ret.Add(Tuple.Create(149, new Color() { A = 255, R = 19, G = 33, B = 38 }));
            ret.Add(Tuple.Create(150, new Color() { A = 255, R = 0, G = 127, B = 255 }));
            ret.Add(Tuple.Create(151, new Color() { A = 255, R = 127, G = 191, B = 255 }));
            ret.Add(Tuple.Create(152, new Color() { A = 255, R = 0, G = 82, B = 165 }));
            ret.Add(Tuple.Create(153, new Color() { A = 255, R = 82, G = 124, B = 165 }));
            ret.Add(Tuple.Create(154, new Color() { A = 255, R = 0, G = 63, B = 127 }));
            ret.Add(Tuple.Create(155, new Color() { A = 255, R = 63, G = 95, B = 127 }));
            ret.Add(Tuple.Create(156, new Color() { A = 255, R = 0, G = 38, B = 76 }));
            ret.Add(Tuple.Create(157, new Color() { A = 255, R = 38, G = 57, B = 76 }));
            ret.Add(Tuple.Create(158, new Color() { A = 255, R = 0, G = 19, B = 38 }));
            ret.Add(Tuple.Create(159, new Color() { A = 255, R = 19, G = 28, B = 38 }));
            ret.Add(Tuple.Create(160, new Color() { A = 255, R = 0, G = 63, B = 255 }));
            ret.Add(Tuple.Create(161, new Color() { A = 255, R = 127, G = 159, B = 255 }));
            ret.Add(Tuple.Create(162, new Color() { A = 255, R = 0, G = 41, B = 165 }));
            ret.Add(Tuple.Create(163, new Color() { A = 255, R = 82, G = 103, B = 165 }));
            ret.Add(Tuple.Create(164, new Color() { A = 255, R = 0, G = 31, B = 127 }));
            ret.Add(Tuple.Create(165, new Color() { A = 255, R = 63, G = 79, B = 127 }));
            ret.Add(Tuple.Create(166, new Color() { A = 255, R = 0, G = 19, B = 76 }));
            ret.Add(Tuple.Create(167, new Color() { A = 255, R = 38, G = 47, B = 76 }));
            ret.Add(Tuple.Create(168, new Color() { A = 255, R = 0, G = 9, B = 38 }));
            ret.Add(Tuple.Create(169, new Color() { A = 255, R = 19, G = 23, B = 38 }));
            ret.Add(Tuple.Create(170, new Color() { A = 255, R = 0, G = 0, B = 255 }));
            ret.Add(Tuple.Create(171, new Color() { A = 255, R = 127, G = 127, B = 255 }));
            ret.Add(Tuple.Create(172, new Color() { A = 255, R = 0, G = 0, B = 165 }));
            ret.Add(Tuple.Create(173, new Color() { A = 255, R = 82, G = 82, B = 165 }));
            ret.Add(Tuple.Create(174, new Color() { A = 255, R = 0, G = 0, B = 127 }));
            ret.Add(Tuple.Create(175, new Color() { A = 255, R = 63, G = 63, B = 127 }));
            ret.Add(Tuple.Create(176, new Color() { A = 255, R = 0, G = 0, B = 76 }));
            ret.Add(Tuple.Create(177, new Color() { A = 255, R = 38, G = 38, B = 76 }));
            ret.Add(Tuple.Create(178, new Color() { A = 255, R = 0, G = 0, B = 38 }));
            ret.Add(Tuple.Create(179, new Color() { A = 255, R = 19, G = 19, B = 38 }));
            ret.Add(Tuple.Create(180, new Color() { A = 255, R = 63, G = 0, B = 255 }));
            ret.Add(Tuple.Create(181, new Color() { A = 255, R = 159, G = 127, B = 255 }));
            ret.Add(Tuple.Create(182, new Color() { A = 255, R = 41, G = 0, B = 165 }));
            ret.Add(Tuple.Create(183, new Color() { A = 255, R = 103, G = 82, B = 165 }));
            ret.Add(Tuple.Create(184, new Color() { A = 255, R = 31, G = 0, B = 127 }));
            ret.Add(Tuple.Create(185, new Color() { A = 255, R = 79, G = 63, B = 127 }));
            ret.Add(Tuple.Create(186, new Color() { A = 255, R = 19, G = 0, B = 76 }));
            ret.Add(Tuple.Create(187, new Color() { A = 255, R = 47, G = 38, B = 76 }));
            ret.Add(Tuple.Create(188, new Color() { A = 255, R = 9, G = 0, B = 38 }));
            ret.Add(Tuple.Create(189, new Color() { A = 255, R = 23, G = 19, B = 38 }));
            ret.Add(Tuple.Create(190, new Color() { A = 255, R = 127, G = 0, B = 255 }));
            ret.Add(Tuple.Create(191, new Color() { A = 255, R = 191, G = 127, B = 255 }));
            ret.Add(Tuple.Create(192, new Color() { A = 255, R = 82, G = 0, B = 165 }));
            ret.Add(Tuple.Create(193, new Color() { A = 255, R = 124, G = 82, B = 165 }));
            ret.Add(Tuple.Create(194, new Color() { A = 255, R = 63, G = 0, B = 127 }));
            ret.Add(Tuple.Create(195, new Color() { A = 255, R = 95, G = 63, B = 127 }));
            ret.Add(Tuple.Create(196, new Color() { A = 255, R = 38, G = 0, B = 76 }));
            ret.Add(Tuple.Create(197, new Color() { A = 255, R = 57, G = 38, B = 76 }));
            ret.Add(Tuple.Create(198, new Color() { A = 255, R = 19, G = 0, B = 38 }));
            ret.Add(Tuple.Create(199, new Color() { A = 255, R = 28, G = 19, B = 38 }));
            ret.Add(Tuple.Create(200, new Color() { A = 255, R = 191, G = 0, B = 255 }));
            ret.Add(Tuple.Create(201, new Color() { A = 255, R = 223, G = 127, B = 255 }));
            ret.Add(Tuple.Create(202, new Color() { A = 255, R = 124, G = 0, B = 165 }));
            ret.Add(Tuple.Create(203, new Color() { A = 255, R = 145, G = 82, B = 165 }));
            ret.Add(Tuple.Create(204, new Color() { A = 255, R = 95, G = 0, B = 127 }));
            ret.Add(Tuple.Create(205, new Color() { A = 255, R = 111, G = 63, B = 127 }));
            ret.Add(Tuple.Create(206, new Color() { A = 255, R = 57, G = 0, B = 76 }));
            ret.Add(Tuple.Create(207, new Color() { A = 255, R = 66, G = 38, B = 76 }));
            ret.Add(Tuple.Create(208, new Color() { A = 255, R = 28, G = 0, B = 38 }));
            ret.Add(Tuple.Create(209, new Color() { A = 255, R = 33, G = 19, B = 38 }));
            ret.Add(Tuple.Create(210, new Color() { A = 255, R = 255, G = 0, B = 255 }));
            ret.Add(Tuple.Create(211, new Color() { A = 255, R = 255, G = 127, B = 255 }));
            ret.Add(Tuple.Create(212, new Color() { A = 255, R = 165, G = 0, B = 165 }));
            ret.Add(Tuple.Create(213, new Color() { A = 255, R = 165, G = 82, B = 165 }));
            ret.Add(Tuple.Create(214, new Color() { A = 255, R = 127, G = 0, B = 127 }));
            ret.Add(Tuple.Create(215, new Color() { A = 255, R = 127, G = 63, B = 127 }));
            ret.Add(Tuple.Create(216, new Color() { A = 255, R = 76, G = 0, B = 76 }));
            ret.Add(Tuple.Create(217, new Color() { A = 255, R = 76, G = 38, B = 76 }));
            ret.Add(Tuple.Create(218, new Color() { A = 255, R = 38, G = 0, B = 38 }));
            ret.Add(Tuple.Create(219, new Color() { A = 255, R = 38, G = 19, B = 38 }));
            ret.Add(Tuple.Create(220, new Color() { A = 255, R = 255, G = 0, B = 191 }));
            ret.Add(Tuple.Create(221, new Color() { A = 255, R = 255, G = 127, B = 223 }));
            ret.Add(Tuple.Create(222, new Color() { A = 255, R = 165, G = 0, B = 124 }));
            ret.Add(Tuple.Create(223, new Color() { A = 255, R = 165, G = 82, B = 145 }));
            ret.Add(Tuple.Create(224, new Color() { A = 255, R = 127, G = 0, B = 95 }));
            ret.Add(Tuple.Create(225, new Color() { A = 255, R = 127, G = 63, B = 111 }));
            ret.Add(Tuple.Create(226, new Color() { A = 255, R = 76, G = 0, B = 57 }));
            ret.Add(Tuple.Create(227, new Color() { A = 255, R = 76, G = 38, B = 66 }));
            ret.Add(Tuple.Create(228, new Color() { A = 255, R = 38, G = 0, B = 28 }));
            ret.Add(Tuple.Create(229, new Color() { A = 255, R = 38, G = 19, B = 33 }));
            ret.Add(Tuple.Create(230, new Color() { A = 255, R = 255, G = 0, B = 127 }));
            ret.Add(Tuple.Create(231, new Color() { A = 255, R = 255, G = 127, B = 191 }));
            ret.Add(Tuple.Create(232, new Color() { A = 255, R = 165, G = 0, B = 82 }));
            ret.Add(Tuple.Create(233, new Color() { A = 255, R = 165, G = 82, B = 124 }));
            ret.Add(Tuple.Create(234, new Color() { A = 255, R = 127, G = 0, B = 63 }));
            ret.Add(Tuple.Create(235, new Color() { A = 255, R = 127, G = 63, B = 95 }));
            ret.Add(Tuple.Create(236, new Color() { A = 255, R = 76, G = 0, B = 38 }));
            ret.Add(Tuple.Create(237, new Color() { A = 255, R = 76, G = 38, B = 57 }));
            ret.Add(Tuple.Create(238, new Color() { A = 255, R = 38, G = 0, B = 19 }));
            ret.Add(Tuple.Create(239, new Color() { A = 255, R = 38, G = 19, B = 28 }));
            ret.Add(Tuple.Create(240, new Color() { A = 255, R = 255, G = 0, B = 63 }));
            ret.Add(Tuple.Create(241, new Color() { A = 255, R = 255, G = 127, B = 159 }));
            ret.Add(Tuple.Create(242, new Color() { A = 255, R = 165, G = 0, B = 41 }));
            ret.Add(Tuple.Create(243, new Color() { A = 255, R = 165, G = 82, B = 103 }));
            ret.Add(Tuple.Create(244, new Color() { A = 255, R = 127, G = 0, B = 31 }));
            ret.Add(Tuple.Create(245, new Color() { A = 255, R = 127, G = 63, B = 79 }));
            ret.Add(Tuple.Create(246, new Color() { A = 255, R = 76, G = 0, B = 19 }));
            ret.Add(Tuple.Create(247, new Color() { A = 255, R = 76, G = 38, B = 47 }));
            ret.Add(Tuple.Create(248, new Color() { A = 255, R = 38, G = 0, B = 9 }));
            ret.Add(Tuple.Create(249, new Color() { A = 255, R = 38, G = 19, B = 23 }));
            ret.Add(Tuple.Create(250, new Color() { A = 255, R = 0, G = 0, B = 0 }));
            ret.Add(Tuple.Create(251, new Color() { A = 255, R = 51, G = 51, B = 51 }));
            ret.Add(Tuple.Create(252, new Color() { A = 255, R = 102, G = 102, B = 102 }));
            ret.Add(Tuple.Create(253, new Color() { A = 255, R = 153, G = 153, B = 153 }));
            ret.Add(Tuple.Create(254, new Color() { A = 255, R = 204, G = 204, B = 204 }));
            ret.Add(Tuple.Create(255, new Color() { A = 255, R = 255, G = 255, B = 255 }));
            return ret;
        }

        private void SyncWithColorPicker()
        {
            // No longer needed as ColorPicker is in a separate window
        }

        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            // Create and show color picker window
            var colorPickerWindow = new ColorPickerWindow();
            
            // Get the parent window
            var parentWindow = Window.GetWindow(this);
            
            // Set up event handlers
            colorPickerWindow.ColorSelected += (s, color) =>
            {
                // Update selected color and index
                _isInternalUpdate = true;
                SelectedColor = color;
                SelectedColorIndex = colorPickerWindow.SelectedColorIndex;
                _isInternalUpdate = false;

                // Thêm màu vào danh sách recent colors
                RecentColorsManager.AddRecentColor(color);

                // Fire events
                ColorChanged?.Invoke(this, color);
            };

            colorPickerWindow.ColorSelectionCancelled += (s, args) =>
            {
                // Do nothing when cancelled
            };

            // Show the color picker window
            colorPickerWindow.ShowColorPicker(SelectedColor, SelectedColorIndex, parentWindow);
        }

        private void ColorPicker_Confirmed(object sender, EventArgs e)
        {
            // This method is no longer used but kept for compatibility
        }

        private void ColorPicker_Cancelled(object sender, EventArgs e)
        {
            // This method is no longer used but kept for compatibility
        }

        private void ColorPicker_ColorChanged(object sender, Color e)
        {
            // This method is no longer used but kept for compatibility
        }

        public CustomPopupPlacement[] CustomPopupPlacementCallback(Size popupSize, Size targetSize, Point offset)
        {
            // This method is no longer used but kept for compatibility
            return new CustomPopupPlacement[]
            {
                new CustomPopupPlacement(new Point(0, targetSize.Height), PopupPrimaryAxis.Vertical)
            };
        }
    }

    /// <summary>
    /// Converter to convert Color to SolidColorBrush
    /// </summary>
    public class ColorToBrushConverter : IValueConverter
    {
        public static readonly ColorToBrushConverter Instance = new ColorToBrushConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                return new SolidColorBrush(color);
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                return brush.Color;
            }
            return Colors.Black;
        }
    }
}
