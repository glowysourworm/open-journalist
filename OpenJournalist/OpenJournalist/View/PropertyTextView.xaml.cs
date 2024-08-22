using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OpenJournalist.View
{
    public partial class PropertyTextView : UserControl
    {
        public static readonly DependencyProperty PropertyNameProperty = 
            DependencyProperty.Register("PropertyName", typeof(string), typeof(PropertyTextView), new PropertyMetadata(OnChanged));

        public static readonly DependencyProperty PropertyValueProperty =
            DependencyProperty.Register("PropertyValue", typeof(string), typeof(PropertyTextView), new PropertyMetadata(OnChanged));

        public static readonly DependencyProperty IsReadonlyProperty =
            DependencyProperty.Register("IsReadonly", typeof(bool), typeof(PropertyTextView), new PropertyMetadata(OnChanged));

        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }

        public string PropertyValue
        {
            get { return (string)GetValue(PropertyValueProperty); }
            set { SetValue(PropertyValueProperty, value); }
        }

        public bool IsReadonly
        {
            get { return (bool)GetValue(IsReadonlyProperty); }
            set { SetValue(IsReadonlyProperty, value); }
        }

        public PropertyTextView()
        {
            InitializeComponent();
        }

        private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PropertyTextView)d;

            control.LabelTB.Text = control.PropertyName;
            control.ValueTB.IsReadOnly = control.IsReadonly;
            control.ValueTB.Text = control.PropertyValue;
        }
    }
}
