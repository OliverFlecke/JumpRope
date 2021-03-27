using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using Serilog;
using Xamarin.Essentials;

namespace SkippingCounter.Services
{
    public class FileDataStore<T> : IDataStore<T> where T : IIdentifiable
    {
        readonly string StorageDirectory = Path.Join(FileSystem.AppDataDirectory, nameof(T));

        readonly ILogger _logger;

        public FileDataStore(ILogger logger)
        {
            _logger = logger;

            if (!Directory.Exists(StorageDirectory)) Directory.CreateDirectory(StorageDirectory);
        }

        public async Task<bool> AddItemAsync(T item)
        {
            using var stream = File.OpenWrite(GetFilename(item.GetID()));
            await JsonSerializer.SerializeAsync(stream, item);

            return true;
        }

        public Task<bool> DeleteItemAsync(string id)
        {
            _logger.Information($"Deleting {id} from {nameof(T)}");

            var filename = GetFilename(id);
            if (File.Exists(filename)) File.Delete(filename);
            else _logger.Warning($"File for {id} in {nameof(T)} does not exists");

            return Task.FromResult(true);
        }

        public ValueTask<T?> GetItemAsync(string id)
        {
            var filename = GetFilename(id);
            if (!File.Exists(filename)) return new ValueTask<T?>(null);

            using var reader = File.OpenRead(filename);
            return JsonSerializer.DeserializeAsync<T?>(reader);
        }

        public Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateItemAsync(T item)
        {
            throw new NotImplementedException();
        }

        string GetFilename(string id) => Path.Join(StorageDirectory, $"{id}.json");
    }
}
