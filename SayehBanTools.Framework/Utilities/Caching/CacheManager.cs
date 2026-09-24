using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SayehBanTools.Framework.Utilities.Caching
{
    /// <summary>
    /// مدیریت کش برای ذخیره و بازیابی داده‌ها در حافظه (IMemoryCache) یا Redis (IDistributedCache)
    /// </summary>
    public class CacheManager
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IMemoryCache _memoryCache;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly bool _useDistributedCache;

        /// <summary>
        /// سازنده کلاس CacheManager
        /// </summary>
        /// <param name="distributedCache">نمونه کش توزیع‌شده (اختیاری)</param>
        /// <param name="memoryCache">نمونه کش حافظه‌ای (اختیاری)</param>
        /// <param name="redisConnection">اتصال به Redis برای عملیات پیشرفته (اختیاری)</param>
        /// <param name="useDistributedCache">مشخص می‌کند که از کش توزیع‌شده استفاده شود یا حافظه‌ای</param>
        public CacheManager(
            IDistributedCache distributedCache = null,
            IMemoryCache memoryCache = null,
            IConnectionMultiplexer redisConnection = null,
            bool useDistributedCache = true)
        {
            _distributedCache = distributedCache;
            _memoryCache = memoryCache;
            _redisConnection = redisConnection;
            _useDistributedCache = useDistributedCache;

            if (_useDistributedCache && _distributedCache == null)
                throw new ArgumentNullException(nameof(distributedCache));
            if (!_useDistributedCache && _memoryCache == null)
                throw new ArgumentNullException(nameof(memoryCache));
        }

        /// <summary>
        /// افزودن یا به‌روزرسانی یک مقدار در کش
        /// </summary>
        /// <param name="key">کلید کش</param>
        /// <param name="value">مقدار برای ذخیره</param>
        /// <param name="options">تنظیمات کش (برای کش توزیع‌شده)</param>
        /// <param name="memoryOptions">تنظیمات کش حافظه‌ای (اختیاری)</param>
        public async Task AddOrUpdateAsync<T>(string key, T value, DistributedCacheEntryOptions options = null, MemoryCacheEntryOptions memoryOptions = null)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            var serializedData = JsonSerializer.Serialize(value);
            if (_useDistributedCache)
            {
                await _distributedCache.SetStringAsync(key, serializedData, options ?? new DistributedCacheEntryOptions());
            }
            else
            {
                _memoryCache.Set(key, value, memoryOptions ?? new MemoryCacheEntryOptions());
            }
        }

        /// <summary>
        /// افزودن یا به‌روزرسانی چندین مقدار در کش
        /// </summary>
        /// <param name="keyValuePairs">دیکشنری کلید-مقدار برای ذخیره</param>
        /// <param name="options">تنظیمات کش (برای کش توزیع‌شده)</param>
        /// <param name="memoryOptions">تنظیمات کش حافظه‌ای (اختیاری)</param>
        public async Task AddOrUpdateBulkAsync<T>(Dictionary<string, T> keyValuePairs, DistributedCacheEntryOptions options = null, MemoryCacheEntryOptions memoryOptions = null)
        {
            if (keyValuePairs == null || !keyValuePairs.Any())
                throw new ArgumentNullException(nameof(keyValuePairs));

            if (_useDistributedCache)
            {
                var tasks = new List<Task>();
                foreach (var kvp in keyValuePairs)
                {
                    var serializedData = JsonSerializer.Serialize(kvp.Value);
                    tasks.Add(_distributedCache.SetStringAsync(kvp.Key, serializedData, options ?? new DistributedCacheEntryOptions()));
                }
                await Task.WhenAll(tasks);
            }
            else
            {
                foreach (var kvp in keyValuePairs)
                {
                    _memoryCache.Set(kvp.Key, kvp.Value, memoryOptions ?? new MemoryCacheEntryOptions());
                }
            }
        }

        /// <summary>
        /// بازیابی یک مقدار از کش
        /// </summary>
        /// <param name="key">کلید کش</param>
        /// <returns>مقدار بازیابی‌شده یا null اگر وجود نداشته باشد</returns>
        public async Task<T> GetAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (_useDistributedCache)
            {
                var cachedData = await _distributedCache.GetStringAsync(key);
                return cachedData != null ? JsonSerializer.Deserialize<T>(cachedData) : default;
            }
            else
            {
                return _memoryCache.TryGetValue(key, out T value) ? value : default;
            }
        }

        /// <summary>
        /// بازیابی چندین مقدار از کش
        /// </summary>
        /// <param name="keys">لیست کلیدها</param>
        /// <returns>دیکشنری کلید-مقدار بازیابی‌شده</returns>
        public async Task<Dictionary<string, T>> GetBulkAsync<T>(IEnumerable<string> keys)
        {
            if (keys == null || !keys.Any())
                throw new ArgumentNullException(nameof(keys));

            var results = new Dictionary<string, T>();
            if (_useDistributedCache)
            {
                foreach (var key in keys)
                {
                    var cachedData = await _distributedCache.GetStringAsync(key);
                    if (cachedData != null)
                    {
                        results[key] = JsonSerializer.Deserialize<T>(cachedData);
                    }
                }
            }
            else
            {
                foreach (var key in keys)
                {
                    if (_memoryCache.TryGetValue(key, out T value))
                    {
                        results[key] = value;
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// حذف یک مقدار از کش
        /// </summary>
        /// <param name="key">کلید کش</param>
        public async Task RemoveAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (_useDistributedCache)
            {
                await _distributedCache.RemoveAsync(key);
            }
            else
            {
                _memoryCache.Remove(key);
            }
        }

        /// <summary>
        /// حذف چندین مقدار از کش
        /// </summary>
        /// <param name="keys">لیست کلیدها</param>
        public async Task RemoveBulkAsync(IEnumerable<string> keys)
        {
            if (keys == null || !keys.Any())
                throw new ArgumentNullException(nameof(keys));

            if (_useDistributedCache)
            {
                var tasks = keys.Select(key => _distributedCache.RemoveAsync(key)).ToArray();
                await Task.WhenAll(tasks);
            }
            else
            {
                foreach (var key in keys)
                {
                    _memoryCache.Remove(key);
                }
            }
        }

        /// <summary>
        /// ریست کل کش با یک پیشوند خاص
        /// </summary>
        /// <param name="prefix">پیشوند کلیدها برای حذف</param>
        public async Task ResetCacheAsync(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                throw new ArgumentNullException(nameof(prefix));

            if (_useDistributedCache)
            {
                if (_redisConnection == null)
                    throw new NotImplementedException("برای ریست کش توزیع‌شده با الگو، نیاز به StackExchange.Redis است.");

                var db = _redisConnection.GetDatabase();
                var server = _redisConnection.GetServer(_redisConnection.GetEndPoints().First());
                var keys = server.Keys(pattern: $"{prefix}*").ToArray();
                if (keys.Any())
                {
                    await db.KeyDeleteAsync(keys);
                }
            }
            else
            {
                _memoryCache.Remove(prefix);
            }
        }

        /// <summary>
        /// جستجو در لیست کش‌شده با شرط
        /// </summary>
        /// <param name="key">کلید کش</param>
        /// <param name="predicate">شرط جستجو</param>
        /// <returns>لیست مقادیر فیلترشده</returns>
        public async Task<IEnumerable<T>> SearchAsync<T>(string key, Func<T, bool> predicate)
        {
            var list = await GetAsync<List<T>>(key);
            return list?.Where(predicate) ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// جستجو با صفحه‌بندی در لیست کش‌شده
        /// </summary>
        /// <param name="key">کلید کش</param>
        /// <param name="predicate">شرط جستجو</param>
        /// <param name="pageNumber">شماره صفحه</param>
        /// <param name="pageSize">اندازه صفحه</param>
        /// <returns>نتایج صفحه‌بندی‌شده و تعداد کل</returns>
        public async Task<(IEnumerable<T> Results, int TotalCount)> SearchWithPaginationAsync<T>(
            string key, Func<T, bool> predicate, int pageNumber, int pageSize)
        {
            var list = await GetAsync<List<T>>(key);
            if (list == null)
                return (Enumerable.Empty<T>(), 0);

            var filteredItems = list.Where(predicate).ToList();
            var totalCount = filteredItems.Count;

            var pagedItems = filteredItems
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (pagedItems, totalCount);
        }

        /// <summary>
        /// حذف یک آیتم خاص از لیست کش‌شده
        /// </summary>
        public async Task RemoveItemAsync<T>(string key, Func<T, bool> predicate, DistributedCacheEntryOptions options = null, MemoryCacheEntryOptions memoryOptions = null)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (_useDistributedCache)
            {
                var cachedData = await _distributedCache.GetStringAsync(key);
                if (cachedData != null)
                {
                    var list = JsonSerializer.Deserialize<List<T>>(cachedData);
                    if (list != null)
                    {
                        var updatedList = list.Where(item => !predicate(item)).ToList();
                        if (updatedList.Count != list.Count)
                        {
                            var serializedData = JsonSerializer.Serialize(updatedList);
                            await _distributedCache.SetStringAsync(key, serializedData, options ?? new DistributedCacheEntryOptions());
                        }
                    }
                }
            }
            else
            {
                if (_memoryCache.TryGetValue(key, out List<T> list) && list != null)
                {
                    var updatedList = list.Where(item => !predicate(item)).ToList();
                    if (updatedList.Count != list.Count)
                    {
                        _memoryCache.Set(key, updatedList, memoryOptions ?? new MemoryCacheEntryOptions());
                    }
                }
            }
        }

        /// <summary>
        /// حذف مجموعه‌ای از آیتم‌ها از لیست کش‌شده
        /// </summary>
        public async Task RemoveItemArrayAsync<T>(string key, Func<T, bool> predicate, DistributedCacheEntryOptions options = null, MemoryCacheEntryOptions memoryOptions = null)
        {
            await RemoveItemAsync(key, predicate, options, memoryOptions);
        }
    }
}