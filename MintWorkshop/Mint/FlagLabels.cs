﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintWorkshop.Mint
{
    public static class FlagLabels
    {
        public static Dictionary<ushort, string> StdTypes = new Dictionary<ushort, string>()
        {
            { 2, "bool" },
            { 5, "uint" },
            { 9, "int" },
            { 11, "float" },
            { 16, "string" },
        };

        public static Dictionary<uint, string> ClassFlags = new Dictionary<uint, string>()
        {
            {  0x2, "enum" },
            {  0x4, "pod" },
            {  0x8, "static" },
        };

        public static Dictionary<uint, string> VariableFlags = new Dictionary<uint, string>()
        {
            {  0x1, "static" },
            {  0x8, "array" },
        };

        public static Dictionary<uint, string> FunctionFlags = new Dictionary<uint, string>()
        {
            {  0x1, "init" },
            {  0x2, "virtual" },
            /* 
             * I don't really think this is "async" in the typical sense, nor am I really sure what it signifies anymore.
             * It also doesn't necessarily have a C# equivalent, since its been around from before Basil,
             * and HAL's IL2BC transpiler could look for an attribute to set this instead, or some other automated
             * method.
             */
            {  0x4, "async" },
            {  0x8, "null" },
            { 0x10, "extern" },
            { 0x20, "static" },
            { 0x80, "return" },
        };

        public static Dictionary<uint, string> EnumFlags = new Dictionary<uint, string>()
        {

        };
    }
}
