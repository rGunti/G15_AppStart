using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libAppStart
{
    public class Preference
    {
        private string myKey;
        private object myValue;

        public Preference(string key, object valueObject)
        {
            myKey = key;
            myValue = valueObject;
        }
        public Preference(string key, string stringValue)
        {
            myKey = key;
            myValue = stringValue;
        }
        public Preference(string key, int intValue)
        {
            myKey = key;
            myValue = intValue;
        }
        public Preference(string key, bool boolValue)
        {
            myKey = key;
            myValue = boolValue;
        }

        public string Key { get { return myKey; } }

        public object Value
        {
            get { return myValue; }
            set
            {
                // Accept all kind of values, when preference has a null value
                // Do not accept value, which are:
                //  - from different types
                //  - null values
                if (myValue == null)
                    myValue = value;
                if (myValue.GetType() == value.GetType() && value != null)
                    myValue = value;
            }
        }

        public Type Type { get { return myValue.GetType(); } }
    }
}
