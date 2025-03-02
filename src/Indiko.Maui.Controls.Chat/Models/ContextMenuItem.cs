using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indiko.Maui.Controls.Chat.Models;

public class ContextMenuItem
{
    public string Name { get; set; }
    public string Tag { get; set; }
    public bool IsDestructive { get; set; }
}
