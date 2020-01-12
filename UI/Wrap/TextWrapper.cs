using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AD.UI.Core;
using UnityEngine.UI;

namespace AD.UI.Wrap
{
    public class TextWrapper : BaseWrapper<Text>, IFieldChangeCb<string>
    {
        private readonly Text text;
        public TextWrapper(Text _text) : base(_text)
        {
            text = _text;
        }

        Action<string> IFieldChangeCb<string>.GetFieldChangeCb()
        {
            return (value) => text.text = value;
        }

    }
}
