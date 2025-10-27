using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using laba_4_2_FNK_BPI2302.helper;
using laba_4_2_FNK_BPI2302.model;
using laba_4_2_FNK_BPI2302.view;

namespace laba_4_2_FNK_BPI2302.viewmodel
{
    public class RoleViewModel : INotifyPropertyChanged
    {
        private Role selectedRole;
        private DataService dataService;

        public Role SelectedRole
        {
            get { return selectedRole; }
            set
            {
                selectedRole = value;
                OnPropertyChanged("SelectedRole");
                if (EditRole != null) EditRole.CanExecute(true);
            }
        }

        public ObservableCollection<Role> ListRole { get; set; }

        public RoleViewModel()
        {
            dataService = new DataService();
            ListRole = dataService.LoadRoles();

            // Если данных нет, добавляем начальные данные
            if (ListRole.Count == 0)
            {
                InitializeDefaultRoles();
                SaveRoles();
            }
        }

        private void InitializeDefaultRoles()
        {
            ListRole.Add(new Role { Id = 1, NameRole = "Директор" });
            ListRole.Add(new Role { Id = 2, NameRole = "Бухгалтер" });
            ListRole.Add(new Role { Id = 3, NameRole = "Менеджер" });
        }

        private void SaveRoles()
        {
            dataService.SaveRoles(ListRole);
        }

        public int MaxId()
        {
            int max = 0;
            foreach (var r in this.ListRole)
            {
                if (max < r.Id)
                {
                    max = r.Id;
                }
            }
            return max;
        }

        #region Команды для ролей

        private RelayCommand addRole;
        public RelayCommand AddRole
        {
            get
            {
                return addRole ??
                    (addRole = new RelayCommand(obj =>
                    {
                        WindowNewRole wnRole = new WindowNewRole
                        {
                            Title = "Новая должность",
                        };

                        int maxIdRole = MaxId() + 1;
                        Role role = new Role { Id = maxIdRole };
                        wnRole.DataContext = role;

                        if (wnRole.ShowDialog() == true)
                        {
                            ListRole.Add(role);
                            SaveRoles();
                        }

                        SelectedRole = role;
                    }));
            }
        }

        private RelayCommand editRole;
        public RelayCommand EditRole
        {
            get
            {
                return editRole ??
                    (editRole = new RelayCommand(obj =>
                    {
                        WindowNewRole wnRole = new WindowNewRole
                        {
                            Title = "Редактирование должности",
                        };

                        Role role = SelectedRole;
                        Role tempRole = new Role();
                        tempRole = role.ShallowCopy();
                        wnRole.DataContext = tempRole;

                        if (wnRole.ShowDialog() == true)
                        {
                            role.NameRole = tempRole.NameRole;
                            SaveRoles();
                        }
                    },
                    (obj) => SelectedRole != null && ListRole.Count > 0));
            }
        }

        private RelayCommand deleteRole;
        public RelayCommand DeleteRole
        {
            get
            {
                return deleteRole ??
                    (deleteRole = new RelayCommand(obj =>
                    {
                        Role role = SelectedRole;
                        MessageBoxResult result = MessageBox.Show(
                            "Удалить данные по должности: " + role.NameRole,
                            "Предупреждение",
                            MessageBoxButton.OKCancel,
                            MessageBoxImage.Warning);

                        if (result == MessageBoxResult.OK)
                        {
                            ListRole.Remove(role);
                            SaveRoles();
                        }
                    },
                    (obj) => SelectedRole != null && ListRole.Count > 0));
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
