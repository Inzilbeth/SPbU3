using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Task1Client;

namespace Task1
{
    /// <summary>
    /// Model for GUI.
    /// </summary>
    public class ClientViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Created client.
        /// </summary>
        private Client client;

        /// <summary>
        /// Port for connection.
        /// </summary>
        private int port;

        /// <summary>
        /// Server IP for connection.
        /// </summary>
        private string server;

        /// <summary>
        /// Path to the current client folder.
        /// </summary>
        private string currentClientPath;

        /// <summary>
        /// Path to the current server folder.
        /// </summary>
        private string сurrentServerDirectory;

        /// <summary>
        /// Current client folder name.
        /// </summary>
        private string currentClientDirectory;

        /// <summary>
        /// Complete path to download folder.
        /// </summary>
        private string downloadPath;

        /// <summary>
        /// Complete paths to current server folders.
        /// </summary>
        private ObservableCollection<(string Name, bool IsFile)> currentServerPaths;

        /// <summary>
        /// Complete paths to current client folders.
        /// </summary>
        private ObservableCollection<string> currentClientPaths;

        /// <summary>
        /// Download folder name.
        /// </summary>
        public string DownloadDirectory
        {
            get
            {
                var tmp = downloadPath.Remove(downloadPath.Length - 1);
                return tmp.Substring(tmp.LastIndexOf('\\') + 1);
            }
            set
            {
                downloadPath = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Whether client is connected to the server.
        /// </summary>
        public bool IsConnected { get; set; } = false;

        /// <summary>
        /// Client's root path.
        /// </summary>
        public string ClientRootPath { get; private set; }

        /// <summary>
        /// Address for connection.
        /// </summary>
        public string Server
        {
            get
            {
                OnConnectionChange();
                return server;
            }
            set
            {
                OnConnectionChange();
                server = value;
            }
        }

        /// <summary>
        /// Displayed server folder names.
        /// </summary>
        public ObservableCollection<string> DisplayedServerList { get; private set; }

        /// <summary>
        /// Displayed client folder names.
        /// </summary>
        public ObservableCollection<string> DisplayedClientList { get; private set; }

        /// <summary>
        /// List of currently running downloads.
        /// </summary>
        public ObservableCollection<string> DownloadsInProgressList { get; private set; }

        /// <summary>
        /// List of finished downloads.
        /// </summary>
        public ObservableCollection<string> DownloadsFinishedList { get; private set; }

        /// <summary>
        /// Port for connection as a string.
        /// </summary>
        public string Port
        {
            get
            {
                OnConnectionChange();
                return port.ToString();
            }
            set
            {
                OnConnectionChange();
                port = Convert.ToInt32(value);
            }
        }

        private void OnConnectionChange()
        {
            Disconnected?.Invoke();
            IsConnected = false;
        }

        /// <summary>
        /// Error handling.
        /// </summary>
        public delegate void ShowErrorMessage(object sender, string message);

        /// <summary>
        /// For showing error messages.
        /// </summary>
        public event ShowErrorMessage ThrowError = (_, __) => { };

        /// <summary>
        /// Handler for changing download folder.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Event for disconnection.
        /// </summary>
        public event Action Disconnected;

        /// <summary>
        /// Model initialization.
        /// </summary>
        public ClientViewModel(string rootClientDirectory)
        {
            Server = "127.0.0.1";
            port = 9999;

            ClientRootPath = rootClientDirectory;

            currentClientPath = ClientRootPath;
            downloadPath = ClientRootPath;

            сurrentServerDirectory = "";
            currentClientDirectory = "";

            DisplayedServerList = new ObservableCollection<string>();
            DisplayedClientList = new ObservableCollection<string>();
            DownloadsInProgressList = new ObservableCollection<string>();
            DownloadsFinishedList = new ObservableCollection<string>();

            InitializeCurrentPathsOnClient();
        }

        /// <summary>
        /// Notifies the change of download folder's name.
        /// </summary>
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
           => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Connection to new server.
        /// </summary>
        public async Task Connect()
        {
            if (IsConnected)
            {
                return;
            }

            client = new Client(Server, port);

            DisplayedServerList.Clear();

            try
            {
                await client.Connect();
                await InitializeCurrentPathsOnServer();
                IsConnected = true;
            }
            catch (Exception e)
            {
                client.Stop();
                IsConnected = false;
                Disconnected?.Invoke();
                ThrowError(this, e.Message);
            }
        }

        /// <summary>
        /// Initializes current client folders.
        /// </summary>
        private void InitializeCurrentPathsOnClient()
        {
            currentClientPaths = new ObservableCollection<string>();

            currentClientPaths.CollectionChanged += OnCurrentClientPathsChanged;

            try
            {
                TryUpdatingCurrentPathsOnClient("");
            }
            catch (Exception e)
            {
                ThrowError(this, e.Message);
            }
        }

        /// <summary>
        /// Initializes current server contents.
        /// </summary>
        private async Task InitializeCurrentPathsOnServer()
        {
            currentServerPaths = new ObservableCollection<(string, bool)>();

            currentServerPaths.CollectionChanged += OnCurrentServerPathsChanged;

            await TryUpdatingCurrentPathsOnServer("");
        }

        /// <summary>
        /// Handler for changing client paths.
        /// </summary>
        private void OnCurrentClientPathsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    DisplayedClientList.Remove(item.ToString());
                }
            }

