using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using laba_4_2_FNK_BPI2302.helper;
using laba_4_2_FNK_BPI2302.model;
using laba_4_2_FNK_BPI2302.view;

namespace laba_4_2_FNK_BPI2302.viewmodel
{
    public class PersonViewModel : INotifyPropertyChanged
    {
        private PersonDpo selectedPersonDpo;
        private DataService dataService;

        public PersonDpo SelectedPersonDpo
        {
            get { return selectedPersonDpo; }
            set
            {
                selectedPersonDpo = value;
                OnPropertyChanged("SelectedPersonDpo");
            }
        }

        public ObservableCollection<Person> ListPerson { get; set; }
        public ObservableCollection<PersonDpo> ListPersonDpo { get; set; }

        public PersonViewModel()
        {
            dataService = new DataService();
            ListPerson = dataService.LoadPersons();
            ListPersonDpo = new ObservableCollection<PersonDpo>();

            // если данных нет, добавляем начальные данные
            if (ListPerson.Count == 0)
            {
                InitializeDefaultPersons();
                SavePersons();
            }

            ListPersonDpo = GetListPersonDpo();
        }

        private void InitializeDefaultPersons()
        {
            ListPerson.Add(new Person { Id = 1, RoleId = 1, FirstName = "Иван", LastName = "Иванов", Birthday = new DateTime(1980, 2, 28) });
            ListPerson.Add(new Person { Id = 2, RoleId = 2, FirstName = "Петр", LastName = "Петров", Birthday = new DateTime(1981, 3, 20) });
            ListPerson.Add(new Person { Id = 3, RoleId = 3, FirstName = "Виктор", LastName = "Викторов", Birthday = new DateTime(1982, 4, 15) });
            ListPerson.Add(new Person { Id = 4, RoleId = 3, FirstName = "Сидор", LastName = "Сидоров", Birthday = new DateTime(1983, 5, 10) });
        }

        private void SavePersons()
        {
            dataService.SavePersons(ListPerson);
        }

        public ObservableCollection<PersonDpo> GetListPersonDpo()
        {
            ListPersonDpo.Clear();
            foreach (var person in ListPerson)
            {
                PersonDpo p = new PersonDpo();
                p = p.CopyFromPerson(person);
                ListPersonDpo.Add(p);
            }
            return ListPersonDpo;
        }

        public int MaxId()
        {
            int max = 0;
            foreach (var r in this.ListPerson)
            {
                if (max < r.Id)
                {
                    max = r.Id;
                }
            }
            return max;
        }

        #region Команды для сотрудников

        private RelayCommand addPerson;
        public RelayCommand AddPerson
        {
            get
            {
                return addPerson ??
                    (addPerson = new RelayCommand(obj =>
                    {
                        WindowNewEmployee wnPerson = new WindowNewEmployee
                        {
                            Title = "Новый сотрудник"
                        };

                        int maxIdPerson = MaxId() + 1;
                        PersonDpo per = new PersonDpo
                        {
                            Id = maxIdPerson,
                            Birthday = DateTime.Now
                        };

                        wnPerson.DataContext = per;

                        if (wnPerson.ShowDialog() == true)
                        {
                            Role r = (Role)wnPerson.CbRole.SelectedItem;
                            if (r != null)
                            {
                                per.RoleName = r.NameRole;
                                ListPersonDpo.Add(per);

                                Person p = new Person();
                                p = p.CopyFromPersonDPO(per);
                                ListPerson.Add(p);
                                SavePersons();
                            }
                        }
                    }));
            }
        }

        private RelayCommand editPerson;
        public RelayCommand EditPerson
        {
            get
            {
                return editPerson ??
                    (editPerson = new RelayCommand(obj =>
                    {
                        WindowNewEmployee wnPerson = new WindowNewEmployee()
                        {
                            Title = "Редактирование данных сотрудника",
                        };

                        PersonDpo personDpo = SelectedPersonDpo;
                        PersonDpo tempPerson = new PersonDpo();
                        tempPerson = personDpo.ShallowCopy();
                        wnPerson.DataContext = tempPerson;

                        if (wnPerson.ShowDialog() == true)
                        {
                            Role r = (Role)wnPerson.CbRole.SelectedItem;
                            if (r != null)
                            {
                                personDpo.RoleName = r.NameRole;
                                personDpo.FirstName = tempPerson.FirstName;
                                personDpo.LastName = tempPerson.LastName;
                                personDpo.Birthday = tempPerson.Birthday;

                                FindPerson finder = new FindPerson(personDpo.Id);
                                var listPerson = new List<Person>(ListPerson);
                                Person p = listPerson.Find(new Predicate<Person>(finder.PersonPredicate));
                                if (p != null)
                                {
                                    p = p.CopyFromPersonDPO(personDpo);
                                    ListPerson.Clear();
                                    foreach (var person in listPerson)
                                    {
                                        ListPerson.Add(person);
                                    }
                                    SavePersons();
                                }
                            }
                        }
                    },
                    (obj) => SelectedPersonDpo != null && ListPersonDpo.Count > 0));
            }
        }

        private RelayCommand deletePerson;
        public RelayCommand DeletePerson
        {
            get
            {
                return deletePerson ??
                (deletePerson = new RelayCommand(obj =>
                {
                    PersonDpo person = SelectedPersonDpo;
                    MessageBoxResult result = MessageBox.Show(
                        "Удалить данные по сотруднику: \n" + person.LastName + " " + person.FirstName,
                        "Предупреждение",
                        MessageBoxButton.OKCancel,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.OK)
                    {
                        // находим соответствующий Person 
                        FindPerson finder = new FindPerson(person.Id);
                        var listPerson = new List<Person>(ListPerson);
                        Person per = listPerson.Find(new Predicate<Person>(finder.PersonPredicate));

                        if (per != null)
                        {
                            ListPerson.Remove(per);
                            ListPersonDpo.Remove(person);
                            SavePersons();

                            if (ListPersonDpo.Count > 0)
                                SelectedPersonDpo = ListPersonDpo[0];
                            else
                                SelectedPersonDpo = null;
                        }
                    }
                },
                (obj) => SelectedPersonDpo != null && ListPersonDpo.Count > 0));
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