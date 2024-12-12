using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Systems.Account.Enum;
using Systems.Account.Model;
using Systems.Multiplayer.Manager;
using Systems.SaveLoad.Interface;
using Systems.SaveLoad.Manager;
using Systems.SaveLoad.Model;
using UnityEngine;

namespace Systems.SaveLoad.Service
{
    public class SaveLoadLocalService
    {
        public TrackableService trackableService = new TrackableService();
        ISerializer serializer = new JsonSerializerService();
        private string dataPath;
        private string fileExtension = "json";
        public SaveLoadLocalService(string fileLocation)
        {
            this.dataPath = fileLocation;
        }
        public string GetDataPath()
        {
            return dataPath;
        }
        public void SetDataPath(string fileLocation)
        {
            this.dataPath = fileLocation;
        }
        public string GetPathFolder(string folderName)
        {
            return Path.Combine(dataPath, folderName);
        }
        public string GetPathToFile(string fileName)
        {
            return Path.Combine(dataPath, string.Concat(fileName, ".", fileExtension));
        }
        public string GetPathToFile(string folderName, string fileName)
        {
            return Path.Combine(dataPath, folderName, string.Concat(fileName, ".", fileExtension));
        }
        public void CreateFolder(string folderName)
        {
            string path = Path.Combine(dataPath, folderName);
            if (!Directory.Exists(path))
                {
                Directory.CreateDirectory(path);
                // Debug.Log($"Folder created at: {path}");
            }
            else
            {
                // Debug.Log($"Folder already exists at: {path}");
            }
        }
        //Save ZONE
        public async Task SaveAsync<T>(string fileName, T data, bool overwrite = true)
        {
            string fileLocation = FindFileSave(fileName, overwrite);
            Debug.Log($"file fileLocation at: {fileLocation}");
            if (fileLocation == string.Empty) return;
            string jsonData = serializer.Serialize(data);
            await File.WriteAllTextAsync(fileLocation, jsonData);
        }
        public async Task SaveAsync<T>(string folderName, string fileName, T data, bool overwrite = true)
        {
            string fileLocation = FindFileSave(folderName, fileName, overwrite);
            Debug.Log($"file fileLocation at: {fileLocation}");
            if (fileLocation == string.Empty) return;
            string jsonData = serializer.Serialize(data);
            await File.WriteAllTextAsync(fileLocation, jsonData);
        }
        private string FindFileSave(string fileName, bool overwrite = true)
        {
            string fileLocation = GetPathToFile(fileName);
            if (overwrite)
                return fileLocation;
            if (!overwrite && !File.Exists(fileLocation))
                return fileLocation;
            return string.Empty;
        }
        private string FindFileSave(string folderName, string fileName, bool overwrite = true)
        {
            CreateFolder(folderName);
            string fileLocation = GetPathToFile(folderName, fileName);
            if (overwrite)
                return fileLocation;
            if (!overwrite && !File.Exists(fileLocation))
                return fileLocation;
            return string.Empty;
        }
        // Save Action ZONE

        // Load ZONE
        public async Task<T> LoadAsync<T>(string fileName)
        {
            string fileLocation = GetPathToFile(fileName);

            if (!File.Exists(fileLocation))
            {
                return default;
            }
            string jsonData = await File.ReadAllTextAsync(fileLocation);
            return serializer.Deserialize<T>(jsonData);
        }
        public async Task<T> LoadAsync<T>(string folderName, string fileName)
        {
            string fileLocation = GetPathToFile(folderName, fileName);

            if (!File.Exists(fileLocation))
            {
                return default;
            }
            string jsonData = await File.ReadAllTextAsync(fileLocation);
            return serializer.Deserialize<T>(jsonData);
        }
        public void DeleteFile(string fileName)
        {
            string fileLocation = GetPathToFile(fileName);

            if (File.Exists(fileLocation))
            {
                File.Delete(fileLocation);
            }
        }
        public void DeleteFile(string folderName, string fileName)
        {
            string fileLocation = GetPathToFile(folderName, fileName);

            if (File.Exists(fileLocation))
            {
                File.Delete(fileLocation);
            }
        }
        public void DeleteFolder(string folderName)
        {
            string fileLocation = GetPathToFile(folderName);
            if (Directory.Exists(fileLocation))
            {
                Directory.Delete(fileLocation);
            }
        }
        public void DeleteAll()
        {
            foreach (string filePath in Directory.GetFiles(dataPath))
            {
                File.Delete(filePath);
            }
        }
        public void DeleteAll(string folderName)
        {
            foreach (string filePath in Directory.GetFiles(dataPath, folderName))
            {
                File.Delete(filePath);
            }
        }
        public IEnumerable<string> ListSaves()
        {
            foreach (string path in Directory.EnumerateFiles(dataPath))
            {
                if (Path.GetExtension(path) == fileExtension)
                {
                    yield return Path.GetFileNameWithoutExtension(path);
                }
            }
        }
        public IEnumerable<string> ListSaves(string folderName)
        {
            foreach (string path in Directory.EnumerateFiles(dataPath, folderName))
            {
                if (Path.GetExtension(path) == fileExtension)
                {
                    yield return Path.GetFileNameWithoutExtension(path);
                }
            }
        }

        // Check ZONE
        public bool CheckFileExists(string nameFile)
        {
            return File.Exists(GetPathToFile(nameFile));
        }
        public bool CheckFolderExists(string nameFolder)
        {
            return Directory.Exists(GetPathFolder(nameFolder));
        }
        public bool CheckFolderToFileExists(string nameFolder, string nameFile)
        {
            return File.Exists(GetPathToFile(nameFolder, nameFile));
        }

        public bool CheckAnonymousFileExists()
        {
            return File.Exists(GetPathToFile("AnonymousAccount"));
        }
        public bool CheckAnonymousFolderExists()
        {
            return Directory.Exists(GetPathFolder("AnonymousAccount"));
        }
        public bool CheckAnonymousFolderToFileExists(string nameFile)
        {
            return File.Exists(GetPathToFile("AnonymousAccount", nameFile));
        }

        public bool CheckPlayerFileExists(string nameFilePlayer)
        {
            return File.Exists(GetPathToFile(nameFilePlayer));
        }
        public bool CheckPlayerFolderExists(string nameFolderPlayer)
        {
            return Directory.Exists(GetPathFolder(nameFolderPlayer));
        }
        public bool CheckPlayerFolderToFileExists(string nameFolderPlayer, string nameFile)
        {
            return File.Exists(GetPathToFile(nameFolderPlayer, nameFile));
        }
    }
}