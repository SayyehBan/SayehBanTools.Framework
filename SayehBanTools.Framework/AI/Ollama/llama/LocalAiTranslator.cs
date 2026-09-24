using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OllamaSharp;

namespace SayehBanTools.Framework.AI.Ollama.llama
{
    /// <summary>
    /// یک ترجمه‌کننده هوشمند محلی (Local AI Translator) که با مدل‌های Ollama کار می‌کند.
    /// قابل تنظیم کامل از بیرون و استفاده مجدد در هر پروژه.
    /// </summary>
    public class LocalAiTranslator : IDisposable
    {
        private readonly OllamaApiClient _ollamaClient;
        private readonly Chat _chat;
        private bool _isInitialized = false;
        private readonly string _pendingSystemPrompt;

        /// <summary>
        /// سازنده کلاس - تمام تنظیمات از بیرون تزریق می‌شود
        /// </summary>
        /// <param name="baseUrl">آدرس پایه Ollama (مثال: http://localhost:11434)</param>
        /// <param name="modelName">نام مدل (مثال: llama3.1, phi3, mistral و ...)</param>
        /// <param name="systemPrompt">پرامپت سیستم برای کنترل رفتار مدل (اختیاری)</param>
        public LocalAiTranslator(string baseUrl, string modelName, string systemPrompt = null)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("آدرس پایه Ollama نمی‌تواند خالی باشد.", nameof(baseUrl));

            if (string.IsNullOrWhiteSpace(modelName))
                throw new ArgumentException("نام مدل نمی‌تواند خالی باشد.", nameof(modelName));

            _ollamaClient = new OllamaApiClient(new Uri(baseUrl), modelName);
            _chat = new Chat(_ollamaClient);

            // فقط برای مدل‌های متنی پرامپت سیستم رو نگه دار
            if (!IsVisionModel(modelName))
            {
                _pendingSystemPrompt = systemPrompt;
            }
        }

        private static bool IsVisionModel(string modelName)
        {
            var visionModels = new[] { "qwen", "llava", "moondream", "bakllava", "phi3-v", "llama3.2-vision" };
            // اصلاح شده برای دات‌نت فریم‌ورک ۴.۸ (به جای string.Contains با StringComparison)
            return visionModels.Any(v => modelName.IndexOf(v, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        /// <summary>
        /// مقداردهی اولیه با پرامپت سیستم (اگر بعداً بخوای تغییر بدی)
        /// </summary>
        public async Task InitializeAsync()
        {
            if (_pendingSystemPrompt != null && !string.IsNullOrWhiteSpace(_pendingSystemPrompt))
            {
                var stream = _chat.SendAsAsync(OllamaSharp.Models.Chat.ChatRole.System, _pendingSystemPrompt);
                var enumerator = stream.GetAsyncEnumerator();
                try
                {
                    while (await enumerator.MoveNextAsync())
                    {
                        // فقط برای مدل‌های متنی اجرا می‌شه
                    }
                }
                finally
                {
                    if (enumerator != null)
                        await enumerator.DisposeAsync();
                }
            }
            _isInitialized = true;
        }

        /// <summary>
        /// ترجمه یک متن از زبان مبدا به مقصد
        /// </summary>
        /// <param name="text">متن برای ترجمه</param>
        /// <param name="sourceLang">زبان مبدا (مثال: en, fa, fr)</param>
        /// <param name="targetLang">زبان مقصد</param>
        /// <returns>متن ترجمه شده</returns>
        public async Task<string> TranslateAsync(string text, string sourceLang = "en", string targetLang = "fa")
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            if (!_isInitialized)
                throw new InvalidOperationException("ترجمه‌کننده مقداردهی اولیه نشده. ابتدا InitializeAsync را فراخوانی کنید یا پرامپت سیستم را در سازنده بدهید.");

            string prompt = $"Translate this text from {sourceLang} to {targetLang} exactly as instructed:\n\n{text}";

            var result = new StringBuilder();

            var stream = _chat.SendAsAsync(OllamaSharp.Models.Chat.ChatRole.User, prompt);
            var enumerator = stream.GetAsyncEnumerator();
            try
            {
                while (await enumerator.MoveNextAsync())
                {
                    result.Append(enumerator.Current);
                }
            }
            finally
            {
                if (enumerator != null)
                    await enumerator.DisposeAsync();
            }

            return result.ToString().Trim().Trim('"').Trim();
        }

        /// <summary>
        /// ارسال یک پرامپت دلخواه و دریافت پاسخ (برای استفاده‌های عمومی، نه فقط ترجمه)
        /// </summary>
        public async Task<string> AskAsync(string userPrompt)
        {
            if (!_isInitialized)
                throw new InvalidOperationException("ترجمه‌کننده مقداردهی اولیه نشده.");

            var result = new StringBuilder();

            var stream = _chat.SendAsAsync(OllamaSharp.Models.Chat.ChatRole.User, userPrompt);
            var enumerator = stream.GetAsyncEnumerator();
            try
            {
                while (await enumerator.MoveNextAsync())
                {
                    result.Append(enumerator.Current);
                }
            }
            finally
            {
                if (enumerator != null)
                    await enumerator.DisposeAsync();
            }

            return result.ToString().Trim();
        }

        /// <summary>
        /// پاک‌سازی منابع
        /// </summary>
        public void Dispose()
        {
            // در صورت نیاز Dispose اشیاء اضافه شود
        }
    }
}