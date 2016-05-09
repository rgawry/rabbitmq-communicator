namespace System.Reactive.Disposables
{
    public static class CompositeDisposableExtensions
    {
        public static TDisposable DisposeWith<TDisposable>(this TDisposable disposable, CompositeDisposable disposer) where TDisposable : IDisposable
        {
            disposer.Add(disposable);
            return disposable;
        }
    }
}
