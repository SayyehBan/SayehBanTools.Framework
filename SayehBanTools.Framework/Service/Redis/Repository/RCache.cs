using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using SayehBanTools.Framework.Service.Redis.Interface;
using SayehBanTools.Framework.Utilities.Caching;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SayehBanTools.Framework.Service.Redis.Repository
{
    /// <summary>
    /// ریپازیتوری کش برای مدیریت داده‌ها در Redis یا حافظه
    /// </summary>
    public class RCache : ICache
    {
        private readonly CacheManager _cacheManager;

        /// <summary>
        /// سازنده کلاس RCache
        /// </summary>
        /// <param name="distributedCache">نمونه کش توزیع‌شده (اختیاری)</param>
        /// <param name="memoryCache">نمونه کش حافظه‌ای (اختیاری)</param>
        /// <param name="redisConnection">اتصال به Redis برای عملیات پیشرفته (اختیاری)</param>
        /// <param name="useDistributedCache">مشخص می‌کند که از کش توزیع‌شده استفاده شود یا حافظه‌ای</param>
        public RCache(
            IDistributedCache distributedCache = null,
            IMemoryCache memoryCache = null,
            IConnectionMultiplexer redisConnection = null,
            bool useDistributedCache = true)
        {
            _cacheManager = new CacheManager(distributedCache, memoryCache, redisConnection, useDistributedCache);
        }

        /// <summary>
        /// افزودن یا به‌روزرسانی یک مقدار در کش
        /// </summary>
        public async Task AddOrUpdateAsync<T>(string cacheKey, T value, DistributedCacheEntryOptions options = null, MemoryCacheEntryOptions memoryOptions = null)
        {
            await _cacheManager.AddOrUpdateAsync(cacheKey, value, options, memoryOptions);
        }

        /// <summary>
        /// افزودن یا به‌روزرسانی چندین مقدار در کش
        /// </summary>
        public async Task AddOrUpdateBulkAsync<T>(Dictionary<string, T> keyValuePairs, DistributedCacheEntryOptions options = null, MemoryCacheEntryOptions memoryOptions = null)
        {
            await _cacheManager.AddOrUpdateBulkAsync(keyValuePairs, options, memoryOptions);
        }

        /// <summary>
        /// ذخیره یک مقدار در کش
        /// </summary>
        public async Task SetAsync<T>(string cacheKey, T value, DistributedCacheEntryOptions options)
        {
            await _cacheManager.AddOrUpdateAsync(cacheKey, value, options);
        }

        /// <summary>
        /// ذخیره چندین مقدار در کش
        /// </summary>
        public async Task SetBulkAsync<T>(Dictionary<string, T> keyValuePairs, DistributedCacheEntryOptions options)
        {
            await _cacheManager.AddOrUpdateBulkAsync(keyValuePairs, options);
        }

        /// <summary>
        /// بازیابی یک مقدار از کش
        /// </summary>
        public async Task<T> GetAsync<T>(string cacheKey)
        {
            return await _cacheManager.GetAsync<T>(cacheKey);
        }

        /// <summary>
        /// بازیابی چندین مقدار از کش
        /// </summary>
        public async Task<Dictionary<string, T>> GetBulkAsync<T>(IEnumerable<string> keys)
        {
            return await _cacheManager.GetBulkAsync<T>(keys);
        }

        /// <summary>
        /// حذف یک مقدار از کش
        /// </summary>
        public async Task RemoveAsync(string cacheKey)
        {
            await _cacheManager.RemoveAsync(cacheKey);
        }

        /// <summary>
        /// حذف چندین مقدار از کش
        /// </summary>
        public async Task RemoveBulkAsync(IEnumerable<string> keys)
        {
            await _cacheManager.RemoveBulkAsync(keys);
        }
        /// <summary>
        /// حذف آیتم تک
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="predicate"></param>
        /// <param name="options"></param>
        /// <param name="memoryOptions"></param>
        /// <returns></returns>
        public async Task RemoveItemAsync<T>(string cacheKey, Func<T, bool> predicate, DistributedCacheEntryOptions options = null, MemoryCacheEntryOptions memoryOptions = null)
        {
            await _cacheManager.RemoveItemAsync(cacheKey, predicate, options, memoryOptions);
        }
        /// <summary>
        /// حذف آیتم به صورت آرایه ای
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="predicate"></param>
        /// <param name="options"></param>
        /// <param name="memoryOptions"></param>
        /// <returns></returns>
        public async Task RemoveItemArrayAsync<T>(string cacheKey, Func<T, bool> predicate, DistributedCacheEntryOptions options = null, MemoryCacheEntryOptions memoryOptions = null)
        {
            await _cacheManager.RemoveItemArrayAsync(cacheKey, predicate, options, memoryOptions);
        }
        /// <summary>
        /// ریست کل کش با پیشوند
        /// </summary>
        public async Task ResetCacheAsync(string prefix)
        {
            await _cacheManager.ResetCacheAsync(prefix);
        }

        /// <summary>
        /// جستجو در لیست کش‌شده
        /// </summary>
        public async Task<IEnumerable<T>> SearchAsync<T>(string cacheKey, Func<T, bool> predicate)
        {
            return await _cacheManager.SearchAsync(cacheKey, predicate);
        }

        /// <summary>
        /// جستجو با صفحه‌بندی در لیست کش‌شده
        /// </summary>
        public async Task<(IEnumerable<T> Results, int TotalCount)> SearchWithPaginationAsync<T>(
            string cacheKey, Func<T, bool> predicate, int pageNumber, int pageSize)
        {
            return await _cacheManager.SearchWithPaginationAsync(cacheKey, predicate, pageNumber, pageSize);
        }

    }
}