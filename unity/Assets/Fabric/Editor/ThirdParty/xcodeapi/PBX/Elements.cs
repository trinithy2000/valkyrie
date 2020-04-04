using System.Collections.Generic;


namespace Fabric.Internal.Editor.ThirdParty.xcodeapi.PBX
{
    internal class PBXElement
    {
        protected PBXElement() { }

        // convenience methods
        public string AsString() { return ((PBXElementString)this).value; }
        public PBXElementArray AsArray() { return (PBXElementArray)this; }
        public PBXElementDict AsDict() { return (PBXElementDict)this; }

        public PBXElement this[string key]
        {
            get { return AsDict()[key]; }
            set { AsDict()[key] = value; }
        }
    }

    internal class PBXElementString : PBXElement
    {
        public PBXElementString(string v) { value = v; }

        public string value;
    }

    internal class PBXElementDict : PBXElement
    {
        public PBXElementDict() : base() { }

        private readonly Dictionary<string, PBXElement> m_PrivateValue = new Dictionary<string, PBXElement>();
        public IDictionary<string, PBXElement> values { get { return m_PrivateValue; } }

        public new PBXElement this[string key]
        {
            get
            {
                if (values.ContainsKey(key))
                {
                    return values[key];
                }

                return null;
            }
            set { values[key] = value; }
        }

        public bool Contains(string key)
        {
            return values.ContainsKey(key);
        }

        public void Remove(string key)
        {
            values.Remove(key);
        }

        public void SetString(string key, string val)
        {
            values[key] = new PBXElementString(val);
        }

        public PBXElementArray CreateArray(string key)
        {
            PBXElementArray v = new PBXElementArray();
            values[key] = v;
            return v;
        }

        public PBXElementDict CreateDict(string key)
        {
            PBXElementDict v = new PBXElementDict();
            values[key] = v;
            return v;
        }
    }

    internal class PBXElementArray : PBXElement
    {
        public PBXElementArray() : base() { }
        public List<PBXElement> values = new List<PBXElement>();

        // convenience methods
        public void AddString(string val)
        {
            values.Add(new PBXElementString(val));
        }

        public PBXElementArray AddArray()
        {
            PBXElementArray v = new PBXElementArray();
            values.Add(v);
            return v;
        }

        public PBXElementDict AddDict()
        {
            PBXElementDict v = new PBXElementDict();
            values.Add(v);
            return v;
        }
    }

} // namespace UnityEditor.iOS.Xcode

