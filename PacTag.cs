using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.Design.WebControls;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace MikeFactorial.XTB.PPCLIx
{
    internal class PacTagListPropertyGridAdapter : ICustomTypeDescriptor
    {
        List<PacTag> _tagList;

        public PacTagListPropertyGridAdapter(List<PacTag> list)
        {
            _tagList = list;
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return _tagList;
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }

        PropertyDescriptorCollection
            System.ComponentModel.ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            ArrayList properties = new ArrayList();
            foreach (PacTag tag in _tagList)
            {
                properties.Add(new PacTagListPropertyDescriptor(_tagList, tag.Name));
            }

            PropertyDescriptor[] props =
                (PropertyDescriptor[])properties.ToArray(typeof(PropertyDescriptor));

            return new PropertyDescriptorCollection(props);
        }

        public List<PacTag> TagList
        {
            get
            {
                return this._tagList;
            }
        }
    }

    class PacTagListPropertyDescriptor : PropertyDescriptor
    {
        List<PacTag> _tagList;
        string _key;

        internal PacTagListPropertyDescriptor(List<PacTag> list, string key)
            : base(key.ToString(), null)
        {
            _tagList = list;
            _key = key;
        }

        public override Type PropertyType
        {
            get 
            {
                
                return _tagList.FirstOrDefault(t=>t.Name == _key).Value.GetType(); 
            }
        }

        public TreeNode GetCurrentTreeNode()
        {
            return _tagList.FirstOrDefault(t => t.Name == _key).Node;
        }
        public override void SetValue(object component, object value)
        {
            _tagList.FirstOrDefault(t => t.Name == _key).Node.Text = $"{_tagList.FirstOrDefault(t => t.Name == _key).Name} \"{value.ToString()}\"";
            _tagList.FirstOrDefault(t => t.Name == _key).Node.Name = $"{_tagList.FirstOrDefault(t => t.Name == _key).Name} \"{value.ToString()}\"";
            _tagList.FirstOrDefault(t => t.Name == _key).Value = value;
        }
        public override string Category => "Command Input Arguments";

        public override string Description
        {
            get
            {
                return _tagList.FirstOrDefault(t => t.Name == _key).HelpText;
            }
        }
        public override object GetValue(object component)
        {
            return _tagList.FirstOrDefault(t => t.Name == _key).Value;
        }
        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type ComponentType
        {
            get { return null; }
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override void ResetValue(object component)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override object GetEditor(Type editorBaseType)
        {
            var baseEditor = base.GetEditor(editorBaseType);
            if (_key.ToLower().Contains("file"))
            {
                return new FileNameEditor();
            }
            else if (_key.ToLower().Contains("path") || _key.ToLower().Contains("folder") || _key.ToLower().Contains("directory"))
            {
                return new FolderNameEditor();
            }
            return baseEditor;
        }
    }
    internal class PacTag
    {
        internal PacTag(ref TreeNode node)
        {
            if (node.Parent != null && node.Parent.Parent == null)
            {
                this.Type = PacTagType.Noun;
            }
            else if (node.Parent != null && node.Parent.Parent != null)
            {
                this.Type = PacTagType.Verb;
            }
            else if (node.Parent != null && node.Parent.Parent != null && node.Parent.Parent.Parent != null)
            {
                this.Type = PacTagType.Argument;
            }
            else
            {
                this.Type = PacTagType.Root;
            }
            Node = node;
            Name = node.Name;
        }
        public enum PacTagType
        {
            Root,
            Noun,
            Verb,
            Argument
        }

        public PacTagType Type { get; set; }
        public TreeNode Node { get; set; }
        public string Name { get; set; }
        public string HelpText { get; set; }
        public object Value { get; set; }
    }
}
