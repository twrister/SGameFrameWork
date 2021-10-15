using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommandTerminal
{
    public static class CustomCommands
    {

        [RegisterCommand(Name = "Add", Help = "Adds 2 numbers (Add 1 2)", MinArgCount = 2, MaxArgCount = 2)]
        static void CommandAdd(CommandArg[] args)
        {
            int a = args[0].Int;
            int b = args[1].Int;

            if (Terminal.IssuedError) return; // Error will be handled by Terminal

            int result = a + b;
            Terminal.Log("{0} + {1} = {2}", a, b, result);
        }

        [RegisterCommand(Name = "wcb", Help = "Write CopyBuffer (wcb info)", MinArgCount = 1)]
        static void CommandWriteCopyBuffer(CommandArg[] args)
        {
            string whiteStr = args[0].String;

            UnityEngine.GUIUtility.systemCopyBuffer = whiteStr;
            if (Terminal.IssuedError) return;

            Terminal.Log("white success! copy buffer = {0}", whiteStr);
        }

        [RegisterCommand(Name = "pcb", Help = "Print CopyBuffer (pcb)")]
        static void CommandPrintCopyBuffer(CommandArg[] args)
        {
            string str = UnityEngine.GUIUtility.systemCopyBuffer;

            if (Terminal.IssuedError) return;

            Terminal.Log("copy buffer = {0}", str);
        }
    }
}

