#nullable enable
namespace UniT.Data
{
    using System;
    using System.Linq;
    using UniT.Data.Storage;
    using UniT.Extensions;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public interface IDataManager
    {
        #region Sync

        public IData[] Load(string[] keys, Type[] types);

        public void Save(string[] keys);

        public void Flush(string[] keys);

        public void SaveAll();

        public void FlushAll();

        #region Shortcuts

        public void SaveAndFlush(string[] keys)
        {
            this.Save(keys);
            this.Flush(keys);
        }

        public void SaveAndFlushAll()
        {
            this.SaveAll();
            this.FlushAll();
        }

        public IData Load(string key, Type type) => this.Load(new[] { key }, new[] { type })[0];

        public void Save(string key) => this.Save(new[] { key });

        public void Flush(string key) => this.Flush(new[] { key });

        public void SaveAndFlush(string key) => this.SaveAndFlush(new[] { key });

        public T Load<T>(string key) where T : IReadableData => (T)this.Load(key, typeof(T));

        public T Load<T>() where T : IReadableData => this.Load<T>(typeof(T).GetKey());

        public void Save<T>() where T : IWritableData => this.Save(typeof(T).GetKey());

        public void Flush<T>() where T : IWritableData => this.Flush(typeof(T).GetKey());

        public void SaveAndFlush<T>() where T : IWritableData => this.SaveAndFlush(typeof(T).GetKey());

        #endregion

        #endregion

        #region Async

        #if UNIT_UNITASK
        public UniTask<IData[]> LoadAsync(string[] keys, Type[] types, IProgress<float>? progress = null, CancellationToken cancellationToken = default);

        public UniTask SaveAsync(string[] keys, IProgress<float>? progress = null, CancellationToken cancellationToken = default);

        public UniTask FlushAsync(string[] keys, IProgress<float>? progress = null, CancellationToken cancellationToken = default);

        public UniTask SaveAllAsync(IProgress<float>? progress = null, CancellationToken cancellationToken = default);

        public UniTask FlushAllAsync(IProgress<float>? progress = null, CancellationToken cancellationToken = default);

        #region Shortcuts

        public async UniTask SaveAndFlushAsync(string[] keys, IProgress<float>? progress = null, CancellationToken cancellationToken = default)
        {
            var subProgresses = progress.CreateSubProgresses(2).ToArray();
            await this.SaveAsync(keys, subProgresses[0], cancellationToken);
            await this.FlushAsync(keys, subProgresses[1], cancellationToken);
        }

        public async UniTask SaveAndFlushAllAsync(IProgress<float>? progress = null, CancellationToken cancellationToken = default)
        {
            var subProgresses = progress.CreateSubProgresses(2).ToArray();
            await this.SaveAllAsync(subProgresses[0], cancellationToken);
            await this.FlushAllAsync(subProgresses[1], cancellationToken);
        }

        public UniTask<IData> LoadAsync(string key, Type type, IProgress<float>? progress = null, CancellationToken cancellationToken = default) => this.LoadAsync(new[] { key }, new[] { type }, progress, cancellationToken).ContinueWith(datas => datas[0]);

        public UniTask SaveAsync(string key, IProgress<float>? progress = null, CancellationToken cancellationToken = default) => this.SaveAsync(new[] { key }, progress, cancellationToken);

        public UniTask FlushAsync(string key, IProgress<float>? progress = null, CancellationToken cancellationToken = default) => this.FlushAsync(new[] { key }, progress, cancellationToken);

        public UniTask SaveAndFlushAsync(string key, IProgress<float>? progress = null, CancellationToken cancellationToken = default) => this.SaveAndFlushAsync(new[] { key }, progress, cancellationToken);

        public UniTask<T> LoadAsync<T>(string key, IProgress<float>? progress = null, CancellationToken cancellationToken = default) where T : IReadableData => this.LoadAsync(key, typeof(T), progress, cancellationToken).ContinueWith(data => (T)data);

        public UniTask<T> LoadAsync<T>(IProgress<float>? progress = null, CancellationToken cancellationToken = default) where T : IReadableData => this.LoadAsync<T>(typeof(T).GetKey(), progress, cancellationToken);

        public UniTask SaveAsync<T>(IProgress<float>? progress = null, CancellationToken cancellationToken = default) where T : IWritableData => this.SaveAsync(typeof(T).GetKey(), progress, cancellationToken);

        public UniTask FlushAsync<T>(IProgress<float>? progress = null, CancellationToken cancellationToken = default) where T : IWritableData => this.FlushAsync(typeof(T).GetKey(), progress, cancellationToken);

        public UniTask SaveAndFlushAsync<T>(IProgress<float>? progress = null, CancellationToken cancellationToken = default) where T : IWritableData => this.SaveAndFlushAsync(typeof(T).GetKey(), progress, cancellationToken);

        #endregion

        #else
        public IEnumerator LoadAsync(string[] keys, Type[] types, Action<IData[]> callback, IProgress<float>? progress = null);

        public IEnumerator SaveAsync(string[] keys, Action? callback = null, IProgress<float>? progress = null);

        public IEnumerator FlushAsync(string[] keys, Action? callback = null, IProgress<float>? progress = null);

        public IEnumerator SaveAllAsync(Action? callback = null, IProgress<float>? progress = null);

        public IEnumerator FlushAllAsync(Action? callback = null, IProgress<float>? progress = null);

        #region Shortcuts

        public IEnumerator SaveAndFlushAsync(string[] keys, Action? callback = null, IProgress<float>? progress = null)
        {
            var subProgresses = progress.CreateSubProgresses(2).ToArray();
            yield return this.SaveAsync(keys, progress: subProgresses[0]);
            yield return this.FlushAsync(keys, progress: subProgresses[1]);
            callback?.Invoke();
        }

        public IEnumerator SaveAndFlushAllAsync(Action? callback = null, IProgress<float>? progress = null)
        {
            var subProgresses = progress.CreateSubProgresses(2).ToArray();
            yield return this.SaveAllAsync(progress: subProgresses[0]);
            yield return this.FlushAllAsync(progress: subProgresses[1]);
            callback?.Invoke();
        }

        public IEnumerator LoadAsync(string key, Type type, Action<IData> callback, IProgress<float>? progress = null) => this.LoadAsync(new[] { key }, new[] { type }, datas => callback(datas[0]), progress);

        public IEnumerator SaveAsync(string key, Action? callback = null, IProgress<float>? progress = null) => this.SaveAsync(new[] { key }, callback, progress);

        public IEnumerator FlushAsync(string key, Action? callback = null, IProgress<float>? progress = null) => this.FlushAsync(new[] { key }, callback, progress);

        public IEnumerator SaveAndFlushAsync(string key, Action? callback = null, IProgress<float>? progress = null) => this.SaveAndFlushAsync(new[] { key }, callback, progress);

        public IEnumerator LoadAsync<T>(string key, Action<T> callback, IProgress<float>? progress = null) where T : IReadableData => this.LoadAsync(key, typeof(T), data => callback((T)data), progress);

        public IEnumerator LoadAsync<T>(Action<T> callback, IProgress<float>? progress = null) where T : IReadableData => this.LoadAsync(typeof(T).GetKey(), callback, progress);

        public IEnumerator SaveAsync<T>(Action? callback = null, IProgress<float>? progress = null) where T : IWritableData => this.SaveAsync(typeof(T).GetKey(), callback, progress);

        public IEnumerator FlushAsync<T>(Action? callback = null, IProgress<float>? progress = null) where T : IWritableData => this.FlushAsync(typeof(T).GetKey(), callback, progress);

        public IEnumerator SaveAndFlushAsync<T>(Action? callback = null, IProgress<float>? progress = null) where T : IWritableData => this.SaveAndFlushAsync(typeof(T).GetKey(), callback, progress);

        #endregion

        #endif

        #endregion
    }
}