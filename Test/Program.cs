using System;
using System.Collections.Generic;
using System.Linq;
using System.NetworkToolkit.IMAP.Server;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            new IMAP_Session().SEARCH(true, "SEARCH", "(HEADER MESSAGE-ID <8beae72utu6oidlidv77n4n6.1468247043591@email.android.com>)");
        }
    }
}
