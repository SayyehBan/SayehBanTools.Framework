using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SayehBanTools.Framework.Service.Redis.Interface
{
    /// <summary>
    /// اینترفیس برای مدیریت کش Redis یا حافظه
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// افزودن یا به‌روزرسانی یک مقدار در کش
        /// </summary>
        Task AddOrUpdateAsync<T>(string cacheKey, T value, DistributedCacheEntryOptions options = null, MemoryCacheEntryOptions memoryOptions = null);

        /// <summary>
        /// افزودن یا به‌روزرسانی چندین مقدار در کش
        /// </summary>
        Task AddOrUpdateBulkAsync<T>(Dictionary<string, T> keyValuePairs, DistributedCacheEntryOptions options = null, MemoryCacheEntryOptions memoryOptions = null);

        /// <summary>
        /// ذخیره یک مقدار در کش
        /// </summary>
        Task SetAsync<T>(string cacheKey, T value, DistributedCacheEntryOptions options);

        /// <summary>
        /// ذخیره چندین مقدار در کش
        /// </summary>
        Task SetBulkAsync<T>(Dictionary<string, T> keyValuePairs, DistributedCacheEntryOptions options);

        /// <summary>
        /// بازیابی یک مقدار از کش
        /// </summary>
        Task<T> GetAsync<T>(string cacheKey);

        /// <summary>
        /// بازیابی چندین مقدار از کش
        /// </summary>
        Task<Dictionary<string, T>> GetBulkAsync<T>(IEnumerable<string> keys);

        /// <summary>
        /// حذف یک مقدار از کش
        /// </summary>
        Task RemoveAsync(string cacheKey);

        /// <summary>
        /// حذف چندین مقدار از کش
        /// </summary>
        Task RemoveBulkAsync(IEnumerable<string> keys);
        /// <summary>
        /// حذف یک آیتم خاص از لیست کش‌شده
        /// </summary>
        Task RemoveItemAsync<T>(string cacheKey, Func<T, bool> predicate, DistributedCacheEntryOptions options = null, MemoryCacheEntryOptions memoryOptions = null);

        /// <summary>
        /// حذف مجموعه‌ای از آیتم‌ها از لیست کش‌شده
        /// </summary>
        Task RemoveItemArrayAsync<T>(string cacheKey, Func<T, bool> predicate, DistributedCacheEntryOptions options = null, MemoryCacheEntryOptions memoryOptions = null);
        /// <summary>
        /// ریست کل کش با پیشوند
        /// </summary>
        Task ResetCacheAsync(string prefix);

        /// <summary>
        /// جستجو در لیست کش‌شده
        /// </summary>
        Task<IEnumerable<T>> SearchAsync<T>(string cacheKey, Func<T, bool> predicate);

        /// <summary>
        /// جستجو با صفحه‌بندی در لیست کش‌شده
        /// </summary>
        Task<(IEnumerable<T> Results, int TotalCount)> SearchWithPaginationAsync<T>(
            string cacheKey, Func<T, bool> predicate, int pageNumber, int pageSize);
    }
}