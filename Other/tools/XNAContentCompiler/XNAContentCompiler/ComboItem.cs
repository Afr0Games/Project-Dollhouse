using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAContentCompiler
{
    public class ComboItem
    {
        public string Name {get;set;}
        public string Value {get; set;}
        public string Other { get; set; }

        public ComboItem(string Name, string Value)
        {
            this.Name = Name;
            this.Value = Value;
        }

        public ComboItem(string Name, string Value, string Other)
        {
            this.Name = Name;
            this.Value = Value;
            this.Other = Other;
        }
    }
    public class ComboItemCollection : List<ComboItem> 
    {
        private string parameter;
        private Boolean PredicateName(ComboItem other)
        {
            return (other.Name == parameter);
        }
        private Boolean PredicateValue(ComboItem other)
        {
            return (other.Value == parameter);
        }

        public Boolean ContainsName(string Name)
        {
            return FindByName(Name) != null;
        }

        public ComboItem FindByName(string Name)
        {
            this.parameter = Name;
            return this.Find(PredicateName);
        }

        public Boolean ContainsValue(string Value)
        {
            this.parameter = Value;
            return this.Find(PredicateValue) != null;
        }
    
    }
}
