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
            if (!File.Exists(filename)) return new ValueTask<T?>();

            return ReadAsync(filename);
        }

        public async IAsyncEnumerable<T> GetItemsAsync(bool forceRefresh = false)
        {
            _logger.Information($"Getting all items for {nameof(T)}");
            foreach (var filename in Directory.GetFiles(StorageDirectory))
            {
                _logger.Debug($"Getting filename {filename}");
                var item = await ReadAsync(filename);
                if (item is not null) yield return item;
            }

            yield break;
        }

        public Task<bool> UpdateItemAsync(T item)
        {
            throw new NotImplementedException();
        }

        async ValueTask<T?> ReadAsync(string filename)
        {
            try
            {
                //using var reader = File.OpenRead(filename);
                var content = await File.ReadAllTextAsync(filename);
                return JsonSerializer.Deserialize<T?>(content);
                //return JsonSerializer.DeserializeAsync<T?>(reader);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unable to read file");

                return default;
            }
        }

        string GetFilename(string id) => Path.Join(StorageDirectory, $"{id}.json");
    }
}
