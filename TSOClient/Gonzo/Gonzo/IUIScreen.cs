using System;
using System.Collections.Generic;
using System.Text;

namespace Gonzo
{
    interface IUIScreen
    {
        void Draw();
        void Update(InputHelper Input);
    }
}
