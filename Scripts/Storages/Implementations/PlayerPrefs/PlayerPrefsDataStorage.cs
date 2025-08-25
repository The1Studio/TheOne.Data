#nullable enable
namespace TheOne.Data.Storage
{
    using System;
    using TheOne.Extensions;
    using UnityEngine;
    using UnityEngine.Scripting;
    #if THEONE_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public sealed class PlayerPrefsDataStorage : WritableDataStorage<string>
    {
        [Preserve]
        public PlayerPrefsDataStorage()
        {
        }

        protected override string? Read(string key) => PlayerPrefs.GetString(key).NullIfWhiteSpace();

        protected override void Write(string key, string value) => PlayerPrefs.SetString(key, value);

        protected override void Flush() => PlayerPrefs.Save();

        #if THEONE_UNITASK
        protected override UniTask<string?> ReadAsync(string key, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            return UniTask.FromResult(this.Read(key));
        }

        protected override UniTask WriteAsync(string key, string value, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            this.Write(key, value);
            return UniTask.CompletedTask;
        }

        protected override UniTask FlushAsync(IProgress<float>? progress, CancellationToken cancellationToken)
        {
            this.Flush();
            return UniTask.CompletedTask;
        }
        #else
        protected override IEnumerator ReadAsync(string key, Action<string?> callback, IProgress<float>? progress)
        {
            callback(this.Read(key));
            yield break;
        }

        protected override IEnumerator WriteAsync(string key, string value, Action? callback, IProgress<float>? progress)
        {
            this.Write(key, value);
            callback?.Invoke();
            yield break;
        }

        protected override IEnumerator FlushAsync(Action? callback, IProgress<float>? progress)
        {
            this.Flush();
            callback?.Invoke();
            yield break;
        }
        #endif
    }
}