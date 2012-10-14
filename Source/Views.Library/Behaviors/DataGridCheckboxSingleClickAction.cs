using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interactivity;
using System.Windows;
using Livet.Behaviors;
using System.Windows.Controls;

namespace BluePlumGit.Behaviors
{
    public class DataGridCheckboxSingleClickAction : TriggerAction<DependencyObject>
    {
        private MethodBinderWithArgument _callbackMethod = new MethodBinderWithArgument();

        public static readonly DependencyProperty MethodTargetProperty =
            DependencyProperty.Register("MethodTarget", typeof(object), typeof(DataGridCheckboxSingleClickAction), new PropertyMetadata(null));

        public static readonly DependencyProperty MethodNameProperty =
            DependencyProperty.Register("MethodName", typeof(string), typeof(DataGridCheckboxSingleClickAction), new PropertyMetadata(null));


        public object MethodTarget
        {
            get { return (object)GetValue(MethodTargetProperty); }
            set { SetValue(MethodTargetProperty, value); }
        }

        public string MethodName
        {
            get { return (string)GetValue(MethodNameProperty); }
            set { SetValue(MethodNameProperty, value); }
        }

        protected override void Invoke(object parameter)
        {
            if (MethodTarget == null) return;
            if (MethodName == null) return;

            DataGridCell cell = (DataGridCell)this.AssociatedObject;

            _callbackMethod.Invoke(MethodTarget, MethodName, cell);
        }
    }
}
