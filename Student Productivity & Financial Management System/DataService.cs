using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json; 
using StudentProductivityApp.Models;

namespace StudentProductivityApp.Services
{
    public static class DataService
    {
        static string DataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "StudentProductivityApp");
        static string UsersFile = Path.Combine(DataFolder, "users.json");

        static JsonSerializerOptions opts = new JsonSerializerOptions { WriteIndented = true };

        static DataService()
        {
            if (!Directory.Exists(DataFolder)) Directory.CreateDirectory(DataFolder);
            if (!File.Exists(UsersFile)) File.WriteAllText(UsersFile, "[]");
        }

        public static List<User> LoadUsers()
        {
            var json = File.ReadAllText(UsersFile);
            return JsonSerializer.Deserialize<List<User>>(json, opts) ?? new List<User>();
        }

        public static void SaveUsers(List<User> users)
        {
            var json = JsonSerializer.Serialize(users, opts);
            File.WriteAllText(UsersFile, json);
        }

        public static User FindUser(string studentId, string password)
        {
            return LoadUsers().FirstOrDefault(u => u.StudentId == studentId && u.Password == password);
        }

        public static bool AddUser(User user)
        {
            var users = LoadUsers();
            if (users.Any(u => u.StudentId == user.StudentId)) return false;
            users.Add(user);
            SaveUsers(users);
            return true;
        }

        public static void UpdateUser(User user)
        {
            var users = LoadUsers();
            var idx = users.FindIndex(u => u.StudentId == user.StudentId);
            if (idx >= 0)
            {
                users[idx] = user;
                SaveUsers(users);
            }
        }
    }
}
