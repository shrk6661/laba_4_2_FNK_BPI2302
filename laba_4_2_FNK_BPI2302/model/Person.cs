using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using laba_4_2_FNK_BPI2302.viewmodel;

namespace laba_4_2_FNK_BPI2302.model
{
    public class Person
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }

        public Person() { }

        public Person(int id, int roleId, string firstName, string lastName, DateTime birthday)
        {
            Id = id;
            RoleId = roleId;
            FirstName = firstName;
            LastName = lastName;
            Birthday = birthday;
        }

        public Person CopyFromPersonDPO(PersonDpo personDpo)
        {
            Person person = new Person();
            person.Id = personDpo.Id;
            person.FirstName = personDpo.FirstName;
            person.LastName = personDpo.LastName;
            person.Birthday = personDpo.Birthday;

            var roleViewModel = new RoleViewModel();
            foreach (var role in roleViewModel.ListRole)
            {
                if (role.NameRole == personDpo.RoleName)
                {
                    person.RoleId = role.Id;
                    break;
                }
            }

            return person;
        }
    }
}
