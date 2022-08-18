using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IdentityModel.Internal;

namespace IdentityModel.Client;

/// <summary>
/// Models a list of request parameters
/// </summary>
public class Parameters : List<KeyValuePair<string, string>>
{
    /// <summary>
    /// Turns anonymous type or dictionary in Parameters (mainly for backwards compatibility)
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static Parameters? FromObject(object values)
    {
        if (values == null)
        {
            return null;
        }

        if (values is Dictionary<string, string> dictionary)
        {
            return new Parameters(dictionary);
        }

        dictionary = new Dictionary<string, string>();

        foreach (var prop in values.GetType().GetRuntimeProperties())
        {
            var value = prop.GetValue(values) as string;
            if (value!.IsPresent())
            {
                dictionary.Add(prop.Name, value!);
            }
        }

        return new Parameters(dictionary);
    }
        
    /// <summary>
    /// ctor
    /// </summary>
    public Parameters()
    { }

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="values"></param>
    public Parameters(IEnumerable<KeyValuePair<string, string>> values)
        : base(values)
    { }
        
    /// <summary>
    /// Adds a key/value to the list
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value</param>
    /// <param name="parameterReplace">Replace behavior.</param>
    public void Add(string key, string value, ParameterReplaceBehavior parameterReplace = ParameterReplaceBehavior.None)
    {
        if (key.IsMissing()) throw new ArgumentNullException(nameof(key));

        if (parameterReplace == ParameterReplaceBehavior.None)
        {
            Add(new KeyValuePair<string, string>(key, value));
            return;
        }

        var existingItems = this.Where(i => i.Key == key).ToList();
        if (existingItems.Count > 1 && parameterReplace == ParameterReplaceBehavior.Single)
        {
            throw new InvalidOperationException("More than one item found to replace.");
        }

        existingItems.ForEach(item => this.Remove(item));
            
        Add(new KeyValuePair<string, string>(key, value));
    }
        
    /// <summary>
    /// Get parameter value(s) based on name
    /// </summary>
    /// <param name="index"></param>
    public IEnumerable<string> this[string index]
    {
        get { return this.Where(i => i.Key.Equals(index)).Select(i => i.Value); }
    }
        
    /// <summary>
    /// Get parameter values based on name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IEnumerable<string> GetValues(string name)
    {
        return this[name];
    }
        
    /// <summary>
    /// Checks the existence of a parameter
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsKey(string key)
    {
        return (this.Any(k => string.Equals(k.Key, key)));
    }

    /// <summary>
    /// Adds a parameter if it has a value
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <param name="allowDuplicates">Allow multiple values of the same parameter.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public void AddOptional(string key, string? value, bool allowDuplicates = false)
    {
        if (key.IsMissing()) throw new ArgumentNullException(nameof(key));
            
        if (allowDuplicates == false)
        {
            if (ContainsKey(key))
            {
                throw new InvalidOperationException($"Duplicate parameter: {key}");
            }
        }

        if (value!.IsPresent())
        {
            Add(key, value!);
        }
    }

    /// <summary>
    /// Adds a required parameter
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <param name="allowDuplicates">Allow multiple values of the same parameter.</param>
    /// <param name="allowEmptyValue">Allow an empty value.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public void AddRequired(string key, string? value, bool allowDuplicates = false, bool allowEmptyValue = false)
    {
        if (key.IsMissing()) throw new ArgumentNullException(nameof(key));
            
        if (allowDuplicates == false)
        {
            if (ContainsKey(key))
            {
                throw new InvalidOperationException($"Duplicate parameter: {key}");
            }
        }
            
        if (value!.IsPresent() || allowEmptyValue)
        {
            Add(key, value!);
        }
        else
        {
            throw new ArgumentException("Parameter is required", key);
        }
    }
        
    /// <summary>
    /// Merge two parameter sets
    /// </summary>
    /// <param name="additionalValues"></param>
    /// <returns>Merged parameters</returns>
    public Parameters Merge(Parameters? additionalValues = null)
    {
        if (additionalValues != null)
        {
            var merged =
                this.Concat(additionalValues.Where(add => !this.ContainsKey(add.Key)))
                    .Select(s => new KeyValuePair<string, string>(s.Key, s.Value));
            
            return new Parameters(merged.ToList());
        }

        return this;
    }
}