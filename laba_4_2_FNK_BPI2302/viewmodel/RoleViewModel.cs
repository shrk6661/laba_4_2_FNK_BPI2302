using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using laba_4_2_FNK_BPI2302.helper;
using laba_4_2_FNK_BPI2302.model;
using laba_4_2_FNK_BPI2302.view;
using Newtonsoft.Json;

namespace laba_4_2_FNK_BPI2302.viewmodel
{
    public class RoleViewModel : INotifyPropertyChanged
    {
       
        readonly string path = @"..\..\DataModels\PersonData.json";

        
        string _jsonRoles = String.Empty;
        public string Error { get; set; }

        private Role selectedRole;

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
            ListRole = new ObservableCollection<Role>();
            ListRole = LoadRole();

            // если данных нет, добавляем начальные данные и сохраняем в JSON
            if (ListRole.Count == 0)
            {
                InitializeDefaultRoles();
                SaveChanges(ListRole);
            }
        }

        private void InitializeDefaultRoles()
        {
            ListRole.Add(new Role { Id = 1, NameRole = "Директор" });
            ListRole.Add(new Role { Id = 2, NameRole = "Бухгалтер" });
            ListRole.Add(new Role { Id = 3, NameRole = "Менеджер" });
        }

        public ObservableCollection<Role> LoadRole()
        {
            try
            {
                if (File.Exists(path))
                {
                    _jsonRoles = File.ReadAllText(path);

                    if (!string.IsNullOrEmpty(_jsonRoles))
                    {
                        ListRole = JsonConvert.DeserializeObject<ObservableCollection<Role>>(_jsonRoles);
                        return ListRole;
                    }
                }
                return new ObservableCollection<Role>();
            }
            catch (Exception e)
            {
                Error = "Ошибка загрузки json файла \n" + e.Message;
                MessageBox.Show("Ошибка загрузки данных: " + e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return new ObservableCollection<Role>();
            }
        }

        private void SaveChanges(ObservableCollection<Role> listRole)
        {
            try
            {
                // СЕРИАЛИЗАЦИЯ КОЛЛЕКЦИИ В JSON
                var jsonRole = JsonConvert.SerializeObject(listRole, Formatting.Indented);

                // ЗАПИСЬ В ФАЙЛ
                using (StreamWriter writer = File.CreateText(path))
                {
                    writer.Write(jsonRole);
                }
            }
            catch (IOException e)
            {
                Error = "Ошибка записи json файла \n" + e.Message;
                MessageBox.Show("Ошибка сохранения данных: " + e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception e)
            {
                Error = "Неизвестная ошибка \n" + e.Message;
                MessageBox.Show("Ошибка: " + e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                            SaveChanges(ListRole); // СОХРАНЯЕМ В JSON
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
                        Role tempRole = role.ShallowCopy();
                        wnRole.DataContext = tempRole;

                        if (wnRole.ShowDialog() == true)
                        {
                            role.NameRole = tempRole.NameRole;
                            SaveChanges(ListRole); // СОХРАНЯЕМ В JSON
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
                            SaveChanges(ListRole); // СОХРАНЯЕМ В JSON
                        }
                    },
                    (obj) => SelectedRole != null && ListRole.Count > 0));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}