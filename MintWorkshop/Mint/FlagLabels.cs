using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintWorkshop.Mint
{
    public static class FlagLabels
    {
        public static Dictionary<uint, string> ClassFlags = new Dictionary<uint, string>()
        {
            {  0x2, "enum" },
            {  0x4, "pod" },
            {  0x8, "util" },
        };

        public static Dictionary<uint, string> VariableFlags = new Dictionary<uint, string>()
        {
            {  0x1, "static" },
            {  0x8, "array" },
        };

        public static Dictionary<uint, string> FunctionFlags = new Dictionary<uint, string>()
        {
            {  0x1, "init" },
            {  0x4, "public" },
            {  0x8, "null" },
            { 0x80, "return" },
        };
    }
}
