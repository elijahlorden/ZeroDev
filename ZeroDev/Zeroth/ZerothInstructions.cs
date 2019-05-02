using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroDev.Zeroth
{
    /*
     * An enum containing named hardware opcodes, referenced in both the Zeroth compiler and the VM.
     * ANY references to a hardware instruction should use this enum.  Magic numbers are bad.
     */
    public enum HardwareInstruction : byte
    {
        NOP =       0,
        HLT =       1,
        // Stack operations
        LIT1 =      2,
        LIT2 =      3,
        DUP =       4,
        DROP =      5,
        SWAP =      6,
        OVER =      7,
        ROT =       8,
        RROT =      9,
        NIP =       10,
        TUCK =      11,
        // Branch operations
        JUMP =      12,
        BRANCH0 =   13,
        BRANCH1 =   14,
        CALL =      15,
        RET =       16,
        // ALU operations
        INC =       17,
        DEC =       18,
        ADD =       19,
        SUB =       20,
        MUL =       21,
        LSHIFT =    22,
        RSHIFT =    23,
        NOT =       24,
        AND =       25,
        OR =        26,
        XOR =       27,
        // ALU comparison operations
        EQZ =       28,
        NEQZ =      29,
        EQ =        30,
        NEQ =       31,
        GT =        32,
        LT =        33,
        GEQ =       34,
        LEQ =       35
        // Memory operations

        // I/O operations

    }
}
