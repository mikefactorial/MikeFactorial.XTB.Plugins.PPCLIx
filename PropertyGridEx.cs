using Newtonsoft.Json.Linq;
using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MikeFactorial.XTB.PPCLIx
{
    internal class PropertyGridEx : PropertyGrid
    {
        protected Control oDocComment;

        public PropertyGridEx()
        {
            oDocComment = (Control)base.GetType().BaseType.InvokeMember("doccomment", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, null, this, null);
        }
        public int DocCommentHeight
        {
            get
            {
                return oDocComment.Height;
            }
            set
            {
                oDocComment.Height = value;
                oDocComment.Location = new System.Drawing.Point(0, this.Size.Height - value);
            }
        }
    }
}
