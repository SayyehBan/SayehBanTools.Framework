using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using SayehBanTools.Framework.Service.Elasticsearch.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SayehBanTools.Framework.Service.Elasticsearch.Repository
{
    /// <summary>
    /// ریپازیتوری برای مدیریت عملیات Elasticsearch مرتبط با دسته‌بندی‌ها و موقعیت‌های جغرافیایی
    /// </summary>
    public class RElasticsearch : IElasticsearch
    {
        private readonly ElasticsearchClient _elasticClient;

        /// <summary>
        /// سازنده کلاس برای مقداردهی اولیه کلاینت Elasticsearch و تنظیمات آن
        /// </summary>
        /// <param name="elasticClient">کلاینت Elasticsearch برای ارتباط با سرور</param>
        /// <exception cref="ArgumentNullException">در صورت null بودن پارامترها</exception>
        public RElasticsearch(ElasticsearchClient elasticClient)
        {
            _elasticClient = elasticClient ?? throw new ArgumentNullException(nameof(elasticClient));
        }
        /// <summary>
        /// hx
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="documents"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public async Task BulkAllAsync<T>(string indexName, IEnumerable<T> documents, Action<BulkAllRequestDescriptor<T>> configure) where T : class
        {
            var tcs = new TaskCompletionSource<bool>();

            var observer = new BulkAllObserver(
                onNext: response => Console.WriteLine($"Indexed {response.Items.Count} documents"),
                onError: ex =>
                {
                    Console.WriteLine($"[BulkAll] Error: {ex.Message}");
                    tcs.TrySetException(ex);
                },
                onCompleted: () =>
                {
                    Console.WriteLine($"[BulkAll] Completed successfully!");
                    tcs.TrySetResult(true);
                });

            var bulkAll = _elasticClient.BulkAll(documents, b => configure(b.Index(indexName.ToLower())));
            bulkAll.Subscribe(observer);

            await tcs.Task;
        }
        /// <summary>
        /// ذخیره و ویراش اطلاعات به صورت Bulk در Elasticsearch
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="documents"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task BulkIndexAsync<T>(string indexName, IEnumerable<T> documents) where T : class
        {
            var bulkResponse = await _elasticClient.BulkAsync(b => b
                .Index(indexName.ToLower())
                .IndexMany(documents)
                .Refresh(Refresh.WaitFor));

            if (bulkResponse.Errors)
            {
                var errorMessages = string.Join("; ", bulkResponse.ItemsWithErrors.Select(e => e.Error.Reason));
                throw new InvalidOperationException($"Failed to bulk index documents: {errorMessages}");
            }
        }

        /// <summary>
        /// حذف اسناد از ایندکس Elasticsearch با استفاده از کوئری مشخص
        /// </summary>
        public async Task DeleteByQueryAsync<T>(string indexName, Action<QueryDescriptor<T>> query)
        {
            try
            {
                var response = await _elasticClient.DeleteByQueryAsync<T>(d => d
                    .Indices(indexName.ToLower())
                    .Query(q => query(q)));

                if (!response.IsValidResponse)
                {
                    throw new InvalidOperationException($"Failed to delete documents from index {indexName.ToLower()}: {response.DebugInformation}");
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// افزودن یک سند به ایندکس Elasticsearch
        /// </summary>
        public async Task IndexDocumentAsync<T>(string indexName, T document)
        {
            var response = await _elasticClient.IndexAsync(document, idx => idx.Index(indexName).Refresh(Refresh.WaitFor));
            if (!response.IsValidResponse)
                throw new InvalidOperationException($"Failed to index document: {response.DebugInformation}");
        }

        /// <summary>
        /// بررسی وجود یک ایندکس در Elasticsearch
        /// </summary>
        public async Task<bool> IndexExistsAsync(string indexName)
        {
            var response = await _elasticClient.Indices.ExistsAsync(indexName.ToLower());
            return response.Exists;
        }

        /// <summary>
        /// مقداردهی اولیه ایندکس‌ها در Elasticsearch برای زبان‌های مشخص
        /// </summary>
        public async Task InitializeIndicesAsync(string indexNamePrefix, Dictionary<string, string> propertyMappings, params string[] languageCodes)
        {
            foreach (var languageCode in languageCodes)
            {
                var indexName = $"{indexNamePrefix.ToLower()}_{languageCode.ToLower()}";
                var createResponse = await _elasticClient.Indices.CreateAsync(indexName, c => c
                    .Mappings(m => m.Properties(props =>
                    {
                        foreach (var mapping in propertyMappings)
                        {
                            switch (mapping.Value.ToLower())
                            {
                                case "keyword":
                                    props.Keyword(mapping.Key);
                                    break;
                                case "text":
                                    props.Text(mapping.Key);
                                    break;
                                case "integer":
                                    props.IntegerNumber(mapping.Key);
                                    break;
                                default:
                                    throw new ArgumentException($"Unsupported mapping type: {mapping.Value}");
                            }
                        }
                    })));
                if (!createResponse.IsValidResponse)
                    throw new InvalidOperationException($"Failed to create index {indexName.ToLower()}: {createResponse.DebugInformation}");
            }
        }
        /// <summary>
        /// مقداردهی اولیه ایندکس‌ها در Elasticsearch برای زبان‌های مشخص
        /// </summary>
        public async Task InitializeIndicesAsync(string indexNamePrefix, params string[] languageCodes)
        {
            foreach (var languageCode in languageCodes)
            {
                var indexName = $"{indexNamePrefix.ToLower()}_{languageCode.ToLower()}";
                var createResponse = await _elasticClient.Indices.CreateAsync(indexName.ToLower(), c => c
                    .Mappings(m => m.Properties(props =>
                    {
                    })));
                if (!createResponse.IsValidResponse)
                    throw new InvalidOperationException($"Failed to create index {indexName.ToLower()}: {createResponse.DebugInformation}");
            }
        }
        /// <summary>
        /// مقداردهی اولیه ایندکس‌ها در Elasticsearch برای زبان‌های مشخص
        /// </summary>
        public async Task InitializeIndicesAsync(string indexNamePrefix)
        {

            var indexName = $"{indexNamePrefix.ToLower()}";
            var createResponse = await _elasticClient.Indices.CreateAsync(indexName.ToLower(), c => c
                .Mappings(m => m.Properties(props =>
                {

                })));
            if (!createResponse.IsValidResponse)
                throw new InvalidOperationException($"Failed to create index {indexName.ToLower()}: {createResponse.DebugInformation}");

        }

        /// <summary>
        /// جستجوی اسناد در ایندکس Elasticsearch با استفاده از تنظیمات جستجو
        /// </summary>
        public async Task<IEnumerable<T>> SearchAsync<T>(string indexName, Action<SearchRequestDescriptor<T>> searchDescriptor)
        {
            var descriptor = new SearchRequestDescriptor<T>().Indices(indexName);
            searchDescriptor?.Invoke(descriptor);
            var response = await _elasticClient.SearchAsync<T>(descriptor);
            return response.Documents;
        }
        /// <summary>
        /// دریافت تمام اسناد یک ایندکس بدون فیلتر یا جستجوی خاص
        /// </summary>
        /// <typeparam name="T">نوع سند</typeparam>
        /// <param name="indexName">نام ایندکس</param>
        /// <param name="Size"></param>
        /// <returns>لیست اسناد موجود در ایندکس</returns>
        public async Task<IEnumerable<T>> GetAllDocumentsAsync<T>(string indexName,int Size) where T : class
        {
            try
            {

                var response = await _elasticClient.SearchAsync<T>(s => s
                    .Indices(indexName.ToLower())
                    .Query(q => q
                        .MatchAll()
                    )
                    .Size(Size) // حداکثر تعداد اسناد در یک درخواست (قابل تنظیم)
                );

                if (!response.IsValidResponse)
                {
                    throw new InvalidOperationException($"Failed to fetch documents from index {indexName.ToLower()}: {response.DebugInformation}");
                }

                return response.Documents;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// جستجوی پیشرفته با دسترسی مستقیم به ElasticsearchClient
        /// </summary>
        public async Task<SearchResponse<T>> AdvancedSearchAsync<T>(string indexName, Action<SearchRequestDescriptor<T>> searchDescriptor) where T : class
        {
            var descriptor = new SearchRequestDescriptor<T>().Indices(indexName.ToLower());
            searchDescriptor?.Invoke(descriptor);
            return await _elasticClient.SearchAsync<T>(descriptor);
        }
    }
}