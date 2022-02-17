using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MadPodRacing.Domain.Tests.Common
{
    public static class ConsoleHelpers
    {
        public static void PushInReader(string row)
        {
            Console.SetIn(new StringReader($"{row}\r\n"));
        }

        public static void PushInReader(List<string> row)
        {
            var sb = new StringBuilder();
            row.ForEach(r => sb.Append($"{r}\r\n"));

            Console.SetIn(new StringReader(sb.ToString()));
        }
    }
}
