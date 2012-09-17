
namespace GordiasClassLibrary.Headquarters
{
    using System.Collections.Concurrent;
    using System.ComponentModel;

    internal static class EventArgsFactory
    {
        private static ConcurrentDictionary<string, PropertyChangedEventArgs> propertyChangedEventArgsDictionary = new ConcurrentDictionary<string, PropertyChangedEventArgs>();

        public static PropertyChangedEventArgs GetPropertyChangedEventArgs(string propertyName)
        {
            return propertyChangedEventArgsDictionary.GetOrAdd(propertyName, name => new PropertyChangedEventArgs(name));
        }
    }
}
