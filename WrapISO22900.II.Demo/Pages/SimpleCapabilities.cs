using Spectre.Console;

namespace ISO22900.II.Demo
{
    class SimpleCapabilities : IReadOnlyCapabilities
    {
        // todo: read somehow from console?
        public ColorSystem ColorSystem { get; } = ColorSystem.Standard;
        public bool Ansi { get; } = true;
        public bool Links { get; } = true;
        public bool Legacy { get; } = false;
        public bool IsTerminal { get; } = true;
        public bool Interactive { get; } = false;
        public bool Unicode { get; } = true;
    }
}