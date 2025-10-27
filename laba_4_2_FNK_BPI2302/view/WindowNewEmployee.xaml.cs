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
using System.Windows.Shapes;
using laba_4_2_FNK_BPI2302.model;
using laba_4_2_FNK_BPI2302.viewmodel;

namespace laba_4_2_FNK_BPI2302.view
{

    public partial class WindowNewEmployee : Window
    {
        public ComboBox CbRole => cbRole;

        public WindowNewEmployee()
        {
            InitializeComponent();
            Loaded += WindowNewEmployee_Loaded;
        }

        private void WindowNewEmployee_Loaded(object sender, RoutedEventArgs e)
        {
            var roleViewModel = new RoleViewModel();
            cbRole.ItemsSource = roleViewModel.ListRole;

            var person = DataContext as PersonDpo;
            if (person != null && !string.IsNullOrEmpty(person.RoleName))
            {
                foreach (Role role in cbRole.Items)
                {
                    if (role.NameRole == person.RoleName)
                    {
                        cbRole.SelectedItem = role;
                        break;
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}