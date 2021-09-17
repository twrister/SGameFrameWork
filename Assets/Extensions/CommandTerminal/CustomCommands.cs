using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommandTerminal
{
    public static class CustomCommands
    {

        [RegisterCommand(Name = "Add", Help = "Adds 2 numbers", MinArgCount = 2, MaxArgCount = 2)]
        static void CommandAdd(CommandArg[] args)
        {
            int a = args[0].Int;
            int b = args[1].Int;

            if (Terminal.IssuedError) return; // Error will be handled by Terminal

            int result = a + b;
            Terminal.Log("{0} + {1} = {2}", a, b, result);
        }
    }
}

