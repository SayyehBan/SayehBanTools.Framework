using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SayehBanTools.Framework.Service.Elasticsearch.Interface
{
    /// <summary>
    /// اینترفیس برای ارتباط با Elasticsearch
    /// </summary>
    public interface IElasticsearch
    {
        /// <summary>
        /// حذف اسناد از ایندکس Elasticsearch با استفاده از کوئری مشخص
        /// </summary>
        Task DeleteByQueryAsync<T>(string indexName, Action<QueryDescriptor<T>> query);

        /// <summary>
        /// افزودن یک سند به ایندکس Elasticsearch
        /// </summary>
        Task IndexDocumentAsync<T>(string indexName, T document);

        /// <summary>
        /// بررسی وجود یک ایندکس در Elasticsearch
        /// </summary>
        Task<bool> IndexExistsAsync(string indexName);

        /// <summary>
        /// مقداردهی اولیه ایندکس‌ها در Elasticsearch برای زبان‌های مشخص
        /// </summary>
        Task InitializeIndicesAsync(string indexNamePrefix, Dictionary<string, string> propertyMappings, params string[] languageCodes);
        /// <summary>
        /// مقداردهی اولیه ایندکس‌ها در Elasticsearch برای زبان‌های مشخص
        /// </summary>
        Task InitializeIndicesAsync(string indexNamePrefix, params string[] languageCodes);
        /// <summary>
        /// مقداردهی اولیه ایندکس‌ها در Elasticsearch برای زبان‌های مشخص
        /// </summary>
        Task InitializeIndicesAsync(string indexNamePrefix);
        /// <summary>
        /// جستجوی اسناد در ایندکس Elasticsearch با استفاده از تنظیمات جستجو
        /// </summary>
        Task<IEnumerable<T>> SearchAsync<T>(string indexName, Action<SearchRequestDescriptor<T>> searchDescriptor);
        /// <summary>
        /// ذخیره و ویرایش اسناد در ایندکس Elasticsearch به صورت Bulk
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="documents"></param>
        /// <returns></returns>
        Task BulkIndexAsync<T>(string indexName, IEnumerable<T> documents) where T : class;
        /// <summary>
        /// ذخیره و ویرایش اسناد در ایندکس Elasticsearch به صورت Bulk
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="documents"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        Task BulkAllAsync<T>(string indexName, IEnumerable<T> documents, Action<BulkAllRequestDescriptor<T>> configure) where T : class;
        /// <summary>
        /// دریافت تمام اسناد یک ایندکس بدون فیلتر یا جستجوی خاص
        /// </summary>
        /// <typeparam name="T">نوع سند</typeparam>
        /// <param name="indexName">نام ایندکس</param>
        /// <param name="Size"></param>
        /// <returns>لیست اسناد موجود در ایندکس</returns>
        Task<IEnumerable<T>> GetAllDocumentsAsync<T>(string indexName, int Size) where T : class;
        /// <summary>
        /// جستجوی پیشرفته با دسترسی مستقیم به ElasticsearchClient
        /// </summary>
        Task<SearchResponse<T>> AdvancedSearchAsync<T>(string indexName, Action<SearchRequestDescriptor<T>> searchDescriptor) where T : class;
    }
}