            if (e.NewItems == null)
            {
                return;
            }

            foreach (var item in e.NewItems)
            {
                DisplayedClientList.Add(item.ToString());
            }
        }

        /// <summary>
        /// Handler for changing server paths.
        /// </summary>
        private void OnCurrentServerPathsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach ((string, bool) pair in e.OldItems)
                {
                    DisplayedServerList.Remove(pair.Item1);
                }
            }

            if (e.NewItems == null) return;

            foreach ((string, bool) pair in e.NewItems)
            {
                DisplayedServerList.Add(pair.Item1);
            }
        }

        /// <summary>
        /// Opening client folder.
        /// </summary>
        public void OpenClientFolder(string folderName)
        {
            var nextDirectoryPath = Path.Combine(currentClientPath, folderName);

            if (Directory.Exists(nextDirectoryPath))
            {
                var nextDirectoryOnClient = Path.Combine(currentClientDirectory, folderName);
                TryUpdatingCurrentPathsOnClient(nextDirectoryOnClient);
                currentClientPath = nextDirectoryPath;
                currentClientDirectory = Path.Combine(currentClientDirectory, folderName);
            }
            else
            {
                ThrowError(this, $"\"{nextDirectoryPath}\" directory not found!");
            }
        }

        /// <summary>
        /// Checks whether an entry is a file.
        /// </summary>
        private bool IsFile(string folderName)
            => (from path in currentServerPaths
                where path.Name == folderName
                select !path.IsFile).FirstOrDefault();

        /// <summary>
        /// Opens a folder or downloads a file.
        /// </summary>
        public async Task OpenServerFolderOrDownloadFile(string folderName)
        {
            if (IsFile(folderName))
            {
                await DownloadFile(folderName);
                return;
            }

            var nextDirectory = Path.Combine(сurrentServerDirectory, folderName);

            await TryUpdatingCurrentPathsOnServer(nextDirectory);
        }

        /// <summary>
        /// Current client paths update.
        /// </summary>
        private void TryUpdatingCurrentPathsOnClient(string folderPath)
        {
            try
            {
                var dirToOpen = Path.Combine(ClientRootPath, folderPath);

                var folders = Directory.EnumerateDirectories(dirToOpen);

                while (currentClientPaths.Count > 0)
                {
                    currentClientPaths.RemoveAt(currentClientPaths.Count - 1);
                }

                foreach (var folder in folders)
                {
                    var name = folder.Substring(folder.LastIndexOf('\\') + 1);
                    currentClientPaths.Add(name);
                }
            }
            catch (Exception e)
            {
                ThrowError(this, e.Message);
            }
        }

        /// <summary>
        /// Current server paths update.
        /// </summary>
        private async Task TryUpdatingCurrentPathsOnServer(string listRequest)
        {
            try
            {
                await client.Connect();
                var (_, serverList) = await client.List(listRequest);

                while (currentServerPaths.Count > 0)
                {
                    currentServerPaths.RemoveAt(currentServerPaths.Count - 1);
                }

                foreach (var path in serverList)
                {
                    var name = path.Item1;

                    name = name.Substring(name.LastIndexOf('\\') + 1);

                    currentServerPaths.Add((name, path.Item2));
                }

                сurrentServerDirectory = listRequest;
            }
            catch (Exception e)
            {
                if (e.Message == "-1")
                {
                    if (listRequest == "")
                    {
                        ThrowError(this, $"Server's root directory not found!" +
                                         $"\nIf you're using attached implementation of FTPServer, " +
                                         $"make sure you have \"bin\\storage\" folder created in the FTPServer folder.");
                    }
                    else
                    {
                        ThrowError(this, $"Relative path to \"{listRequest}\" not found!");
                    }
                    return;
                }

                ThrowError(this, e.Message);
            }
        }

        /// <summary>
        /// Going one client folder up.
        /// </summary>
        public void GoBackClient()
        {
            if (currentClientDirectory == "")
            {
                ThrowError(this, "Already inside the root directory!");
                return;
            }

            try
            {
                var index = currentClientDirectory.LastIndexOf('\\');
                string toOpen;

                if (index > 0)
                {
                    toOpen = currentClientDirectory.Substring(0, currentClientDirectory.LastIndexOf('\\'));
                }
                else
                {
                    toOpen = "";
                }

                TryUpdatingCurrentPathsOnClient(toOpen);
                currentClientDirectory = toOpen;
                currentClientPath = Directory.GetParent(currentClientPath)?.ToString();
            }
            catch (Exception e)
            {
                ThrowError(this, e.Message);
            }
        }

        /// <summary>
        /// Going one server folder up.
        /// </summary>
        public async Task GoBackServer()
        {
            if (сurrentServerDirectory == "")
            {
                ThrowError(this, "Already inside the root directory!");
                return;
            }

            var toOpen = "";

            try
            {
                var index = сurrentServerDirectory.LastIndexOf('\\');

                if (index > 0)
                {
                    toOpen = сurrentServerDirectory.Substring(0, сurrentServerDirectory.LastIndexOf('\\'));
                }
                else
                {
                    toOpen = "";
                }

                await TryUpdatingCurrentPathsOnServer(toOpen);
            }
            catch (Exception e)
            {
                if (e.Message == "-1")
                {
                    ThrowError(this, $"\"{toOpen}\" directory not found!");
                    return;
                }

                ThrowError(this, e.Message);
            }
        }

        /// <summary>
        /// Updates download folder.
        /// </summary>
        public void UpdateDownloadFolder()
        {
            if (downloadPath == currentClientPath)
            {
                return;
            }

            DownloadDirectory = currentClientPath + "\\";
        }

        /// <summary>
        /// Downloads file.
        /// </summary>
        public async Task DownloadFile(string fileName)
        {
            try
            {
                var pathToFile = Path.Combine(сurrentServerDirectory, fileName);

                DownloadsInProgressList.Add(fileName);

                await client.Connect();
                await client.Get(pathToFile, downloadPath);

                DownloadsInProgressList.Remove(fileName);
                DownloadsFinishedList.Add(fileName);
            }
            catch (Exception e)
            {
                ThrowError(this, e.Message);
            }
        }

        /// <summary>
        /// Downloads all files from current folder.
        /// </summary>
        public async Task DownloadAllFilesInCurrentDirectory()
        {
            try
            {
                foreach (var (name, isFile) in currentServerPaths)
                {
                    if (!isFile)
                    {
                        await DownloadFile(name);
                    }
                }
            }
            catch (Exception e)
            {
                ThrowError(this, e.Message);
            }
        }
    }
}
