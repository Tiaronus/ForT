using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.LayoutControl;
using KEngine.Attributes.UIA;
using KEngine.Classes.Entities;
using KEngine.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace KEngine.Controls
{
    public class UC_EditorBase<T> : UserControl where T : KBaseEntityGen<T>, new()
    {
        private T Entity { get; set; }

        private Dictionary<string, BindingExpressionBase> Bindings = new Dictionary<string, BindingExpressionBase>();

        public UC_EditorBase() { }

        public UC_EditorBase(T e)
        {
            Init(e);
        }

        public void Init(T e)
        {
            Bindings.Clear();
            Entity = e;
            InitComponents();
            _initShortcuts();
        }

        private void _initShortcuts()
        {
            RoutedCommand rcCancel = new RoutedCommand();
            rcCancel.InputGestures.Add(new KeyGesture(Key.Escape));
            ((Window)this.Parent).CommandBindings.Add(new CommandBinding(rcCancel, _btnCancel_Click));

            RoutedCommand rcSave = new RoutedCommand();
            rcSave.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            ((Window)this.Parent).CommandBindings.Add(new CommandBinding(rcSave, _btnSave_Click));
        }

        private BindingExpressionBase _initBinding(string name, FrameworkElement fe, DependencyProperty dp)
        {
            Binding b = new Binding(name);
            b.Source = Entity;
            b.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
            b.Mode = BindingMode.TwoWay;
            return fe.SetBinding(dp, b);
        }

        private void InitComponents()
        {
            this.Content = null;
            BackgroundPanel _pnl = new BackgroundPanel() { MinWidth = 400, MinHeight = 200 };
            Content = _pnl;
            LayoutControl _lc = new LayoutControl();
            _pnl.Content = _lc;
            LayoutGroup _lg = _lc.CreateGroup();
            _lc.Children.Add(_lg);
            _lg.View = LayoutGroupView.GroupBox;
            _lg.Orientation = Orientation.Vertical;
            _lg.Header = typeof(T).Name;
            _lg.Margin = new System.Windows.Thickness(5);
            foreach (var prop in typeof(T).GetProperties().AsEnumerable().Where(a => Attribute.IsDefined(a, typeof(UIAPropertyAttribute))))
            {
                foreach (UIAPropertyAttribute attr in prop.GetCustomAttributes(typeof(UIAPropertyAttribute), true))
                {
                    LayoutItem _i = new LayoutItem() { Label = attr.Label };
                    object e = null;
                    switch (attr.EditorType)
                    {
                        case EUIAEditorType.ComboBox:
                            Type tttt = attr.TypeOfValues;
                            e = new ComboBoxEdit();
                            _i.Content = e as UIElement;
                            ((ComboBoxEdit)e).ValueMember = "ID";
                            ((ComboBoxEdit)e).DisplayMember = "ListBoxDisplay";
                            ((ComboBoxEdit)e).AutoComplete = true;
                            var _v = Activator.CreateInstance(attr.TypeOfValues);
                            MethodInfo _mi = tttt.GetMethod("LoadAll");
                            if (_mi != null) ((ComboBoxEdit)e).ItemsSource = _mi.Invoke(_v, null);
                            Bindings.Add(prop.Name, _initBinding(prop.Name, ((ComboBoxEdit)_i.Content) as FrameworkElement, ComboBoxEdit.EditValueProperty));
                            break;

                        case EUIAEditorType.IntEdit:
                            e = new SpinEdit();
                            ((SpinEdit)e).IsFloatValue = false;
                            _i.Content = e as UIElement;
                            Bindings.Add(prop.Name, _initBinding(prop.Name, ((SpinEdit)_i.Content) as FrameworkElement, SpinEdit.EditValueProperty));
                            break;

                        case EUIAEditorType.DecEdit:
                            e = new TextEdit();
                            ((TextEdit)e).MaskType = MaskType.Numeric;
                            _i.Content = e as UIElement;
                            Bindings.Add(prop.Name, _initBinding(prop.Name, ((TextEdit)_i.Content) as FrameworkElement, TextEdit.TextProperty));
                            break;

                        case EUIAEditorType.IPEdit:
                            _i.Content = new TextEdit();
                            ((TextEdit)_i.Content).MaskType = MaskType.RegEx;
                            ((TextEdit)_i.Content).Mask = @"((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)";
                            foreach (StringLengthAttribute propSL in prop.GetCustomAttributes(typeof(StringLengthAttribute), true)) ((TextEdit)_i.Content).MaxLength = propSL.MaximumLength;
                            Bindings.Add(prop.Name, _initBinding(prop.Name, ((TextEdit)_i.Content) as FrameworkElement, TextEdit.TextProperty));
                            break;

                        case EUIAEditorType.PassEdit:
                            _i.Content = new PasswordBoxEdit();
                            Bindings.Add(prop.Name, _initBinding(prop.Name, ((PasswordBoxEdit)_i.Content) as FrameworkElement, PasswordBoxEdit.PasswordProperty));
                            break;

                        case EUIAEditorType.DateEdit:
                            _i.Content = new DateEdit();
                            Bindings.Add(prop.Name, _initBinding(prop.Name, ((DateEdit)_i.Content) as FrameworkElement, DateEdit.DateTimeProperty));
                            break;
                                

                        case EUIAEditorType.TextEdit:
                        default:
                            _i.Content = new TextEdit();
                            foreach (StringLengthAttribute propSL in prop.GetCustomAttributes(typeof(StringLengthAttribute), true))
                            {
                                ((TextEdit)_i.Content).MaxLength = propSL.MaximumLength;
                            }
                            Bindings.Add(prop.Name, _initBinding(prop.Name, ((TextEdit)_i.Content) as FrameworkElement, TextEdit.TextProperty));
                            break;
                    }
                    _lg.Children.Add(_i);
                }

            }
            LayoutGroup _lgC = _lc.CreateGroup();
            _lg.Children.Add(_lgC);

            Button _btnSave = new Button() { Content = "Применить" };
            Button _btnCancel = new Button() { Content = "Отменить" };

            LayoutItem li_Save = new LayoutItem() { Content = _btnSave };
            LayoutItem li_Cancel = new LayoutItem() { Content = _btnCancel };

            _btnSave.Click += _btnSave_Click;
            _btnCancel.Click += _btnCancel_Click;

            _lgC.Children.Add(li_Save);
            _lgC.Children.Add(li_Cancel);
        }

        private void _btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _closeOwner();
        }

        private void _btnSave_Click(object sender, RoutedEventArgs e)
        {
            foreach (BindingExpression b in Bindings.Values) b.UpdateSource();
            Entity.ForceLBDChanged();
            Entity.Save();
            _closeOwner();
        }

        private void _closeOwner()
        {
            ((Window)this.Parent).Close();
        }
    }
}
