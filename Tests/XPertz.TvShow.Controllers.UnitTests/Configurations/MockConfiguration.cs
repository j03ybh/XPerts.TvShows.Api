using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XPertz.TvShows.Controllers.UnitTests.Configurations
{
    internal class MockConfiguration : IConfiguration
    {
        private readonly IEnumerable<IConfigurationSection> _sections;

        public MockConfiguration(IEnumerable<IConfigurationSection> sections)
        {
            _sections = sections;
        }

        public string this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IEnumerable<IConfigurationSection> GetChildren() => _sections;

        public IChangeToken GetReloadToken()
        {
            throw new NotImplementedException();
        }

        public IConfigurationSection GetSection(string key)
        {
            return _sections.FirstOrDefault(x => x.Key == key);
        }
    }
}