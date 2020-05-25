using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IdentityModel.Internal;

namespace IdentityModel.Client
{
    public class Parameters : List<KeyValuePair<string, string>>
    {
        public Parameters() : base()
        { }
        
        public Parameters(List<KeyValuePair<string, string>> values)
            : base(values)
        { }
        
        public void Add(string key, string value)
        {
            Add(new KeyValuePair<string, string>(key, value));
        }

        public bool Contains(string key)
        {
            return (this.Any(k => string.Equals(k.Key, key)));
        }
        
        public void AddOptional(string key, string value, bool allowDuplicates = false, bool allowEmpty = true)
        {
            if (key.IsMissing()) throw new ArgumentNullException(nameof(key));

            else if (value.IsPresent() || allowEmpty)
            {
                if (allowDuplicates == false)
                {
                    if (Contains(key))
                    {
                        throw new InvalidOperationException($"Duplicate parameter: {key}");    
                    }
                }
                
                Add(key, value);
            }
            else
            {
                throw new ArgumentException($"Parameter is required", key);
            }
        }
        
        public void AddRequired(string key, string value, bool allowDuplicates = false, bool allowEmpty = false)
        {
            if (key.IsMissing()) throw new ArgumentNullException(nameof(key));

            if (allowEmpty == false)
            {
                if (value.IsMissing()) throw new ArgumentException($"Parameter is required", key);
            }
            
            AddOptional(key, value, allowDuplicates, allowEmpty);
        }
    }
}