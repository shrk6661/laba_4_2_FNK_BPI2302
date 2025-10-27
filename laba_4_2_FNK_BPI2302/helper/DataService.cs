using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using laba_4_2_FNK_BPI2302.model;
using Newtonsoft.Json;

namespace laba_4_2_FNK_BPI2302.helper
{
    public class DataService
    {
        private readonly string _dataDirectory = "Data";
        private readonly string _rolesFile = "roles.json";
        private readonly string _personsFile = "persons.json";

        public DataService()
        {
            // Создаем директорию для данных, если она не существует
            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
            }
        }

        // Методы для работы с ролями
        public ObservableCollection<Role> LoadRoles()
        {
            string filePath = Path.Combine(_dataDirectory, _rolesFile);
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<ObservableCollection<Role>>(json);
            }
            else
            {
                // Возвращаем пустую коллекцию, если файл не существует
                return new ObservableCollection<Role>();
            }
        }

        public void SaveRoles(ObservableCollection<Role> roles)
        {
            string filePath = Path.Combine(_dataDirectory, _rolesFile);
            string json = JsonConvert.SerializeObject(roles, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        // Методы для работы с сотрудниками
        public ObservableCollection<Person> LoadPersons()
        {
            string filePath = Path.Combine(_dataDirectory, _personsFile);
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<ObservableCollection<Person>>(json);
            }
            else
            {
                // Возвращаем пустую коллекцию, если файл не существует
                return new ObservableCollection<Person>();
            }
        }

        public void SavePersons(ObservableCollection<Person> persons)
        {
            string filePath = Path.Combine(_dataDirectory, _personsFile);
            string json = JsonConvert.SerializeObject(persons, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
    }
}
