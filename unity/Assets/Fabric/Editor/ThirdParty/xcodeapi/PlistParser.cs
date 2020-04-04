using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Fabric.Internal.Editor.ThirdParty.xcodeapi
{

    public class PlistElement
    {
        protected PlistElement() { }

        // convenience methods
        public string AsString() { return ((PlistElementString)this).value; }
        public int AsInteger() { return ((PlistElementInteger)this).value; }
        public bool AsBoolean() { return ((PlistElementBoolean)this).value; }
        public PlistElementArray AsArray() { return (PlistElementArray)this; }
        public PlistElementDict AsDict() { return (PlistElementDict)this; }

        public PlistElement this[string key]
        {
            get { return AsDict()[key]; }
            set { AsDict()[key] = value; }
        }
    }

    public class PlistElementString : PlistElement
    {
        public PlistElementString(string v) { value = v; }

        public string value;
    }

    public class PlistElementInteger : PlistElement
    {
        public PlistElementInteger(int v) { value = v; }

        public int value;
    }

    public class PlistElementBoolean : PlistElement
    {
        public PlistElementBoolean(bool v) { value = v; }

        public bool value;
    }

    public class PlistElementDict : PlistElement
    {
        public PlistElementDict() : base() { }

        private readonly SortedDictionary<string, PlistElement> m_PrivateValue = new SortedDictionary<string, PlistElement>();
        public IDictionary<string, PlistElement> values { get { return m_PrivateValue; } }

        public new PlistElement this[string key]
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


        // convenience methods
        public void SetInteger(string key, int val)
        {
            values[key] = new PlistElementInteger(val);
        }

        public void SetString(string key, string val)
        {
            values[key] = new PlistElementString(val);
        }

        public void SetBoolean(string key, bool val)
        {
            values[key] = new PlistElementBoolean(val);
        }

        public PlistElementArray CreateArray(string key)
        {
            PlistElementArray v = new PlistElementArray();
            values[key] = v;
            return v;
        }

        public PlistElementDict CreateDict(string key)
        {
            PlistElementDict v = new PlistElementDict();
            values[key] = v;
            return v;
        }
    }

    public class PlistElementArray : PlistElement
    {
        public PlistElementArray() : base() { }
        public List<PlistElement> values = new List<PlistElement>();

        // convenience methods
        public void AddString(string val)
        {
            values.Add(new PlistElementString(val));
        }

        public void AddInteger(int val)
        {
            values.Add(new PlistElementInteger(val));
        }

        public void AddBoolean(bool val)
        {
            values.Add(new PlistElementBoolean(val));
        }

        public PlistElementArray AddArray()
        {
            PlistElementArray v = new PlistElementArray();
            values.Add(v);
            return v;
        }

        public PlistElementDict AddDict()
        {
            PlistElementDict v = new PlistElementDict();
            values.Add(v);
            return v;
        }
    }

    public class PlistDocument
    {
        public PlistElementDict root;
        public string version;

        public PlistDocument()
        {
            root = new PlistElementDict();
            version = "1.0";
        }

        // Parses a string that contains a XML file. No validation is done.
        internal static XDocument ParseXmlNoDtd(string text)
        {
            XmlReaderSettings settings = new XmlReaderSettings
            {
                ProhibitDtd = false,
                XmlResolver = null // prevent DTD download
            };

            XmlReader xmlReader = XmlReader.Create(new StringReader(text), settings);
            return XDocument.Load(xmlReader);
        }

        // LINQ serializes XML DTD declaration with an explicit empty 'internal subset'
        // (a pair of square brackets at the end of Doctype declaration).
        // Even though this is valid XML, XCode does not like it, hence this workaround.
        internal static string CleanDtdToString(XDocument doc)
        {
            // LINQ does not support changing the DTD of existing XDocument instances,
            // so we create a dummy document for printing of the Doctype declaration.
            // A single dummy element is added to force LINQ not to omit the declaration.
            // Also, utf-8 encoding is forced since this is the encoding we use when writing to file in UpdateInfoPlist.
            if (doc.DocumentType != null)
            {
                XDocument tmpDoc =
                    new XDocument(new XDeclaration("1.0", "utf-8", null),
                                  new XDocumentType(doc.DocumentType.Name, doc.DocumentType.PublicId, doc.DocumentType.SystemId, null),
                                  new XElement(doc.Root.Name));
                return "" + tmpDoc.Declaration + "\n" + tmpDoc.DocumentType + "\n" + doc.Root;
            }
            else
            {
                XDocument tmpDoc = new XDocument(new XDeclaration("1.0", "utf-8", null), new XElement(doc.Root.Name));
                return "" + tmpDoc.Declaration + Environment.NewLine + doc.Root;
            }
        }

        private static string GetText(XElement xml)
        {
            return String.Join("", xml.Nodes().OfType<XText>().Select(x => x.Value).ToArray());
        }

        private static PlistElement ReadElement(XElement xml)
        {
            switch (xml.Name.LocalName)
            {
                case "dict":
                    {
                        List<XElement> children = xml.Elements().ToList();
                        PlistElementDict el = new PlistElementDict();

                        if (children.Count % 2 == 1)
                        {
                            throw new Exception("Malformed plist file");
                        }

                        for (int i = 0; i < children.Count - 1; i++)
                        {
                            if (children[i].Name != "key")
                            {
                                throw new Exception("Malformed plist file");
                            }

                            string key = GetText(children[i]).Trim();
                            PlistElement newChild = ReadElement(children[i + 1]);
                            if (newChild != null)
                            {
                                i++;
                                el[key] = newChild;
                            }
                        }
                        return el;
                    }
                case "array":
                    {
                        List<XElement> children = xml.Elements().ToList();
                        PlistElementArray el = new PlistElementArray();

                        foreach (XElement childXml in children)
                        {
                            PlistElement newChild = ReadElement(childXml);
                            if (newChild != null)
                            {
                                el.values.Add(newChild);
                            }
                        }
                        return el;
                    }
                case "string":
                    return new PlistElementString(GetText(xml));
                case "integer":
                    {
                        int r;
                        if (int.TryParse(GetText(xml), out r))
                        {
                            return new PlistElementInteger(r);
                        }

                        return null;
                    }
                case "true":
                    return new PlistElementBoolean(true);
                case "false":
                    return new PlistElementBoolean(false);
                default:
                    return null;
            }
        }

        public void ReadFromFile(string path)
        {
            ReadFromString(File.ReadAllText(path));
        }

        public void ReadFromStream(TextReader tr)
        {
            ReadFromString(tr.ReadToEnd());
        }

        public void ReadFromString(string text)
        {
            XDocument doc = ParseXmlNoDtd(text);
            version = (string)doc.Root.Attribute("version");
            XElement xml = doc.XPathSelectElement("plist/dict");

            PlistElement dict = ReadElement(xml);
            if (dict == null)
            {
                throw new Exception("Error parsing plist file");
            }

            root = dict as PlistElementDict;
            if (root == null)
            {
                throw new Exception("Malformed plist file");
            }
        }

        private static XElement WriteElement(PlistElement el)
        {
            if (el is PlistElementBoolean)
            {
                PlistElementBoolean realEl = el as PlistElementBoolean;
                return new XElement(realEl.value ? "true" : "false");
            }
            if (el is PlistElementInteger)
            {
                PlistElementInteger realEl = el as PlistElementInteger;
                return new XElement("integer", realEl.value.ToString());
            }
            if (el is PlistElementString)
            {
                PlistElementString realEl = el as PlistElementString;
                return new XElement("string", realEl.value);
            }
            if (el is PlistElementDict)
            {
                PlistElementDict realEl = el as PlistElementDict;
                XElement dictXml = new XElement("dict");
                foreach (KeyValuePair<string, PlistElement> kv in realEl.values)
                {
                    XElement keyXml = new XElement("key", kv.Key);
                    XElement valueXml = WriteElement(kv.Value);
                    if (valueXml != null)
                    {
                        dictXml.Add(keyXml);
                        dictXml.Add(valueXml);
                    }
                }
                return dictXml;
            }
            if (el is PlistElementArray)
            {
                PlistElementArray realEl = el as PlistElementArray;
                XElement arrayXml = new XElement("array");
                foreach (PlistElement v in realEl.values)
                {
                    XElement elXml = WriteElement(v);
                    if (elXml != null)
                    {
                        arrayXml.Add(elXml);
                    }
                }
                return arrayXml;
            }
            return null;
        }

        public void WriteToFile(string path)
        {
            File.WriteAllText(path, WriteToString());
        }

        public void WriteToStream(TextWriter tw)
        {
            tw.Write(WriteToString());
        }

        public string WriteToString()
        {
            XElement el = WriteElement(root);
            XElement rootEl = new XElement("plist");
            rootEl.Add(new XAttribute("version", version));
            rootEl.Add(el);

            XDocument doc = new XDocument();
            doc.Add(rootEl);
            return CleanDtdToString(doc);
        }
    }

} // namespace UnityEditor.iOS.XCode
