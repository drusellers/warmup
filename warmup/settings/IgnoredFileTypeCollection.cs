// Copyright 2007-2010 The Apache Software Foundation.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace warmup.settings
{
    using System.Configuration;

    /// <summary>
    /// Ignored file config collection
    /// </summary>
    public class IgnoredFileTypeCollection : ConfigurationElementCollection //, IEnumerable<IgnoredFileType>
    {
        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new IgnoredFileType();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((IgnoredFileType) element).Extension;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            BaseClear();
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="ignoredFileType">Type of the ignored file.</param>
        /// <returns></returns>
        public int IndexOf(IgnoredFileType ignoredFileType)
        {
            return BaseIndexOf(ignoredFileType);
        }

        /// <summary>
        /// Adds the specified ignored file type.
        /// </summary>
        /// <param name="ignoredFileType">Type of the ignored file.</param>
        public void Add(IgnoredFileType ignoredFileType)
        {
            BaseAdd(ignoredFileType);
        }
    }

    /// <summary>
    /// Ignored file extension item
    /// </summary>
    public class IgnoredFileType : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the extension.
        /// </summary>
        /// <value>
        /// The extension.
        /// </value>
        [ConfigurationProperty("ext", IsRequired = true, IsKey = true)]
        public string Extension
        {
            get { return (string) this["ext"]; }
            set { this["ext"] = value; }
        }
    }
}