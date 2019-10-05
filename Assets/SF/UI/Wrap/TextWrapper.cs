using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.UI.Core;
using UnityEngine.UI;

namespace Assets.SF.UI.Wrap
{
    public class TextWrapper : BaseWrapper<Text>, IBindData<string>, IBindData<bool>
    {
        private readonly Text text;
        public TextWrapper(Text _text) : base(_text)
        {
            text = _text;
        }

        public Action<string> GetBindFieldFunc()
        {
            return (value) => text.text = value;
        }

        Action<bool> IBindData<bool>.GetBindFieldFunc()
        {
            return (value) => text.gameObject.SetActive(value);
        }
    }
}
