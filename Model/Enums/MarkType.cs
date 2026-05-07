using System.ComponentModel;

namespace Shared.Model.Enums
{
    public enum MarkType
    {
        [Description("🔒")]
        Lock,

        [Description("+")]
        Plus,

        [Description("🛇")]
        Stop,

        [Description("🗸")]
        CheckLight,

        [Description("!")]
        Exclamation,

        [Description("⮜")]
        BackArrow,

        [Description("◄")]
        BackArrowLight,

        [Description("🔁")]
        Repeat,

        [Description("↷")]
        Inbound,

        [Description("‼")]
        DoubleExclamation,

        [Description("✔")]
        Check
    }
}
