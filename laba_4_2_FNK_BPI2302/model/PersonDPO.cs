using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using laba_4_2_FNK_BPI2302.viewmodel;

namespace laba_4_2_FNK_BPI2302.model
{
    public class PersonDpo : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        private string _birthday;
        public string Birthday
        {
            get { return _birthday; }
            set
            {
                _birthday = value;
                OnPropertyChanged("Birthday");
            }
        }

        public PersonDpo() { }

        public PersonDpo ShallowCopy()
        {
            return (PersonDpo)this.MemberwiseClone();
        }

        public PersonDpo CopyFromPerson(Person person, RoleViewModel vmRole)
        {
            this.Id = person.Id;
            this.FirstName = person.FirstName;
            this.LastName = person.LastName;
            this.Birthday = person.Birthday;
            this.RoleId = person.RoleId;

            foreach (var role in vmRole.ListRole)
            {
                if (role.Id == person.RoleId)
                {
                    this.RoleName = role.NameRole;
                    break;
                }
            }
            return this;
        }

        public Person CopyToPerson(RoleViewModel vmRole)
        {
            var person = new Person();
            person.Id = this.Id;
            person.FirstName = this.FirstName;
            person.LastName = this.LastName;
            person.Birthday = this.Birthday;
            person.RoleId = this.RoleId;
            return person;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}