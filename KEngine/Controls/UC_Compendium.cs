using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using KEngine.Classes;
using KEngine.Classes.Entities;
using KEngine.Enums;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace KEngine.Controls
{
    public class UC_Compendium<T> : UserControl where T : KBaseEntityGen<T>, new()
    {
        private BackgroundPanel _bpMain;
        private BarManager _bmMain;
        private Bar _bMainMenu;
        private ListBoxEdit _lbeMain;

        private BarButtonItem _btn_Refresh;
        private BarButtonItem _btn_Add;
        private BarButtonItem _btn_Edit;
        private BarButtonItem _btn_Delete;

        private BarEditItem _bei_Filter;

        private UC_EditorBase<T> _editor = new UC_EditorBase<T>();

        private ObservableCollection<T> _mainSource = null;
        private ObservableCollection<T> _filteredSource = new ObservableCollection<T>();

        public UC_Compendium()
        {
            _initializeComponents();
            _initShortcuts();
            _btn_Refresh.PerformClick();
        }

        private void _initShortcuts()
        {
            RoutedCommand rcRefresh = new RoutedCommand();
            rcRefresh.InputGestures.Add(new KeyGesture(Key.F5, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(rcRefresh, _cbRefresh));

            RoutedCommand rcAdd = new RoutedCommand();
            rcAdd.InputGestures.Add(new KeyGesture(Key.F5));
            CommandBindings.Add(new CommandBinding(rcAdd, _cbAdd));

            RoutedCommand rcEdit = new RoutedCommand();
            rcEdit.InputGestures.Add(new KeyGesture(Key.F4));
            CommandBindings.Add(new CommandBinding(rcEdit, _cbEdit));

            RoutedCommand rcDelete = new RoutedCommand();
            rcDelete.InputGestures.Add(new KeyGesture(Key.F8));
            CommandBindings.Add(new CommandBinding(rcDelete, _cbDelete));
        }

        private void _cbRefresh(object sender, ExecutedRoutedEventArgs e)
        {
            _btn_Refresh.PerformClick();
        }
        private void _cbAdd(object sender, ExecutedRoutedEventArgs e)
        {
            _btn_Add.PerformClick();
        }
        private void _cbEdit(object sender, ExecutedRoutedEventArgs e)
        {
            _btn_Edit.PerformClick();
        }
        private void _cbDelete(object sender, ExecutedRoutedEventArgs e)
        {
            _btn_Delete.PerformClick();
        }

        private void _initializeComponents()
        {
            _bpMain = new BackgroundPanel();
            this.Content = _bpMain;
            _bmMain = new BarManager() { AllowCustomization = false, AllowQuickCustomization = false };
            _bpMain.Content = _bmMain;
            _bMainMenu = new Bar() { IsMainMenu = true, AllowCustomizationMenu = false, AllowQuickCustomization = DevExpress.Utils.DefaultBoolean.False, DockInfo = new BarDockInfo() { ContainerType = BarContainerType.Top } };
            _bmMain.Bars.Add(_bMainMenu);

            _btn_Refresh = new BarButtonItem() { Content = "Refresh", Glyph = new BitmapImage(((DXImageInfo)new DXImageConverter().ConvertFromString("Refresh_16x16.png")).MakeUri()) };
            _btn_Refresh.ItemClick += _btn_Refresh_ItemClick;
            _btn_Add = new BarButtonItem() { Content = "Add", Glyph = new BitmapImage(((DXImageInfo)new DXImageConverter().ConvertFromString("AddNewDataSource_16x16.png")).MakeUri()) };
            _btn_Add.ItemClick += _btn_Add_ItemClick;
            _btn_Edit = new BarButtonItem() { Content = "Edit", Glyph = new BitmapImage(((DXImageInfo)new DXImageConverter().ConvertFromString("EditDataSource_16x16.png")).MakeUri()) };
            _btn_Edit.ItemClick += _btn_Edit_ItemClick;
            _btn_Delete = new BarButtonItem() { Content = "Delete", Glyph = new BitmapImage(((DXImageInfo)new DXImageConverter().ConvertFromString("DeleteDataSource_16x16.png")).MakeUri()) };
            _btn_Delete.ItemClick += _btn_Delete_ItemClick;

            _bei_Filter = new BarEditItem() { EditSettings = new TextEditSettings(), EditWidth = 150, Content = "Filter" };
            _bei_Filter.EditValueChanged += _bei_Filter_EditValueChanged;

            _bMainMenu.Items.Add(_btn_Refresh);
            _bMainMenu.Items.Add(_btn_Add);
            _bMainMenu.Items.Add(_btn_Edit);
            _bMainMenu.Items.Add(_btn_Delete);

            _bMainMenu.Items.Add(_bei_Filter);

            _bmMain.Child = _generateView();
        }

        private void _bei_Filter_EditValueChanged(object sender, RoutedEventArgs e)
        {
            _filteredSource.Clear();
            var comp = StringComparison.OrdinalIgnoreCase;
            string flt = string.Empty;
            if (_bei_Filter.EditValue != null) flt = (string)_bei_Filter.EditValue;
            if (string.IsNullOrWhiteSpace(flt))
            {
                foreach (var v in _mainSource.OrderBy(a => a.ListBoxDisplay)) _filteredSource.Add(v);
            }
            else
            {
                foreach (var v in _mainSource.Where(a => !a.ListBoxDisplay.IndexOf(flt, comp).Equals(-1)).OrderBy(a => a.ListBoxDisplay))
                    _filteredSource.Add(v);
            }
        }

        private UIElement _generateView()
        {
            _lbeMain = new ListBoxEdit() { DisplayMember = "ListBoxDisplay" };
            _lbeMain.MouseDoubleClick += _lbeMain_MouseDoubleClick;
            return _lbeMain;
        }

        private void _lbeMain_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_lbeMain.SelectedItem != null) _btn_Edit.PerformClick();
        }

        private void _btn_Delete_ItemClick(object sender, ItemClickEventArgs e)
        {

            var v = _lbeMain.SelectedItem;
            if (v != null)
            {
                ((T)v).Delete();
                _mainSource.Remove(v as T);
                _filteredSource.Remove(v as T);
            }
            else KCore.ShowKError("Nothing Selected!");
        }

        private void _btn_Edit_ItemClick(object sender, ItemClickEventArgs e)
        {
            var v = _lbeMain.SelectedItem;
            if (v != null) _initEditor(v as T, EUC_EditorMode.Edit);
            else KCore.ShowKError("Nothing Selected!");
        }

        private void _btn_Add_ItemClick(object sender, ItemClickEventArgs e)
        {
            var v = (new T()).New();
            if (v != null) _initEditor(v as T);
            else KCore.ShowKError("Failed to create new entity!");
        }

        private void _initEditor(T v, EUC_EditorMode m = EUC_EditorMode.Add)
        {
            string s = string.Empty;
            switch (m)
            {
                case EUC_EditorMode.Add:
                    s = "ADD";
                    break;

                case EUC_EditorMode.Edit:
                    s = "EDIT";
                    break;

                default:
                    s = "UNK";
                    break;
            }
            Window wnd = KCore.GenerateWindow($"Dynamic KEditor [{typeof(T).Name}] {{{s}}}", _editor, Window.GetWindow(this));
            _editor.Init(v);
            wnd.ShowDialog();
            if (((T)v).ID != null && m == EUC_EditorMode.Add)
            {
                _mainSource.Add(v as T);
                _filteredSource.Add(v as T);
            }
        }

        private void _btn_Refresh_ItemClick(object sender, ItemClickEventArgs e)
        {
            _mainSource = (new T()).LoadAll();
            _filteredSource.Clear();
            foreach (var v in _mainSource.OrderBy(a => a.ListBoxDisplay)) _filteredSource.Add(v);
            _lbeMain.ItemsSource = _filteredSource;
        }
    }
}
