using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SayehBanTools.Framework.Infrastructure.Elasticsearch
{
    /// <summary>
    /// مدیری برای مدیریت ایندکس‌های Elasticsearch
    /// </summary>
    public class ElasticsearchIndexManager
    {
        private readonly ElasticsearchClient _elasticClient;

        /// <summary>
        /// سازنده کلاس با تزریق کلاینت Elasticsearch
        /// </summary>
        /// <param name="elasticClient">کلاینت Elasticsearch</param>
        public ElasticsearchIndexManager(ElasticsearchClient elasticClient)
        {
            _elasticClient = elasticClient ?? throw new ArgumentNullException(nameof(elasticClient));
        }

        /// <summary>
        /// مقداردهی اولیه ایندکس‌های Elasticsearch به‌صورت داینامیک با پشتیبانی از کدهای زبان
        /// </summary>
        /// <param name="indexNamePrefix">پیشوند نام ایندکس</param>
        /// <param name="propertyMappings">دیکشنری پراپرتی‌ها و نوع داده‌هایشان</param>
        /// <param name="languageCodes">لیست کدهای زبان</param>
        /// <returns></returns>
        public async Task InitializelanguageIndicesAsync(
            string indexNamePrefix,
            Dictionary<string, string> propertyMappings,
            params string[] languageCodes)
        {
            // تبدیل کدها به حروف کوچک برای یکپارچگی
            var allowedLanguages = languageCodes.Select(l => l.ToLower()).ToHashSet();

            // تعریف داینامیک پراپرتی‌ها
            var properties = new Properties();
            foreach (var prop in propertyMappings)
            {
                switch (prop.Value.ToLower())
                {
                    case "text":
                        properties.Add(prop.Key, new TextProperty());
                        break;
                    case "integer":
                        properties.Add(prop.Key, new IntegerNumberProperty());
                        break;
                    case "keyword":
                        properties.Add(prop.Key, new KeywordProperty());
                        break;
                    case "date":
                        properties.Add(prop.Key, new DateProperty());
                        break;
                    default:
                        throw new ArgumentException($"Unsupported property type: {prop.Value} for property {prop.Key}");
                }
            }

            // ایجاد ایندکس‌ها برای هر زبان
            foreach (var lang in allowedLanguages)
            {
                var indexName = $"{indexNamePrefix}_{lang}";
                var existsResponse = await _elasticClient.Indices.ExistsAsync(indexName);

                if (!existsResponse.Exists)
                {
                    try
                    {
                        await _elasticClient.Indices.CreateAsync(indexName, c => c
                            .Mappings(m => m.Properties(properties))
                        );
                        Console.WriteLine($"[DynamicIndices] Index {indexName} created successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[DynamicIndices] Failed to create index {indexName}: {ex.Message}");
                        throw;
                    }
                }
                else
                {
                    Console.WriteLine($"[DynamicIndices] Index {indexName} already exists.");
                }
            }
        }

        /// <summary>
        /// مقداردهی اولیه یک ایندکس Elasticsearch به‌صورت داینامیک بدون کد زبان
        /// </summary>
        /// <param name="indexName">نام ایندکس</param>
        /// <param name="propertyMappings">دیکشنری پراپرتی‌ها و نوع داده‌هایشان</param>
        /// <returns></returns>
        public async Task InitializeIndexAsync(
            string indexName,
            Dictionary<string, string> propertyMappings)
        {
            // تعریف داینامیک پراپرتی‌ها
            var properties = new Properties();
            foreach (var prop in propertyMappings)
            {
                switch (prop.Value.ToLower())
                {
                    case "text":
                        properties.Add(prop.Key, new TextProperty());
                        break;
                    case "integer":
                        properties.Add(prop.Key, new IntegerNumberProperty());
                        break;
                    case "keyword":
                        properties.Add(prop.Key, new KeywordProperty());
                        break;
                    case "date":
                        properties.Add(prop.Key, new DateProperty());
                        break;
                    default:
                        throw new ArgumentException($"Unsupported property type: {prop.Value} for property {prop.Key}");
                }
            }

            // بررسی وجود ایندکس
            var existsResponse = await _elasticClient.Indices.ExistsAsync(indexName);

            if (!existsResponse.Exists)
            {
                try
                {
                    await _elasticClient.Indices.CreateAsync(indexName, c => c
                        .Mappings(m => m.Properties(properties))
                    );
                    Console.WriteLine($"[DynamicIndices] Index {indexName} created successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DynamicIndices] Failed to create index {indexName}: {ex.Message}");
                    throw;
                }
            }
            else
            {
                Console.WriteLine($"[DynamicIndices] Index {indexName} already exists.");
            }
        }
    }


    /*
     نحوه استفاده
    var elasticClient = new ElasticsearchClient( تنظیمات );
    var indexManager = new ElasticsearchIndexManager(elasticClient);
    var propertyMappings = new Dictionary<string, string>
    {
        { "name", "text" },
        { "age", "integer" },
        { "email", "keyword" },
        { "created_at", "date" }
    };
    await indexManager.InitializeIndexAsync("mymap_index", propertyMappings);



    public async Task CategoriesAndLocationsGetInitializeIndicesAsync(params string[] languageCodes)
        {
            var propertyMappings = new Dictionary<string, string>
            {
                { "hierarchyPathString", "text" },
                { "categoryName", "text" },
                { "locationName", "text" },
                { "fullPathCategory", "text" },
                { "fullPathLocation", "text" },
                { "locationId", "integer" },
                { "categoryId", "integer" },
                { "locationNameId", "integer" },
                { "level", "integer" }
            };

            var elasticClient = new ElasticsearchClient(new ElasticsearchClientSettings(new Uri("http://localhost:9200")));
            var indexManager = new ElasticsearchIndexManager(elasticClient);

            await indexManager.InitializelanguageIndicesAsync(
                indexNamePrefix: StaticsValue.CategoriesAndLocationsGets,
                propertyMappings: propertyMappings,
                languageCodes: languageCodes
            );
        }
     */
}