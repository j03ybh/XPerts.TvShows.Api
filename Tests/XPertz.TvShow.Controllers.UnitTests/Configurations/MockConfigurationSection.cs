using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XPertz.TvShows.Controllers.UnitTests.Configurations
{
    internal class MockConfigurationSection : IConfigurationSection
    {
        private readonly IEnumerable<IConfigurationSection> _values;
        private readonly string _key;
        private readonly string _parentKey;

        public MockConfigurationSection(string parentKey, string key, string value, IEnumerable<IConfigurationSection> values)
        {
            _parentKey = parentKey;
            _key = key;
            _values = values;
            Value = value;
        }

        public string this[string key]
        {
            get => _values.FirstOrDefault(x => x.Key == key)?.Value ?? string.Empty;
            set => throw new NotImplementedException();
        }

        public string Key => _key;

        public string Path => !string.IsNullOrWhiteSpace(_parentKey) ? $"{_parentKey}:{_key}" : _key;

        public string Value { get; set; }

        public IEnumerable<IConfigurationSection> GetChildren() => _values;

        public IChangeToken GetReloadToken()
        {
            throw new NotImplementedException();
        }

        public IConfigurationSection GetSection(string key)
        {
            return _values
                .FirstOrDefault(x => x.Key == key) ?? new MockConfigurationSection(null, null, null, null);
        }
    }
}