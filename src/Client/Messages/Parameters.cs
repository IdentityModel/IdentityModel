using System;
using System.Collections.Generic;
using System.Linq;
using IdentityModel.Internal;

namespace IdentityModel.Client
{
    /// <summary>
    /// Models a list of request parameters
    /// </summary>
    public class Parameters : List<KeyValuePair<string, string>>
    {
        /// <summary>
        /// ctor
        /// </summary>
        public Parameters() : base()
        { }
        
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="values"></param>
        public Parameters(List<KeyValuePair<string, string>> values)
            : base(values)
        { }
        
        /// <summary>
        /// Adds a key/value to the list
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, string value)
        {
            Add(new KeyValuePair<string, string>(key, value));
        }

        /// <summary>
        /// Checks the existence of a parameter
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            return (this.Any(k => string.Equals(k.Key, key)));
        }

        /// <summary>
        /// Adds a parameter if it has a value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="allowDuplicates"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void AddOptional(string key, string value, bool allowDuplicates = false)
        {
            if (key.IsMissing()) throw new ArgumentNullException(nameof(key));
            
            if (allowDuplicates == false)
            {
                if (Contains(key))
                {
                    throw new InvalidOperationException($"Duplicate parameter: {key}");
                }
            }

            if (value.IsPresent())
            {
                Add(key, value);
            }
        }
        
        /// <summary>
        /// Adds a required parameter
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="allowDuplicates"></param>
        /// <param name="allowEmpty"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void AddRequired(string key, string value, bool allowDuplicates = false, bool allowEmpty = false)
        {
            if (key.IsMissing()) throw new ArgumentNullException(nameof(key));
            
            if (allowDuplicates == false)
            {
                if (Contains(key))
                {
                    throw new InvalidOperationException($"Duplicate parameter: {key}");
                }
            }
            
            if (value.IsPresent() || allowEmpty)
            {
                Add(key, value);
            }
            else
            {
                throw new ArgumentException($"Parameter is required", key);
            }
        }
        
        /// <summary>
        /// Merge two parameter sets
        /// </summary>
        /// <param name="additionalValues"></param>
        /// <returns></returns>
        public Parameters Merge(Parameters additionalValues = null)
        {
             if (additionalValues != null)
             {
                 var merged =
                     this.Concat(additionalValues.Where(add => !this.Contains(add.Key)))
                         .Select(s => new KeyValuePair<string, string>(s.Key, s.Value));
            
                 return new Parameters(merged.ToList());
             }

            return this;
        }
    }
}