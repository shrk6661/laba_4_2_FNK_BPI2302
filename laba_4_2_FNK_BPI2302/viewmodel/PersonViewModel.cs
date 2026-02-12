using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using laba_4_2_FNK_BPI2302.helper;
using laba_4_2_FNK_BPI2302.model;
using laba_4_2_FNK_BPI2302.view;
using Newtonsoft.Json;

namespace laba_4_2_FNK_BPI2302.viewmodel
{
    public class PersonViewModel : INotifyPropertyChanged
    {
        readonly string path = @"D:\blablabla\laba_4_2_FNK_BPI2302\laba_4_2_FNK_BPI2302\DataModels\PersonData.json";

        private PersonDpo _selectedPersonDpo;
        public PersonDpo SelectedPersonDpo
        {
            get { return _selectedPersonDpo; }
            set
            {
                _selectedPersonDpo = value;
                OnPropertyChanged("SelectedPersonDpo");
            }
        }

        public ObservableCollection<Person> ListPerson { get; set; }
        public ObservableCollection<PersonDpo> ListPersonDpo { get; set; }

        public PersonViewModel()
        {
            ListPerson = new ObservableCollection<Person>();
            ListPersonDpo = new ObservableCollection<PersonDpo>();

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                if (File.Exists(path))
                {
                    var json = File.ReadAllText(path);
                    ListPerson = JsonConvert.DeserializeObject<ObservableCollection<Person>>(json)
                        ?? new ObservableCollection<Person>();
                }
            }
            catch { }

            if (ListPerson.Count == 0)
            {
                ListPerson.Add(new Person { Id = 1, RoleId = 1, FirstName = "Иван", LastName = "Иванов", Birthday = "28.02.1980" });
                ListPerson.Add(new Person { Id = 2, RoleId = 2, FirstName = "Петр", LastName = "Петров", Birthday = "20.03.1981" });
                ListPerson.Add(new Person { Id = 3, RoleId = 3, FirstName = "Виктор", LastName = "Викторов", Birthday = "15.04.1982" });
                ListPerson.Add(new Person { Id = 4, RoleId = 3, FirstName = "Сидор", LastName = "Сидоров", Birthday = "10.05.1983" });
                SaveData();
            }

            UpdatePersonDpoList();
        }

        private void SaveData()
        {
            try
            {
                var json = JsonConvert.SerializeObject(ListPerson, Formatting.Indented);
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message);
            }
        }

        private void UpdatePersonDpoList()
        {
            ListPersonDpo.Clear();
            var roleVM = new RoleViewModel();

            foreach (var person in ListPerson)
            {
                var p = new PersonDpo();
                p.CopyFromPerson(person, roleVM);
                ListPersonDpo.Add(p);
            }
        }

        public int MaxId()
        {
            return ListPerson.Count > 0 ? ListPerson.Max(x => x.Id) : 0;
        }

        private RelayCommand _addPerson;
        public RelayCommand AddPerson
        {
            get
            {
                return _addPerson ?? (_addPerson = new RelayCommand(obj =>
                {
                    var wnPerson = new WindowNewEmployee();
                    var newPerson = new PersonDpo
                    {
                        Id = MaxId() + 1,
                        Birthday = DateTime.Now.ToString("dd.MM.yyyy")
                    };
                    wnPerson.DataContext = newPerson;

                    if (wnPerson.ShowDialog() == true)
                    {
                        var role = (Role)wnPerson.CbRole.SelectedItem;
                        if (role != null)
                        {
                            newPerson.RoleId = role.Id;
                            newPerson.RoleName = role.NameRole;

                            ListPersonDpo.Add(newPerson);

                            var roleVM = new RoleViewModel();
                            var person = newPerson.CopyToPerson(roleVM);
                            ListPerson.Add(person);

                            SaveData();
                        }
                    }
                }));
            }
        }

        private RelayCommand _editPerson;
        public RelayCommand EditPerson
        {
            get
            {
                return _editPerson ?? (_editPerson = new RelayCommand(obj =>
                {
                    if (SelectedPersonDpo == null) return;

                    var wnPerson = new WindowNewEmployee();
                    var tempPerson = SelectedPersonDpo.ShallowCopy();
                    wnPerson.DataContext = tempPerson;

                    if (wnPerson.ShowDialog() == true)
                    {
                        var role = (Role)wnPerson.CbRole.SelectedItem;
                        if (role != null)
                        {
                            SelectedPersonDpo.RoleId = role.Id;
                            SelectedPersonDpo.RoleName = role.NameRole;
                            SelectedPersonDpo.FirstName = tempPerson.FirstName;
                            SelectedPersonDpo.LastName = tempPerson.LastName;
                            SelectedPersonDpo.Birthday = tempPerson.Birthday;

                            var person = ListPerson.FirstOrDefault(p => p.Id == SelectedPersonDpo.Id);
                            if (person != null)
                            {
                                var roleVM = new RoleViewModel();
                                person.RoleId = role.Id;
                                person.FirstName = SelectedPersonDpo.FirstName;
                                person.LastName = SelectedPersonDpo.LastName;
                                person.Birthday = SelectedPersonDpo.Birthday;

                                SaveData();
                            }
                        }
                    }
                }, (obj) => SelectedPersonDpo != null));
            }
        }

        private RelayCommand _deletePerson;
        public RelayCommand DeletePerson
        {
            get
            {
                return _deletePerson ?? (_deletePerson = new RelayCommand(obj =>
                {
                    if (SelectedPersonDpo == null) return;

                    var result = MessageBox.Show($"Удалить сотрудника {SelectedPersonDpo.LastName} {SelectedPersonDpo.FirstName}?",
                        "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        var person = ListPerson.FirstOrDefault(p => p.Id == SelectedPersonDpo.Id);
                        if (person != null)
                        {
                            ListPerson.Remove(person);
                            ListPersonDpo.Remove(SelectedPersonDpo);
                            SaveData();
                        }
                    }
                }, (obj) => SelectedPersonDpo != null));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}