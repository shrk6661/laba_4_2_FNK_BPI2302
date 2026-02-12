using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using laba_4_2_FNK_BPI2302.viewmodel;
using Newtonsoft.Json;

namespace laba_4_2_FNK_BPI2302.model
{
    public class Person
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Birthday КАК string ДЛЯ JSON СЕРИАЛИЗАЦИИ
        public string Birthday { get; set; }

        public Person() { }

        public Person(int id, int roleId, string firstName, string lastName, string birthday)
        {
            Id = id;
            RoleId = roleId;
            FirstName = firstName;
            LastName = lastName;
            Birthday = birthday;
        }

        public Person CopyFromPersonDPO(PersonDpo personDpo, RoleViewModel roleViewModel)
        {
            this.Id = personDpo.Id;
            this.FirstName = personDpo.FirstName;
            this.LastName = personDpo.LastName;
            this.Birthday = GetStringFromBirthday(personDpo.Birthday);

            if (roleViewModel != null && roleViewModel.ListRole != null)
            {
                foreach (var role in roleViewModel.ListRole)
                {
                    if (role.NameRole == personDpo.RoleName)
                    {
                        this.RoleId = role.Id;
                        break;
                    }
                }
            }
            return this;
        }

        public Person CopyFromPersonDPO(PersonDpo personDpo)
        {
            var roleViewModel = new RoleViewModel();
            return CopyFromPersonDPO(personDpo, roleViewModel);
        }

        public PersonDpo CopyToPersonDpo(RoleViewModel roleViewModel)
        {
            PersonDpo personDpo = new PersonDpo();
            personDpo.Id = this.Id;
            personDpo.FirstName = this.FirstName;
            personDpo.LastName = this.LastName;
            personDpo.Birthday = this.Birthday;

            if (roleViewModel != null && roleViewModel.ListRole != null)
            {
                foreach (var role in roleViewModel.ListRole)
                {
                    if (role.Id == this.RoleId)
                    {
                        personDpo.RoleName = role.NameRole;
                        personDpo.RoleId = role.Id;
                        break;
                    }
                }
            }
            return personDpo;
        }

        public PersonDpo CopyToPersonDpo()
        {
            var roleViewModel = new RoleViewModel();
            return CopyToPersonDpo(roleViewModel);
        }

        public static string GetStringFromBirthday(object birthday)
        {
            if (birthday == null)
                return DateTime.Now.ToString("dd.MM.yyyy");

            if (birthday is DateTime dt)
                return dt.ToString("dd.MM.yyyy");
            else if (birthday is string str)
            {
                if (!string.IsNullOrEmpty(str) && str.Length == 10 &&
                    str[2] == '.' && str[5] == '.')
                    return str;

                try
                {
                    if (DateTime.TryParse(str, out DateTime result))
                        return result.ToString("dd.MM.yyyy");
                }
                catch { }
            }
            return DateTime.Now.ToString("dd.MM.yyyy");
        }

        public DateTime GetBirthdayAsDateTime()
        {
            try
            {
                return DateTime.ParseExact(this.Birthday, "dd.MM.yyyy", null);
            }
            catch
            {
                try
                {
                    return DateTime.Parse(this.Birthday);
                }
                catch
                {
                    return DateTime.Now;
                }
            }
        }

        public bool IsBirthdayValid()
        {
            try
            {
                DateTime.ParseExact(this.Birthday, "dd.MM.yyyy", null);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override string ToString()
        {
            return $"{LastName} {FirstName}";
        }
    }
}