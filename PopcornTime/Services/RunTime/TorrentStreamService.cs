using PopcornTime.Extensions;
using PopcornTime.Services.Interfaces;
using PopcornTime.Utilities;
using Universal.Torrent.Client;
using Universal.Torrent.Common;

namespace PopcornTime.Services.RunTime
{
    internal class TorrentStreamService : ITorrentStreamService
    {
        private readonly ClientEngine _engine;

        public TorrentStreamService(ClientEngine engine)
        {
            _engine = engine;
        }

        public void CreateManager(Torrent torrent)
        {
            var manager = torrent.CreateManager(_engine.Settings.SaveFolder);
            _engine.Register(manager);
            StreamManager = new TorrentStreamManager(manager);
        }

        public async void Stop()
        {
            var managerCopy = StreamManager;
            StreamManager.TorrentManager.TorrentStateChanged += (sender, args) =>
            {
                if (args.NewState == TorrentState.Stopped)
                    _engine.Unregister(managerCopy.TorrentManager);
            };
            StreamManager.Dispose();
            var torrentsFolder = _engine.Settings.SaveFolder;
            await StorageHelper.DeleteFolderContentAsync(torrentsFolder);
            StreamManager = null;
        }

        public TorrentStreamManager StreamManager { get; set; }
    }
}